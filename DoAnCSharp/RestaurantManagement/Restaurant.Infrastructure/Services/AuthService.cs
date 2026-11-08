using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Security;

namespace Restaurant.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private const string DummyPasswordHash =
        "PBKDF2-SHA256$310000$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

    private readonly RestaurantDbContext _context;

    public AuthService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<AuthenticatedUserDto?> AuthenticateAsync(
        string tenDangNhap,
        string matKhau,
        CancellationToken cancellationToken)
    {
        var account = await (
                from taiKhoan in _context.TaiKhoans
                join vaiTro in _context.VaiTros on taiKhoan.MaVaiTro equals vaiTro.MaVaiTro
                join nhanVien in _context.NhanViens
                    on taiKhoan.MaNhanVien equals (int?)nhanVien.MaNhanVien
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

        var passwordHash = account?.MatKhauHash ?? DummyPasswordHash;
        var passwordIsValid = PasswordHashing.Verify(matKhau, passwordHash);
        if (account is null ||
            !passwordIsValid ||
            !account.TrangThai ||
            !account.TrangThaiLamViec ||
            account.TenVaiTro is not ("QuanLy" or "NhanVien"))
        {
            return null;
        }

        await _context.TaiKhoans
            .Where(taiKhoan => taiKhoan.MaTaiKhoan == account.MaTaiKhoan)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    taiKhoan => taiKhoan.LanDangNhapCuoi,
                    DateTime.Now),
                cancellationToken);

        return new AuthenticatedUserDto(
            account.MaTaiKhoan,
            account.TenDangNhap,
            account.TenVaiTro);
    }
}
