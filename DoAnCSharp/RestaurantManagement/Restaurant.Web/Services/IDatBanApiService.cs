using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services;

public interface IDatBanApiService
{
    Task<DatBanResponse> CreateAsync(
        CreateDatBanRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DatBanResponse>> GetMyReservationsAsync(
        CancellationToken cancellationToken);
}
