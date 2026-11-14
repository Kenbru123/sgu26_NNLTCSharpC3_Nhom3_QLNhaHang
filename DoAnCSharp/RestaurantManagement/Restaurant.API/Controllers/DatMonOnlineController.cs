using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/orders/online")]
public sealed class OnlineOrdersController : ControllerBase
{
    private readonly IOnlineOrderService _onlineOrderService;

    public OnlineOrdersController(IOnlineOrderService onlineOrderService)
    {
        _onlineOrderService = onlineOrderService;
    }

    [HttpPost]
    [AllowAnonymous]
    [EndpointSummary("Tạo đơn đặt món online")]
    [ProducesResponseType(typeof(OnlineOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OnlineOrderResponse>> Create(
        CreateOnlineOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _onlineOrderService.CreateAsync(
                request,
                cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = order.MaDonHang },
                order);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Thông tin đặt món không hợp lệ.",
                Detail = exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Món ăn không khả dụng.",
                Detail = exception.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Xem đơn đặt món online")]
    [ProducesResponseType(typeof(OnlineOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OnlineOrderResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _onlineOrderService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }
}
