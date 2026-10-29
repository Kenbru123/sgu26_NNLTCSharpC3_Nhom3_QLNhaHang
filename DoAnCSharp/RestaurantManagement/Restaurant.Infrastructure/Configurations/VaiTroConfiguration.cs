using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations;

public sealed class VaiTroConfiguration : IEntityTypeConfiguration<VaiTro>
{
    public void Configure(EntityTypeBuilder<VaiTro> builder)
    {
        builder.ToTable("VaiTro");
        builder.HasKey(e => e.MaVaiTro);
        builder.Property(e => e.MaVaiTro).ValueGeneratedOnAdd();
        builder.Property(e => e.TenVaiTro)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.MoTa)
            .HasMaxLength(255);
        builder.HasIndex(e => e.TenVaiTro).IsUnique();
    }
}
