using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Domain.Entities
{
    public class TaiKhoan
    {
        public int MaTaiKhoan { get; set; }
        public int MaVaiTro { get; set; }
        public int? MaNhanVien { get; set; }
        public int? MaKhachHang { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        // Không lưu mật khẩu dạng rõ.
        public string MatKhauHash { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
        public DateTime? LanDangNhapCuoi { get; set; }
    }
}
