using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Restaurant.API.Authentication;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
[Route("api/kho")]
public sealed class KhoController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public KhoController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("nguyen-lieu")]
    public async Task<ActionResult<IReadOnlyList<NguyenLieuDto>>> GetIngredients(
        CancellationToken cancellationToken)
    {
        return Ok(await _inventoryService.GetIngredientsAsync(cancellationToken));
    }

    [HttpGet("nguyen-lieu/ton-thap")]
    public async Task<ActionResult<IReadOnlyList<NguyenLieuDto>>> GetLowStockIngredients(
        CancellationToken cancellationToken)
    {
        return Ok(await _inventoryService.GetLowStockIngredientsAsync(cancellationToken));
    }

    [HttpGet("nguyen-lieu/{id:int}")]
    public async Task<ActionResult<NguyenLieuDto>> GetIngredientById(
        int id,
        CancellationToken cancellationToken)
    {
        var item = await _inventoryService.GetIngredientByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("nguyen-lieu")]
    public async Task<ActionResult<NguyenLieuDto>> CreateIngredient(
        CreateNguyenLieuRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _inventoryService.CreateIngredientAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetIngredientById), new { id = item.MaNguyenLieu }, item);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPut("nguyen-lieu/{id:int}")]
    public async Task<ActionResult<NguyenLieuDto>> UpdateIngredient(
        int id,
        UpdateNguyenLieuRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _inventoryService.UpdateIngredientAsync(id, request, cancellationToken);
            return item is null ? NotFound() : Ok(item);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpGet("nhap")]
    public async Task<ActionResult<IReadOnlyList<PhieuNhapKhoDto>>> GetPurchaseReceipts(
        CancellationToken cancellationToken)
    {
        return Ok(await _inventoryService.GetPurchaseReceiptsAsync(cancellationToken));
    }

    [HttpGet("xuat")]
    public async Task<ActionResult<IReadOnlyList<PhieuXuatKhoDto>>> GetIssueReceipts(
        CancellationToken cancellationToken)
    {
        return Ok(await _inventoryService.GetIssueReceiptsAsync(cancellationToken));
    }

    [HttpPost("nhap")]
    public async Task<ActionResult<PhieuNhapKhoDto>> CreatePurchaseReceipt(
        CreatePhieuNhapRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employeeClaim = User.FindFirstValue("employeeId");
            if (!int.TryParse(employeeClaim, out var employeeId) || employeeId <= 0)
            {
                return BadRequest(new ProblemDetails { Title = "Tài khoản hiện tại chưa được liên kết với nhân viên." });
            }

            var receipt = await _inventoryService.CreatePurchaseReceiptAsync(
                request with { MaNhanVien = employeeId },
                cancellationToken);
            return Ok(receipt);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPost("xuat")]
    public async Task<ActionResult<PhieuXuatKhoDto>> CreateIssueReceipt(
        CreatePhieuXuatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var receipt = await _inventoryService.CreateIssueReceiptAsync(request, cancellationToken);
            return Ok(receipt);
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

    [HttpPatch("nhap/{receiptId:int}/trang-thai")]
    public async Task<ActionResult<PhieuNhapKhoDto>> UpdatePurchaseReceiptStatus(
        int receiptId,
        [FromBody] string trangThai,
        CancellationToken cancellationToken)
    {
        try
        {
            var receipt = await _inventoryService.UpdatePurchaseReceiptStatusAsync(
                receiptId,
                trangThai,
                cancellationToken);
            return receipt is null ? NotFound() : Ok(receipt);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
    }

    [HttpPatch("xuat/{receiptId:int}/trang-thai")]
    public async Task<ActionResult<PhieuXuatKhoDto>> UpdateIssueReceiptStatus(
        int receiptId,
        [FromBody] string trangThai,
        CancellationToken cancellationToken)
    {
        try
        {
            var receipt = await _inventoryService.UpdateIssueReceiptStatusAsync(
                receiptId,
                trangThai,
                cancellationToken);
            return receipt is null ? NotFound() : Ok(receipt);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
    }
}
