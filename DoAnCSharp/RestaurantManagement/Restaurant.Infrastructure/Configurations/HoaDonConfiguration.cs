using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class HoaDonConfiguration : IEntityTypeConfiguration<HoaDon>
{
    public void Configure(EntityTypeBuilder<HoaDon> builder)
    {
        builder.ToTable("HoaDon", table =>
        {
            table.HasCheckConstraint("CK_HoaDon_SoTien", "[TienHang] >= 0 AND [PhiGiaoChot] >= 0 AND [TienVAT] >= 0 AND [TongThanhToan] >= 0");
            table.HasCheckConstraint("CK_HoaDon_TyLeVAT", "[TyLeVAT] >= 0 AND [TyLeVAT] <= 100");
            table.HasCheckConstraint("CK_HoaDon_TrangThaiThanhToan", "[TrangThaiThanhToan] IN (N'ChuaThanhToan',N'DaThanhToan',N'DaHuy')");
        });
        builder.HasKey(e => e.MaHoaDon);
        builder.Property(e => e.MaHoaDon).ValueGeneratedOnAdd();
        builder.Property(e => e.TienHang)
            .HasColumnType("decimal(18,2)");
        builder.Property(e => e.PhiGiaoChot)
            .HasColumnType("decimal(18,2)");
        builder.Property(e => e.TyLeVAT)
            .HasColumnType("decimal(5,2)");
        builder.Property(e => e.TienVAT)
            .HasColumnType("decimal(18,2)");
        builder.Property(e => e.TongThanhToan)
            .HasColumnType("decimal(18,2)");
        builder.Property(e => e.PhuongThucThanhToan)
            .HasMaxLength(20);
        builder.Property(e => e.TrangThaiThanhToan)
            .HasMaxLength(25)
            .IsRequired()
            .HasDefaultValue("ChuaThanhToan");
        builder.HasIndex(e => e.MaDonHang).IsUnique();
        builder.HasOne<DonHang>()
            .WithMany()
            .HasForeignKey(e => e.MaDonHang)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(e => e.MaNhanVienLap)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
