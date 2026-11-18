using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Web.Models;
using Restaurant.Web.Services;

namespace Restaurant.Web.Controllers;

[Authorize]
public sealed class DatBanController : Controller
{
    private readonly IDatBanApiService _datBanApiService;

    public DatBanController(IDatBanApiService datBanApiService)
    {
        _datBanApiService = datBanApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var history = await _datBanApiService.GetMyReservationsAsync(cancellationToken);
        return View(new DatBanViewModel { LichSu = history });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        DatBanViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.LichSu = await GetHistoryAsync(cancellationToken);
            return View(model);
        }

        try
        {
            await _datBanApiService.CreateAsync(
                new CreateDatBanRequest
                {
                    ThoiGianDen = model.ThoiGianDen,
                    SoLuongKhach = model.SoLuongKhach,
                    GhiChu = model.GhiChu
                },
                cancellationToken);

            ViewData["SuccessMessage"] =
                "Đặt bàn thành công. Nhà hàng sẽ xác nhận yêu cầu của bạn sớm nhất.";
            return View(new DatBanViewModel
            {
                ThoiGianDen = DateTime.Now.AddHours(1),
                SoLuongKhach = 2,
                LichSu = await GetHistoryAsync(cancellationToken)
            });
        }
        catch (HttpRequestException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            model.LichSu = await GetHistoryAsync(cancellationToken);
            return View(model);
        }
    }

    private async Task<IReadOnlyList<DatBanResponse>> GetHistoryAsync(
        CancellationToken cancellationToken)
    {
        return await _datBanApiService.GetMyReservationsAsync(cancellationToken);
    }
}
