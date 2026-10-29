using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class KhuVucConfiguration : IEntityTypeConfiguration<KhuVuc>
{
    public void Configure(EntityTypeBuilder<KhuVuc> builder)
    {
        builder.ToTable("KhuVuc");
        builder.HasKey(e => e.MaKhuVuc);
        builder.Property(e => e.MaKhuVuc).ValueGeneratedOnAdd();
        builder.Property(e => e.TenKhuVuc)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.MoTa)
            .HasMaxLength(255);
        builder.HasIndex(e => e.TenKhuVuc).IsUnique();
    }
}
