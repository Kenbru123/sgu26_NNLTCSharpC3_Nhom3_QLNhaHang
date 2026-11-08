using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

/// <summary>Data required to create a table in an existing area.</summary>
public sealed record CreateBanAnRequest
{
    /// <summary>Identifier of an existing restaurant area.</summary>
    [Range(1, int.MaxValue)]
    public int MaKhuVuc { get; init; }

    /// <summary>Table name, unique within its area.</summary>
    [Required, StringLength(40)]
    public required string TenBan { get; init; }

    /// <summary>Number of seats at the table; must be positive.</summary>
    [Range(1, int.MaxValue)]
    public int SoCho { get; init; }
}

/// <summary>Data used to update an existing table.</summary>
public sealed record UpdateBanAnRequest
{
    /// <summary>Identifier of an existing restaurant area.</summary>
    [Range(1, int.MaxValue)]
    public int MaKhuVuc { get; init; }

    /// <summary>Table name, unique within its area.</summary>
    [Required, StringLength(40)]
    public required string TenBan { get; init; }

    /// <summary>Number of seats at the table; must be positive.</summary>
    [Range(1, int.MaxValue)]
    public int SoCho { get; init; }

}

/// <summary>Data used to change a table's active status.</summary>
public sealed record UpdateBanAnStatusRequest
{
    /// <summary>Whether the table is active.</summary>
    public required bool TrangThaiHoatDong { get; init; }
}
