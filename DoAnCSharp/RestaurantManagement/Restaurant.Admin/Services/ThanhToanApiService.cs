using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class ThanhToanApiService
{
    private readonly HttpClient _httpClient;

    public ThanhToanApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7053/")
        };
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public Task<HoaDonDto?> GetInvoiceAsync(int orderId) =>
        GetAsync<HoaDonDto?>($"api/thanh-toan/don-hang/{orderId}");

    public Task<HoaDonDto> CreateOrUpdateInvoiceAsync(int orderId, PaymentInvoiceRequest request) =>
        SendAsync<HoaDonDto>(HttpMethod.Post, $"api/thanh-toan/don-hang/{orderId}", request);

    public Task<HoaDonDto?> UpdateDeliveryStatusAsync(int orderId, string status) =>
        SendAsync<HoaDonDto?>(HttpMethod.Patch, $"api/thanh-toan/don-hang/{orderId}/trang-thai-giao", status);

    private async Task<T> GetAsync<T>(string path)
    {
        using var response = await _httpClient.GetAsync(path);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default!;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("API không trả về dữ liệu như mong đợi.");
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? request = null)
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
                if (document.RootElement.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                {
                    message = title.GetString() ?? message;
                }
                else if (document.RootElement.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String)
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
