using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<NguyenLieuDto>> GetIngredientsAsync(CancellationToken cancellationToken);

    Task<NguyenLieuDto?> GetIngredientByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<NguyenLieuDto>> GetLowStockIngredientsAsync(CancellationToken cancellationToken);

    Task<NguyenLieuDto> CreateIngredientAsync(CreateNguyenLieuRequest request, CancellationToken cancellationToken);

    Task<NguyenLieuDto?> UpdateIngredientAsync(int id, UpdateNguyenLieuRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<PhieuNhapKhoDto>> GetPurchaseReceiptsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<PhieuXuatKhoDto>> GetIssueReceiptsAsync(CancellationToken cancellationToken);

    Task<PhieuNhapKhoDto> CreatePurchaseReceiptAsync(CreatePhieuNhapRequest request, CancellationToken cancellationToken);

    Task<PhieuXuatKhoDto> CreateIssueReceiptAsync(CreatePhieuXuatRequest request, CancellationToken cancellationToken);

    Task<PhieuNhapKhoDto?> UpdatePurchaseReceiptStatusAsync(int receiptId, string trangThai, CancellationToken cancellationToken);

    Task<PhieuXuatKhoDto?> UpdateIssueReceiptStatusAsync(int receiptId, string trangThai, CancellationToken cancellationToken);
}
