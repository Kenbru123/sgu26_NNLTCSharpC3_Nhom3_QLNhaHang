using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Services;

namespace Restaurant.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAdminAsync(
        RestaurantDbContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        // Chỉ chạy seed khi được bật trong cấu hình môi trường.
        var seedSettings = configuration.GetSection("AdminSeed");
        if (!bool.TryParse(seedSettings["Enabled"], out var enabled) || !enabled)
        {
            return;
        }

        var userName = seedSettings["UserName"]?.Trim();
        var password = seedSettings["Password"];
        var fullName = seedSettings["FullName"]?.Trim();
        var phoneNumber = seedSettings["PhoneNumber"]?.Trim();

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException(
                "Cấu hình AdminSeed cần có UserName, Password và FullName.");
        }

        if (password.Length < 6)
        {
            throw new InvalidOperationException(
                "AdminSeed:Password phải có ít nhất 6 ký tự.");
        }

        // Chỉ reset mật khẩu tài khoản hiện có khi được bật rõ ràng bằng User Secret.
        var existingAccount = await dbContext.TaiKhoans
            .SingleOrDefaultAsync(
                account => account.TenDangNhap == userName,
                cancellationToken);

        if (existingAccount is not null)
        {
            var shouldResetPassword = bool.TryParse(
                seedSettings["ResetPassword"],
                out var resetPassword) && resetPassword;

            if (shouldResetPassword)
            {
                existingAccount.MatKhauHash = PasswordHasher.Hash(password);
                existingAccount.TrangThai = true;
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        // Lấy hoặc tạo role Admin.
        var adminRole = await dbContext.VaiTros
            .SingleOrDefaultAsync(role => role.TenVaiTro == "Admin", cancellationToken);

        if (adminRole is null)
        {
            adminRole = new VaiTro
            {
                TenVaiTro = "Admin",
                MoTa = "Quản trị hệ thống"
            };

            dbContext.VaiTros.Add(adminRole);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Tạo hồ sơ nhân viên quản trị.
        var employee = new NhanVien
        {
            HoTen = fullName,
            SDT = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
            TrangThaiLamViec = true
        };

        dbContext.NhanViens.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Tạo tài khoản Admin với mật khẩu đã hash.
        var account = new TaiKhoan
        {
            MaVaiTro = adminRole.MaVaiTro,
            MaNhanVien = employee.MaNhanVien,
            TenDangNhap = userName,
            MatKhauHash = PasswordHasher.Hash(password),
            TrangThai = true
        };

        dbContext.TaiKhoans.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
