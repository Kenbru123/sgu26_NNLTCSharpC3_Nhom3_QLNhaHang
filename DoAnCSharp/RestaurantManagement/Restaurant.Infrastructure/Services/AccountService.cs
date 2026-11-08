using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs.Account;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class AccountService : IAccountService
{
    private readonly RestaurantDbContext _dbContext;

    public AccountService(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EmployeeAccountResponse>> GetEmployeeAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        // Chỉ lấy tài khoản có liên kết với nhân viên, không lấy tài khoản khách hàng.
        return await (
            from account in _dbContext.TaiKhoans.AsNoTracking()
            join employee in _dbContext.NhanViens.AsNoTracking()
                on account.MaNhanVien equals employee.MaNhanVien
            join role in _dbContext.VaiTros.AsNoTracking()
                on account.MaVaiTro equals role.MaVaiTro
            orderby account.MaTaiKhoan
            select new EmployeeAccountResponse
            {
                AccountId = account.MaTaiKhoan,
                EmployeeId = employee.MaNhanVien,
                UserName = account.TenDangNhap,
                FullName = employee.HoTen,
                PhoneNumber = employee.SDT,
                BirthDate = employee.NgaySinh,
                RoleId = role.MaVaiTro,
                RoleName = role.TenVaiTro,
                IsActive = account.TrangThai,
                LastLoginUtc = account.LanDangNhapCuoi
            }).ToListAsync(cancellationToken);
    }

    public async Task<AccountResult> CreateEmployeeAccountAsync(
        CreateEmployeeAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        // Chuẩn hóa dữ liệu trước khi kiểm tra.
        var userName = request.UserName.Trim();
        var fullName = request.FullName.Trim();
        var roleName = request.RoleName.Trim();

        // Kiểm tra các trường bắt buộc.
        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(roleName))
        {
            return AccountResult.Failed("Tên đăng nhập, mật khẩu, họ tên và vai trò là bắt buộc.");
        }

        if (request.Password.Length < 6)
        {
            return AccountResult.Failed("Mật khẩu phải có ít nhất 6 ký tự.");
        }

        // Username dùng chung cho cả tài khoản khách hàng và nhân viên.
        if (await _dbContext.TaiKhoans.AnyAsync(
                account => account.TenDangNhap == userName,
                cancellationToken))
        {
            return AccountResult.Failed("Tên đăng nhập đã tồn tại.");
        }

        // Vai trò phải được tồn tại hoặc được tạo bởi thao tác quản trị.
        var role = await FindOrCreateRoleAsync(roleName, cancellationToken);
        var employee = new NhanVien
        {
            HoTen = fullName,
            SDT = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : request.PhoneNumber.Trim(),
            NgaySinh = request.BirthDate,
            TrangThaiLamViec = true
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Lưu hồ sơ nhân viên trước để lấy mã nhân viên.
        _dbContext.NhanViens.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Tạo tài khoản nhân viên và lưu mật khẩu dưới dạng hash.
        var account = new TaiKhoan
        {
            MaVaiTro = role.MaVaiTro,
            MaNhanVien = employee.MaNhanVien,
            TenDangNhap = userName,
            MatKhauHash = PasswordHasher.Hash(request.Password),
            TrangThai = true
        };

        _dbContext.TaiKhoans.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return AccountResult.Success(MapToResponse(account, employee, role));
    }

    public async Task<AccountResult> UpdateRoleAsync(
        int accountId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        // Chỉ cho phép cập nhật vai trò của tài khoản nhân viên.
        var account = await GetEmployeeAccountAsync(accountId, cancellationToken);
        if (account is null)
        {
            return AccountResult.Failed("Không tìm thấy tài khoản nhân viên.");
        }

        var accountData = account.Value;
        var roleName = request.RoleName.Trim();
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return AccountResult.Failed("Tên vai trò là bắt buộc.");
        }

        // Tìm hoặc tạo vai trò mới rồi cập nhật tài khoản.
        var role = await FindOrCreateRoleAsync(roleName, cancellationToken);
        accountData.Account.MaVaiTro = role.MaVaiTro;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return AccountResult.Success(MapToResponse(accountData.Account, accountData.Employee, role));
    }

    public async Task<AccountResult> UpdateStatusAsync(
        int accountId,
        UpdateAccountStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        // Không cho phép thay đổi trạng thái tài khoản khách hàng qua API này.
        var account = await GetEmployeeAccountAsync(accountId, cancellationToken);
        if (account is null)
        {
            return AccountResult.Failed("Không tìm thấy tài khoản nhân viên.");
        }

        var accountData = account.Value;
        accountData.Account.TrangThai = request.IsActive;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var role = await _dbContext.VaiTros
            .AsNoTracking()
            .SingleAsync(item => item.MaVaiTro == accountData.Account.MaVaiTro, cancellationToken);

        return AccountResult.Success(MapToResponse(accountData.Account, accountData.Employee, role));
    }

    public async Task<AccountResult> ChangePasswordAsync(
        int accountId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        // Kiểm tra tài khoản mục tiêu có phải tài khoản nhân viên hay không.
        var account = await GetEmployeeAccountAsync(accountId, cancellationToken);
        if (account is null)
        {
            return AccountResult.Failed("Không tìm thấy tài khoản nhân viên.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            return AccountResult.Failed("Mật khẩu mới phải có ít nhất 6 ký tự.");
        }

        // Hash lại mật khẩu, không lưu mật khẩu dạng rõ.
        var accountData = account.Value;
        accountData.Account.MatKhauHash = PasswordHasher.Hash(request.NewPassword);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var role = await _dbContext.VaiTros
            .AsNoTracking()
            .SingleAsync(item => item.MaVaiTro == accountData.Account.MaVaiTro, cancellationToken);

        return AccountResult.Success(MapToResponse(accountData.Account, accountData.Employee, role));
    }

    private async Task<(TaiKhoan Account, NhanVien Employee)?> GetEmployeeAccountAsync(
        int accountId,
        CancellationToken cancellationToken)
    {
        // Tải tài khoản cùng nhân viên để thực hiện cập nhật.
        var result = await (
            from account in _dbContext.TaiKhoans
            join employee in _dbContext.NhanViens
                on account.MaNhanVien equals employee.MaNhanVien
            where account.MaTaiKhoan == accountId
            select new { Account = account, Employee = employee })
            .SingleOrDefaultAsync(cancellationToken);

        return result is null ? null : (result.Account, result.Employee);
    }

    private async Task<VaiTro> FindOrCreateRoleAsync(
        string roleName,
        CancellationToken cancellationToken)
    {
        // Sử dụng vai trò đã tồn tại nếu tìm thấy.
        var role = await _dbContext.VaiTros
            .SingleOrDefaultAsync(item => item.TenVaiTro == roleName, cancellationToken);

        if (role is not null)
        {
            return role;
        }

        // Tạo vai trò mới theo yêu cầu của quản trị viên.
        role = new VaiTro
        {
            TenVaiTro = roleName,
            MoTa = $"Vai trò {roleName}"
        };

        _dbContext.VaiTros.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return role;
    }

    private static EmployeeAccountResponse MapToResponse(
        TaiKhoan account,
        NhanVien employee,
        VaiTro role)
    {
        // Chuyển entity sang response, không trả về MatKhauHash.
        return new EmployeeAccountResponse
        {
            AccountId = account.MaTaiKhoan,
            EmployeeId = employee.MaNhanVien,
            UserName = account.TenDangNhap,
            FullName = employee.HoTen,
            PhoneNumber = employee.SDT,
            BirthDate = employee.NgaySinh,
            RoleId = role.MaVaiTro,
            RoleName = role.TenVaiTro,
            IsActive = account.TrangThai,
            LastLoginUtc = account.LanDangNhapCuoi
        };
    }
}
