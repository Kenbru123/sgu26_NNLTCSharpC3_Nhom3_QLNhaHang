namespace Restaurant.Application.DTOs.Account;

public sealed class UpdateRoleRequest
{
    // Tên vai trò mới của tài khoản nhân viên.
    public string RoleName { get; set; } = string.Empty;
}
