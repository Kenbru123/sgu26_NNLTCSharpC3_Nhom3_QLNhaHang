using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services;

public interface ICustomerAccountApiService
{
    Task<CustomerProfileResponse> GetProfileAsync(CancellationToken cancellationToken);

    Task<CustomerProfileResponse> UpdateProfileAsync(
        UpdateCustomerProfileRequest request,
        CancellationToken cancellationToken);

    Task ChangePasswordAsync(
        ChangeCustomerPasswordRequest request,
        CancellationToken cancellationToken);
}
