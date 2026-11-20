using System.ComponentModel.DataAnnotations;
using Restaurant.Application.DTOs;

namespace Restaurant.Web.Models;

public sealed class DatBanViewModel
{
    [Display(Name = "Thời gian đến")]
    [Required(ErrorMessage = "Vui lòng chọn thời gian đến.")]
    public DateTime ThoiGianDen { get; set; } = DateTime.Now.AddHours(1);

    [Display(Name = "Số lượng khách")]
    [Range(1, 100, ErrorMessage = "Số lượng khách phải từ 1 đến 100.")]
    public int SoLuongKhach { get; set; } = 2;

    [Display(Name = "Ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; set; }

    public IReadOnlyList<DatBanResponse> LichSu { get; set; } = [];
}
