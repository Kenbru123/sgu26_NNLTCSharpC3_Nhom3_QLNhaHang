using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services
{
    public class MonAnService : IMonAnService
    {
        private readonly RestaurantDbContext _context;

        public MonAnService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<MonAnDto>> GetAllAsync()
        {
            return await _context.MonAns
                .AsNoTracking()
                .Select(x => new MonAnDto
                {
                    MaMon = x.MaMon,
                    TenMon = x.TenMon,
                    MoTa = x.MoTa,
                    DonGia = x.DonGia,
                    HinhAnh = x.HinhAnh,
                    TrangThai = x.TrangThai,
                    MaDanhMuc = x.MaDanhMuc
                })
                .ToListAsync();
        }
    }
}