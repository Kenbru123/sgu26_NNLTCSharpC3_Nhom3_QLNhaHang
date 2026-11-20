using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.API.Authentication;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dat-ban")]
public sealed class DatBanController : ControllerBase
{
    private readonly IDatBanService _datBanService;

    public DatBanController(IDatBanService datBanService)
    {
        _datBanService = datBanService;
    }

    [HttpGet("cua-toi")]
    public async Task<ActionResult<IReadOnlyList<DatBanResponse>>> GetMyReservations(
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirst("customerId")?.Value, out var customerId))
        {
            return Forbid();
        }

        return Ok(await _datBanService.GetByCustomerIdAsync(customerId, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(DatBanResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<DatBanResponse>> Create(
        CreateDatBanRequest request,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirst("customerId")?.Value, out var customerId))
        {
            return Forbid();
        }

        try
        {
            var reservation = await _datBanService.CreateAsync(
                customerId,
                request,
                cancellationToken);
            return CreatedAtAction(nameof(GetMyReservations), reservation);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
    public async Task<ActionResult<IReadOnlyList<DatBanManagementResponse>>> GetAll(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        return Ok(await _datBanService.GetAllAsync(status, cancellationToken));
    }

    [HttpPatch("{id:int}/trang-thai")]
    [Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
    public async Task<ActionResult<DatBanManagementResponse>> UpdateStatus(
        int id,
        UpdateDatBanStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reservation = await _datBanService.UpdateStatusAsync(
                id,
                request.TrangThai,
                cancellationToken);
            return reservation is null ? NotFound() : Ok(reservation);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }
}
