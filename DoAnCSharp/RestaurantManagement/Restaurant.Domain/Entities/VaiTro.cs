using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class VaiTro
    {
        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }
}
