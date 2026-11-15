using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Authentication;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;

namespace Restaurant.API.Controllers;

[ApiController]
[Authorize(Roles = RestaurantRoles.ManagerOrEmployee)]
[Route("api/thanh-toan")]
public sealed class ThanhToanController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly RestaurantDbContext _context;

    public ThanhToanController(
        IPaymentService paymentService,
        RestaurantDbContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    [HttpGet("don-hang/{orderId:int}")]
    public async Task<ActionResult<HoaDonDto>> GetInvoice(
        int orderId,
        CancellationToken cancellationToken)
    {
        var invoice = await _paymentService.GetInvoiceAsync(orderId, cancellationToken);
        return invoice is null ? NotFound() : Ok(invoice);
    }

    [HttpPost("don-hang/{orderId:int}")]
    public async Task<ActionResult<HoaDonDto>> CreateOrUpdateInvoice(
        int orderId,
        PaymentInvoiceRequest request,
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
            var employee = await GetEmployeeIdAsync(accountId, cancellationToken);
            var invoice = await _paymentService.CreateOrUpdateInvoiceAsync(
                orderId,
                employee,
                request,
                cancellationToken);
            return Ok(invoice);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message });
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

    [HttpPatch("don-hang/{orderId:int}/trang-thai-giao")]
    public async Task<ActionResult<HoaDonDto>> UpdateDeliveryStatus(
        int orderId,
        [FromBody] string trangThaiGiao,
        CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _paymentService.UpdateDeliveryStatusAsync(
                orderId,
                trangThaiGiao,
                cancellationToken);
            return invoice is null ? NotFound() : Ok(invoice);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message });
        }
    }

    private async Task<int> GetEmployeeIdAsync(
        int accountId,
        CancellationToken cancellationToken)
    {
        var employeeId = await _context.TaiKhoans
            .Where(account => account.MaTaiKhoan == accountId)
            .Select(account => account.MaNhanVien)
            .SingleOrDefaultAsync(cancellationToken);

        if (!employeeId.HasValue)
        {
            throw new InvalidOperationException(
                "Tài khoản chưa được liên kết với hồ sơ nhân viên.");
        }

        return employeeId.Value;
    }
}
