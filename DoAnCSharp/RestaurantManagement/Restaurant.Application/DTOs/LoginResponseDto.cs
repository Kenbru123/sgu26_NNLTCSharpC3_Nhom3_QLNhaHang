namespace Restaurant.Application.DTOs;

/// <summary>Bearer token and employee identity returned after a successful login.</summary>
public sealed record LoginResponseDto(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string TenDangNhap,
    string VaiTro);
