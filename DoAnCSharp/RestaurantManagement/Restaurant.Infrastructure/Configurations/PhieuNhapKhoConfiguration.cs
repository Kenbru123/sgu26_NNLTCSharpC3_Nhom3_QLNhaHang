using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class PhieuNhapKhoConfiguration : IEntityTypeConfiguration<PhieuNhapKho>
{
    public void Configure(EntityTypeBuilder<PhieuNhapKho> builder)
    {
        builder.ToTable("PhieuNhapKho", table =>
        {
            table.HasCheckConstraint("CK_PhieuNhapKho_TrangThai", "[TrangThai] IN (N'Nhap',N'XacNhan',N'Huy')");
        });
        builder.HasKey(e => e.MaPhieuNhap);
        builder.Property(e => e.MaPhieuNhap).ValueGeneratedOnAdd();
        builder.Property(e => e.TrangThai)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Nhap");
        builder.Property(e => e.GhiChu)
            .HasMaxLength(500);
        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(e => e.MaNhanVien)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
