namespace Restaurant.Application.DTOs;

public sealed record GoiMonBanDto(
    int MaBan,
    string TenKhuVuc,
    string TenBan,
    int? MaDonHang,
    string? TrangThaiPhieu)
{
    public string TenHienThi =>
        $"{TenKhuVuc} - {TenBan}" +
        (MaDonHang is null ? string.Empty : $" (phiếu #{MaDonHang})");
}

public sealed record GoiMonDanhMucDto(
    int MaDanhMuc,
    string TenDanhMuc);

public sealed record GoiMonMonAnDto(
    int MaMon,
    string TenMon,
    string TenDanhMuc,
    decimal DonGia);

public sealed record GoiMonPhieuDto(
    int MaDonHang,
    int MaBan,
    string TenBan,
    DateTime ThoiGianTao,
    string TrangThaiDon,
    int SoMon,
    decimal TongTien);

public sealed record GoiMonChiTietDto(
    int MaChiTietDonHang,
    int MaMon,
    string TenMon,
    int SoLuong,
    decimal DonGia,
    decimal ThanhTien,
    string? GhiChuMon,
    string TrangThaiCheBien,
    DateTime? ThoiGianBatDau,
    DateTime? ThoiGianHoanThanh);

public sealed record GoiMonPhieuChiTietDto(
    GoiMonPhieuDto Phieu,
    IReadOnlyList<GoiMonChiTietDto> MonAn,
    string? GhiChu);

public sealed record TaoPhieuGoiMonRequest(int MaBan, string? GhiChu);

public sealed record CapNhatPhieuGoiMonRequest(string? GhiChu);

public sealed record ThemMonVaoPhieuRequest(
    int MaMon,
    int SoLuong,
    string? GhiChuMon);

public sealed record CapNhatSoLuongMonRequest(int SoLuong);

public sealed record CapNhatTrangThaiPhieuRequest(string TrangThaiDon);

public sealed record ThanhToanPhieuRequest(
    string PhuongThucThanhToan,
    decimal TyLeVAT);

public sealed record CapNhatTrangThaiMonRequest(string TrangThaiCheBien);
