using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class PhieuNhapKho
    {
        public int MaPhieuNhap { get; set; }
        public int MaNhanVien { get; set; }
        public DateTime ThoiGianNhap { get; set; }
        public string TrangThai { get; set; } = "Nhap";
        public string? GhiChu { get; set; }
    }
}
