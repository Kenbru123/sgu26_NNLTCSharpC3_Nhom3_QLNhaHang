using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

/// <summary>Thông tin một món ăn trong thực đơn.</summary>
public sealed record FoodResponse(
    int MaMon,
    string TenMon,
    string? MoTa,
    decimal DonGia,
    string? HinhAnh,
    bool TrangThai,
    int MaDanhMuc,
    string TenDanhMuc);

/// <summary>Thông tin danh mục món ăn đang được sử dụng.</summary>
public sealed record FoodCategoryResponse(int MaDanhMuc, string TenDanhMuc);

/// <summary>Dữ liệu tạo danh mục món ăn mới.</summary>
public sealed record CreateFoodCategoryRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public required string TenDanhMuc { get; init; }

    [StringLength(500)]
    public string? MoTa { get; init; }
}

/// <summary>Dữ liệu tạo món ăn mới.</summary>
public sealed record CreateFoodRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string TenMon { get; init; }

    [StringLength(500)]
    public string? MoTa { get; init; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal DonGia { get; init; }

    [StringLength(500)]
    public string? HinhAnh { get; init; }

    [Range(1, int.MaxValue)]
    public int MaDanhMuc { get; init; }

    public bool TrangThai { get; init; } = true;
}

/// <summary>Dữ liệu cập nhật món ăn.</summary>
public sealed record UpdateFoodRequest
{
    [Required, StringLength(150, MinimumLength = 1)]
    public required string TenMon { get; init; }

    [StringLength(500)]
    public string? MoTa { get; init; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal DonGia { get; init; }

    [StringLength(500)]
    public string? HinhAnh { get; init; }

    [Range(1, int.MaxValue)]
    public int MaDanhMuc { get; init; }

    public bool TrangThai { get; init; }
}

/// <summary>Món ăn và số lượng khách muốn đặt.</summary>
public sealed record OnlineOrderItemRequest(
    [param: Range(1, int.MaxValue)] int MaMon,
    [param: Range(1, 99)] int SoLuong);

/// <summary>Thông tin khách hàng gửi khi đặt món online.</summary>
public sealed record CreateOnlineOrderRequest
{
    [Required]
    [RegularExpression("OnlineNhanTaiQuay|OnlineGiaoHang")]
    public required string LoaiDon { get; init; }

    [Required, StringLength(120, MinimumLength = 1)]
    public required string TenNguoiNhan { get; init; }

    [Required, Phone, StringLength(20, MinimumLength = 7)]
    public required string SDTNguoiNhan { get; init; }

    [StringLength(500)]
    public string? DiaChiGiao { get; init; }

    [StringLength(500)]
    public string? GhiChu { get; init; }

    public int? MaKhachHang { get; init; }

    [Required, MinLength(1), MaxLength(50)]
    public required IReadOnlyList<OnlineOrderItemRequest> MonAn { get; init; }
}

/// <summary>Một món thuộc đơn đặt online đã tạo.</summary>
public sealed record OnlineOrderItemResponse(
    int MaMon,
    string TenMon,
    int SoLuong,
    decimal DonGia,
    decimal ThanhTien);

/// <summary>Kết quả xác nhận đơn đặt món online.</summary>
public sealed record OnlineOrderResponse(
    int MaDonHang,
    string LoaiDon,
    string TrangThaiDon,
    string? TrangThaiGiao,
    string TenNguoiNhan,
    string SDTNguoiNhan,
    string? DiaChiGiao,
    decimal PhiGiao,
    DateTimeOffset ThoiGianTao,
    decimal TongTien,
    IReadOnlyList<OnlineOrderItemResponse> MonAn);
