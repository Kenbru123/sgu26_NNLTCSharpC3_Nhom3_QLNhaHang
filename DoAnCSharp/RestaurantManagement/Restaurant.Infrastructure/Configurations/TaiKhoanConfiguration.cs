using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class TaiKhoanConfiguration : IEntityTypeConfiguration<TaiKhoan>
{
    public void Configure(EntityTypeBuilder<TaiKhoan> builder)
    {
        builder.ToTable("TaiKhoan", table =>
        {
            table.HasCheckConstraint("CK_TaiKhoan_ChuThe", "([MaNhanVien] IS NOT NULL AND [MaKhachHang] IS NULL) OR ([MaNhanVien] IS NULL AND [MaKhachHang] IS NOT NULL)");
        });
        builder.HasKey(e => e.MaTaiKhoan);
        builder.Property(e => e.MaTaiKhoan).ValueGeneratedOnAdd();
        builder.Property(e => e.TenDangNhap)
            .HasMaxLength(80)
            .IsRequired();
        builder.Property(e => e.MatKhauHash)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.TrangThai)
            .HasDefaultValue(true);
        builder.HasIndex(e => e.TenDangNhap).IsUnique();
        builder.HasIndex(e => e.MaNhanVien).IsUnique().HasFilter("[MaNhanVien] IS NOT NULL");
        builder.HasIndex(e => e.MaKhachHang).IsUnique().HasFilter("[MaKhachHang] IS NOT NULL");
        builder.HasOne<VaiTro>()
            .WithMany()
            .HasForeignKey(e => e.MaVaiTro)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(e => e.MaNhanVien)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<KhachHang>()
            .WithMany()
            .HasForeignKey(e => e.MaKhachHang)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
