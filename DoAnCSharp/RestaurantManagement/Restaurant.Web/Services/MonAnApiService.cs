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

        public async Task<List<MonAnDto>> GetAllAsync()
        {
            var foods = await _httpClient
                .GetFromJsonAsync<List<MonAnDto>>("api/foods");

            return foods ?? new List<MonAnDto>();
        }
    }
}