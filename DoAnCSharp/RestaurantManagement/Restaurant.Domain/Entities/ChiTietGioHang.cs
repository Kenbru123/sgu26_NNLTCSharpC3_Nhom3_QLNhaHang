using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class ChiTietGioHang
    {
        public int MaChiTietGioHang { get; set; }
        public int MaGioHang { get; set; }
        public int MaMon { get; set; }
        public int SoLuong { get; set; } = 1;
    }
}
