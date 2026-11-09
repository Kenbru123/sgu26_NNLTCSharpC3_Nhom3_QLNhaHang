using System.Net.Http.Headers;
using System.Security.Claims;

namespace Restaurant.Web.Services;

public sealed class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Lấy JWT từ claim đã được lưu trong cookie đăng nhập Web.
        var accessToken = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue("access_token");

        // Gắn JWT vào request để API thực hiện xác thực và phân quyền.
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
