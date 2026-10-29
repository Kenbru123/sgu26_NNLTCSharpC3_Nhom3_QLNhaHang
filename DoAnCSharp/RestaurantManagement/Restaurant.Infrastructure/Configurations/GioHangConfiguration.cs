using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class GioHangConfiguration : IEntityTypeConfiguration<GioHang>
{
    public void Configure(EntityTypeBuilder<GioHang> builder)
    {
        builder.ToTable("GioHang");
        builder.HasKey(e => e.MaGioHang);
        builder.Property(e => e.MaGioHang).ValueGeneratedOnAdd();
        builder.HasIndex(e => e.MaKhachHang).IsUnique();
        builder.HasOne<KhachHang>()
            .WithMany()
            .HasForeignKey(e => e.MaKhachHang)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
