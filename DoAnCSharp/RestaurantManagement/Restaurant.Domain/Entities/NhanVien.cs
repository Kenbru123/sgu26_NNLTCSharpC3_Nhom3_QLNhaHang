using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class NhanVien
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? SDT { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public bool TrangThaiLamViec { get; set; } = true;
    }
}
