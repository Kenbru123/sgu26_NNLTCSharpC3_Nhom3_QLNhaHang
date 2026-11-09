namespace Restaurant.Application.DTOs.Account;

public sealed class ChangePasswordRequest
{
    // Mật khẩu mới sẽ được hash trước khi lưu.
    public string NewPassword { get; set; } = string.Empty;
}
