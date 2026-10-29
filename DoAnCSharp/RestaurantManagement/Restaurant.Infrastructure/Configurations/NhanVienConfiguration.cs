using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class NhanVienConfiguration : IEntityTypeConfiguration<NhanVien>
{
    public void Configure(EntityTypeBuilder<NhanVien> builder)
    {
        builder.ToTable("NhanVien");
        builder.HasKey(e => e.MaNhanVien);
        builder.Property(e => e.MaNhanVien).ValueGeneratedOnAdd();
        builder.Property(e => e.HoTen)
            .HasMaxLength(120)
            .IsRequired();
        builder.Property(e => e.SDT)
            .HasMaxLength(20);
        builder.Property(e => e.NgaySinh)
            .HasColumnType("date");
        builder.Property(e => e.TrangThaiLamViec)
            .HasDefaultValue(true);
    }
}
