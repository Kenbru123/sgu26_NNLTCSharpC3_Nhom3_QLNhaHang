namespace Restaurant.Application.DTOs;

/// <summary>Identity and role details for an authenticated employee account.</summary>
public sealed record AuthenticatedUserDto(
    int MaTaiKhoan,
    string TenDangNhap,
    string VaiTro);
