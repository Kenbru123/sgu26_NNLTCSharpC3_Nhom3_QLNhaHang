using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class ChiTietPhieuNhap
    {
        public int MaChiTietNhap { get; set; }
        public int MaPhieuNhap { get; set; }
        public int MaNguyenLieu { get; set; }
        public decimal SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
    }
}
