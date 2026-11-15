using System.Net.Http.Json;
using System.Text.Json;
using Restaurant.Application.DTOs.Account;

namespace Restaurant.Admin.Services;

public sealed class AccountApiService
{
    private readonly HttpClient _httpClient;

    public AccountApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7053/")
        };
        ApiSession.ApplyAuthorization(_httpClient);
    }

    public Task<List<EmployeeAccountResponse>> GetEmployeesAsync() =>
        GetAsync<List<EmployeeAccountResponse>>("api/accounts/employees");

    public Task<EmployeeAccountResponse> CreateEmployeeAsync(CreateEmployeeAccountRequest request) =>
        SendAsync<EmployeeAccountResponse>(HttpMethod.Post, "api/accounts/employees", request);

    private async Task<T> GetAsync<T>(string path)
    {
        using var response = await _httpClient.GetAsync(path);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("API không trả về dữ liệu như mong đợi.");
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object request)
    {
        using var message = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(request)
        };
        using var response = await _httpClient.SendAsync(message);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("API không trả về dữ liệu như mong đợi.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        var message = response.ReasonPhrase ?? "Lỗi gọi API.";
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;
                if (root.TryGetProperty("message", out var error) && error.ValueKind == JsonValueKind.String)
                {
                    message = error.GetString() ?? message;
                }
                else if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
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
}
