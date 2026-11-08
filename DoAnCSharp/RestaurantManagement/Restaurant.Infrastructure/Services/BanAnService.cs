using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class BanAnService : IBanAnService
{
    private readonly RestaurantDbContext _context;

    public BanAnService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BanAnDto>> GetAllAsync(
        int? maKhuVuc,
        bool? trangThaiHoatDong,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _context.BanAns.AsNoTracking();

        if (maKhuVuc.HasValue)
        {
            query = query.Where(ban => ban.MaKhuVuc == maKhuVuc.Value);
        }

        if (trangThaiHoatDong.HasValue)
        {
            query = query.Where(ban => ban.TrangThaiHoatDong == trangThaiHoatDong.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();
            query = int.TryParse(searchText, out var maBan)
                ? query.Where(ban => ban.MaBan == maBan || ban.TenBan.Contains(searchText))
                : query.Where(ban => ban.TenBan.Contains(searchText));
        }

        return await query
            .OrderBy(ban => ban.MaBan)
            .Select(ban => new BanAnDto(
                ban.MaBan,
                ban.MaKhuVuc,
                ban.KhuVuc.TenKhuVuc,
                ban.TenBan,
                ban.SoCho,
                ban.TrangThaiHoatDong))
            .ToListAsync(cancellationToken);
    }

    public Task<BanAnDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _context.BanAns
            .AsNoTracking()
            .Where(ban => ban.MaBan == id)
            .Select(ban => new BanAnDto(
                ban.MaBan,
                ban.MaKhuVuc,
                ban.KhuVuc.TenKhuVuc,
                ban.TenBan,
                ban.SoCho,
                ban.TrangThaiHoatDong))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BanAnDto> CreateAsync(
        CreateBanAnRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureAreaExistsAsync(request.MaKhuVuc, cancellationToken);
        var tenBan = request.TenBan.Trim();
        await EnsureNameIsAvailableAsync(request.MaKhuVuc, tenBan, null, cancellationToken);

        var banAn = new BanAn
        {
            MaKhuVuc = request.MaKhuVuc,
            TenBan = tenBan,
            SoCho = request.SoCho,
            TrangThaiHoatDong = true
        };

        _context.BanAns.Add(banAn);
        await SaveChangesAsync(request.MaKhuVuc, tenBan, cancellationToken);
        return (await GetByIdAsync(banAn.MaBan, cancellationToken))!;
    }

    public async Task<BanAnDto?> UpdateAsync(
        int id,
        UpdateBanAnRequest request,
        CancellationToken cancellationToken)
    {
        var banAn = await _context.BanAns
            .FirstOrDefaultAsync(ban => ban.MaBan == id, cancellationToken);
        if (banAn is null)
        {
            return null;
        }

        await EnsureAreaExistsAsync(request.MaKhuVuc, cancellationToken);
        if (banAn.MaKhuVuc != request.MaKhuVuc &&
            await IsInUseAsync(id, cancellationToken))
        {
            throw new BanAnInUseException();
        }

        var tenBan = request.TenBan.Trim();
        await EnsureNameIsAvailableAsync(request.MaKhuVuc, tenBan, id, cancellationToken);

        banAn.MaKhuVuc = request.MaKhuVuc;
        banAn.TenBan = tenBan;
        banAn.SoCho = request.SoCho;
        await SaveChangesAsync(request.MaKhuVuc, tenBan, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<BanAnDto?> UpdateStatusAsync(
        int id,
        bool trangThaiHoatDong,
        CancellationToken cancellationToken)
    {
        var banAn = await _context.BanAns
            .FirstOrDefaultAsync(ban => ban.MaBan == id, cancellationToken);
        if (banAn is null)
        {
            return null;
        }

        if (banAn.TrangThaiHoatDong == trangThaiHoatDong)
        {
            return await GetByIdAsync(id, cancellationToken);
        }

        if (!trangThaiHoatDong && await IsInUseAsync(id, cancellationToken))
        {
            throw new BanAnInUseException();
        }

        banAn.TrangThaiHoatDong = trangThaiHoatDong;
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<BanAnDeleteResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var banAn = await _context.BanAns
            .FirstOrDefaultAsync(ban => ban.MaBan == id, cancellationToken);
        if (banAn is null)
        {
            return BanAnDeleteResult.NotFound;
        }

        if (await IsInUseAsync(id, cancellationToken))
        {
            return BanAnDeleteResult.InUse;
        }

        _context.BanAns.Remove(banAn);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsSqlError(exception, 547))
        {
            return BanAnDeleteResult.InUse;
        }

        return BanAnDeleteResult.Deleted;
    }

    private async Task EnsureAreaExistsAsync(int maKhuVuc, CancellationToken cancellationToken)
    {
        if (!await _context.KhuVucs.AnyAsync(
                khuVuc => khuVuc.MaKhuVuc == maKhuVuc,
                cancellationToken))
        {
            throw new InvalidBanAreaException(maKhuVuc);
        }
    }

    private async Task<bool> IsInUseAsync(int maBan, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var hasActiveReservation = await _context.DatBans.AnyAsync(
            datBan => datBan.MaBan == maBan &&
                      (datBan.TrangThai == "ChoXacNhan" ||
                       datBan.TrangThai == "DaXacNhan" ||
                       datBan.TrangThai == "DaDen") &&
                      datBan.ThoiGianKetThucDuKien > now,
            cancellationToken);
        if (hasActiveReservation)
        {
            return true;
        }

        return await _context.DonHangs.AnyAsync(
            donHang => donHang.MaBan == maBan &&
                       donHang.TrangThaiDon != "HoanTat" &&
                       donHang.TrangThaiDon != "DaHuy",
            cancellationToken);
    }

    private async Task EnsureNameIsAvailableAsync(
        int maKhuVuc,
        string tenBan,
        int? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await _context.BanAns.AnyAsync(
            ban => ban.MaKhuVuc == maKhuVuc &&
                   ban.TenBan == tenBan &&
                   (!excludedId.HasValue || ban.MaBan != excludedId.Value),
            cancellationToken);
        if (exists)
        {
            throw new DuplicateBanNameException(tenBan);
        }
    }

    private async Task SaveChangesAsync(
        int maKhuVuc,
        string tenBan,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsSqlError(exception, 2601, 2627))
        {
            throw new DuplicateBanNameException(tenBan, exception);
        }
        catch (DbUpdateException exception) when (IsSqlError(exception, 547))
        {
            throw new InvalidBanAreaException(maKhuVuc, exception);
        }
    }

    private static bool IsSqlError(DbUpdateException exception, params int[] numbers)
    {
        for (var cause = exception.InnerException; cause is not null; cause = cause.InnerException)
        {
            if (cause is SqlException sqlException && numbers.Contains(sqlException.Number))
            {
                return true;
            }
        }

        return false;
    }
}
