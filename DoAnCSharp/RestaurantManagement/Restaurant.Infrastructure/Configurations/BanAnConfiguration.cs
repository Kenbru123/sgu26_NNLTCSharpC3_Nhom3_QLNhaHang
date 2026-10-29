using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class BanAnConfiguration : IEntityTypeConfiguration<BanAn>
{
    public void Configure(EntityTypeBuilder<BanAn> builder)
    {
        builder.ToTable("BanAn", table =>
        {
            table.HasCheckConstraint("CK_BanAn_SoCho", "[SoCho] > 0");
        });
        builder.HasKey(e => e.MaBan);
        builder.Property(e => e.MaBan).ValueGeneratedOnAdd();
        builder.Property(e => e.TenBan)
            .HasMaxLength(40)
            .IsRequired();
        builder.Property(e => e.TrangThaiHoatDong)
            .HasDefaultValue(true);
        builder.HasIndex(e => new { e.MaKhuVuc, e.TenBan }).IsUnique();
        builder.HasOne<KhuVuc>()
            .WithMany()
            .HasForeignKey(e => e.MaKhuVuc)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
