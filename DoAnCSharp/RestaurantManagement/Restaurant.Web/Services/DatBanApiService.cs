using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services;

public sealed class DatBanApiService : IDatBanApiService
{
    private readonly HttpClient _httpClient;

    public DatBanApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DatBanResponse> CreateAsync(
        CreateDatBanRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/dat-ban",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<DatBanResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException(
                   "API không trả về thông tin đặt bàn.");
    }

    public async Task<IReadOnlyList<DatBanResponse>> GetMyReservationsAsync(
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(
            "api/dat-ban/cua-toi",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<List<DatBanResponse>>(
                   cancellationToken: cancellationToken)
               ?? [];
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = response.ReasonPhrase ?? "Không thể xử lý đặt bàn.";
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String)
            {
                message = title.GetString() ?? message;
            }
        }
        catch (JsonException)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                message = body;
            }
        }

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
