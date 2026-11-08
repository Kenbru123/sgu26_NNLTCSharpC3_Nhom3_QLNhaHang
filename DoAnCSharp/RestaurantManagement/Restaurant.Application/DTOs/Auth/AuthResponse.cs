namespace Restaurant.Application.DTOs.Auth;

public sealed class AuthResponse
{
    // JWT dùng cho các request cần xác thực.
    public string AccessToken { get; init; } = string.Empty;
    // Thời điểm JWT hết hạn theo UTC.
    public DateTime ExpiresAtUtc { get; init; }
    // Mã tài khoản trong bảng TaiKhoan.
    public int AccountId { get; init; }
    // Tên đăng nhập.
    public string UserName { get; init; } = string.Empty;
    // Tên vai trò dùng cho phân quyền.
    public string Role { get; init; } = string.Empty;
    // Mã khách hàng nếu tài khoản thuộc khách hàng.
    public int? CustomerId { get; init; }
    // Mã nhân viên nếu tài khoản thuộc nhân viên.
    public int? EmployeeId { get; init; }
}
