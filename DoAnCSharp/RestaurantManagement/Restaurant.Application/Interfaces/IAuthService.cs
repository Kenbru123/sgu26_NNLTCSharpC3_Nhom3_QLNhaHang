using Restaurant.Application.DTOs.Auth;

namespace Restaurant.Application.Interfaces;

public interface IAuthService
{
    // Đăng ký tài khoản khách hàng và tạo JWT.
    Task<AuthResult> RegisterCustomerAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    // Xác thực tài khoản khách hàng hoặc nhân viên và tạo JWT.
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
