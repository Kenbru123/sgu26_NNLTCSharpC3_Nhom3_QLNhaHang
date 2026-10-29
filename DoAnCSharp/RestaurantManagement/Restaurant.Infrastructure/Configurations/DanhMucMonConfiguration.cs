using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Configurations
{
    public class DanhMucMonConfiguration
        : IEntityTypeConfiguration<DanhMucMon>
    {
        public void Configure(
            EntityTypeBuilder<DanhMucMon> builder)
        {
            builder.ToTable("DanhMucMon");

            builder.HasKey(x => x.MaDanhMuc);

            builder.Property(x => x.MaDanhMuc)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TenDanhMuc)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MoTa)
                .HasMaxLength(500);

            builder.Property(x => x.TrangThai)
                .HasDefaultValue(true);
        }
    }
}
