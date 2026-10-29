using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class PhieuXuatKho
    {
        public int MaPhieuXuat { get; set; }
        public int? MaNhanVien { get; set; }
        public int? MaChiTietDonHang { get; set; }
        public DateTime ThoiGianXuat { get; set; }
        // CheBien | ThuCong
        public string LoaiXuat { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Nhap";
        public string? LyDo { get; set; }
    }
}
