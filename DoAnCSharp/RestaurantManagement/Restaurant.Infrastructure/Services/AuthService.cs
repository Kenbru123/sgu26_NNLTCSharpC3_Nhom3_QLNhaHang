using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.DTOs.Auth;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    // Tên vai trò mặc định dành cho khách hàng tự đăng ký.
    private const string CustomerRoleName = "KhachHang";
    private readonly RestaurantDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(RestaurantDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterCustomerAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // Chuẩn hóa dữ liệu đầu vào trước khi kiểm tra và lưu database.
        var userName = request.UserName.Trim();
        var password = request.Password;
        var fullName = request.FullName.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        // Kiểm tra các trường bắt buộc của tài khoản.
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResult.Failed("Tên đăng nhập và mật khẩu là bắt buộc.");
        }

        // Kiểm tra thông tin cá nhân bắt buộc của khách hàng.
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber))
        {
            return AuthResult.Failed("Họ tên và số điện thoại là bắt buộc.");
        }

        // Không chấp nhận mật khẩu quá ngắn.
        if (password.Length < 6)
        {
            return AuthResult.Failed("Mật khẩu phải có ít nhất 6 ký tự.");
        }

        // Tên đăng nhập phải là duy nhất trong hệ thống.
        if (await _dbContext.TaiKhoans.AnyAsync(
                account => account.TenDangNhap == userName,
                cancellationToken))
        {
            return AuthResult.Failed("Tên đăng nhập đã tồn tại.");
        }

        // Lấy hoặc tạo vai trò khách hàng mặc định.
        var customerRole = await FindOrCreateCustomerRoleAsync(cancellationToken);

        // Tạo hồ sơ khách hàng từ thông tin đăng ký.
        var customer = new KhachHang
        {
            HoTen = fullName,
            SDT = phoneNumber,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim()
        };

        // Tạo tài khoản và chỉ lưu mật khẩu dưới dạng hash.
        var account = new TaiKhoan
        {
            MaVaiTro = customerRole.MaVaiTro,
            MaKhachHang = customer.MaKhachHang,
            TenDangNhap = userName,
            MatKhauHash = PasswordHasher.Hash(password),
            TrangThai = true
        };

        // Lưu hồ sơ khách hàng và tài khoản trong cùng một transaction.
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        _dbContext.KhachHangs.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Cập nhật mã khách hàng sau khi SQL Server sinh khóa chính.
        account.MaKhachHang = customer.MaKhachHang;
        _dbContext.TaiKhoans.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // Trả về JWT ngay sau khi đăng ký thành công.
        return AuthResult.Success(CreateResponse(account, customerRole.TenVaiTro));
    }

    public async Task<AuthResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        // Tìm tài khoản theo tên đăng nhập.
        var userName = request.UserName.Trim();
        var account = await _dbContext.TaiKhoans
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.TenDangNhap == userName,
                cancellationToken);

        // Kiểm tra tài khoản tồn tại, đang hoạt động và mật khẩu hợp lệ.
        if (account is null || !account.TrangThai || !PasswordHasher.Verify(request.Password, account.MatKhauHash))
        {
            return AuthResult.Failed("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        // Lấy vai trò để đưa vào claim của JWT.
        var role = await _dbContext.VaiTros
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.MaVaiTro == account.MaVaiTro, cancellationToken);

        if (role is null)
        {
            return AuthResult.Failed("Tài khoản chưa được cấu hình vai trò.");
        }

        // Lưu thời điểm đăng nhập gần nhất.
        await _dbContext.TaiKhoans
            .Where(item => item.MaTaiKhoan == account.MaTaiKhoan)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(item => item.LanDangNhapCuoi, DateTime.UtcNow),
                cancellationToken);

        // Tạo response chứa access token và thông tin định danh.
        return AuthResult.Success(CreateResponse(account, role.TenVaiTro));
    }

    private async Task<VaiTro> FindOrCreateCustomerRoleAsync(CancellationToken cancellationToken)
    {
        // Ưu tiên sử dụng vai trò đã tồn tại trong database.
        var role = await _dbContext.VaiTros
            .SingleOrDefaultAsync(item => item.TenVaiTro == CustomerRoleName, cancellationToken);

        if (role is not null)
        {
            return role;
        }

        // Tạo vai trò mặc định nếu database chưa có dữ liệu vai trò.
        role = new VaiTro
        {
            TenVaiTro = CustomerRoleName,
            MoTa = "Tài khoản khách hàng"
        };

        _dbContext.VaiTros.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return role;
    }

    private AuthResponse CreateResponse(TaiKhoan account, string roleName)
    {
        // Đọc cấu hình dùng để ký và xác thực JWT.
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.TryParse(jwtSettings["ExpirationMinutes"], out var configuredExpiration)
            ? configuredExpiration
            : 60;

        // Không phát hành token nếu cấu hình bảo mật chưa hợp lệ.
        if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32 ||
            string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("Cấu hình JWT chưa đầy đủ hoặc SecretKey quá ngắn.");
        }

        // Tạo thời điểm hết hạn và các claim cần thiết cho phân quyền.
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.MaTaiKhoan.ToString()),
            new(ClaimTypes.Name, account.TenDangNhap),
            new(ClaimTypes.Role, roleName),
            new("accountId", account.MaTaiKhoan.ToString())
        };

        // Thêm mã khách hàng hoặc nhân viên tùy loại tài khoản.
        if (account.MaKhachHang.HasValue)
        {
            claims.Add(new Claim("customerId", account.MaKhachHang.Value.ToString()));
        }

        if (account.MaNhanVien.HasValue)
        {
            claims.Add(new Claim("employeeId", account.MaNhanVien.Value.ToString()));
        }

        // Ký token bằng khóa bí mật và thuật toán HMAC-SHA256.
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        // Đóng gói token và thông tin tài khoản trả về cho client.
        return new AuthResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            AccountId = account.MaTaiKhoan,
            UserName = account.TenDangNhap,
            Role = roleName,
            CustomerId = account.MaKhachHang,
            EmployeeId = account.MaNhanVien
        };
    }
}

internal static class PasswordHasher
{
    // Tham số PBKDF2 dùng để tạo hash mật khẩu.
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static string Hash(string password)
    {
        // Tạo salt ngẫu nhiên riêng cho từng mật khẩu.
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // Lưu thuật toán, số vòng lặp, salt và hash trong một chuỗi.
        return $"pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string encodedHash)
    {
        // Tách và kiểm tra định dạng hash đã lưu.
        var parts = encodedHash.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" ||
            !int.TryParse(parts[1], out var iterations))
        {
            return false;
        }

        try
        {
            // Tạo lại hash từ mật khẩu nhập vào để so sánh.
            var salt = Convert.FromBase64String(parts[2]);
            var expectedHash = Convert.FromBase64String(parts[3]);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            // So sánh an toàn để hạn chế timing attack.
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
