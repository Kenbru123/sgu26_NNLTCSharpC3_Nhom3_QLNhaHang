using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class NguyenLieuConfiguration : IEntityTypeConfiguration<NguyenLieu>
{
    public void Configure(EntityTypeBuilder<NguyenLieu> builder)
    {
        builder.ToTable("NguyenLieu", table =>
        {
            table.HasCheckConstraint("CK_NguyenLieu_TonToiThieu", "[MucTonToiThieu] >= 0");
        });
        builder.HasKey(e => e.MaNguyenLieu);
        builder.Property(e => e.MaNguyenLieu).ValueGeneratedOnAdd();
        builder.Property(e => e.TenNguyenLieu)
            .HasMaxLength(150)
            .IsRequired();
        builder.Property(e => e.DonViTinh)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.MucTonToiThieu)
            .HasColumnType("decimal(18,3)");
        builder.Property(e => e.TrangThai)
            .HasDefaultValue(true);
    }
}
