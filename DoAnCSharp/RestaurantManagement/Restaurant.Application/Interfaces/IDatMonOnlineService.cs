using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IOnlineOrderService
{
    Task<IReadOnlyList<OnlineOrderResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse?> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);
}
