
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Security;

// Phân biệt các DTO của hai nhánh
using AuthResult = Restaurant.Application.DTOs.Auth.AuthResult;
using AuthResponse = Restaurant.Application.DTOs.Auth.AuthResponse;
using RegisterRequest = Restaurant.Application.DTOs.Auth.RegisterRequest;
using CustomerLoginRequest = Restaurant.Application.DTOs.Auth.LoginRequest;
using AuthenticatedUserDto = Restaurant.Application.DTOs.AuthenticatedUserDto;

namespace Restaurant.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private const string CustomerRoleName = "KhachHang";

    // Giữ hash giả của nhánh main để xử lý trường hợp
    // tên đăng nhập nhân viên không tồn tại.
    private const string DummyPasswordHash =
        "PBKDF2-SHA256$310000$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

    private readonly RestaurantDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(
        RestaurantDbContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    // 1. Đăng ký tài khoản khách hàng
    public async Task<AuthResult> RegisterCustomerAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var userName = request.UserName.Trim();
        var password = request.Password;
        var fullName = request.FullName.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return AuthResult.Failed(
                "Tên đăng nhập và mật khẩu là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(phoneNumber))
        {
            return AuthResult.Failed(
                "Họ tên và số điện thoại là bắt buộc.");
        }

        if (password.Length < 6)
        {
            return AuthResult.Failed(
                "Mật khẩu phải có ít nhất 6 ký tự.");
        }

        var exists = await _dbContext.TaiKhoans.AnyAsync(
            account => account.TenDangNhap == userName,
            cancellationToken);

        if (exists)
        {
            return AuthResult.Failed(
                "Tên đăng nhập đã tồn tại.");
        }

        var customerRole =
            await FindOrCreateCustomerRoleAsync(cancellationToken);

        var customer = new KhachHang
        {
            HoTen = fullName,
            SDT = phoneNumber,
            Email = string.IsNullOrWhiteSpace(request.Email)
                ? null
                : request.Email.Trim()
        };

        var account = new TaiKhoan
        {
            MaVaiTro = customerRole.MaVaiTro,
            TenDangNhap = userName,
            MatKhauHash = PasswordHasher.Hash(password),
            TrangThai = true
        };

        // Lưu khách hàng và tài khoản trong cùng transaction
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        _dbContext.KhachHangs.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        account.MaKhachHang = customer.MaKhachHang;

        _dbContext.TaiKhoans.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return AuthResult.Success(
            CreateResponse(account, customerRole.TenVaiTro));
    }

    // 2. Đăng nhập khách hàng Website
    public async Task<AuthResult> LoginAsync(
        CustomerLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var userName = request.UserName.Trim();

        var account = await _dbContext.TaiKhoans
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.TenDangNhap == userName &&
                        item.MaKhachHang != null,
                cancellationToken);

        if (account is null ||
            !account.TrangThai ||
            !VerifyStoredPassword(
                request.Password,
                account.MatKhauHash))
        {
            return AuthResult.Failed(
                "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        var role = await _dbContext.VaiTros
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.MaVaiTro == account.MaVaiTro,
                cancellationToken);

        if (role is null ||
            role.TenVaiTro != CustomerRoleName)
        {
            return AuthResult.Failed(
                "Tài khoản không có quyền đăng nhập khách hàng.");
        }

        await UpdateLastLoginAsync(
            account.MaTaiKhoan,
            cancellationToken);

        return AuthResult.Success(
            CreateResponse(account, role.TenVaiTro));
    }

    // 3. Xác thực nhân viên Desktop - giữ từ main
    public async Task<AuthenticatedUserDto?> AuthenticateAsync(
        string tenDangNhap,
        string matKhau,
        CancellationToken cancellationToken = default)
    {
        var account = await (
            from taiKhoan in _dbContext.TaiKhoans

            join vaiTro in _dbContext.VaiTros
                on taiKhoan.MaVaiTro equals vaiTro.MaVaiTro

            join nhanVien in _dbContext.NhanViens
                on taiKhoan.MaNhanVien
                equals (int?)nhanVien.MaNhanVien

            where taiKhoan.TenDangNhap == tenDangNhap

            select new
            {
                taiKhoan.MaTaiKhoan,
                taiKhoan.TenDangNhap,
                taiKhoan.MatKhauHash,
                taiKhoan.TrangThai,
                nhanVien.TrangThaiLamViec,
                vaiTro.TenVaiTro
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        var passwordHash =
            account?.MatKhauHash ?? DummyPasswordHash;

        var passwordIsValid =
            VerifyStoredPassword(matKhau, passwordHash);

        if (account is null ||
            !passwordIsValid ||
            !account.TrangThai ||
            !account.TrangThaiLamViec ||
            account.TenVaiTro is not ("Admin" or "QuanLy" or "NhanVien"))
        {
            return null;
        }

        await UpdateLastLoginAsync(
            account.MaTaiKhoan,
            cancellationToken);

        return new AuthenticatedUserDto(
            account.MaTaiKhoan,
            account.TenDangNhap,
            account.TenVaiTro);
    }

    // Hỗ trợ hai định dạng mật khẩu hiện có
    private static bool VerifyStoredPassword(
        string password,
        string storedHash)
    {
        if (storedHash.StartsWith(
            "pbkdf2-sha256$",
            StringComparison.Ordinal))
        {
            // Hash từ nhánh feature/auth-dashboard
            return PasswordHasher.Verify(password, storedHash);
        }

        // Hash từ nhánh main (Admin/nhân viên)
        return PasswordHashing.Verify(password, storedHash);
    }

    private async Task UpdateLastLoginAsync(
        int accountId,
        CancellationToken cancellationToken)
    {
        await _dbContext.TaiKhoans
            .Where(item => item.MaTaiKhoan == accountId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    item => item.LanDangNhapCuoi,
                    DateTime.UtcNow),
                cancellationToken);
    }

    private async Task<VaiTro> FindOrCreateCustomerRoleAsync(
        CancellationToken cancellationToken)
    {
        var role = await _dbContext.VaiTros
            .SingleOrDefaultAsync(
                item => item.TenVaiTro == CustomerRoleName,
                cancellationToken);

        if (role is not null)
        {
            return role;
        }

        role = new VaiTro
        {
            TenVaiTro = CustomerRoleName,
            MoTa = "Tài khoản khách hàng"
        };

        _dbContext.VaiTros.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return role;
    }

    // Tạo JWT cho khách hàng
    private AuthResponse CreateResponse(
        TaiKhoan account,
        string roleName)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        // Ưu tiên tên SigningKey dùng bởi nhánh main
        var secretKey =
            jwtSettings["SigningKey"] ??
            jwtSettings["SecretKey"];

        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var expirationMinutes =
            int.TryParse(
                jwtSettings["ExpirationMinutes"],
                out var configuredExpiration) &&
            configuredExpiration > 0
                ? configuredExpiration
                : 60;

        if (string.IsNullOrWhiteSpace(secretKey) ||
            Encoding.UTF8.GetByteCount(secretKey) < 32 ||
            string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException(
                "Cấu hình JWT chưa đầy đủ hoặc khóa ký quá ngắn.");
        }

        var expiresAtUtc =
            DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                account.MaTaiKhoan.ToString()),

            new(
                ClaimTypes.NameIdentifier,
                account.MaTaiKhoan.ToString()),

            new(
                ClaimTypes.Name,
                account.TenDangNhap),

            new(
                ClaimTypes.Role,
                roleName),

            new(
                "accountId",
                account.MaTaiKhoan.ToString())
        };

        if (account.MaKhachHang.HasValue)
        {
            claims.Add(new Claim(
                "customerId",
                account.MaKhachHang.Value.ToString()));
        }

        if (account.MaNhanVien.HasValue)
        {
            claims.Add(new Claim(
                "employeeId",
                account.MaNhanVien.Value.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthResponse
        {
            AccessToken =
                new JwtSecurityTokenHandler().WriteToken(token),

            ExpiresAtUtc = expiresAtUtc,
            AccountId = account.MaTaiKhoan,
            UserName = account.TenDangNhap,
            Role = roleName,
            CustomerId = account.MaKhachHang,
            EmployeeId = account.MaNhanVien
        };
    }
}

// Giữ bộ băm mật khẩu của nhánh Đại để tương thích
// với những tài khoản đã đăng ký bằng phiên bản cũ.
internal static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static string Hash(string password)
    {
        var salt =
            RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return
            $"pbkdf2-sha256${Iterations}$" +
            $"{Convert.ToBase64String(salt)}$" +
            $"{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(
        string password,
        string encodedHash)
    {
        var parts = encodedHash.Split('$');

        if (parts.Length != 4 ||
            parts[0] != "pbkdf2-sha256" ||
            !int.TryParse(parts[1], out var iterations) ||
            iterations <= 0)
        {
            return false;
        }

        try
        {
            var salt =
                Convert.FromBase64String(parts[2]);

            var expectedHash =
                Convert.FromBase64String(parts[3]);

            if (salt.Length != SaltSize ||
                expectedHash.Length != HashSize)
            {
                return false;
            }

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(
                actualHash,
                expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
