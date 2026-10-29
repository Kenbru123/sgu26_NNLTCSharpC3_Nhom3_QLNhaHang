using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class DonHangConfiguration : IEntityTypeConfiguration<DonHang>
{
    public void Configure(EntityTypeBuilder<DonHang> builder)
    {
        builder.ToTable("DonHang", table =>
        {
            table.HasCheckConstraint("CK_DonHang_PhiGiao", "[PhiGiao] >= 0");
            table.HasCheckConstraint("CK_DonHang_LoaiDon", "[LoaiDon] IN (N'TaiBan',N'OnlineNhanTaiQuay',N'OnlineGiaoHang')");
            table.HasCheckConstraint("CK_DonHang_ThongTinNhan", "([LoaiDon] = N'TaiBan' AND [MaBan] IS NOT NULL) OR ([LoaiDon] <> N'TaiBan' AND [MaBan] IS NULL AND [TenNguoiNhan] IS NOT NULL AND LEN(LTRIM(RTRIM([TenNguoiNhan]))) > 0 AND [SDTNguoiNhan] IS NOT NULL AND LEN(LTRIM(RTRIM([SDTNguoiNhan]))) > 0)");
            table.HasCheckConstraint("CK_DonHang_DiaChiGiao", "[LoaiDon] <> N'OnlineGiaoHang' OR ([DiaChiGiao] IS NOT NULL AND LEN(LTRIM(RTRIM([DiaChiGiao]))) > 0)");
            table.HasCheckConstraint("CK_DonHang_TrangThaiGiao", "[LoaiDon] = N'OnlineGiaoHang' OR [TrangThaiGiao] IS NULL");
        });
        builder.HasKey(e => e.MaDonHang);
        builder.Property(e => e.MaDonHang).ValueGeneratedOnAdd();
        builder.Property(e => e.LoaiDon)
            .HasMaxLength(30)
            .IsRequired();
        builder.Property(e => e.TenNguoiNhan)
            .HasMaxLength(120);
        builder.Property(e => e.SDTNguoiNhan)
            .HasMaxLength(20);
        builder.Property(e => e.DiaChiGiao)
            .HasMaxLength(500);
        builder.Property(e => e.PhiGiao)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);
        builder.Property(e => e.TrangThaiDon)
            .HasMaxLength(25)
            .IsRequired()
            .HasDefaultValue("ChoXacNhan");
        builder.Property(e => e.TrangThaiGiao)
            .HasMaxLength(25);
        builder.Property(e => e.GhiChu)
            .HasMaxLength(500);
        builder.HasOne<KhachHang>()
            .WithMany()
            .HasForeignKey(e => e.MaKhachHang)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<BanAn>()
            .WithMany()
            .HasForeignKey(e => e.MaBan)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(e => e.MaNhanVienPhucVu)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
