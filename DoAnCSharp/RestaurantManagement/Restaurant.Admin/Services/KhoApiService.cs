using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class KhoApiService
{
    private readonly HttpClient _httpClient;

    public KhoApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7053/")
        };
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public Task<List<NguyenLieuDto>> GetIngredientsAsync() =>
        GetAsync<List<NguyenLieuDto>>("api/kho/nguyen-lieu");

    public Task<List<NguyenLieuDto>> GetLowStockIngredientsAsync() =>
        GetAsync<List<NguyenLieuDto>>("api/kho/nguyen-lieu/ton-thap");

    public Task<NguyenLieuDto> CreateIngredientAsync(CreateNguyenLieuRequest request) =>
        SendAsync<NguyenLieuDto>(HttpMethod.Post, "api/kho/nguyen-lieu", request);

    public Task<PhieuNhapKhoDto> CreatePurchaseReceiptAsync(CreatePhieuNhapRequest request) =>
        SendAsync<PhieuNhapKhoDto>(HttpMethod.Post, "api/kho/nhap", request);

    public Task<PhieuXuatKhoDto> CreateIssueReceiptAsync(CreatePhieuXuatRequest request) =>
        SendAsync<PhieuXuatKhoDto>(HttpMethod.Post, "api/kho/xuat", request);

    public Task<List<PhieuNhapKhoDto>> GetPurchaseReceiptsAsync() =>
        GetAsync<List<PhieuNhapKhoDto>>("api/kho/nhap");

    public Task<List<PhieuXuatKhoDto>> GetIssueReceiptsAsync() =>
        GetAsync<List<PhieuXuatKhoDto>>("api/kho/xuat");

    private async Task<T> GetAsync<T>(string path)
        where T : class
    {
        using var response = await _httpClient.GetAsync(path);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("API không trả về dữ liệu như mong đợi.");
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? request = null)
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
            ?? throw new InvalidOperationException("API không trả về dữ liệu như mong đợi.");
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
                if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                {
                    message = title.GetString() ?? message;
                }
                else if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
                {
                    message = detail.GetString() ?? message;
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
