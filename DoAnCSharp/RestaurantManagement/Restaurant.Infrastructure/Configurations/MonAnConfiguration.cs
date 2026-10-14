using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations
{
    public class MonAnConfiguration
        : IEntityTypeConfiguration<MonAn>
    {
        public void Configure(
            EntityTypeBuilder<MonAn> builder)
        {
            builder.ToTable("MonAn");

            builder.HasKey(x => x.MaMon);

            builder.Property(x => x.MaMon)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TenMon)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.MoTa)
                .HasMaxLength(500);

            builder.Property(x => x.DonGia)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.HinhAnh)
                .HasMaxLength(500);

            builder.Property(x => x.TrangThai)
                .HasDefaultValue(true);

            builder.HasOne(x => x.DanhMuc)
                .WithMany(x => x.MonAns)
                .HasForeignKey(x => x.MaDanhMuc)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
