using System.Net.Http.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class OnlineOrderApiService
{
    private readonly HttpClient _httpClient;

    public OnlineOrderApiService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7053/") };
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public async Task<List<OnlineOrderResponse>> GetAllAsync()
    {
        using var response = await _httpClient.GetAsync("api/orders/online");
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Không thể tải đơn online ({(int)response.StatusCode}).");
        }

        return await response.Content.ReadFromJsonAsync<List<OnlineOrderResponse>>()
            ?? throw new InvalidOperationException("API không trả về danh sách đơn online.");
    }

    public async Task<OnlineOrderResponse> UpdateStatusAsync(int orderId, string status)
    {
        using var response = await _httpClient.PatchAsJsonAsync($"api/orders/online/{orderId}/trang-thai", status);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Không thể cập nhật đơn online ({(int)response.StatusCode}).");
        }

        return await response.Content.ReadFromJsonAsync<OnlineOrderResponse>()
            ?? throw new InvalidOperationException("API không trả về đơn online đã cập nhật.");
    }

    public async Task<OnlineOrderResponse> UpdateDeliveryStatusAsync(int orderId, string status)
    {
        using var response = await _httpClient.PatchAsJsonAsync(
            $"api/orders/online/{orderId}/trang-thai-giao", status);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Không thể cập nhật giao hàng ({(int)response.StatusCode}).");
        }

        return await response.Content.ReadFromJsonAsync<OnlineOrderResponse>()
            ?? throw new InvalidOperationException("API không trả về đơn online đã cập nhật.");
    }
}
