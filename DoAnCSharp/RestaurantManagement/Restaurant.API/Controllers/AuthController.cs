using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs.Auth;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    // Service xử lý toàn bộ nghiệp vụ đăng ký và đăng nhập.
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        // Đăng ký tài khoản khách hàng mới.
        var result = await _authService.RegisterCustomerAsync(request, cancellationToken);

        // Tên đăng nhập trùng hoặc dữ liệu đăng ký không hợp lệ.
        if (!result.Succeeded)
        {
            return Conflict(new { message = result.Error });
        }

        // Đăng ký thành công và trả về thông tin tài khoản cùng JWT.
        return StatusCode(StatusCodes.Status201Created, result.Response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        // Kiểm tra tài khoản và cấp JWT cho người dùng.
        var result = await _authService.LoginAsync(request, cancellationToken);

        // Không tìm thấy tài khoản, sai mật khẩu hoặc tài khoản bị khóa.
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = result.Error });
        }

        // Đăng nhập thành công.
        return Ok(result.Response);
    }
}
