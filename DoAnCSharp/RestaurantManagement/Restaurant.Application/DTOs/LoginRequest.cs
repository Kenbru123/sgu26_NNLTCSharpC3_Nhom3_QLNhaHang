using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

/// <summary>Credentials submitted to sign in to the restaurant management API.</summary>
public sealed record LoginRequest
{
    /// <summary>Account username.</summary>
    [Required, StringLength(80)]
    public required string TenDangNhap { get; init; }

    /// <summary>Account password.</summary>
    [Required, StringLength(200, MinimumLength = 1)]
    public required string MatKhau { get; init; }
}
