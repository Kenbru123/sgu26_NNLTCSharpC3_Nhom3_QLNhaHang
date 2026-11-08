using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Authentication;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>Authenticates an employee and issues a short-lived JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequest request,
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
            new Claim(JwtRegisteredClaimNames.Sub, user.MaTaiKhoan.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan.ToString()),
            new Claim(ClaimTypes.Name, user.TenDangNhap),
            new Claim(ClaimTypes.Role, user.VaiTro)
        };
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256));

        return Ok(new LoginResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            user.TenDangNhap,
            user.VaiTro));
    }
}
