using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class ChiTietPhieuNhapConfiguration : IEntityTypeConfiguration<ChiTietPhieuNhap>
{
    public void Configure(EntityTypeBuilder<ChiTietPhieuNhap> builder)
    {
        builder.ToTable("ChiTietPhieuNhap", table =>
        {
            table.HasCheckConstraint("CK_ChiTietPhieuNhap_SoLuongNhap", "[SoLuongNhap] > 0");
            table.HasCheckConstraint("CK_ChiTietPhieuNhap_DonGiaNhap", "[DonGiaNhap] >= 0");
        });
        builder.HasKey(e => e.MaChiTietNhap);
        builder.Property(e => e.MaChiTietNhap).ValueGeneratedOnAdd();
        builder.Property(e => e.SoLuongNhap)
            .HasColumnType("decimal(18,3)");
        builder.Property(e => e.DonGiaNhap)
            .HasColumnType("decimal(18,2)");
        builder.HasOne<PhieuNhapKho>()
            .WithMany()
            .HasForeignKey(e => e.MaPhieuNhap)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<NguyenLieu>()
            .WithMany()
            .HasForeignKey(e => e.MaNguyenLieu)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
