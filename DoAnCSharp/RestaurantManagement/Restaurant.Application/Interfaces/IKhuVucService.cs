using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IKhuVucService
{
    Task<IReadOnlyList<KhuVucDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<KhuVucDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<KhuVucDto> CreateAsync(
        CreateKhuVucRequest request,
        CancellationToken cancellationToken);

    Task<KhuVucDto?> UpdateAsync(
        int id,
        UpdateKhuVucRequest request,
        CancellationToken cancellationToken);

    Task<KhuVucDeleteResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}

public enum KhuVucDeleteResult
{
    Deleted,
    NotFound,
    HasTables
}
