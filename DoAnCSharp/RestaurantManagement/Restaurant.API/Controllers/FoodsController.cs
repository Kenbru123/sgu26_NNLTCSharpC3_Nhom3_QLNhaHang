using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/foods")]
public sealed class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodsController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("Lấy thực đơn đang bán")]
    [ProducesResponseType(typeof(IReadOnlyList<FoodResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FoodResponse>>> GetMenu(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        return Ok(await _foodService.GetMenuAsync(
            search,
            categoryId,
            cancellationToken));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [EndpointSummary("Xem chi tiết món ăn")]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var food = await _foodService.GetByIdAsync(
            id,
            includeInactive: false,
            cancellationToken);
        return food is null ? NotFound() : Ok(food);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    [EndpointSummary("Lấy danh mục đang hoạt động")]
    [ProducesResponseType(typeof(IReadOnlyList<FoodCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FoodCategoryResponse>>> GetCategories(
        CancellationToken cancellationToken)
    {
        return Ok(await _foodService.GetCategoriesAsync(
            includeInactive: false,
            cancellationToken));
    }

    [HttpGet("admin")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Lấy toàn bộ món ăn để quản lý")]
    [ProducesResponseType(typeof(IReadOnlyList<FoodResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FoodResponse>>> GetAllForManagement(
        CancellationToken cancellationToken)
    {
        return Ok(await _foodService.GetAllForManagementAsync(cancellationToken));
    }

    [HttpGet("admin/categories")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Lấy danh mục để quản lý món ăn")]
    [ProducesResponseType(typeof(IReadOnlyList<FoodCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<FoodCategoryResponse>>> GetCategoriesForManagement(
        CancellationToken cancellationToken)
    {
        return Ok(await _foodService.GetCategoriesAsync(
            includeInactive: false,
            cancellationToken));
    }

    [HttpPost("categories")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Tạo danh mục món ăn")]
    [ProducesResponseType(typeof(FoodCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FoodCategoryResponse>> CreateCategory(
        CreateFoodCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _foodService.CreateCategoryAsync(
                request,
                cancellationToken);
            return StatusCode(StatusCodes.Status201Created, category);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Thông tin danh mục không hợp lệ.",
                Detail = exception.Message
            });
        }
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Xem món ăn trong màn hình quản lý")]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodResponse>> GetByIdForManagement(
        int id,
        CancellationToken cancellationToken)
    {
        var food = await _foodService.GetByIdAsync(
            id,
            includeInactive: true,
            cancellationToken);
        return food is null ? NotFound() : Ok(food);
    }

    [HttpPost]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Tạo món ăn")]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FoodResponse>> Create(
        CreateFoodRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var food = await _foodService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetByIdForManagement),
                new { id = food.MaMon },
                food);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Thông tin món ăn không hợp lệ.",
                Detail = exception.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Cập nhật món ăn")]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodResponse>> Update(
        int id,
        UpdateFoodRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var food = await _foodService.UpdateAsync(id, request, cancellationToken);
            return food is null ? NotFound() : Ok(food);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Thông tin món ăn không hợp lệ.",
                Detail = exception.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ManagementOnly")]
    [EndpointSummary("Ngừng bán món ăn")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        int id,
        CancellationToken cancellationToken)
    {
        return await _foodService.DeactivateAsync(id, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}
