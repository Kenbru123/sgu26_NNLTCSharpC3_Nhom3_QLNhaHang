using Restaurant.Application.DTOs.Account;

namespace Restaurant.Application.Interfaces;

public interface IAccountService
{
    Task<IReadOnlyList<EmployeeAccountResponse>> GetEmployeeAccountsAsync(
        CancellationToken cancellationToken = default);

    Task<AccountResult> CreateEmployeeAccountAsync(
        CreateEmployeeAccountRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountResult> UpdateRoleAsync(
        int accountId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountResult> UpdateStatusAsync(
        int accountId,
        UpdateAccountStatusRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountResult> ChangePasswordAsync(
        int accountId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default);
}
