using System.ComponentModel.DataAnnotations;

namespace Restaurant.Application.DTOs.Account;

public sealed class CreateAdminAccountRequest
{
    [Required, StringLength(80, MinimumLength = 1)]
    public string UserName { get; set; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 12)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(120, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public DateOnly? BirthDate { get; set; }
}
