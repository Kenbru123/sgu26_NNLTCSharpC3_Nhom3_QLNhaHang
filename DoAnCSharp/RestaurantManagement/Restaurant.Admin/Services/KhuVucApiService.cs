using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class KhuVucApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7053/")
    };

    public KhuVucApiService()
    {
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public async Task<List<KhuVucDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            "api/khu-vuc",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<KhuVucDto>>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về danh sách khu vực.");
    }

    public async Task<KhuVucDto> CreateAsync(
        CreateKhuVucRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/khu-vuc",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await ReadKhuVucAsync(response, cancellationToken);
    }

    public async Task<KhuVucDto> UpdateAsync(
        int id,
        UpdateKhuVucRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/khu-vuc/{id}",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await ReadKhuVucAsync(response, cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync(
            $"api/khu-vuc/{id}",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task<KhuVucDto> ReadKhuVucAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return await response.Content.ReadFromJsonAsync<KhuVucDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về khu vực vừa lưu.");
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
        var message = body;
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("detail", out var detail))
            {
                message = detail.GetString() ?? body;
            }
            else if (document.RootElement.TryGetProperty("title", out var title))
            {
                message = title.GetString() ?? body;
            }
        }
        catch (JsonException)
        {
        }

        throw new HttpRequestException(
            $"API trả về {(int)response.StatusCode} ({response.StatusCode}): {message}",
            null,
            response.StatusCode);
    }
}
