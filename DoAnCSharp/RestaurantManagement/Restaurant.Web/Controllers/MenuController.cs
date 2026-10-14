using Microsoft.AspNetCore.Mvc;
using Restaurant.Web.Services;

namespace Restaurant.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMonAnApiService _monAnApiService;

        public MenuController(IMonAnApiService monAnApiService)
        {
            _monAnApiService = monAnApiService;
        }

        public async Task<IActionResult> Index()
        {
            var foods = await _monAnApiService.GetAllAsync();

            return View(foods);
        }
    }
}