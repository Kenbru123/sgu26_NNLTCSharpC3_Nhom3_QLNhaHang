using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class ChiTietDonHang
    {
        public int MaChiTietDonHang { get; set; }
        public int MaDonHang { get; set; }
        public int MaMon { get; set; }
        public int SoLuong { get; set; } = 1;
        public decimal DonGia { get; set; }
        public string? GhiChuMon { get; set; }
        public string TrangThaiCheBien { get; set; } = "ChoCheBien";
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianHoanThanh { get; set; }
    }
}
