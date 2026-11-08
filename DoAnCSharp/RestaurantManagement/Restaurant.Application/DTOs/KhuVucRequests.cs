using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

public sealed record CreateKhuVucRequest
{
    [Required, StringLength(100)]
    public required string TenKhuVuc { get; init; }

    [StringLength(255)]
    public string? MoTa { get; init; }
}

public sealed record UpdateKhuVucRequest
{
    [Required, StringLength(100)]
    public required string TenKhuVuc { get; init; }

    [StringLength(255)]
    public string? MoTa { get; init; }
}
