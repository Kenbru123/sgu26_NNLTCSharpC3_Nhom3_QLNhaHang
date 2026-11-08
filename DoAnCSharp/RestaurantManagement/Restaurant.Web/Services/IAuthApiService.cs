using Restaurant.Application.DTOs.Auth;

namespace Restaurant.Web.Services;

public interface IAuthApiService
{
    Task<AuthApiResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
