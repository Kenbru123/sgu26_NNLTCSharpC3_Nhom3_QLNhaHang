using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs.Auth;

namespace Restaurant.Web.Services;

public sealed class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthApiResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        // Gọi API đăng nhập và nhận JWT từ backend.
        using var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            request,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(
                cancellationToken: cancellationToken);

            return authResponse is null
                ? AuthApiResult.Failed("API không trả về thông tin đăng nhập.")
                : AuthApiResult.Success(authResponse);
        }

        // Đọc message lỗi do API trả về để hiển thị trên giao diện.
        var error = await ReadErrorAsync(response, cancellationToken, "Đăng nhập thất bại.");
        return AuthApiResult.Failed(error);
    }

    public async Task<AuthApiResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // Gọi API đăng ký khách hàng.
        using var response = await _httpClient.PostAsJsonAsync(
            "api/auth/register",
            request,
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(
                cancellationToken: cancellationToken);

            return authResponse is null
                ? AuthApiResult.Failed("API không trả về thông tin đăng ký.")
                : AuthApiResult.Success(authResponse);
        }

        // Đọc lỗi trùng username hoặc dữ liệu đăng ký không hợp lệ.
        var error = await ReadErrorAsync(response, cancellationToken, "Đăng ký thất bại.");
        return AuthApiResult.Failed(error);
    }

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken,
        string fallbackMessage)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(
                cancellationToken: cancellationToken);
            return error?.Message ?? fallbackMessage;
        }
        catch (JsonException)
        {
            return fallbackMessage;
        }
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; init; }
    }
}
