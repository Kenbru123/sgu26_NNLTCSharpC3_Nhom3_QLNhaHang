using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Authentication;
using Restaurant.Application.DTOs;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
[Route("api/goi-mon")]
public sealed class GoiMonController : ControllerBase
{
    private readonly IGoiMonService _goiMonService;

    public GoiMonController(IGoiMonService goiMonService)
    {
        _goiMonService = goiMonService;
    }

    [HttpGet("ban-an")]
    public async Task<ActionResult<IReadOnlyList<GoiMonBanDto>>> GetTables(
        CancellationToken cancellationToken)
    {
        return Ok(await _goiMonService.GetTablesAsync(cancellationToken));
    }

    [HttpGet("danh-muc")]
    public async Task<ActionResult<IReadOnlyList<GoiMonDanhMucDto>>> GetCategories(
        CancellationToken cancellationToken)
    {
        return Ok(await _goiMonService.GetCategoriesAsync(cancellationToken));
    }

    [HttpGet("mon-an")]
    public async Task<ActionResult<IReadOnlyList<GoiMonMonAnDto>>> SearchFoods(
        [FromQuery] string? search,
        [FromQuery] int? maDanhMuc,
        CancellationToken cancellationToken)
    {
        return Ok(await _goiMonService.SearchFoodsAsync(
            search,
            maDanhMuc,
            cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GoiMonPhieuDto>>> GetOrders(
        CancellationToken cancellationToken)
    {
        return Ok(await _goiMonService.GetOrdersAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GoiMonPhieuChiTietDto>> GetOrderById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _goiMonService.GetOrderByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _goiMonService.DeleteOrderAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<GoiMonPhieuDto>> CreateOrder(
        TaoPhieuGoiMonRequest request,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var accountId))
        {
            return Unauthorized();
        }

        try
        {
            var order = await _goiMonService.CreateOrderAsync(
                accountId,
                request,
                cancellationToken);
            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = order.MaDonHang },
                order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GoiMonPhieuDto>> UpdateOrder(
        int id,
        CapNhatPhieuGoiMonRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _goiMonService.UpdateOrderAsync(
                id,
                request,
                cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPatch("{id:int}/trang-thai")]
    public async Task<ActionResult<GoiMonPhieuDto>> UpdateOrderStatus(
        int id,
        CapNhatTrangThaiPhieuRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _goiMonService.UpdateOrderStatusAsync(
                id,
                request,
                cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPost("{id:int}/thanh-toan")]
    public async Task<ActionResult<GoiMonPhieuDto>> PayOrder(
        int id,
        ThanhToanPhieuRequest request,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var accountId))
        {
            return Unauthorized();
        }

        try
        {
            var order = await _goiMonService.PayOrderAsync(
                id,
                accountId,
                request,
                cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPost("{id:int}/mon-an")]
    public async Task<ActionResult<GoiMonPhieuChiTietDto>> AddFood(
        int id,
        ThemMonVaoPhieuRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _goiMonService.AddFoodAsync(
                id,
                request,
                cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPut("{id:int}/mon-an/{detailId:int}")]
    public async Task<ActionResult<GoiMonPhieuChiTietDto>> UpdateFoodQuantity(
        int id,
        int detailId,
        CapNhatSoLuongMonRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _goiMonService.UpdateFoodQuantityAsync(
                id,
                detailId,
                request,
                cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpDelete("{id:int}/mon-an/{detailId:int}")]
    public async Task<ActionResult<GoiMonPhieuChiTietDto>> RemoveFood(
        int id,
        int detailId,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _goiMonService.RemoveFoodAsync(
                id,
                detailId,
                cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (GoiMonNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPatch("{id:int}/mon-an/{detailId:int}/trang-thai")]
    public async Task<ActionResult<GoiMonPhieuChiTietDto>> UpdateFoodStatus(
        int id,
        int detailId,
        CapNhatTrangThaiMonRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _goiMonService.UpdateFoodStatusAsync(
                id,
                detailId,
                request,
                cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
        catch (GoiMonConflictException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }
}
