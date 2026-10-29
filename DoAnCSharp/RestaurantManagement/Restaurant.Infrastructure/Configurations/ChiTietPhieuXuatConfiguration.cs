using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class ChiTietPhieuXuatConfiguration : IEntityTypeConfiguration<ChiTietPhieuXuat>
{
    public void Configure(EntityTypeBuilder<ChiTietPhieuXuat> builder)
    {
        builder.ToTable("ChiTietPhieuXuat", table =>
        {
            table.HasCheckConstraint("CK_ChiTietPhieuXuat_SoLuongXuat", "[SoLuongXuat] > 0");
        });
        builder.HasKey(e => e.MaChiTietXuat);
        builder.Property(e => e.MaChiTietXuat).ValueGeneratedOnAdd();
        builder.Property(e => e.SoLuongXuat)
            .HasColumnType("decimal(18,3)");
        builder.HasOne<PhieuXuatKho>()
            .WithMany()
            .HasForeignKey(e => e.MaPhieuXuat)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<NguyenLieu>()
            .WithMany()
            .HasForeignKey(e => e.MaNguyenLieu)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
