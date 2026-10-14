using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class DanhMucMon
    {
        public int MaDanhMuc { get; set; }

        public string TenDanhMuc { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public ICollection<MonAn> MonAns { get; set; }
            = new List<MonAn>();
    }
}