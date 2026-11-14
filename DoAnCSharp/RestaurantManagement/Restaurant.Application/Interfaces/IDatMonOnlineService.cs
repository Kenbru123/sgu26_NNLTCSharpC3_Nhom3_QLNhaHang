using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IOnlineOrderService
{
    Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);
}
