using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Web.Models;
using Restaurant.Web.Services;

namespace Restaurant.Web.Controllers;

public sealed class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IMonAnApiService _foodApiService;
    private readonly IOnlineOrderApiService _orderApiService;

    public CartController(
        ICartService cartService,
        IMonAnApiService foodApiService,
        IOnlineOrderApiService orderApiService)
    {
        _cartService = cartService;
        _foodApiService = foodApiService;
        _orderApiService = orderApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var cartItems = _cartService.GetItems();
        var foods = await _foodApiService.GetAllAsync(
            cancellationToken: cancellationToken);
        var foodById = foods.ToDictionary(food => food.MaMon);

        return View("~/Views/Cart/GioHang.cshtml", new CartViewModel
        {
            Items = cartItems
                .Select(item => new CartLineViewModel(
                    item.MaMon,
                    item.SoLuong,
                    foodById.TryGetValue(item.MaMon, out var food) ? food : null))
                .ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        int foodId,
        int quantity = 1,
        CancellationToken cancellationToken = default)
    {
        if (quantity is < 1 or > 99)
        {
            TempData["CartError"] = "Số lượng phải từ 1 đến 99.";
            return RedirectToAction("Index", "Menu");
        }

        if (await _foodApiService.GetByIdAsync(foodId, cancellationToken) is null)
        {
            TempData["CartError"] = "Món ăn không tồn tại hoặc đã ngừng bán.";
            return RedirectToAction("Index", "Menu");
        }

        _cartService.Add(foodId, quantity);
        TempData["CartMessage"] = "Đã thêm món vào giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int foodId, int quantity)
    {
        if (quantity is < 1 or > 99)
        {
            TempData["CartError"] = "Số lượng phải từ 1 đến 99.";
            return RedirectToAction(nameof(Index));
        }

        _cartService.SetQuantity(foodId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int foodId)
    {
        _cartService.Remove(foodId);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
    {
        if (!await HasAvailableItemsAsync(cancellationToken))
        {
            TempData["CartError"] =
                "Giỏ hàng đang trống hoặc có món đã ngừng bán.";
            return RedirectToAction(nameof(Index));
        }

        return View("~/Views/Cart/ThongTinDatHang.cshtml", new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(
        CheckoutViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.LoaiDon == "OnlineGiaoHang" &&
            string.IsNullOrWhiteSpace(model.DiaChiGiao))
        {
            ModelState.AddModelError(
                nameof(model.DiaChiGiao),
                "Vui lòng nhập địa chỉ giao hàng.");
        }

        if (!await HasAvailableItemsAsync(cancellationToken))
        {
            ModelState.AddModelError(
                string.Empty,
                "Giỏ hàng đang trống hoặc có món đã ngừng bán.");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Cart/ThongTinDatHang.cshtml", model);
        }

        var request = new CreateOnlineOrderRequest
        {
            LoaiDon = model.LoaiDon,
            TenNguoiNhan = model.TenNguoiNhan.Trim(),
            SDTNguoiNhan = model.SDTNguoiNhan.Trim(),
            DiaChiGiao = model.LoaiDon == "OnlineGiaoHang"
                ? model.DiaChiGiao?.Trim()
                : null,
            GhiChu = string.IsNullOrWhiteSpace(model.GhiChu)
                ? null
                : model.GhiChu.Trim(),
            MonAn = _cartService.GetItems()
                .Select(item => new OnlineOrderItemRequest(
                    item.MaMon,
                    item.SoLuong))
                .ToList()
        };

        try
        {
            var order = await _orderApiService.CreateAsync(
                request,
                cancellationToken);
            _cartService.Clear();
            return View("~/Views/Cart/XacNhanDonHang.cshtml", order);
        }
        catch (HttpRequestException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Cart/ThongTinDatHang.cshtml", model);
        }
    }

    private async Task<bool> HasAvailableItemsAsync(
        CancellationToken cancellationToken)
    {
        var cartItems = _cartService.GetItems();
        if (cartItems.Count == 0)
        {
            return false;
        }

        var foods = await _foodApiService.GetAllAsync(
            cancellationToken: cancellationToken);
        var activeFoodIds = foods.Select(food => food.MaMon).ToHashSet();
        return cartItems.All(item => activeFoodIds.Contains(item.MaMon));
    }
}
