using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class DonHang
    {
        public int MaDonHang { get; set; }
        public int? MaKhachHang { get; set; }
        public int? MaBan { get; set; }
        public int? MaNhanVienPhucVu { get; set; }
        // TaiBan | OnlineNhanTaiQuay | OnlineGiaoHang
        public string LoaiDon { get; set; } = string.Empty;
        public string? TenNguoiNhan { get; set; }
        public string? SDTNguoiNhan { get; set; }
        public string? DiaChiGiao { get; set; }
        public decimal PhiGiao { get; set; }
        public DateTime? ThoiGianNhanDuKien { get; set; }
        public string TrangThaiDon { get; set; } = "ChoXacNhan";
        public string? TrangThaiGiao { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public string? GhiChu { get; set; }
    }
}
