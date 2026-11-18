using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs;

namespace Restaurant.Web.Services;

public sealed class CustomerAccountApiService : ICustomerAccountApiService
{
    private readonly HttpClient _httpClient;

    public CustomerAccountApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerProfileResponse> GetProfileAsync(
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(
            "api/account/profile",
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<CustomerProfileResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về thông tin tài khoản.");
    }

    public async Task<CustomerProfileResponse> UpdateProfileAsync(
        UpdateCustomerProfileRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            "api/account/profile",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<CustomerProfileResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("API không trả về hồ sơ đã cập nhật.");
    }

    public async Task ChangePasswordAsync(
        ChangeCustomerPasswordRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/account/change-password",
            request,
            cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = response.ReasonPhrase ?? "Không thể xử lý tài khoản.";
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
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
