using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin.Services;

public sealed class AuthApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7053/")
    };

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            request,
            cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = body;
            try
            {
                using var document = JsonDocument.Parse(body);
                if (document.RootElement.TryGetProperty("title", out var title))
                {
                    message = title.GetString() ?? body;
                }
            }
            catch (JsonException)
            {
            }

            throw new HttpRequestException(
                $"Đăng nhập thất bại ({(int)response.StatusCode}): {message}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<LoginResponseDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về thông tin phiên đăng nhập.");
    }
}
