using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class KhuVucService : IKhuVucService
{
    private readonly RestaurantDbContext _context;

    public KhuVucService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<KhuVucDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.KhuVucs
            .AsNoTracking()
            .OrderBy(khuVuc => khuVuc.MaKhuVuc)
            .Select(khuVuc => new KhuVucDto(
                khuVuc.MaKhuVuc,
                khuVuc.TenKhuVuc,
                khuVuc.MoTa))
            .ToListAsync(cancellationToken);
    }

    public async Task<KhuVucDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.KhuVucs
            .AsNoTracking()
            .Where(khuVuc => khuVuc.MaKhuVuc == id)
            .Select(khuVuc => new KhuVucDto(
                khuVuc.MaKhuVuc,
                khuVuc.TenKhuVuc,
                khuVuc.MoTa))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<KhuVucDto> CreateAsync(
        CreateKhuVucRequest request,
        CancellationToken cancellationToken)
    {
        var tenKhuVuc = request.TenKhuVuc.Trim();
        await EnsureNameIsAvailableAsync(tenKhuVuc, null, cancellationToken);

        var khuVuc = new KhuVuc
        {
            TenKhuVuc = tenKhuVuc,
            MoTa = NormalizeDescription(request.MoTa)
        };

        _context.KhuVucs.Add(khuVuc);
        await SaveChangesAsync(tenKhuVuc, cancellationToken);

        return ToDto(khuVuc);
    }

    public async Task<KhuVucDto?> UpdateAsync(
        int id,
        UpdateKhuVucRequest request,
        CancellationToken cancellationToken)
    {
        var khuVuc = await _context.KhuVucs
            .FirstOrDefaultAsync(khuVuc => khuVuc.MaKhuVuc == id, cancellationToken);
        if (khuVuc is null)
        {
            return null;
        }

        var tenKhuVuc = request.TenKhuVuc.Trim();
        await EnsureNameIsAvailableAsync(tenKhuVuc, id, cancellationToken);

        khuVuc.TenKhuVuc = tenKhuVuc;
        khuVuc.MoTa = NormalizeDescription(request.MoTa);
        await SaveChangesAsync(tenKhuVuc, cancellationToken);

        return ToDto(khuVuc);
    }

    public async Task<KhuVucDeleteResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var khuVuc = await _context.KhuVucs
            .FirstOrDefaultAsync(khuVuc => khuVuc.MaKhuVuc == id, cancellationToken);
        if (khuVuc is null)
        {
            return KhuVucDeleteResult.NotFound;
        }

        if (await _context.BanAns.AnyAsync(
                ban => ban.MaKhuVuc == id,
                cancellationToken))
        {
            return KhuVucDeleteResult.HasTables;
        }

        _context.KhuVucs.Remove(khuVuc);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            return KhuVucDeleteResult.HasTables;
        }

        return KhuVucDeleteResult.Deleted;
    }

    private async Task EnsureNameIsAvailableAsync(
        string name,
        int? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await _context.KhuVucs.AnyAsync(
            khuVuc => khuVuc.TenKhuVuc == name &&
                      (!excludedId.HasValue || khuVuc.MaKhuVuc != excludedId.Value),
            cancellationToken);
        if (exists)
        {
            throw new DuplicateKhuVucNameException(name);
        }
    }

    private async Task SaveChangesAsync(
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            throw new DuplicateKhuVucNameException(name, exception);
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        for (var cause = exception.InnerException; cause is not null; cause = cause.InnerException)
        {
            if (cause is SqlException { Number: 2601 or 2627 })
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsForeignKeyViolation(DbUpdateException exception)
    {
        for (var cause = exception.InnerException; cause is not null; cause = cause.InnerException)
        {
            if (cause is SqlException { Number: 547 })
            {
                return true;
            }
        }

        return false;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static KhuVucDto ToDto(KhuVuc khuVuc)
    {
        return new KhuVucDto(khuVuc.MaKhuVuc, khuVuc.TenKhuVuc, khuVuc.MoTa);
    }
}
