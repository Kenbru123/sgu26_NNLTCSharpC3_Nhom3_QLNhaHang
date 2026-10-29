using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public int MaDonHang { get; set; }
        public int MaNhanVienLap { get; set; }
        public DateTime ThoiGianLap { get; set; }
        public DateTime? ThoiGianThanhToan { get; set; }
        public decimal TienHang { get; set; }
        public decimal PhiGiaoChot { get; set; }
        public decimal TyLeVAT { get; set; }
        public decimal TienVAT { get; set; }
        public decimal TongThanhToan { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan";
    }
}
