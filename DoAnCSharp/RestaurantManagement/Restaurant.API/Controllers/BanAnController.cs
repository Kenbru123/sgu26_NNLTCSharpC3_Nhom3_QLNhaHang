using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Restaurant.Application.DTOs;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Interfaces;
using Restaurant.API.Authentication;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
[Route("api/ban-an")]
public sealed class BanAnController : ControllerBase
{
    private readonly IBanAnService _banAnService;

    public BanAnController(IBanAnService banAnService)
    {
        _banAnService = banAnService;
    }

    /// <summary>Returns restaurant tables, optionally filtered by area, status, or code/name.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<BanAnDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BanAnDto>>> GetAll(
        [FromQuery] int? maKhuVuc,
        [FromQuery] bool? trangThaiHoatDong,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        return Ok(await _banAnService.GetAllAsync(
            maKhuVuc,
            trangThaiHoatDong,
            search,
            cancellationToken));
    }

    /// <summary>Returns a restaurant table by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<BanAnDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BanAnDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var banAn = await _banAnService.GetByIdAsync(id, cancellationToken);
        return banAn is null ? NotFound() : Ok(banAn);
    }

    /// <summary>Creates a restaurant table in an existing area.</summary>
    [HttpPost]
    [Authorize(Roles = RestaurantRoles.Manager)]
    [ProducesResponseType<BanAnDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BanAnDto>> Create(
        CreateBanAnRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenBan))
        {
            return BadRequest(new ProblemDetails { Title = "Tên bàn không được để trống." });
        }

        try
        {
            var banAn = await _banAnService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = banAn.MaBan }, banAn);
        }
        catch (InvalidBanAreaException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (DuplicateBanNameException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    /// <summary>Updates a restaurant table and its area assignment.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = RestaurantRoles.Manager)]
    [ProducesResponseType<BanAnDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BanAnDto>> Update(
        int id,
        UpdateBanAnRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TenBan))
        {
            return BadRequest(new ProblemDetails { Title = "Tên bàn không được để trống." });
        }

        try
        {
            var banAn = await _banAnService.UpdateAsync(id, request, cancellationToken);
            return banAn is null ? NotFound() : Ok(banAn);
        }
        catch (InvalidBanAreaException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (DuplicateBanNameException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
        catch (BanAnInUseException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    /// <summary>Changes whether a restaurant table is active when it is not in use.</summary>
    [HttpPatch("{id:int}/trang-thai")]
    [ProducesResponseType<BanAnDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateBanAnStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _banAnService.UpdateStatusAsync(
                id,
                request.TrangThaiHoatDong,
                cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (BanAnInUseException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    /// <summary>Deletes a restaurant table that has no related reservation or order records.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = RestaurantRoles.Manager)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _banAnService.DeleteAsync(id, cancellationToken);
        return result switch
        {
            BanAnDeleteResult.Deleted => NoContent(),
            BanAnDeleteResult.NotFound => NotFound(),
            BanAnDeleteResult.InUse => Conflict(new ProblemDetails
            {
                Title = "Không thể xóa bàn đang được sử dụng hoặc còn dữ liệu đơn hàng/đặt bàn liên quan.",
                Detail = "Hãy đổi trạng thái bàn sang ngừng hoạt động nếu không còn sử dụng."
            }),
            _ => throw new InvalidOperationException("Kết quả xóa bàn không hợp lệ.")
        };
    }
}
