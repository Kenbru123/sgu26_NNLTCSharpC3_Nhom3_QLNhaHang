using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Services;

public sealed class FoodService : IFoodService
{
    private readonly RestaurantDbContext _context;

    public FoodService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FoodResponse>> GetMenuAsync(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        var foods = _context.MonAns
            .AsNoTracking()
            .Where(food => food.TrangThai && food.DanhMuc.TrangThai);

        if (categoryId.HasValue)
        {
            foods = foods.Where(food => food.MaDanhMuc == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            foods = foods.Where(food =>
                food.TenMon.Contains(keyword) ||
                (food.MoTa != null && food.MoTa.Contains(keyword)) ||
                food.DanhMuc.TenDanhMuc.Contains(keyword));
        }

        return await Project(foods
            .OrderBy(food => food.DanhMuc.TenDanhMuc)
            .ThenBy(food => food.TenMon))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FoodResponse>> GetAllForManagementAsync(
        CancellationToken cancellationToken)
    {
        return await Project(_context.MonAns
            .AsNoTracking()
            .OrderBy(food => food.MaMon))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FoodCategoryResponse>> GetCategoriesAsync(
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var categories = _context.DanhMucMons.AsNoTracking();
        if (!includeInactive)
        {
            categories = categories.Where(category => category.TrangThai);
        }

        return await categories
            .OrderBy(category => category.TenDanhMuc)
            .Select(category => new FoodCategoryResponse(
                category.MaDanhMuc,
                category.TenDanhMuc))
            .ToListAsync(cancellationToken);
    }

    public async Task<FoodResponse?> GetByIdAsync(
        int id,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var food = _context.MonAns.AsNoTracking().Where(item => item.MaMon == id);
        if (!includeInactive)
        {
            food = food.Where(item => item.TrangThai && item.DanhMuc.TrangThai);
        }

        return await Project(food).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<FoodResponse> CreateAsync(
        CreateFoodRequest request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _context.DanhMucMons.AnyAsync(
            category => category.MaDanhMuc == request.MaDanhMuc &&
                        category.TrangThai,
            cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException("Danh mục không tồn tại hoặc đã ngừng hoạt động.");
        }

        var food = new MonAn
        {
            TenMon = request.TenMon.Trim(),
            MoTa = Normalize(request.MoTa),
            DonGia = request.DonGia,
            HinhAnh = Normalize(request.HinhAnh),
            MaDanhMuc = request.MaDanhMuc,
            TrangThai = request.TrangThai
        };
        _context.MonAns.Add(food);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(food.MaMon, true, cancellationToken))!;
    }

    public async Task<FoodResponse?> UpdateAsync(
        int id,
        UpdateFoodRequest request,
        CancellationToken cancellationToken)
    {
        var food = await _context.MonAns
            .SingleOrDefaultAsync(item => item.MaMon == id, cancellationToken);
        if (food is null)
        {
            return null;
        }

        var categoryExists = await _context.DanhMucMons.AnyAsync(
            category => category.MaDanhMuc == request.MaDanhMuc &&
                        category.TrangThai,
            cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException("Danh mục không tồn tại hoặc đã ngừng hoạt động.");
        }

        food.TenMon = request.TenMon.Trim();
        food.MoTa = Normalize(request.MoTa);
        food.DonGia = request.DonGia;
        food.HinhAnh = Normalize(request.HinhAnh);
        food.MaDanhMuc = request.MaDanhMuc;
        food.TrangThai = request.TrangThai;

        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, true, cancellationToken);
    }

    public async Task<bool> DeactivateAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var food = await _context.MonAns
            .SingleOrDefaultAsync(item => item.MaMon == id, cancellationToken);
        if (food is null)
        {
            return false;
        }

        food.TrangThai = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<FoodResponse> Project(IQueryable<MonAn> foods)
    {
        return foods.Select(food => new FoodResponse(
            food.MaMon,
            food.TenMon,
            food.MoTa,
            food.DonGia,
            food.HinhAnh,
            food.TrangThai,
            food.MaDanhMuc,
            food.DanhMuc.TenDanhMuc));
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
