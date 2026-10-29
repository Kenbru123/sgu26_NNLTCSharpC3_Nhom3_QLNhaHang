using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class ChiTietDonHangConfiguration : IEntityTypeConfiguration<ChiTietDonHang>
{
    public void Configure(EntityTypeBuilder<ChiTietDonHang> builder)
    {
        builder.ToTable("ChiTietDonHang", table =>
        {
            table.HasCheckConstraint("CK_ChiTietDonHang_SoLuong", "[SoLuong] > 0");
            table.HasCheckConstraint("CK_ChiTietDonHang_DonGia", "[DonGia] >= 0");
            table.HasCheckConstraint("CK_ChiTietDonHang_TrangThaiCheBien", "[TrangThaiCheBien] IN (N'ChoCheBien',N'DangCheBien',N'HoanThanh',N'DaHuy')");
        });
        builder.HasKey(e => e.MaChiTietDonHang);
        builder.Property(e => e.MaChiTietDonHang).ValueGeneratedOnAdd();
        builder.Property(e => e.DonGia)
            .HasColumnType("decimal(18,2)");
        builder.Property(e => e.GhiChuMon)
            .HasMaxLength(500);
        builder.Property(e => e.TrangThaiCheBien)
            .HasMaxLength(25)
            .IsRequired()
            .HasDefaultValue("ChoCheBien");
        builder.HasOne<DonHang>()
            .WithMany()
            .HasForeignKey(e => e.MaDonHang)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<MonAn>()
            .WithMany()
            .HasForeignKey(e => e.MaMon)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
