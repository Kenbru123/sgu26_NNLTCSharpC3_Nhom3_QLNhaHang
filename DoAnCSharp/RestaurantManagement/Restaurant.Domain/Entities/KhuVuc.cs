using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class KhuVuc
    {
        public int MaKhuVuc { get; set; }
        public string TenKhuVuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }
}
