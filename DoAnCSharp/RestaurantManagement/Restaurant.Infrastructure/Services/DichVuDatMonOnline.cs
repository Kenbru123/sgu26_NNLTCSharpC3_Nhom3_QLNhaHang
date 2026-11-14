using System.Data;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class OnlineOrderService : IOnlineOrderService
{
    private readonly RestaurantDbContext _context;

    public OnlineOrderService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenNguoiNhan) ||
            string.IsNullOrWhiteSpace(request.SDTNguoiNhan) ||
            request.MonAn is null ||
            request.MonAn.Count == 0)
        {
            throw new ArgumentException(
                "Vui lòng nhập thông tin người nhận và ít nhất một món.");
        }

        if (request.LoaiDon is not "OnlineGiaoHang" and not "OnlineNhanTaiQuay")
        {
            throw new ArgumentException("Cách nhận món không hợp lệ.");
        }

        if (request.LoaiDon == "OnlineGiaoHang" &&
            string.IsNullOrWhiteSpace(request.DiaChiGiao))
        {
            throw new ArgumentException("Vui lòng nhập địa chỉ giao hàng.");
        }

        if (request.MonAn.Any(item =>
                item is null ||
                item.MaMon <= 0 ||
                item.SoLuong is < 1 or > 99))
        {
            throw new ArgumentException("Danh sách món hoặc số lượng không hợp lệ.");
        }

        if (request.MonAn.GroupBy(item => item.MaMon)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Mỗi món chỉ được xuất hiện một lần trong đơn.");
        }

        var requestedIds = request.MonAn.Select(item => item.MaMon).ToArray();
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var foods = await _context.MonAns
            .AsNoTracking()
            .Where(food =>
                requestedIds.Contains(food.MaMon) &&
                food.TrangThai &&
                food.DanhMuc.TrangThai)
            .Select(food => new
            {
                food.MaMon,
                food.TenMon,
                food.DonGia
            })
            .ToListAsync(cancellationToken);

        if (foods.Count != requestedIds.Length)
        {
            throw new KeyNotFoundException(
                "Một hoặc nhiều món không tồn tại hoặc đã ngừng bán.");
        }

        var foodById = foods.ToDictionary(food => food.MaMon);
        var order = new DonHang
        {
            LoaiDon = request.LoaiDon,
            TenNguoiNhan = request.TenNguoiNhan.Trim(),
            SDTNguoiNhan = request.SDTNguoiNhan.Trim(),
            DiaChiGiao = request.LoaiDon == "OnlineGiaoHang"
                ? request.DiaChiGiao?.Trim()
                : null,
            PhiGiao = 0m,
            TrangThaiDon = "ChoXacNhan",
            ThoiGianTao = DateTime.Now,
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu)
                ? null
                : request.GhiChu.Trim()
        };

        _context.DonHangs.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        var responseItems = new List<OnlineOrderItemResponse>(request.MonAn.Count);
        foreach (var requestedItem in request.MonAn)
        {
            var food = foodById[requestedItem.MaMon];
            responseItems.Add(new OnlineOrderItemResponse(
                food.MaMon,
                food.TenMon,
                requestedItem.SoLuong,
                food.DonGia,
                requestedItem.SoLuong * food.DonGia));
            _context.ChiTietDonHangs.Add(new ChiTietDonHang
            {
                MaDonHang = order.MaDonHang,
                MaMon = food.MaMon,
                SoLuong = requestedItem.SoLuong,
                DonGia = food.DonGia
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        var response = new OnlineOrderResponse(
            order.MaDonHang,
            order.LoaiDon,
            order.TrangThaiDon,
            order.TenNguoiNhan ?? string.Empty,
            order.SDTNguoiNhan ?? string.Empty,
            order.DiaChiGiao,
            order.PhiGiao,
            new DateTimeOffset(
                DateTime.SpecifyKind(order.ThoiGianTao, DateTimeKind.Local)),
            responseItems.Sum(item => item.ThanhTien) + order.PhiGiao,
            responseItems);
        await transaction.CommitAsync(cancellationToken);

        return response;
    }

    public async Task<OnlineOrderResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _context.DonHangs
            .AsNoTracking()
            .Where(item => item.MaDonHang == id &&
                           item.LoaiDon != "TaiBan")
            .Select(item => new
            {
                item.MaDonHang,
                item.LoaiDon,
                item.TrangThaiDon,
                item.TenNguoiNhan,
                item.SDTNguoiNhan,
                item.DiaChiGiao,
                item.PhiGiao,
                item.ThoiGianTao
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (order is null)
        {
            return null;
        }

        var items = await _context.ChiTietDonHangs
            .AsNoTracking()
            .Where(item => item.MaDonHang == id)
            .Join(
                _context.MonAns.AsNoTracking(),
                detail => detail.MaMon,
                food => food.MaMon,
                (detail, food) => new OnlineOrderItemResponse(
                    food.MaMon,
                    food.TenMon,
                    detail.SoLuong,
                    detail.DonGia,
                    detail.SoLuong * detail.DonGia))
            .ToListAsync(cancellationToken);

        var createdAt = new DateTimeOffset(
            DateTime.SpecifyKind(order.ThoiGianTao, DateTimeKind.Local));
        return new OnlineOrderResponse(
            order.MaDonHang,
            order.LoaiDon,
            order.TrangThaiDon,
            order.TenNguoiNhan ?? string.Empty,
            order.SDTNguoiNhan ?? string.Empty,
            order.DiaChiGiao,
            order.PhiGiao,
            createdAt,
            items.Sum(item => item.ThanhTien) + order.PhiGiao,
            items);
    }
}
