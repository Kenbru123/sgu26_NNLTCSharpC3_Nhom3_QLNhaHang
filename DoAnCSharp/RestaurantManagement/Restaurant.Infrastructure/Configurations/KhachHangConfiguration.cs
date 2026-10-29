using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class KhachHangConfiguration : IEntityTypeConfiguration<KhachHang>
{
    public void Configure(EntityTypeBuilder<KhachHang> builder)
    {
        builder.ToTable("KhachHang");
        builder.HasKey(e => e.MaKhachHang);
        builder.Property(e => e.MaKhachHang).ValueGeneratedOnAdd();
        builder.Property(e => e.HoTen)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(e => e.SDT)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.Email)
            .HasMaxLength(150);
    }
}
