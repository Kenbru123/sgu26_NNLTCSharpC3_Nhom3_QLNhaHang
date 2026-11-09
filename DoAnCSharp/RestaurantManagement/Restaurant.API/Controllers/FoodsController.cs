
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodsController : ControllerBase
    {
        private readonly IMonAnService _monAnService;

        public FoodsController(IMonAnService monAnService)
        {
            _monAnService = monAnService;
        }

        // Khách hàng chưa đăng nhập vẫn được xem thực đơn.
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFoods()
        {
            var foods = await _monAnService.GetAllAsync();

            return Ok(foods);
        }
    }
}
