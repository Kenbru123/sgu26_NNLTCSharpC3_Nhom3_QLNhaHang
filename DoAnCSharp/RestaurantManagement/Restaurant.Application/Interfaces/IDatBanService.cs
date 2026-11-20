using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IDatBanService
{
    Task<DatBanResponse> CreateAsync(
        int customerId,
        CreateDatBanRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DatBanResponse>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DatBanManagementResponse>> GetAllAsync(
        string? status,
        CancellationToken cancellationToken);

    Task<DatBanManagementResponse?> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken);
}
