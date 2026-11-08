using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Security;

namespace Restaurant.API.Commands;

public static class AdminBootstrapCommand
{
    public static async Task RunAsync()
    {
        var builder = WebApplication.CreateBuilder();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string 'DefaultConnection'. Configure it with user-secrets or the ConnectionStrings__DefaultConnection environment variable.");
        }

        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        await using var context = new RestaurantDbContext(options);
        await using var transaction = await context.Database.BeginTransactionAsync();

        if (await context.TaiKhoans.AnyAsync())
        {
            throw new InvalidOperationException(
                "Admin bootstrap is only allowed before the first account exists.");
        }

        Console.Write("Tên đăng nhập quản lý: ");
        var username = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(username) || username.Length > 80)
        {
            throw new InvalidOperationException("Tên đăng nhập phải có từ 1 đến 80 ký tự.");
        }

        Console.Write("Họ tên nhân viên quản lý: ");
        var fullName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 120)
        {
            throw new InvalidOperationException("Họ tên phải có từ 1 đến 120 ký tự.");
        }

        var password = ReadPassword("Mật khẩu (ít nhất 12 ký tự): ");
        if (password.Length < 12 || password.Length > 200)
        {
            throw new InvalidOperationException("Mật khẩu phải có từ 12 đến 200 ký tự.");
        }

        var managerRole = await FindOrCreateRoleAsync(
            context,
            "QuanLy",
            "Quản lý hệ thống");
        await FindOrCreateRoleAsync(context, "NhanVien", "Nhân viên nhà hàng");

        var employee = new NhanVien { HoTen = fullName };
        context.NhanViens.Add(employee);
        await context.SaveChangesAsync();

        context.TaiKhoans.Add(new TaiKhoan
        {
            MaVaiTro = managerRole.MaVaiTro,
            MaNhanVien = employee.MaNhanVien,
            TenDangNhap = username,
            MatKhauHash = PasswordHashing.Hash(password),
            TrangThai = true
        });
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        Console.WriteLine("Đã tạo tài khoản Quản lý ban đầu. Hãy lưu thông tin đăng nhập ở nơi an toàn.");
    }

    private static async Task<VaiTro> FindOrCreateRoleAsync(
        RestaurantDbContext context,
        string roleName,
        string description)
    {
        var role = await context.VaiTros.FirstOrDefaultAsync(
            vaiTro => vaiTro.TenVaiTro == roleName);
        if (role is not null)
        {
            return role;
        }

        role = new VaiTro { TenVaiTro = roleName, MoTa = description };
        context.VaiTros.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    private static string ReadPassword(string prompt)
    {
        Console.Write(prompt);
        var password = new System.Text.StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return password.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Length--;
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
            }
        }
    }
}
