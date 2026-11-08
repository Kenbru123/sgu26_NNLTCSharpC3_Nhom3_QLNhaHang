namespace Restaurant.Application.DTOs.Auth;

public sealed class LoginRequest
{
    // Thông tin dùng để tìm tài khoản.
    public string UserName { get; set; } = string.Empty;
    // Mật khẩu người dùng nhập vào để xác thực.
    public string Password { get; set; } = string.Empty;
}
