using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class DatBan
    {
        public int MaDatBan { get; set; }
        public int MaKhachHang { get; set; }
        public int? MaBan { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianDen { get; set; }
        public DateTime ThoiGianKetThucDuKien { get; set; }
        public int SoLuongKhach { get; set; }
        public string TrangThai { get; set; } = "ChoXacNhan";
        public string? GhiChu { get; set; }
    }
}
