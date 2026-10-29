using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class KhachHang
    {
        public int MaKhachHang { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
