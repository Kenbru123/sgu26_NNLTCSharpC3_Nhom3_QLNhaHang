using Restaurant.Application.DTOs.Auth;

namespace Restaurant.Web.Services;

public interface IAuthApiService
{
    // Gọi API đăng nhập và nhận JWT.
    Task<AuthApiResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    // Gọi API đăng ký tài khoản khách hàng.
    Task<AuthApiResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);
}
