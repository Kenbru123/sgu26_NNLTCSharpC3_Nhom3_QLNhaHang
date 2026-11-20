using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs;

public sealed record CustomerProfileResponse(
    int CustomerId,
    string UserName,
    string HoTen,
    string SDT,
    string? Email);

public sealed record UpdateCustomerProfileRequest
{
    [Required, StringLength(100)]
    public required string HoTen { get; init; }

    [Required, StringLength(20)]
    public required string SDT { get; init; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; init; }
}

public sealed record ChangeCustomerPasswordRequest
{
    [Required]
    public required string MatKhauHienTai { get; init; }

    [Required, MinLength(6), StringLength(200)]
    public required string MatKhauMoi { get; init; }
}

public sealed record AccountOperationResult(bool Succeeded, string? Error = null);
