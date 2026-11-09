using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IGoiMonService
{
    Task<IReadOnlyList<GoiMonBanDto>> GetTablesAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<GoiMonDanhMucDto>> GetCategoriesAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<GoiMonMonAnDto>> SearchFoodsAsync(
        string? search,
        int? maDanhMuc,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<GoiMonPhieuDto>> GetOrdersAsync(
        CancellationToken cancellationToken);

    Task<GoiMonPhieuChiTietDto?> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuDto> CreateOrderAsync(
        int accountId,
        TaoPhieuGoiMonRequest request,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuDto?> UpdateOrderAsync(
        int id,
        CapNhatPhieuGoiMonRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteOrderAsync(
        int id,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuDto?> UpdateOrderStatusAsync(
        int id,
        CapNhatTrangThaiPhieuRequest request,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuDto?> PayOrderAsync(
        int id,
        int accountId,
        ThanhToanPhieuRequest request,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuChiTietDto> AddFoodAsync(
        int id,
        ThemMonVaoPhieuRequest request,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuChiTietDto?> UpdateFoodQuantityAsync(
        int id,
        int detailId,
        CapNhatSoLuongMonRequest request,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuChiTietDto?> RemoveFoodAsync(
        int id,
        int detailId,
        CancellationToken cancellationToken);

    Task<GoiMonPhieuChiTietDto?> UpdateFoodStatusAsync(
        int id,
        int detailId,
        CapNhatTrangThaiMonRequest request,
        CancellationToken cancellationToken);
}
