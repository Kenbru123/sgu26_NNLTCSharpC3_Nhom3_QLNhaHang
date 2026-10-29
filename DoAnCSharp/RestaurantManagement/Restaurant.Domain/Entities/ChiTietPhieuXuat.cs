using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class ChiTietPhieuXuat
    {
        public int MaChiTietXuat { get; set; }
        public int MaPhieuXuat { get; set; }
        public int MaNguyenLieu { get; set; }
        public decimal SoLuongXuat { get; set; }
    }
}
