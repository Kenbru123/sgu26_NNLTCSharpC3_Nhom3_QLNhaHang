using System.ComponentModel.DataAnnotations;

namespace Restaurant.Web.Models;

public sealed class CheckoutViewModel
{
    [Required]
    [RegularExpression("OnlineNhanTaiQuay|OnlineGiaoHang",
        ErrorMessage = "Vui lòng chọn cách nhận món.")]
    public string LoaiDon { get; set; } = "OnlineNhanTaiQuay";

    [Required, StringLength(120, MinimumLength = 1)]
    public string TenNguoiNhan { get; set; } = string.Empty;

    [Required, Phone, StringLength(20, MinimumLength = 7)]
    public string SDTNguoiNhan { get; set; } = string.Empty;

    [StringLength(500)]
    public string? DiaChiGiao { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
