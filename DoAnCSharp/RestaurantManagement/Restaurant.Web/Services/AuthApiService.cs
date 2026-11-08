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
        var error = await ReadErrorAsync(response, cancellationToken);
        return AuthApiResult.Failed(error);
    }

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(
                cancellationToken: cancellationToken);
            return error?.Message ?? "Đăng nhập thất bại.";
        }
        catch (JsonException)
        {
            return "Không thể kết nối đến API xác thực.";
        }
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; init; }
    }
}
