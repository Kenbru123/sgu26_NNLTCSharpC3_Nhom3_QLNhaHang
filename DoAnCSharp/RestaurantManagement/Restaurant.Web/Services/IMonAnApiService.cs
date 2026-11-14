using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services
{
    public interface IMonAnApiService
    {
        Task<List<MonAnDto>> GetAllAsync(
            string? search = null,
            int? categoryId = null,
            CancellationToken cancellationToken = default);

        Task<List<FoodCategoryResponse>> GetCategoriesAsync(
            CancellationToken cancellationToken = default);

        Task<MonAnDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}