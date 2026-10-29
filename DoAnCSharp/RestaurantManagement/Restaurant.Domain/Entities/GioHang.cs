using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class GioHang
    {
        public int MaGioHang { get; set; }
        public int MaKhachHang { get; set; }
        public DateTime NgayCapNhat { get; set; }
    }
}
