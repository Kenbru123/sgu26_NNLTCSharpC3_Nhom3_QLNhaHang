using System.Net.Http.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services
{
    public class MonAnApiService : IMonAnApiService
    {
        private readonly HttpClient _httpClient;

        public MonAnApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MonAnDto>> GetAllAsync(
            string? search = null,
            int? categoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query.Add($"search={Uri.EscapeDataString(search.Trim())}");
            }

            if (categoryId.HasValue)
            {
                query.Add($"categoryId={categoryId.Value}");
            }

            var path = "api/foods";
            if (query.Count > 0)
            {
                path += "?" + string.Join("&", query);
            }

            var foods = await _httpClient
                .GetFromJsonAsync<List<MonAnDto>>(path, cancellationToken);

            return foods ?? new List<MonAnDto>();
        }

        public async Task<List<FoodCategoryResponse>> GetCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            var categories = await _httpClient
                .GetFromJsonAsync<List<FoodCategoryResponse>>(
                    "api/foods/categories",
                    cancellationToken);

            return categories ?? new List<FoodCategoryResponse>();
        }

        public Task<MonAnDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default) =>
            _httpClient.GetFromJsonAsync<MonAnDto>(
                $"api/foods/{id}",
                cancellationToken);
    }
}