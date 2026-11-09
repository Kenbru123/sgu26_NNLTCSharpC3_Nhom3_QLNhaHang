namespace Restaurant.Application.DTOs.Account;

public sealed class CreateEmployeeAccountRequest
{
    // Thông tin đăng nhập của nhân viên.
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Thông tin hồ sơ nhân viên.
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateOnly? BirthDate { get; set; }

    // Vai trò được cấp cho tài khoản.
    public string RoleName { get; set; } = string.Empty;
}
