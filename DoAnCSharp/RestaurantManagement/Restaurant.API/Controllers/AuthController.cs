
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Authentication;
using Restaurant.Application.Interfaces;

// Hai nhánh có LoginRequest ở hai namespace khác nhau.
// Dùng tên thay thế để tránh trùng tên class.
using CustomerRegisterRequest =
    Restaurant.Application.DTOs.Auth.RegisterRequest;

using CustomerLoginRequest =
    Restaurant.Application.DTOs.Auth.LoginRequest;

using EmployeeLoginRequest =
    Restaurant.Application.DTOs.LoginRequest;

using EmployeeLoginResponse =
    Restaurant.Application.DTOs.LoginResponseDto;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        IAuthService authService,
        IOptions<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _jwtOptions = jwtOptions.Value;
    }

    // 1. Khách hàng đăng ký tài khoản
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] CustomerRegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterCustomerAsync(
            request, cancellationToken);

        if (!result.Succeeded)
        {
            return Conflict(new { message = result.Error });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Response);
    }

    // 2. Khách hàng đăng nhập Website
    [HttpPost("customer/login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> CustomerLogin(
        [FromBody] CustomerLoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            request, cancellationToken);

        if (!result.Succeeded)
        {
            return Unauthorized(
                new { message = result.Error });
        }

        return Ok(result.Response);
    }

    // 3. Nhân viên đăng nhập Desktop
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType<EmployeeLoginResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EmployeeLoginResponse>> Login(
        [FromBody] EmployeeLoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _authService.AuthenticateAsync(
            request.TenDangNhap.Trim(),
            request.MatKhau,
            cancellationToken);

        if (user is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Tên đăng nhập hoặc mật khẩu không đúng."
            });
        }

        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.MaTaiKhoan.ToString()),

            new Claim(
                ClaimTypes.NameIdentifier,
                user.MaTaiKhoan.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.TenDangNhap),

            new Claim(
                ClaimTypes.Role,
                user.VaiTro)
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256));

        return Ok(new EmployeeLoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            user.TenDangNhap,
            user.VaiTro));
    }
}
