using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class BanAn
    {
        public int MaBan { get; set; }
        public int MaKhuVuc { get; set; }
        public string TenBan { get; set; } = string.Empty;
        public int SoCho { get; set; }
        public bool TrangThaiHoatDong { get; set; } = true;
    }
}
