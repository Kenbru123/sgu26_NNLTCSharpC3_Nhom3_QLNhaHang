using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services;

public interface IOnlineOrderApiService
{
    Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        CancellationToken cancellationToken);
}
