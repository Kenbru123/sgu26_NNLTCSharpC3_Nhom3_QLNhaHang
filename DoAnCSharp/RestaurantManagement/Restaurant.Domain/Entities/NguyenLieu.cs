using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class NguyenLieu
    {
        public int MaNguyenLieu { get; set; }
        public string TenNguyenLieu { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public decimal MucTonToiThieu { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
