namespace Restaurant.Application.DTOs.Auth;

public sealed class RegisterRequest
{
    // Tên đăng nhập duy nhất của khách hàng.
    public string UserName { get; set; } = string.Empty;
    // Mật khẩu dạng rõ chỉ tồn tại trong request, service sẽ hash trước khi lưu.
    public string Password { get; set; } = string.Empty;
    // Họ tên khách hàng.
    public string FullName { get; set; } = string.Empty;
    // Số điện thoại khách hàng.
    public string PhoneNumber { get; set; } = string.Empty;
    // Email không bắt buộc.
    public string? Email { get; set; }
}
