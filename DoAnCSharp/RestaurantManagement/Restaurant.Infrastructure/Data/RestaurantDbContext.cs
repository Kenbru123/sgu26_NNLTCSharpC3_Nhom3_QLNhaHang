using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(
            DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhMucMon> DanhMucMons { get; set; }
        public DbSet<MonAn> MonAns { get; set; }
        // Tài khoản và nhân viên
        public DbSet<VaiTro> VaiTros => Set<VaiTro>();
        public DbSet<NhanVien> NhanViens => Set<NhanVien>();
        public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
        public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();

        // Bàn và đặt bàn
        public DbSet<KhuVuc> KhuVucs => Set<KhuVuc>();
        public DbSet<BanAn> BanAns => Set<BanAn>();
        public DbSet<DatBan> DatBans => Set<DatBan>();

        // Giỏ hàng và đơn hàng
        public DbSet<GioHang> GioHangs => Set<GioHang>();
        public DbSet<ChiTietGioHang> ChiTietGioHangs
            => Set<ChiTietGioHang>();
        public DbSet<DonHang> DonHangs => Set<DonHang>();
        public DbSet<ChiTietDonHang> ChiTietDonHangs
            => Set<ChiTietDonHang>();

        // Thanh toán
        public DbSet<HoaDon> HoaDons => Set<HoaDon>();

        // Quản lý kho
        public DbSet<NguyenLieu> NguyenLieus => Set<NguyenLieu>();
        public DbSet<CongThucMon> CongThucMons => Set<CongThucMon>();
        public DbSet<PhieuNhapKho> PhieuNhapKhos => Set<PhieuNhapKho>();
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps
            => Set<ChiTietPhieuNhap>();
        public DbSet<PhieuXuatKho> PhieuXuatKhos => Set<PhieuXuatKho>();
        public DbSet<ChiTietPhieuXuat> ChiTietPhieuXuats
            => Set<ChiTietPhieuXuat>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(RestaurantDbContext).Assembly);
        }
    }
}