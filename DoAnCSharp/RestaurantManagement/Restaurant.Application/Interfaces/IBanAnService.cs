using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IBanAnService
{
    Task<IReadOnlyList<BanAnDto>> GetAllAsync(
        int? maKhuVuc,
        bool? trangThaiHoatDong,
        string? search,
        CancellationToken cancellationToken);

    Task<BanAnDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<BanAnDto> CreateAsync(
        CreateBanAnRequest request,
        CancellationToken cancellationToken);

    Task<BanAnDto?> UpdateAsync(
        int id,
        UpdateBanAnRequest request,
        CancellationToken cancellationToken);

    Task<BanAnDto?> UpdateStatusAsync(
        int id,
        bool trangThaiHoatDong,
        CancellationToken cancellationToken);

    Task<BanAnDeleteResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}

public enum BanAnDeleteResult
{
    Deleted,
    NotFound,
    InUse
}
