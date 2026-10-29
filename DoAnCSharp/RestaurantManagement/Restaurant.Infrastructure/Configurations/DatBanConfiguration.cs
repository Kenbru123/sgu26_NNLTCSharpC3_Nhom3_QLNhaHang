using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class DatBanConfiguration : IEntityTypeConfiguration<DatBan>
{
    public void Configure(EntityTypeBuilder<DatBan> builder)
    {
        builder.ToTable("DatBan", table =>
        {
            table.HasCheckConstraint("CK_DatBan_SoLuongKhach", "[SoLuongKhach] > 0");
            table.HasCheckConstraint("CK_DatBan_ThoiGian", "[ThoiGianKetThucDuKien] > [ThoiGianDen]");
            table.HasCheckConstraint("CK_DatBan_TrangThai", "[TrangThai] IN (N'ChoXacNhan',N'DaXacNhan',N'DaDen',N'HoanTat',N'DaHuy',N'KhongDen')");
        });
        builder.HasKey(e => e.MaDatBan);
        builder.Property(e => e.MaDatBan).ValueGeneratedOnAdd();
        builder.Property(e => e.TrangThai)
            .HasMaxLength(25)
            .IsRequired()
            .HasDefaultValue("ChoXacNhan");
        builder.Property(e => e.GhiChu)
            .HasMaxLength(500);
        builder.HasOne<KhachHang>()
            .WithMany()
            .HasForeignKey(e => e.MaKhachHang)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<BanAn>()
            .WithMany()
            .HasForeignKey(e => e.MaBan)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
