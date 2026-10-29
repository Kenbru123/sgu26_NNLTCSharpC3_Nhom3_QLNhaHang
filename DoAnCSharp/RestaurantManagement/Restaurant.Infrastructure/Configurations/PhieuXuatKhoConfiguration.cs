using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class PhieuXuatKhoConfiguration : IEntityTypeConfiguration<PhieuXuatKho>
{
    public void Configure(EntityTypeBuilder<PhieuXuatKho> builder)
    {
        builder.ToTable("PhieuXuatKho", table =>
        {
            table.HasCheckConstraint("CK_PhieuXuatKho_LoaiXuat", "[LoaiXuat] IN (N'CheBien',N'ThuCong')");
            table.HasCheckConstraint("CK_PhieuXuatKho_TrangThai", "[TrangThai] IN (N'Nhap',N'XacNhan',N'Huy')");
            table.HasCheckConstraint("CK_PhieuXuatKho_NguonXuat", "([LoaiXuat] = N'CheBien' AND [MaChiTietDonHang] IS NOT NULL) OR ([LoaiXuat] = N'ThuCong' AND [MaNhanVien] IS NOT NULL AND [LyDo] IS NOT NULL AND LEN(LTRIM(RTRIM([LyDo]))) > 0)");
        });
        builder.HasKey(e => e.MaPhieuXuat);
        builder.Property(e => e.MaPhieuXuat).ValueGeneratedOnAdd();
        builder.Property(e => e.LoaiXuat)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.TrangThai)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Nhap");
        builder.Property(e => e.LyDo)
            .HasMaxLength(500);
        builder.HasIndex(e => e.MaChiTietDonHang).IsUnique().HasFilter("[MaChiTietDonHang] IS NOT NULL");
        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(e => e.MaNhanVien)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<ChiTietDonHang>()
            .WithMany()
            .HasForeignKey(e => e.MaChiTietDonHang)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
