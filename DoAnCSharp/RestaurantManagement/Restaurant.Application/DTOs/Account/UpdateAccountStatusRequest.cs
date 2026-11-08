namespace Restaurant.Application.DTOs.Account;

public sealed class UpdateAccountStatusRequest
{
    // true: mở khóa, false: khóa tài khoản.
    public bool IsActive { get; set; }
}
