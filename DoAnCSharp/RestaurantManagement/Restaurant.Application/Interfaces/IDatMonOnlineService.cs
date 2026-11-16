using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IOnlineOrderService
{
    Task<IReadOnlyList<OnlineOrderResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse?> UpdateDeliveryStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OnlineOrderResponse>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse?> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        int? customerId,
        CancellationToken cancellationToken);

    Task<OnlineOrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OnlineOrderResponse>> LookupAsync(
        string phoneNumber,
        CancellationToken cancellationToken);
}
