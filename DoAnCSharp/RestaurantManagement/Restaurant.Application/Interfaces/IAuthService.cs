using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IAuthService
{
    Task<AuthenticatedUserDto?> AuthenticateAsync(
        string tenDangNhap,
        string matKhau,
        CancellationToken cancellationToken);
}
