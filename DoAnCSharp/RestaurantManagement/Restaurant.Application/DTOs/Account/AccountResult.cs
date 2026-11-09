namespace Restaurant.Application.DTOs.Account;

public sealed class AccountResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public EmployeeAccountResponse? Account { get; init; }

    public static AccountResult Failed(string error) => new()
    {
        Error = error
    };

    public static AccountResult Success(EmployeeAccountResponse account) => new()
    {
        Succeeded = true,
        Account = account
    };
}
