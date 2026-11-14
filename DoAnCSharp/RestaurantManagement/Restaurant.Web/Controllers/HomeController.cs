using Microsoft.AspNetCore.Mvc;
using Restaurant.Web.Services;
using Restaurant.Web.Models;
using System.Diagnostics;

namespace Restaurant.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMonAnApiService _monAnApiService;

        public HomeController(IMonAnApiService monAnApiService)
        {
            _monAnApiService = monAnApiService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var foods = await _monAnApiService.GetAllAsync(
                cancellationToken: cancellationToken);
            return View(foods.Take(6).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
