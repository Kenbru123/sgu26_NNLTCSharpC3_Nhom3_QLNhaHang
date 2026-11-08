using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Restaurant.API.Authentication;
using Restaurant.Application.DTOs;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
[Route("api/khu-vuc")]
public sealed class KhuVucController : ControllerBase
{
    private readonly IKhuVucService _khuVucService;

    public KhuVucController(IKhuVucService khuVucService)
    {
        _khuVucService = khuVucService;
    }

    /// <summary>Returns all restaurant areas.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<KhuVucDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<KhuVucDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(await _khuVucService.GetAllAsync(cancellationToken));
    }

    /// <summary>Returns one restaurant area by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<KhuVucDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KhuVucDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var khuVuc = await _khuVucService.GetByIdAsync(id, cancellationToken);
        return khuVuc is null ? NotFound() : Ok(khuVuc);
    }

    /// <summary>Creates a restaurant area.</summary>
    [HttpPost]
    [Authorize(Roles = RestaurantRoles.Manager)]
    [ProducesResponseType<KhuVucDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<KhuVucDto>> Create(
        CreateKhuVucRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenKhuVuc))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tên khu vực không được để trống."
            });
        }

        try
        {
            var khuVuc = await _khuVucService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = khuVuc.MaKhuVuc }, khuVuc);
        }
        catch (DuplicateKhuVucNameException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    /// <summary>Updates a restaurant area.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = RestaurantRoles.Manager)]
    [ProducesResponseType<KhuVucDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<KhuVucDto>> Update(
        int id,
        UpdateKhuVucRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenKhuVuc))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tên khu vực không được để trống."
            });
        }

        try
        {
            var khuVuc = await _khuVucService.UpdateAsync(id, request, cancellationToken);
            return khuVuc is null ? NotFound() : Ok(khuVuc);
        }
        catch (DuplicateKhuVucNameException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    /// <summary>Deletes an area that does not contain any tables.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = RestaurantRoles.Manager)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _khuVucService.DeleteAsync(id, cancellationToken);
        return result switch
        {
            KhuVucDeleteResult.Deleted => NoContent(),
            KhuVucDeleteResult.NotFound => NotFound(),
            KhuVucDeleteResult.HasTables => Conflict(new ProblemDetails
            {
                Title = "Không thể xóa khu vực đang có bàn.",
                Detail = "Hãy chuyển hoặc xóa các bàn thuộc khu vực này trước."
            }),
            _ => throw new InvalidOperationException("Kết quả xóa khu vực không hợp lệ.")
        };
    }
}
