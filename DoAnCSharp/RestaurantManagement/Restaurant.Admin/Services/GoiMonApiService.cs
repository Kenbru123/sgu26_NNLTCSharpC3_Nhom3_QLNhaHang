using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class GoiMonApiService
{
    private readonly HttpClient _httpClient;

    public GoiMonApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7053/")
        };
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public Task<List<GoiMonBanDto>> GetTablesAsync() =>
        GetAsync<List<GoiMonBanDto>>("api/goi-mon/ban-an");

    public Task<List<GoiMonDanhMucDto>> GetCategoriesAsync() =>
        GetAsync<List<GoiMonDanhMucDto>>("api/goi-mon/danh-muc");

    public Task<List<GoiMonMonAnDto>> SearchFoodsAsync(
        string? search,
        int? maDanhMuc)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (maDanhMuc.HasValue)
        {
            query.Add($"maDanhMuc={maDanhMuc.Value}");
        }

        var path = "api/goi-mon/mon-an";
        if (query.Count > 0)
        {
            path += "?" + string.Join("&", query);
        }

        return GetAsync<List<GoiMonMonAnDto>>(path);
    }

    public Task<List<GoiMonPhieuDto>> GetOrdersAsync() =>
        GetAsync<List<GoiMonPhieuDto>>("api/goi-mon");

    public Task<GoiMonPhieuChiTietDto> GetOrderAsync(int id) =>
        GetAsync<GoiMonPhieuChiTietDto>($"api/goi-mon/{id}");

    public Task<GoiMonPhieuDto> CreateOrderAsync(
        TaoPhieuGoiMonRequest request) =>
        SendAsync<GoiMonPhieuDto>(
            HttpMethod.Post,
            "api/goi-mon",
            request);

    public Task<GoiMonPhieuDto> UpdateOrderAsync(
        int id,
        CapNhatPhieuGoiMonRequest request) =>
        SendAsync<GoiMonPhieuDto>(
            HttpMethod.Put,
            $"api/goi-mon/{id}",
            request);

    public Task DeleteOrderAsync(int id) =>
        SendWithoutResponseAsync(
            HttpMethod.Delete,
            $"api/goi-mon/{id}");

    public Task<GoiMonPhieuDto> UpdateOrderStatusAsync(
        int id,
        CapNhatTrangThaiPhieuRequest request) =>
        SendAsync<GoiMonPhieuDto>(
            HttpMethod.Patch,
            $"api/goi-mon/{id}/trang-thai",
            request);

    public Task<GoiMonPhieuDto> PayOrderAsync(
        int id,
        ThanhToanPhieuRequest request) =>
        SendAsync<GoiMonPhieuDto>(
            HttpMethod.Post,
            $"api/goi-mon/{id}/thanh-toan",
            request);

    public Task<GoiMonPhieuChiTietDto> AddFoodAsync(
        int orderId,
        ThemMonVaoPhieuRequest request) =>
        SendAsync<GoiMonPhieuChiTietDto>(
            HttpMethod.Post,
            $"api/goi-mon/{orderId}/mon-an",
            request);

    public Task<GoiMonPhieuChiTietDto> UpdateFoodQuantityAsync(
        int orderId,
        int detailId,
        CapNhatSoLuongMonRequest request) =>
        SendAsync<GoiMonPhieuChiTietDto>(
            HttpMethod.Put,
            $"api/goi-mon/{orderId}/mon-an/{detailId}",
            request);

    public Task<GoiMonPhieuChiTietDto> RemoveFoodAsync(
        int orderId,
        int detailId) =>
        SendAsync<GoiMonPhieuChiTietDto>(
            HttpMethod.Delete,
            $"api/goi-mon/{orderId}/mon-an/{detailId}");

    public Task<GoiMonPhieuChiTietDto> UpdateFoodStatusAsync(
        int orderId,
        int detailId,
        CapNhatTrangThaiMonRequest request) =>
        SendAsync<GoiMonPhieuChiTietDto>(
            HttpMethod.Patch,
            $"api/goi-mon/{orderId}/mon-an/{detailId}/trang-thai",
            request);

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
        object? request = null)
        where T : class
    {
        using var message = new HttpRequestMessage(method, path);
        if (request is not null)
        {
            message.Content = JsonContent.Create(request);
        }

        using var response = await _httpClient.SendAsync(message);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException(
                "API không trả về dữ liệu như mong đợi.");
    }

    private async Task SendWithoutResponseAsync(
        HttpMethod method,
        string path)
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
                if (root.TryGetProperty("title", out var title) &&
                    title.ValueKind == JsonValueKind.String)
                {
                    message = title.GetString() ?? message;
                }
                else if (root.TryGetProperty("message", out var detail) &&
                         detail.ValueKind == JsonValueKind.String)
                {
                    message = detail.GetString() ?? message;
                }
            }
            catch (JsonException)
            {
                message = body;
            }
        }

        throw new HttpRequestException(
            message,
            null,
            response.StatusCode);
    }
}
