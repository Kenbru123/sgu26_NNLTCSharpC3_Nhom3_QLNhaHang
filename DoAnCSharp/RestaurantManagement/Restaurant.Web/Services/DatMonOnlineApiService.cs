using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services;

public sealed class OnlineOrderApiService : IOnlineOrderApiService
{
    private readonly HttpClient _httpClient;

    public OnlineOrderApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OnlineOrderResponse> CreateAsync(
        CreateOnlineOrderRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/orders/online",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = response.ReasonPhrase ?? "Không thể tạo đơn hàng.";
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var document = JsonDocument.Parse(body);
                    if (document.RootElement.TryGetProperty("detail", out var detail) &&
                        detail.ValueKind == JsonValueKind.String)
                    {
                        message = detail.GetString() ?? message;
                    }
                    else if (document.RootElement.TryGetProperty("title", out var title) &&
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

        return await response.Content.ReadFromJsonAsync<OnlineOrderResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException(
                   "API không trả về thông tin đơn hàng.");
    }

    public async Task<IReadOnlyList<OnlineOrderResponse>> GetMyOrdersAsync(
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(
            "api/orders/online/cua-toi", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OnlineOrderResponse>>(
                   cancellationToken: cancellationToken)
               ?? [];
    }

    public async Task<IReadOnlyList<OnlineOrderResponse>?> LookupAsync(
        GuestOrderLookupRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/orders/online/tra-cuu", request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OnlineOrderResponse>>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về danh sách đơn hàng.");
    }
}
