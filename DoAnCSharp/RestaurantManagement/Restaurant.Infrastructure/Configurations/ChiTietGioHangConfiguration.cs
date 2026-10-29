using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class ChiTietGioHangConfiguration : IEntityTypeConfiguration<ChiTietGioHang>
{
    public void Configure(EntityTypeBuilder<ChiTietGioHang> builder)
    {
        builder.ToTable("ChiTietGioHang", table =>
        {
            table.HasCheckConstraint("CK_ChiTietGioHang_SoLuong", "[SoLuong] > 0");
        });
        builder.HasKey(e => e.MaChiTietGioHang);
        builder.Property(e => e.MaChiTietGioHang).ValueGeneratedOnAdd();
        builder.HasIndex(e => new { e.MaGioHang, e.MaMon }).IsUnique();
        builder.HasOne<GioHang>()
            .WithMany()
            .HasForeignKey(e => e.MaGioHang)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<MonAn>()
            .WithMany()
            .HasForeignKey(e => e.MaMon)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
