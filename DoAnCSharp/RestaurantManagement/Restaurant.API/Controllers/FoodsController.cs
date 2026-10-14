using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IMonAnService _monAnService;

        public FoodsController(IMonAnService monAnService)
        {
            _monAnService = monAnService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFoods()
        {
            var foods = await _monAnService.GetAllAsync();

            return Ok(foods);
        }
    }
}