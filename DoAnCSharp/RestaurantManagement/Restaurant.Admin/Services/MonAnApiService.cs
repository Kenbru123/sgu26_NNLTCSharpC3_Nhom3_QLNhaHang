using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services
{
    public class MonAnApiService
    {
        private readonly HttpClient _httpClient;

        public MonAnApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7053/")
            };
        }

        public async Task<List<MonAnDto>> GetAllAsync()
        {
            var foods = await _httpClient
                .GetFromJsonAsync<List<MonAnDto>>("api/foods");

            return foods ?? new List<MonAnDto>();
        }
    }
}