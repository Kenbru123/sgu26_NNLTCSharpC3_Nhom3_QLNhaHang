using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

public sealed record CreateDatBanRequest
{
    [Required]
    public DateTime ThoiGianDen { get; init; }

    [Range(1, 100)]
    public int SoLuongKhach { get; init; }

    [StringLength(500)]
    public string? GhiChu { get; init; }
}

public sealed record DatBanResponse(
    int MaDatBan,
    string? TenBan,
    string? TenKhuVuc,
    DateTime ThoiGianTao,
    DateTime ThoiGianDen,
    DateTime ThoiGianKetThucDuKien,
    int SoLuongKhach,
    string TrangThai,
    string? GhiChu);
