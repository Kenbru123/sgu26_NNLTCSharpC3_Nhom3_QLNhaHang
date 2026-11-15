using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

public sealed record NguyenLieuDto(
    int MaNguyenLieu,
    string TenNguyenLieu,
    string DonViTinh,
    decimal MucTonToiThieu,
    decimal TonKho,
    bool TrangThai);

public sealed record CreateNguyenLieuRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string TenNguyenLieu { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public required string DonViTinh { get; init; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal MucTonToiThieu { get; init; }

    public bool TrangThai { get; init; } = true;
}

public sealed record UpdateNguyenLieuRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string TenNguyenLieu { get; init; }

    [Required, StringLength(50, MinimumLength = 1)]
    public required string DonViTinh { get; init; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal MucTonToiThieu { get; init; }

    public bool TrangThai { get; init; }
}

public sealed record NguyenLieuItemRequest(
    int MaNguyenLieu,
    decimal SoLuong,
    decimal DonGia);

public sealed record CreatePhieuNhapRequest
{
    [MinLength(1)]
    public required IReadOnlyList<NguyenLieuItemRequest> ChiTiet { get; init; }

    [StringLength(500)]
    public string? GhiChu { get; init; }
}

public sealed record CreatePhieuXuatRequest
{
    [Required]
    [RegularExpression("CheBien|ThuCong")]
    public required string LoaiXuat { get; init; }

    [MinLength(1)]
    public required IReadOnlyList<NguyenLieuItemRequest> ChiTiet { get; init; }

    public int? MaChiTietDonHang { get; init; }

    public int? MaNhanVien { get; init; }

    [StringLength(500)]
    public string? LyDo { get; init; }

    [StringLength(500)]
    public string? GhiChu { get; init; }
}

public sealed record PhieuNhapKhoDto(
    int MaPhieuNhap,
    DateTime ThoiGianNhap,
    string TrangThai,
    string? GhiChu,
    IReadOnlyList<ChiTietPhieuKhoDto> ChiTiet);

public sealed record PhieuXuatKhoDto(
    int MaPhieuXuat,
    DateTime ThoiGianXuat,
    string LoaiXuat,
    string TrangThai,
    string? LyDo,
    string? GhiChu,
    IReadOnlyList<ChiTietPhieuKhoDto> ChiTiet);

public sealed record ChiTietPhieuKhoDto(
    int MaChiTiet,
    int MaNguyenLieu,
    string TenNguyenLieu,
    string DonViTinh,
    decimal SoLuong,
    decimal DonGia);

public sealed record HoaDonDto(
    int MaHoaDon,
    int MaDonHang,
    int MaNhanVienLap,
    DateTime ThoiGianLap,
    DateTime? ThoiGianThanhToan,
    decimal TienHang,
    decimal PhiGiaoChot,
    decimal TyLeVAT,
    decimal TienVAT,
    decimal TongThanhToan,
    string? PhuongThucThanhToan,
    string TrangThaiThanhToan);

public sealed record PaymentInvoiceRequest(
    string PhuongThucThanhToan,
    decimal TyLeVAT);
