using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize(Roles = "KhachHang")]
[Route("api/account")]
public sealed class CustomerAccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public CustomerAccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<CustomerProfileResponse>> GetProfile(
        CancellationToken cancellationToken)
    {
        if (!TryGetCustomerId(out var customerId))
        {
            return Forbid();
        }

        var profile = await _authService.GetCustomerProfileAsync(
            customerId,
            cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<CustomerProfileResponse>> UpdateProfile(
        UpdateCustomerProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCustomerId(out var customerId))
        {
            return Forbid();
        }

        var profile = await _authService.UpdateCustomerProfileAsync(
            customerId,
            request,
            cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangeCustomerPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCustomerId(out var customerId))
        {
            return Forbid();
        }

        var result = await _authService.ChangeCustomerPasswordAsync(
            customerId,
            request,
            cancellationToken);
        return result.Succeeded
            ? NoContent()
            : BadRequest(new ProblemDetails { Title = result.Error });
    }

    private bool TryGetCustomerId(out int customerId)
    {
        return int.TryParse(
            User.FindFirst("customerId")?.Value,
            out customerId);
    }
}
