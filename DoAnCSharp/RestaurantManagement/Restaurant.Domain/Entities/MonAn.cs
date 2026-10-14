using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class MonAn
    {
        public int MaMon { get; set; }

        public string TenMon { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public decimal DonGia { get; set; }

        public string? HinhAnh { get; set; }

        public bool TrangThai { get; set; } = true;

        public int MaDanhMuc { get; set; }

        public DanhMucMon DanhMuc { get; set; } = null!;
    }
}
