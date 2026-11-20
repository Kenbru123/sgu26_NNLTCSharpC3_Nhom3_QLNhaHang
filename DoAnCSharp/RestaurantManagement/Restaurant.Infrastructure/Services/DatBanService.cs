using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class DatBanService : IDatBanService
{
    private static readonly string[] ActiveStatuses =
    ["ChoXacNhan", "DaXacNhan", "DaDen"];

    private readonly RestaurantDbContext _context;

    public DatBanService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<DatBanResponse> CreateAsync(
        int customerId,
        CreateDatBanRequest request,
        CancellationToken cancellationToken)
    {
        var start = request.ThoiGianDen;
        var end = start.AddHours(2);
        var now = DateTime.Now;

        if (start <= now.AddMinutes(15))
        {
            throw new ArgumentException("Thời gian đến phải cách hiện tại ít nhất 15 phút.");
        }

        if (request.SoLuongKhach <= 0)
        {
            throw new ArgumentException("Số lượng khách phải lớn hơn 0.");
        }

        if (!await _context.KhachHangs.AnyAsync(
                customer => customer.MaKhachHang == customerId,
                cancellationToken))
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin khách hàng.");
        }

        var reservedTableIds = await _context.DatBans
            .Where(reservation =>
                reservation.MaBan.HasValue &&
                ActiveStatuses.Contains(reservation.TrangThai) &&
                reservation.ThoiGianDen < end &&
                reservation.ThoiGianKetThucDuKien > start)
            .Select(reservation => reservation.MaBan!.Value)
            .ToListAsync(cancellationToken);

        var table = await _context.BanAns
            .AsNoTracking()
            .Where(table =>
                table.TrangThaiHoatDong &&
                table.SoCho >= request.SoLuongKhach &&
                !reservedTableIds.Contains(table.MaBan))
            .OrderBy(table => table.SoCho)
            .ThenBy(table => table.MaBan)
            .FirstOrDefaultAsync(cancellationToken);

        if (table is null)
        {
            throw new InvalidOperationException(
                "Không còn bàn phù hợp trong thời gian đã chọn. Vui lòng chọn thời gian khác.");
        }

        var reservation = new DatBan
        {
            MaKhachHang = customerId,
            MaBan = table.MaBan,
            ThoiGianTao = now,
            ThoiGianDen = start,
            ThoiGianKetThucDuKien = end,
            SoLuongKhach = request.SoLuongKhach,
            TrangThai = "ChoXacNhan",
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu)
                ? null
                : request.GhiChu.Trim()
        };

        _context.DatBans.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return new DatBanResponse(
            reservation.MaDatBan,
            table.TenBan,
            null,
            reservation.ThoiGianTao,
            reservation.ThoiGianDen,
            reservation.ThoiGianKetThucDuKien,
            reservation.SoLuongKhach,
            reservation.TrangThai,
            reservation.GhiChu);
    }

    public async Task<IReadOnlyList<DatBanResponse>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken)
    {
        return await (
            from reservation in _context.DatBans.AsNoTracking()
            join table in _context.BanAns on reservation.MaBan equals table.MaBan into tables
            from table in tables.DefaultIfEmpty()
            join area in _context.KhuVucs on table.MaKhuVuc equals area.MaKhuVuc into areas
            from area in areas.DefaultIfEmpty()
            where reservation.MaKhachHang == customerId
            orderby reservation.ThoiGianDen descending
            select new DatBanResponse(
                reservation.MaDatBan,
                table == null ? null : table.TenBan,
                area == null ? null : area.TenKhuVuc,
                reservation.ThoiGianTao,
                reservation.ThoiGianDen,
                reservation.ThoiGianKetThucDuKien,
                reservation.SoLuongKhach,
                reservation.TrangThai,
                reservation.GhiChu))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DatBanManagementResponse>> GetAllAsync(
        string? status,
        CancellationToken cancellationToken)
    {
        var query =
            from reservation in _context.DatBans.AsNoTracking()
            join customer in _context.KhachHangs
                on reservation.MaKhachHang equals customer.MaKhachHang
            join table in _context.BanAns
                on reservation.MaBan equals table.MaBan into tables
            from table in tables.DefaultIfEmpty()
            join area in _context.KhuVucs
                on table.MaKhuVuc equals area.MaKhuVuc into areas
            from area in areas.DefaultIfEmpty()
            select new { reservation, customer, table, area };

        if (!string.IsNullOrWhiteSpace(status) && status != "TatCa")
        {
            query = query.Where(item => item.reservation.TrangThai == status);
        }

        return await query
            .OrderBy(item => item.reservation.ThoiGianDen)
            .Select(item => new DatBanManagementResponse(
                item.reservation.MaDatBan,
                item.customer.HoTen,
                item.customer.SDT,
                item.table == null ? null : item.table.TenBan,
                item.area == null ? null : item.area.TenKhuVuc,
                item.reservation.ThoiGianTao,
                item.reservation.ThoiGianDen,
                item.reservation.ThoiGianKetThucDuKien,
                item.reservation.SoLuongKhach,
                item.reservation.TrangThai,
                item.reservation.GhiChu))
            .ToListAsync(cancellationToken);
    }

    public async Task<DatBanManagementResponse?> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken)
    {
        var reservation = await _context.DatBans.FirstOrDefaultAsync(
            item => item.MaDatBan == id,
            cancellationToken);
        if (reservation is null)
        {
            return null;
        }

        if (!IsValidTransition(reservation.TrangThai, status))
        {
            throw new InvalidOperationException(
                $"Không thể chuyển trạng thái từ {reservation.TrangThai} sang {status}.");
        }

        reservation.TrangThai = status;
        await _context.SaveChangesAsync(cancellationToken);

        return await (
            from item in _context.DatBans.AsNoTracking()
            join customer in _context.KhachHangs
                on item.MaKhachHang equals customer.MaKhachHang
            join table in _context.BanAns
                on item.MaBan equals table.MaBan into tables
            from table in tables.DefaultIfEmpty()
            join area in _context.KhuVucs
                on table.MaKhuVuc equals area.MaKhuVuc into areas
            from area in areas.DefaultIfEmpty()
            where item.MaDatBan == id
            select new DatBanManagementResponse(
                item.MaDatBan,
                customer.HoTen,
                customer.SDT,
                table == null ? null : table.TenBan,
                area == null ? null : area.TenKhuVuc,
                item.ThoiGianTao,
                item.ThoiGianDen,
                item.ThoiGianKetThucDuKien,
                item.SoLuongKhach,
                item.TrangThai,
                item.GhiChu))
            .SingleAsync(cancellationToken);
    }

    private static bool IsValidTransition(string current, string next)
    {
        return current switch
        {
            "ChoXacNhan" => next is "DaXacNhan" or "DaHuy",
            "DaXacNhan" => next is "DaDen" or "DaHuy",
            "DaDen" => next == "HoanTat",
            _ => false
        };
    }
}
