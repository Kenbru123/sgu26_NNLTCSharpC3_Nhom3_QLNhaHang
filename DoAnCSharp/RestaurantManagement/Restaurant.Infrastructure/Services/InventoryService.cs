using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly RestaurantDbContext _context;

    public InventoryService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<NguyenLieuDto>> GetIngredientsAsync(
        CancellationToken cancellationToken)
    {
        var ingredients = await _context.NguyenLieus
            .AsNoTracking()
            .OrderBy(item => item.TenNguyenLieu)
            .ToListAsync(cancellationToken);

        var stockById = await GetStockByIngredientAsync(
            ingredients.Select(item => item.MaNguyenLieu),
            cancellationToken);

        return ingredients
            .Select(item => new NguyenLieuDto(
                item.MaNguyenLieu,
                item.TenNguyenLieu,
                item.DonViTinh,
                item.MucTonToiThieu,
                stockById.GetValueOrDefault(item.MaNguyenLieu, 0m),
                item.TrangThai))
            .ToList();
    }

    public async Task<NguyenLieuDto?> GetIngredientByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await _context.NguyenLieus
            .AsNoTracking()
            .SingleOrDefaultAsync(
                candidate => candidate.MaNguyenLieu == id,
                cancellationToken);

        if (item is null)
        {
            return null;
        }

        var stock = await GetAvailableQuantityAsync(
            id,
            cancellationToken);

        return new NguyenLieuDto(
            item.MaNguyenLieu,
            item.TenNguyenLieu,
            item.DonViTinh,
            item.MucTonToiThieu,
            stock,
            item.TrangThai);
    }

    public async Task<IReadOnlyList<NguyenLieuDto>> GetLowStockIngredientsAsync(
        CancellationToken cancellationToken)
    {
        var ingredients = await _context.NguyenLieus
            .AsNoTracking()
            .OrderBy(item => item.TenNguyenLieu)
            .ToListAsync(cancellationToken);

        var stockById = await GetStockByIngredientAsync(
            ingredients.Select(item => item.MaNguyenLieu),
            cancellationToken);

        return ingredients
            .Where(item => stockById.GetValueOrDefault(item.MaNguyenLieu, 0m) <= item.MucTonToiThieu)
            .Select(item => new NguyenLieuDto(
                item.MaNguyenLieu,
                item.TenNguyenLieu,
                item.DonViTinh,
                item.MucTonToiThieu,
                stockById.GetValueOrDefault(item.MaNguyenLieu, 0m),
                item.TrangThai))
            .ToList();
    }

    public async Task<NguyenLieuDto> CreateIngredientAsync(
        CreateNguyenLieuRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenNguyenLieu))
        {
            throw new ArgumentException("Tên nguyên liệu không được rỗng.");
        }

        if (string.IsNullOrWhiteSpace(request.DonViTinh))
        {
            throw new ArgumentException("Đơn vị tính không được rỗng.");
        }

        if (request.MucTonToiThieu < 0)
        {
            throw new ArgumentException("Mức tồn tối thiểu không được âm.");
        }

        var exists = await _context.NguyenLieus
            .AnyAsync(
                item => item.TenNguyenLieu == request.TenNguyenLieu.Trim(),
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Nguyên liệu đã tồn tại trong hệ thống.");
        }

        var ingredient = new NguyenLieu
        {
            TenNguyenLieu = request.TenNguyenLieu.Trim(),
            DonViTinh = request.DonViTinh.Trim(),
            MucTonToiThieu = request.MucTonToiThieu,
            TrangThai = request.TrangThai
        };

        _context.NguyenLieus.Add(ingredient);
        await _context.SaveChangesAsync(cancellationToken);

        return new NguyenLieuDto(
            ingredient.MaNguyenLieu,
            ingredient.TenNguyenLieu,
            ingredient.DonViTinh,
            ingredient.MucTonToiThieu,
            0m,
            ingredient.TrangThai);
    }

    public async Task<NguyenLieuDto?> UpdateIngredientAsync(
        int id,
        UpdateNguyenLieuRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenNguyenLieu))
        {
            throw new ArgumentException("Tên nguyên liệu không được rỗng.");
        }

        if (string.IsNullOrWhiteSpace(request.DonViTinh))
        {
            throw new ArgumentException("Đơn vị tính không được rỗng.");
        }

        if (request.MucTonToiThieu < 0)
        {
            throw new ArgumentException("Mức tồn tối thiểu không được âm.");
        }

        var ingredient = await _context.NguyenLieus
            .SingleOrDefaultAsync(
                item => item.MaNguyenLieu == id,
                cancellationToken);

        if (ingredient is null)
        {
            return null;
        }

        ingredient.TenNguyenLieu = request.TenNguyenLieu.Trim();
        ingredient.DonViTinh = request.DonViTinh.Trim();
        ingredient.MucTonToiThieu = request.MucTonToiThieu;
        ingredient.TrangThai = request.TrangThai;

        await _context.SaveChangesAsync(cancellationToken);

        var stock = await GetAvailableQuantityAsync(id, cancellationToken);

        return new NguyenLieuDto(
            ingredient.MaNguyenLieu,
            ingredient.TenNguyenLieu,
            ingredient.DonViTinh,
            ingredient.MucTonToiThieu,
            stock,
            ingredient.TrangThai);
    }

    public async Task<IReadOnlyList<PhieuNhapKhoDto>> GetPurchaseReceiptsAsync(
        CancellationToken cancellationToken)
    {
        var receipts = await _context.PhieuNhapKhos
            .AsNoTracking()
            .OrderByDescending(item => item.ThoiGianNhap)
            .ToListAsync(cancellationToken);

        var result = new List<PhieuNhapKhoDto>(receipts.Count);
        foreach (var receipt in receipts)
        {
            var items = await _context.ChiTietPhieuNhaps
                .AsNoTracking()
                .Where(detail => detail.MaPhieuNhap == receipt.MaPhieuNhap)
                .Join(
                    _context.NguyenLieus.AsNoTracking(),
                    detail => detail.MaNguyenLieu,
                    ingredient => ingredient.MaNguyenLieu,
                    (detail, ingredient) => new ChiTietPhieuKhoDto(
                        detail.MaChiTietNhap,
                        detail.MaNguyenLieu,
                        ingredient.TenNguyenLieu,
                        ingredient.DonViTinh,
                        detail.SoLuongNhap,
                        detail.DonGiaNhap))
                .ToListAsync(cancellationToken);

            result.Add(new PhieuNhapKhoDto(
                receipt.MaPhieuNhap,
                receipt.ThoiGianNhap,
                receipt.TrangThai,
                receipt.GhiChu,
                items));
        }

        return result;
    }

    public async Task<IReadOnlyList<PhieuXuatKhoDto>> GetIssueReceiptsAsync(
        CancellationToken cancellationToken)
    {
        var receipts = await _context.PhieuXuatKhos
            .AsNoTracking()
            .OrderByDescending(item => item.ThoiGianXuat)
            .ToListAsync(cancellationToken);

        var result = new List<PhieuXuatKhoDto>(receipts.Count);
        foreach (var receipt in receipts)
        {
            var items = await _context.ChiTietPhieuXuats
                .AsNoTracking()
                .Where(detail => detail.MaPhieuXuat == receipt.MaPhieuXuat)
                .Join(
                    _context.NguyenLieus.AsNoTracking(),
                    detail => detail.MaNguyenLieu,
                    ingredient => ingredient.MaNguyenLieu,
                    (detail, ingredient) => new ChiTietPhieuKhoDto(
                        detail.MaChiTietXuat,
                        detail.MaNguyenLieu,
                        ingredient.TenNguyenLieu,
                        ingredient.DonViTinh,
                        detail.SoLuongXuat,
                        0m))
                .ToListAsync(cancellationToken);

            result.Add(new PhieuXuatKhoDto(
                receipt.MaPhieuXuat,
                receipt.ThoiGianXuat,
                receipt.LoaiXuat,
                receipt.TrangThai,
                receipt.LyDo,
                null,
                items));
        }

        return result;
    }

    public async Task<PhieuNhapKhoDto> CreatePurchaseReceiptAsync(
        CreatePhieuNhapRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ChiTiet is null || request.ChiTiet.Count == 0)
        {
            throw new ArgumentException("Phiếu nhập phải có ít nhất một nguyên liệu.");
        }

        foreach (var item in request.ChiTiet)
        {
            if (item.MaNguyenLieu <= 0 || item.SoLuong <= 0 || item.DonGia < 0)
            {
                throw new ArgumentException("Dữ liệu chi tiết nhập kho không hợp lệ.");
            }
        }

        var ingredientIds = request.ChiTiet
            .Select(item => item.MaNguyenLieu)
            .Distinct()
            .ToArray();

        var exists = await _context.NguyenLieus
            .AsNoTracking()
            .Where(item => ingredientIds.Contains(item.MaNguyenLieu))
            .Select(item => item.MaNguyenLieu)
            .ToListAsync(cancellationToken);

        var missing = ingredientIds.Except(exists).ToArray();
        if (missing.Length > 0)
        {
            throw new KeyNotFoundException(
                "Một hoặc nhiều nguyên liệu không tồn tại: " +
                string.Join(", ", missing));
        }

        var receipt = new PhieuNhapKho
        {
            ThoiGianNhap = DateTime.UtcNow,
            MaNhanVien = request.MaNhanVien ?? throw new ArgumentException("Chưa xác định được nhân viên nhập kho."),
            TrangThai = "Nhap",
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu)
                ? null
                : request.GhiChu.Trim()
        };

        _context.PhieuNhapKhos.Add(receipt);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var item in request.ChiTiet)
        {
            _context.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
            {
                MaPhieuNhap = receipt.MaPhieuNhap,
                MaNguyenLieu = item.MaNguyenLieu,
                SoLuongNhap = item.SoLuong,
                DonGiaNhap = item.DonGia
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildReceiptDtoAsync(receipt.MaPhieuNhap, cancellationToken);
    }

    public async Task<PhieuXuatKhoDto> CreateIssueReceiptAsync(
        CreatePhieuXuatRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ChiTiet is null || request.ChiTiet.Count == 0)
        {
            throw new ArgumentException("Phiếu xuất phải có ít nhất một nguyên liệu.");
        }

        if (request.LoaiXuat is not ("CheBien" or "ThuCong"))
        {
            throw new ArgumentException("Loại xuất kho không hợp lệ.");
        }

        if (request.LoaiXuat == "CheBien" && request.MaChiTietDonHang is null)
        {
            throw new ArgumentException("Xuất chế biến phải gắn với chi tiết đơn hàng.");
        }

        if (request.LoaiXuat == "ThuCong" && request.MaNhanVien is null)
        {
            throw new ArgumentException("Xuất thủ công phải có nhân viên thực hiện.");
        }

        if (request.LoaiXuat == "ThuCong" &&
            string.IsNullOrWhiteSpace(request.LyDo))
        {
            throw new ArgumentException("Vui lòng ghi lý do xuất thủ công.");
        }

        var ingredientIds = request.ChiTiet
            .Select(item => item.MaNguyenLieu)
            .Distinct()
            .ToArray();

        var ingredients = await _context.NguyenLieus
            .AsNoTracking()
            .Where(item => ingredientIds.Contains(item.MaNguyenLieu))
            .Select(item => new { item.MaNguyenLieu, item.TenNguyenLieu })
            .ToListAsync(cancellationToken);

        if (ingredients.Count != ingredientIds.Length)
        {
            throw new KeyNotFoundException("Một hoặc nhiều nguyên liệu không tồn tại.");
        }

        foreach (var item in request.ChiTiet)
        {
            if (item.MaNguyenLieu <= 0 || item.SoLuong <= 0)
            {
                throw new ArgumentException("Số lượng xuất kho không hợp lệ.");
            }

            var available = await GetAvailableQuantityAsync(
                item.MaNguyenLieu,
                cancellationToken);
            if (available < item.SoLuong)
            {
                throw new InvalidOperationException(
                    $"Nguyên liệu '{ingredients.First(candidate => candidate.MaNguyenLieu == item.MaNguyenLieu).TenNguyenLieu}' không đủ tồn kho.");
            }
        }

        var receipt = new PhieuXuatKho
        {
            MaNhanVien = request.MaNhanVien,
            MaChiTietDonHang = request.MaChiTietDonHang,
            ThoiGianXuat = DateTime.UtcNow,
            LoaiXuat = request.LoaiXuat,
            TrangThai = "Nhap",
            LyDo = string.IsNullOrWhiteSpace(request.LyDo)
                ? null
                : request.LyDo.Trim()
        };

        _context.PhieuXuatKhos.Add(receipt);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var item in request.ChiTiet)
        {
            _context.ChiTietPhieuXuats.Add(new ChiTietPhieuXuat
            {
                MaPhieuXuat = receipt.MaPhieuXuat,
                MaNguyenLieu = item.MaNguyenLieu,
                SoLuongXuat = item.SoLuong
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await BuildIssueDtoAsync(receipt.MaPhieuXuat, cancellationToken);
    }

    public async Task<PhieuNhapKhoDto?> UpdatePurchaseReceiptStatusAsync(
        int receiptId,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var valid = new[] { "Nhap", "XacNhan", "Huy" };
        if (!valid.Contains(trangThai))
        {
            throw new ArgumentException("Trạng thái phiếu nhập không hợp lệ.");
        }

        var receipt = await _context.PhieuNhapKhos
            .SingleOrDefaultAsync(
                item => item.MaPhieuNhap == receiptId,
                cancellationToken);

        if (receipt is null)
        {
            return null;
        }

        receipt.TrangThai = trangThai;
        await _context.SaveChangesAsync(cancellationToken);
        return await BuildReceiptDtoAsync(receipt.MaPhieuNhap, cancellationToken);
    }

    public async Task<PhieuXuatKhoDto?> UpdateIssueReceiptStatusAsync(
        int receiptId,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var valid = new[] { "Nhap", "XacNhan", "Huy" };
        if (!valid.Contains(trangThai))
        {
            throw new ArgumentException("Trạng thái phiếu xuất không hợp lệ.");
        }

        var receipt = await _context.PhieuXuatKhos
            .SingleOrDefaultAsync(
                item => item.MaPhieuXuat == receiptId,
                cancellationToken);

        if (receipt is null)
        {
            return null;
        }

        receipt.TrangThai = trangThai;
        await _context.SaveChangesAsync(cancellationToken);
        return await BuildIssueDtoAsync(receipt.MaPhieuXuat, cancellationToken);
    }

    private async Task<decimal> GetAvailableQuantityAsync(
        int ingredientId,
        CancellationToken cancellationToken)
    {
        var totalInbound = await _context.ChiTietPhieuNhaps
            .Where(item => item.MaNguyenLieu == ingredientId)
            .Join(
                _context.PhieuNhapKhos
                    .Where(receipt => receipt.TrangThai == "XacNhan"),
                detail => detail.MaPhieuNhap,
                receipt => receipt.MaPhieuNhap,
                (detail, _) => detail.SoLuongNhap)
            .SumAsync(cancellationToken);

        var totalOutbound = await _context.ChiTietPhieuXuats
            .Where(item => item.MaNguyenLieu == ingredientId)
            .Join(
                _context.PhieuXuatKhos
                    .Where(receipt => receipt.TrangThai == "XacNhan"),
                detail => detail.MaPhieuXuat,
                receipt => receipt.MaPhieuXuat,
                (detail, _) => detail.SoLuongXuat)
            .SumAsync(cancellationToken);

        return totalInbound - totalOutbound;
    }

    private async Task<Dictionary<int, decimal>> GetStockByIngredientAsync(
        IEnumerable<int> ingredientIds,
        CancellationToken cancellationToken)
    {
        var ids = ingredientIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return new Dictionary<int, decimal>();
        }

        var entries = new Dictionary<int, decimal>();
        foreach (var id in ids)
        {
            entries[id] = await GetAvailableQuantityAsync(id, cancellationToken);
        }

        return entries;
    }

    private async Task<PhieuNhapKhoDto> BuildReceiptDtoAsync(
        int receiptId,
        CancellationToken cancellationToken)
    {
        var receipt = await _context.PhieuNhapKhos
            .AsNoTracking()
            .SingleAsync(item => item.MaPhieuNhap == receiptId, cancellationToken);

        var items = await _context.ChiTietPhieuNhaps
            .AsNoTracking()
            .Where(detail => detail.MaPhieuNhap == receiptId)
            .Join(
                _context.NguyenLieus.AsNoTracking(),
                detail => detail.MaNguyenLieu,
                ingredient => ingredient.MaNguyenLieu,
                (detail, ingredient) => new ChiTietPhieuKhoDto(
                    detail.MaChiTietNhap,
                    detail.MaNguyenLieu,
                    ingredient.TenNguyenLieu,
                    ingredient.DonViTinh,
                    detail.SoLuongNhap,
                    detail.DonGiaNhap))
            .ToListAsync(cancellationToken);

        return new PhieuNhapKhoDto(
            receipt.MaPhieuNhap,
            receipt.ThoiGianNhap,
            receipt.TrangThai,
            receipt.GhiChu,
            items);
    }

    private async Task<PhieuXuatKhoDto> BuildIssueDtoAsync(
        int receiptId,
        CancellationToken cancellationToken)
    {
        var receipt = await _context.PhieuXuatKhos
            .AsNoTracking()
            .SingleAsync(item => item.MaPhieuXuat == receiptId, cancellationToken);

        var items = await _context.ChiTietPhieuXuats
            .AsNoTracking()
            .Where(detail => detail.MaPhieuXuat == receiptId)
            .Join(
                _context.NguyenLieus.AsNoTracking(),
                detail => detail.MaNguyenLieu,
                ingredient => ingredient.MaNguyenLieu,
                (detail, ingredient) => new ChiTietPhieuKhoDto(
                    detail.MaChiTietXuat,
                    detail.MaNguyenLieu,
                    ingredient.TenNguyenLieu,
                    ingredient.DonViTinh,
                    detail.SoLuongXuat,
                    0m))
            .ToListAsync(cancellationToken);

        return new PhieuXuatKhoDto(
            receipt.MaPhieuXuat,
            receipt.ThoiGianXuat,
            receipt.LoaiXuat,
            receipt.TrangThai,
            receipt.LyDo,
            null,
            items);
    }
}
