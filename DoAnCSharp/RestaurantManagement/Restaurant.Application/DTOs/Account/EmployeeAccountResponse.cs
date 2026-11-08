namespace Restaurant.Application.DTOs.Account;

public sealed class EmployeeAccountResponse
{
    public int AccountId { get; init; }
    public int EmployeeId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public DateOnly? BirthDate { get; init; }
    public int RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime? LastLoginUtc { get; init; }
}
