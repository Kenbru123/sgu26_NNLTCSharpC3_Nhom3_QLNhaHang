using Microsoft.AspNetCore.Mvc;
using Restaurant.Web.Models;
using Restaurant.Web.Services;

namespace Restaurant.Web.Controllers;

public sealed class MenuController : Controller
{
    private readonly IMonAnApiService _monAnApiService;

    public MenuController(IMonAnApiService monAnApiService)
    {
        _monAnApiService = monAnApiService;
    }

    public async Task<IActionResult> Index(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        var foods = await _monAnApiService.GetAllAsync(
            search,
            categoryId,
            cancellationToken);
        var categories = await _monAnApiService.GetCategoriesAsync(cancellationToken);

        return View(new MenuViewModel
        {
            Foods = foods,
            Categories = categories,
            Search = search,
            CategoryId = categoryId
        });
    }

    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var food = await _monAnApiService.GetByIdAsync(id, cancellationToken);
        return food is null
            ? NotFound()
            : View("~/Views/Menu/ChiTietMon.cshtml", food);
    }
}
