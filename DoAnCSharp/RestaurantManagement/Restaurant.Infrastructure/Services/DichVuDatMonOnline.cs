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

    public async Task<IReadOnlyList<OnlineOrderResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var orders = await _context.DonHangs
            .AsNoTracking()
            .Where(item => item.LoaiDon == "OnlineGiaoHang" || item.LoaiDon == "OnlineNhanTaiQuay")
            .OrderByDescending(item => item.ThoiGianTao)
            .Select(item => new
            {
                item.MaDonHang,
                item.LoaiDon,
                item.TrangThaiDon,
                item.TrangThaiGiao,
                item.TenNguoiNhan,
                item.SDTNguoiNhan,
                item.DiaChiGiao,
                item.PhiGiao,
                item.ThoiGianTao
            })
            .ToListAsync(cancellationToken);

        var orderIds = orders.Select(item => item.MaDonHang).ToArray();
        var items = await _context.ChiTietDonHangs
            .AsNoTracking()
            .Where(item => orderIds.Contains(item.MaDonHang))
            .Join(_context.MonAns.AsNoTracking(), detail => detail.MaMon, food => food.MaMon,
                (detail, food) => new { detail.MaDonHang, food.MaMon, food.TenMon, detail.SoLuong, detail.DonGia })
            .ToListAsync(cancellationToken);

        return orders.Select(order =>
        {
            var responseItems = items
                .Where(item => item.MaDonHang == order.MaDonHang)
                .Select(item => new OnlineOrderItemResponse(
                    item.MaMon, item.TenMon, item.SoLuong, item.DonGia, item.SoLuong * item.DonGia))
                .ToList();
            return new OnlineOrderResponse(
                order.MaDonHang,
                order.LoaiDon,
                order.TrangThaiDon,
                order.TrangThaiGiao,
                order.TenNguoiNhan ?? string.Empty,
                order.SDTNguoiNhan ?? string.Empty,
                order.DiaChiGiao,
                order.PhiGiao,
                new DateTimeOffset(DateTime.SpecifyKind(order.ThoiGianTao, DateTimeKind.Local)),
                responseItems.Sum(item => item.ThanhTien) + order.PhiGiao,
                responseItems);
        }).ToList();
    }

    public async Task<OnlineOrderResponse?> UpdateDeliveryStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken)
    {
        if (status is not ("ChoLayHang" or "DangGiao" or "DaGiao" or "DaNhan" or "HoanTat"))
        {
            throw new ArgumentException("Trạng thái giao hàng không hợp lệ.");
        }

        var order = await _context.DonHangs.SingleOrDefaultAsync(
            item => item.MaDonHang == id && item.LoaiDon == "OnlineGiaoHang",
            cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.TrangThaiGiao = status;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<OnlineOrderResponse>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken)
    {
        var orders = await _context.DonHangs
            .AsNoTracking()
            .Where(item => item.MaKhachHang == customerId &&
                           (item.LoaiDon == "OnlineGiaoHang" || item.LoaiDon == "OnlineNhanTaiQuay"))
            .OrderByDescending(item => item.ThoiGianTao)
            .Select(item => item.MaDonHang)
            .ToListAsync(cancellationToken);

        var result = new List<OnlineOrderResponse>(orders.Count);
        foreach (var orderId in orders)
        {
            var order = await GetByIdAsync(orderId, cancellationToken);
            if (order is not null)
            {
                result.Add(order);
            }
        }

        return result;
    }

    public async Task<OnlineOrderResponse?> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken)
    {
        if (status is not ("DangPhucVu" or "DaHuy"))
        {
            throw new ArgumentException("Trạng thái đơn online phải là Đang phục vụ hoặc Đã hủy.");
        }

        var order = await _context.DonHangs.SingleOrDefaultAsync(
            item => item.MaDonHang == id &&
                    (item.LoaiDon == "OnlineGiaoHang" || item.LoaiDon == "OnlineNhanTaiQuay"),
            cancellationToken);
        if (order is null)
        {
            return null;
        }

        if (order.TrangThaiDon == status)
        {
            return await GetByIdAsync(id, cancellationToken);
        }

        if (order.TrangThaiDon != "ChoXacNhan")
        {
            throw new InvalidOperationException("Chỉ đơn đang chờ xác nhận mới được xác nhận hoặc hủy.");
        }

        order.TrangThaiDon = status;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        int? customerId,
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
            MaKhachHang = customerId,
            TenNguoiNhan = request.TenNguoiNhan.Trim(),
            SDTNguoiNhan = request.SDTNguoiNhan.Trim(),
            DiaChiGiao = request.LoaiDon == "OnlineGiaoHang"
                ? request.DiaChiGiao?.Trim()
                : null,
            PhiGiao = 0m,
            TrangThaiDon = "ChoXacNhan",
            TrangThaiGiao = request.LoaiDon == "OnlineGiaoHang" ? "ChoLayHang" : null,
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
            order.TrangThaiGiao,
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
                item.TrangThaiGiao,
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
            order.TrangThaiGiao,
            order.TenNguoiNhan ?? string.Empty,
            order.SDTNguoiNhan ?? string.Empty,
            order.DiaChiGiao,
            order.PhiGiao,
            createdAt,
            items.Sum(item => item.ThanhTien) + order.PhiGiao,
            items);
    }
}
