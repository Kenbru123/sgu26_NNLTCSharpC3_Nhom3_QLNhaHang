using System.Net.Http.Headers;

namespace Restaurant.Admin.Services;

public static class ApiSession
{
    public const string ManagerRole = "QuanLy";
    public const string AdminRole = "Admin";
    public const string EmployeeRole = "NhanVien";

    public static string? AccessToken { get; private set; }

    public static string? Role { get; private set; }

    public static bool IsManager => Role is ManagerRole or AdminRole;

    public static void Set(string accessToken, string role)
    {
        AccessToken = accessToken;
        Role = role;
    }

    public static void ApplyAuthorization(HttpClient client)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            throw new InvalidOperationException("Vui lòng đăng nhập trước khi gọi API.");
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AccessToken);
    }
}
