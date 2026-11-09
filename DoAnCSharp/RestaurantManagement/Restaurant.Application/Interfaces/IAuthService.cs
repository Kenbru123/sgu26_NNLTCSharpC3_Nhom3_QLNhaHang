
using System.Threading;
using System.Threading.Tasks;

// DTO đăng ký và đăng nhập từ Branch của Đại
using AuthResult = Restaurant.Application.DTOs.Auth.AuthResult;
using RegisterRequest = Restaurant.Application.DTOs.Auth.RegisterRequest;
using CustomerLoginRequest = Restaurant.Application.DTOs.Auth.LoginRequest;

// DTO xác thực nhân viên từ Branch main
using AuthenticatedUserDto = Restaurant.Application.DTOs.AuthenticatedUserDto;

namespace Restaurant.Application.Interfaces;

public interface IAuthService
{
    // 1. Đăng ký tài khoản khách hàng
    Task<AuthResult> RegisterCustomerAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    // 2. Đăng nhập theo luồng xác thực của Đại
    Task<AuthResult> LoginAsync(
        CustomerLoginRequest request,
        CancellationToken cancellationToken = default);

    // 3. Xác thực nhân viên cho Desktop
    Task<AuthenticatedUserDto?> AuthenticateAsync(
        string tenDangNhap,
        string matKhau,
        CancellationToken cancellationToken = default);
}
