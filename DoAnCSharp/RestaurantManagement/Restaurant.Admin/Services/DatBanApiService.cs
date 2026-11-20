using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class DatBanApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7053/")
    };

    public DatBanApiService()
    {
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public async Task<List<DatBanManagementResponse>> GetAllAsync(
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var path = string.IsNullOrWhiteSpace(status) || status == "TatCa"
            ? "api/dat-ban"
            : $"api/dat-ban?status={Uri.EscapeDataString(status)}";
        using var response = await _httpClient.GetAsync(path, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<DatBanManagementResponse>>(
                   cancellationToken: cancellationToken)
               ?? [];
    }

    public async Task<DatBanManagementResponse> UpdateStatusAsync(
        int id,
        string status,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PatchAsJsonAsync(
            $"api/dat-ban/{id}/trang-thai",
            new UpdateDatBanStatusRequest { TrangThai = status },
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<DatBanManagementResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về đặt bàn đã cập nhật.");
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
