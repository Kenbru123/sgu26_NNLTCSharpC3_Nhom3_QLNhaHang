using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class CongThucMonConfiguration : IEntityTypeConfiguration<CongThucMon>
{
    public void Configure(EntityTypeBuilder<CongThucMon> builder)
    {
        builder.ToTable("CongThucMon", table =>
        {
            table.HasCheckConstraint("CK_CongThucMon_DinhLuong", "[DinhLuong] > 0");
        });
        builder.HasKey(e => new { e.MaMon, e.MaNguyenLieu });
        builder.Property(e => e.DinhLuong)
            .HasColumnType("decimal(18,3)");
        builder.HasOne<MonAn>()
            .WithMany()
            .HasForeignKey(e => e.MaMon)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<NguyenLieu>()
            .WithMany()
            .HasForeignKey(e => e.MaNguyenLieu)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
