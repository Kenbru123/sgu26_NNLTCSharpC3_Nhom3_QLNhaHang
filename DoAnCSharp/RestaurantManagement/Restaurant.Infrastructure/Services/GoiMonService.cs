using System.Data;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class GoiMonService : IGoiMonService
{
    private readonly RestaurantDbContext _context;

    public GoiMonService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GoiMonBanDto>> GetTablesAsync(
        CancellationToken cancellationToken)
    {
        var tables = await _context.BanAns
            .AsNoTracking()
            .Where(table => table.TrangThaiHoatDong)
            .OrderBy(table => table.KhuVuc.TenKhuVuc)
            .ThenBy(table => table.TenBan)
            .Select(table => new
            {
                table.MaBan,
                TenKhuVuc = table.KhuVuc.TenKhuVuc,
                table.TenBan
            })
            .ToListAsync(cancellationToken);

        var openOrders = await _context.DonHangs
            .AsNoTracking()
            .Where(order =>
                order.LoaiDon == "TaiBan" &&
                order.TrangThaiDon != "HoanThanh" &&
                order.TrangThaiDon != "DaThanhToan" &&
                order.TrangThaiDon != "DaHuy")
            .OrderByDescending(order => order.ThoiGianTao)
            .Select(order => new
            {
                order.MaDonHang,
                order.MaBan,
                order.TrangThaiDon
            })
            .ToListAsync(cancellationToken);

        var orderByTable = openOrders
            .Where(order => order.MaBan.HasValue)
            .GroupBy(order => order.MaBan!.Value)
            .ToDictionary(group => group.Key, group => group.First());

        return tables.Select(table =>
        {
            orderByTable.TryGetValue(table.MaBan, out var order);
            return new GoiMonBanDto(
                table.MaBan,
                table.TenKhuVuc,
                table.TenBan,
                order?.MaDonHang,
                order?.TrangThaiDon);
        }).ToList();
    }

    public async Task<IReadOnlyList<GoiMonDanhMucDto>> GetCategoriesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.DanhMucMons
            .AsNoTracking()
            .Where(category =>
                category.TrangThai &&
                category.MonAns.Any(food => food.TrangThai))
            .OrderBy(category => category.TenDanhMuc)
            .Select(category => new GoiMonDanhMucDto(
                category.MaDanhMuc,
                category.TenDanhMuc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GoiMonMonAnDto>> SearchFoodsAsync(
        string? search,
        int? maDanhMuc,
        CancellationToken cancellationToken)
    {
        var foods = _context.MonAns
            .AsNoTracking()
            .Where(food =>
                food.TrangThai &&
                food.DanhMuc.TrangThai);

        if (maDanhMuc.HasValue)
        {
            foods = foods.Where(food => food.MaDanhMuc == maDanhMuc.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            foods = foods.Where(food =>
                food.TenMon.Contains(keyword) ||
                food.DanhMuc.TenDanhMuc.Contains(keyword));
        }

        return await foods
            .OrderBy(food => food.DanhMuc.TenDanhMuc)
            .ThenBy(food => food.TenMon)
            .Select(food => new GoiMonMonAnDto(
                food.MaMon,
                food.TenMon,
                food.DanhMuc.TenDanhMuc,
                food.DonGia))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GoiMonPhieuDto>> GetOrdersAsync(
        CancellationToken cancellationToken)
    {
        return await _context.DonHangs
            .AsNoTracking()
            .Where(order => order.LoaiDon == "TaiBan")
            .OrderByDescending(order => order.ThoiGianTao)
            .Select(order => new GoiMonPhieuDto(
                order.MaDonHang,
                order.MaBan ?? 0,
                _context.BanAns
                    .Where(table => table.MaBan == order.MaBan)
                    .Select(table => table.TenBan)
                    .FirstOrDefault() ?? "(Bàn đã xóa)",
                order.ThoiGianTao,
                order.TrangThaiDon,
                _context.ChiTietDonHangs
                    .Where(detail => detail.MaDonHang == order.MaDonHang)
                    .Sum(detail => (int?)detail.SoLuong) ?? 0,
                _context.ChiTietDonHangs
                    .Where(detail => detail.MaDonHang == order.MaDonHang)
                    .Sum(detail => (decimal?)(detail.SoLuong * detail.DonGia)) ?? 0m))
            .ToListAsync(cancellationToken);
    }

    public async Task<GoiMonPhieuChiTietDto?> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _context.DonHangs
            .AsNoTracking()
            .Where(order =>
                order.MaDonHang == id &&
                order.LoaiDon == "TaiBan")
            .Select(order => new
            {
                order.MaDonHang,
                MaBan = order.MaBan ?? 0,
                order.ThoiGianTao,
                order.TrangThaiDon,
                order.GhiChu
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return null;
        }

        var tableName = await _context.BanAns
            .AsNoTracking()
            .Where(table => table.MaBan == order.MaBan)
            .Select(table => table.TenBan)
            .SingleOrDefaultAsync(cancellationToken) ?? "(Bàn đã xóa)";

        var details = await (
            from detail in _context.ChiTietDonHangs.AsNoTracking()
            join food in _context.MonAns.AsNoTracking()
                on detail.MaMon equals food.MaMon
            where detail.MaDonHang == id
            orderby detail.MaChiTietDonHang
            select new GoiMonChiTietDto(
                    detail.MaChiTietDonHang,
                    detail.MaMon,
                    food.TenMon,
                    detail.SoLuong,
                    detail.DonGia,
                    detail.SoLuong * detail.DonGia,
                    detail.GhiChuMon,
                    detail.TrangThaiCheBien,
                    detail.ThoiGianBatDau,
                    detail.ThoiGianHoanThanh))
            .ToListAsync(cancellationToken);

        return new GoiMonPhieuChiTietDto(
            BuildOrderSummary(
                order.MaDonHang,
                order.MaBan,
                tableName,
                order.ThoiGianTao,
                order.TrangThaiDon,
                details),
            details,
            order.GhiChu);
    }

    public async Task<GoiMonPhieuDto> CreateOrderAsync(
        int accountId,
        TaoPhieuGoiMonRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MaBan <= 0)
        {
            throw new ArgumentException("Vui lòng chọn bàn ăn.");
        }

        if (request.GhiChu?.Length > 500)
        {
            throw new ArgumentException("Ghi chú không được vượt quá 500 ký tự.");
        }

        var employeeId = await _context.TaiKhoans
            .Where(account => account.MaTaiKhoan == accountId)
            .Select(account => account.MaNhanVien)
            .SingleOrDefaultAsync(cancellationToken);

        if (!employeeId.HasValue)
        {
            throw new GoiMonConflictException(
                "Tài khoản chưa được liên kết với hồ sơ nhân viên.");
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        var table = await _context.BanAns
            .SingleOrDefaultAsync(
                candidate => candidate.MaBan == request.MaBan,
                cancellationToken);

        if (table is null || !table.TrangThaiHoatDong)
        {
            throw new GoiMonNotFoundException(
                "Bàn không tồn tại hoặc đã ngừng hoạt động.");
        }

        var hasOpenOrder = await _context.DonHangs
            .AnyAsync(order =>
                order.MaBan == request.MaBan &&
                order.LoaiDon == "TaiBan" &&
                order.TrangThaiDon != "HoanThanh" &&
                order.TrangThaiDon != "DaHuy",
                cancellationToken);

        if (hasOpenOrder)
        {
            throw new GoiMonConflictException(
                "Bàn đã có phiếu gọi món đang xử lý. Hãy mở phiếu hiện có.");
        }

        var order = new Domain.Entities.DonHang
        {
            MaBan = request.MaBan,
            MaNhanVienPhucVu = employeeId.Value,
            LoaiDon = "TaiBan",
            TrangThaiDon = "DangPhucVu",
            ThoiGianTao = DateTime.UtcNow,
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu)
                ? null
                : request.GhiChu.Trim()
        };

        _context.DonHangs.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return BuildOrderSummary(
            order.MaDonHang,
            request.MaBan,
            table.TenBan,
            order.ThoiGianTao,
            order.TrangThaiDon,
            []);
    }

    public async Task<GoiMonPhieuDto?> UpdateOrderAsync(
        int id,
        CapNhatPhieuGoiMonRequest request,
        CancellationToken cancellationToken)
    {
        if (request.GhiChu?.Length > 500)
        {
            throw new ArgumentException("Ghi chú không được vượt quá 500 ký tự.");
        }

        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                order => order.MaDonHang == id && order.LoaiDon == "TaiBan",
                cancellationToken);

        if (order is null)
        {
            return null;
        }

        EnsureOrderOpen(order);
        order.GhiChu = string.IsNullOrWhiteSpace(request.GhiChu)
            ? null
            : request.GhiChu.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetOrderByIdAsync(id, cancellationToken)
            ?? throw new GoiMonNotFoundException(
                "Phiếu gọi món không còn tồn tại.")).Phieu;
    }

    public async Task<bool> DeleteOrderAsync(
        int id,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaDonHang == id &&
                    candidate.LoaiDon == "TaiBan",
                cancellationToken);

        if (order is null)
        {
            return false;
        }

        EnsureOrderOpen(order);

        var hasInvoice = await _context.HoaDons
            .AnyAsync(invoice => invoice.MaDonHang == id, cancellationToken);
        if (hasInvoice)
        {
            throw new GoiMonConflictException(
                "Phiếu đã có hóa đơn, không thể xóa.");
        }

        var details = await _context.ChiTietDonHangs
            .Where(detail => detail.MaDonHang == id)
            .ToListAsync(cancellationToken);

        if (details.Any(detail => detail.TrangThaiCheBien != "ChoCheBien"))
        {
            throw new GoiMonConflictException(
                "Phiếu có món đã chuyển chế biến, không thể xóa.");
        }

        _context.ChiTietDonHangs.RemoveRange(details);
        _context.DonHangs.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<GoiMonPhieuDto?> UpdateOrderStatusAsync(
        int id,
        CapNhatTrangThaiPhieuRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TrangThaiDon != "DangPhucVu")
        {
            throw new ArgumentException(
                "Trạng thái phiếu hợp lệ là Đang phục vụ hoặc Đã thanh toán.");
        }

        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaDonHang == id &&
                    candidate.LoaiDon == "TaiBan",
                cancellationToken);

        if (order is null)
        {
            return null;
        }

        EnsureOrderOpen(order);
        order.TrangThaiDon = "DangPhucVu";
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetOrderByIdAsync(id, cancellationToken)
            ?? throw new GoiMonNotFoundException(
                "Phiếu gọi món không còn tồn tại.")).Phieu;
    }

    public async Task<GoiMonPhieuDto?> PayOrderAsync(
        int id,
        int accountId,
        ThanhToanPhieuRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PhuongThucThanhToan))
        {
            throw new ArgumentException(
                "Vui lòng chọn phương thức thanh toán.");
        }

        var paymentMethod = request.PhuongThucThanhToan.Trim();
        if (paymentMethod is not ("TienMat" or "ChuyenKhoan" or "The"))
        {
            throw new ArgumentException(
                "Phương thức thanh toán không hợp lệ.");
        }

        if (request.TyLeVAT is < 0 or > 100 ||
            decimal.Round(request.TyLeVAT, 2) != request.TyLeVAT)
        {
            throw new ArgumentException(
                "VAT phải nằm trong khoảng 0-100% và có tối đa 2 chữ số thập phân.");
        }

        var employeeId = await _context.TaiKhoans
            .Where(account => account.MaTaiKhoan == accountId)
            .Select(account => account.MaNhanVien)
            .SingleOrDefaultAsync(cancellationToken);

        if (!employeeId.HasValue)
        {
            throw new GoiMonConflictException(
                "Tài khoản chưa được liên kết với hồ sơ nhân viên.");
        }

        await using var transaction = await _context.Database
            .BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaDonHang == id &&
                    candidate.LoaiDon == "TaiBan",
                cancellationToken);

        if (order is null)
        {
            return null;
        }

        EnsureOrderOpen(order);

        var hasItems = await _context.ChiTietDonHangs
            .AnyAsync(detail => detail.MaDonHang == id, cancellationToken);
        if (!hasItems)
        {
            throw new GoiMonConflictException(
                "Không thể thanh toán phiếu chưa có món.");
        }

        var subtotal = await _context.ChiTietDonHangs
            .Where(detail => detail.MaDonHang == id)
            .SumAsync(
                detail => (decimal?)(detail.SoLuong * detail.DonGia),
                cancellationToken) ?? 0m;

        var now = DateTime.UtcNow;
        var invoice = await _context.HoaDons
            .SingleOrDefaultAsync(
                candidate => candidate.MaDonHang == id,
                cancellationToken);
        if (invoice?.TrangThaiThanhToan == "DaThanhToan")
        {
            throw new GoiMonConflictException(
                "Phiếu này đã được thanh toán.");
        }

        if (invoice?.TrangThaiThanhToan == "DaHuy")
        {
            throw new GoiMonConflictException(
                "Hóa đơn của phiếu đã bị hủy, không thể ghi nhận thanh toán.");
        }

        var vatAmount = decimal.Round(
            subtotal * request.TyLeVAT / 100m,
            2,
            MidpointRounding.AwayFromZero);

        if (invoice is null)
        {
            invoice = new Domain.Entities.HoaDon
            {
                MaDonHang = id,
                MaNhanVienLap = employeeId.Value,
                ThoiGianLap = now
            };
            _context.HoaDons.Add(invoice);
        }

        invoice.MaNhanVienLap = employeeId.Value;
        invoice.ThoiGianThanhToan = now;
        invoice.TienHang = subtotal;
        invoice.PhiGiaoChot = 0m;
        invoice.TyLeVAT = request.TyLeVAT;
        invoice.TienVAT = vatAmount;
        invoice.TongThanhToan = subtotal + vatAmount;
        invoice.PhuongThucThanhToan = paymentMethod;
        invoice.TrangThaiThanhToan = "DaThanhToan";
        order.TrangThaiDon = "DaThanhToan";

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return (await GetOrderByIdAsync(id, cancellationToken)
            ?? throw new GoiMonNotFoundException(
                "Phiếu gọi món không còn tồn tại.")).Phieu;
    }

    public async Task<GoiMonPhieuChiTietDto> AddFoodAsync(
        int id,
        ThemMonVaoPhieuRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MaMon <= 0 || request.SoLuong <= 0)
        {
            throw new ArgumentException("Món ăn và số lượng phải hợp lệ.");
        }

        if (request.GhiChuMon?.Length > 500)
        {
            throw new ArgumentException("Ghi chú món không được vượt quá 500 ký tự.");
        }

        var order = await FindOrderAsync(id, cancellationToken);
        EnsureOrderOpen(order);

        var food = await _context.MonAns
            .AsNoTracking()
            .Where(candidate =>
                candidate.MaMon == request.MaMon &&
                candidate.TrangThai &&
                candidate.DanhMuc.TrangThai)
            .Select(candidate => new
            {
                candidate.MaMon,
                candidate.DonGia
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (food is null)
        {
            throw new GoiMonNotFoundException(
                "Món ăn không tồn tại hoặc đã ngừng kinh doanh.");
        }

        var note = string.IsNullOrWhiteSpace(request.GhiChuMon)
            ? null
            : request.GhiChuMon.Trim();
        var detail = await _context.ChiTietDonHangs
            .Where(candidate =>
                candidate.MaDonHang == id &&
                candidate.MaMon == request.MaMon &&
                candidate.TrangThaiCheBien == "ChoCheBien" &&
                candidate.GhiChuMon == note)
            .OrderBy(candidate => candidate.MaChiTietDonHang)
            .FirstOrDefaultAsync(cancellationToken);

        if (detail is not null)
        {
            if (request.SoLuong > int.MaxValue - detail.SoLuong)
            {
                throw new ArgumentException(
                    "Tổng số lượng món vượt quá giới hạn cho phép.");
            }

            detail.SoLuong = checked(detail.SoLuong + request.SoLuong);
        }
        else
        {
            _context.ChiTietDonHangs.Add(new Domain.Entities.ChiTietDonHang
            {
                MaDonHang = id,
                MaMon = food.MaMon,
                SoLuong = request.SoLuong,
                DonGia = food.DonGia,
                GhiChuMon = note,
                TrangThaiCheBien = "ChoCheBien"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetOrderByIdAsync(id, cancellationToken)
            ?? throw new GoiMonNotFoundException(
                "Phiếu gọi món không còn tồn tại.");
    }

    public async Task<GoiMonPhieuChiTietDto?> UpdateFoodQuantityAsync(
        int id,
        int detailId,
        CapNhatSoLuongMonRequest request,
        CancellationToken cancellationToken)
    {
        if (request.SoLuong <= 0)
        {
            throw new ArgumentException("Số lượng phải lớn hơn 0.");
        }

        var order = await FindOrderAsync(id, cancellationToken);
        EnsureOrderOpen(order);

        var detail = await _context.ChiTietDonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaChiTietDonHang == detailId &&
                    candidate.MaDonHang == id,
                cancellationToken);

        if (detail is null)
        {
            return null;
        }

        EnsureFoodNotBeingPrepared(detail);
        detail.SoLuong = request.SoLuong;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetOrderByIdAsync(id, cancellationToken);
    }

    public async Task<GoiMonPhieuChiTietDto?> RemoveFoodAsync(
        int id,
        int detailId,
        CancellationToken cancellationToken)
    {
        var order = await FindOrderAsync(id, cancellationToken);
        EnsureOrderOpen(order);

        var detail = await _context.ChiTietDonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaChiTietDonHang == detailId &&
                    candidate.MaDonHang == id,
                cancellationToken);

        if (detail is null)
        {
            return null;
        }

        EnsureFoodNotBeingPrepared(detail);
        _context.ChiTietDonHangs.Remove(detail);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetOrderByIdAsync(id, cancellationToken);
    }

    public async Task<GoiMonPhieuChiTietDto?> UpdateFoodStatusAsync(
        int id,
        int detailId,
        CapNhatTrangThaiMonRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TrangThaiCheBien is not ("DangCheBien" or "HoanThanh"))
        {
            throw new ArgumentException(
                "Trạng thái món hợp lệ là Đang chế biến hoặc Đã chế biến.");
        }

        var order = await FindOrderAsync(id, cancellationToken);
        EnsureOrderOpen(order);

        var detail = await _context.ChiTietDonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaChiTietDonHang == detailId &&
                    candidate.MaDonHang == id,
                cancellationToken);

        if (detail is null)
        {
            return null;
        }

        if (request.TrangThaiCheBien == "DangCheBien" &&
            detail.TrangThaiCheBien == "ChoCheBien")
        {
            detail.TrangThaiCheBien = "DangCheBien";
            detail.ThoiGianBatDau = DateTime.UtcNow;
        }
        else if (request.TrangThaiCheBien == "HoanThanh" &&
                 detail.TrangThaiCheBien == "DangCheBien")
        {
            detail.TrangThaiCheBien = "HoanThanh";
            detail.ThoiGianHoanThanh = DateTime.UtcNow;
        }
        else if (detail.TrangThaiCheBien != request.TrangThaiCheBien)
        {
            throw new GoiMonConflictException(
                "Không thể chuyển trạng thái món theo thứ tự này.");
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetOrderByIdAsync(id, cancellationToken);
    }

    private async Task<Domain.Entities.DonHang> FindOrderAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.MaDonHang == id &&
                    candidate.LoaiDon == "TaiBan",
                cancellationToken);

        return order ?? throw new GoiMonNotFoundException(
            "Không tìm thấy phiếu gọi món.");
    }

    private static void EnsureOrderOpen(Domain.Entities.DonHang order)
    {
        if (order.TrangThaiDon is "HoanThanh" or "DaThanhToan" or "DaHuy")
        {
            throw new GoiMonConflictException(
                "Phiếu đã hoàn tất hoặc đã hủy, không thể chỉnh sửa.");
        }
    }

    private static void EnsureFoodNotBeingPrepared(
        Domain.Entities.ChiTietDonHang detail)
    {
        if (detail.TrangThaiCheBien != "ChoCheBien")
        {
            throw new GoiMonConflictException(
                "Món đã chuyển chế biến nên không thể sửa số lượng hoặc xóa.");
        }
    }

    private static GoiMonPhieuDto BuildOrderSummary(
        int orderId,
        int tableId,
        string tableName,
        DateTime createdAt,
        string status,
        IReadOnlyList<GoiMonChiTietDto> details)
    {
        return new GoiMonPhieuDto(
            orderId,
            tableId,
            tableName,
            createdAt,
            status,
            details.Sum(detail => detail.SoLuong),
            details.Sum(detail => detail.ThanhTien));
    }
}
