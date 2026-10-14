using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(
            DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhMucMon> DanhMucMons { get; set; }
        public DbSet<MonAn> MonAns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(RestaurantDbContext).Assembly);
        }
    }
}