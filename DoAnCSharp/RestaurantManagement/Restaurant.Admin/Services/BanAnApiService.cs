using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class BanAnApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7053/")
    };

    public BanAnApiService()
    {
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public async Task<List<BanAnDto>> GetAllAsync(
        int? maKhuVuc = null,
        bool? trangThaiHoatDong = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (maKhuVuc.HasValue)
        {
            query.Add($"maKhuVuc={maKhuVuc.Value}");
        }

        if (trangThaiHoatDong.HasValue)
        {
            query.Add($"trangThaiHoatDong={trangThaiHoatDong.Value.ToString().ToLowerInvariant()}");
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        var path = "api/ban-an";
        if (query.Count > 0)
        {
            path += "?" + string.Join("&", query);
        }

        using var response = await _httpClient.GetAsync(path, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<BanAnDto>>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về danh sách bàn ăn.");
    }

    public async Task<BanAnDto> CreateAsync(
        CreateBanAnRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/ban-an",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await ReadBanAnAsync(response, cancellationToken);
    }

    public async Task<BanAnDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/ban-an/{id}",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await ReadBanAnAsync(response, cancellationToken);
    }

    public async Task<BanAnDto> UpdateAsync(
        int id,
        UpdateBanAnRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/ban-an/{id}",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await ReadBanAnAsync(response, cancellationToken);
    }

    public async Task<BanAnDto> UpdateStatusAsync(
        int id,
        UpdateBanAnStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PatchAsJsonAsync(
            $"api/ban-an/{id}/trang-thai",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await ReadBanAnAsync(response, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync(
            $"api/ban-an/{id}",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task<BanAnDto> ReadBanAnAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        return await response.Content.ReadFromJsonAsync<BanAnDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về bàn ăn vừa lưu.");
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
