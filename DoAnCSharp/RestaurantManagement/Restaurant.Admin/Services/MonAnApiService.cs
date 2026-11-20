using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class MonAnApiService
{
    private readonly HttpClient _httpClient;

    public MonAnApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7053/")
        };
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public Task<List<MonAnDto>> GetAllAsync() =>
        GetAsync<List<MonAnDto>>(
            ApiSession.IsManager ? "api/foods/admin" : "api/foods");

    public Task<List<FoodCategoryResponse>> GetCategoriesAsync() =>
        GetAsync<List<FoodCategoryResponse>>(
            ApiSession.IsManager
                ? "api/foods/admin/categories"
                : "api/foods/categories");

    public Task<FoodCategoryResponse> CreateCategoryAsync(
        CreateFoodCategoryRequest request) =>
        SendAsync<FoodCategoryResponse>(
            HttpMethod.Post,
            "api/foods/categories",
            request);

    public Task<MonAnDto> CreateAsync(CreateFoodRequest request) =>
        SendAsync<MonAnDto>(HttpMethod.Post, "api/foods", request);

    public Task<MonAnDto> UpdateAsync(int id, UpdateFoodRequest request) =>
        SendAsync<MonAnDto>(HttpMethod.Put, $"api/foods/{id}", request);

    public Task DeleteAsync(int id) =>
        SendWithoutResponseAsync(HttpMethod.Delete, $"api/foods/{id}");

    private async Task<T> GetAsync<T>(string path)
        where T : class
    {
        using var response = await _httpClient.GetAsync(path);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException(
                "API không trả về dữ liệu như mong đợi.");
    }

    private async Task<T> SendAsync<T>(
        HttpMethod method,
        string path,
        object request)
        where T : class
    {
        using var message = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(request)
        };
        using var response = await _httpClient.SendAsync(message);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException(
                "API không trả về dữ liệu như mong đợi.");
    }

    private async Task SendWithoutResponseAsync(HttpMethod method, string path)
    {
        using var request = new HttpRequestMessage(method, path);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        var message = response.ReasonPhrase ?? "Lỗi gọi API.";
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;
                if (root.TryGetProperty("detail", out var detail) &&
                    detail.ValueKind == JsonValueKind.String)
                {
                    message = detail.GetString() ?? message;
                }
                else if (root.TryGetProperty("title", out var title) &&
                         title.ValueKind == JsonValueKind.String)
                {
                    message = title.GetString() ?? message;
                }
            }
            catch (JsonException)
            {
                message = body;
            }
        }

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
