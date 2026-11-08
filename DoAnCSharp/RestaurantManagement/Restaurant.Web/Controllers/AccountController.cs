using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs.Auth;
using Restaurant.Web.Models;
using Restaurant.Web.Services;

namespace Restaurant.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;

    public AccountController(IAuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Nếu đã đăng nhập thì không cần hiển thị lại trang login.
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Gọi API để kiểm tra username và password.
        var result = await _authApiService.LoginAsync(
            new LoginRequest
            {
                UserName = model.UserName,
                Password = model.Password
            },
            cancellationToken);

        if (!result.Succeeded || result.Response is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Đăng nhập thất bại.");
            return View(model);
        }

        // Chuyển thông tin JWT thành claims để Web sử dụng phân quyền.
        var authResponse = result.Response;
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, authResponse.UserName),
            new(ClaimTypes.Role, authResponse.Role),
            new("accountId", authResponse.AccountId.ToString()),
            new("access_token", authResponse.AccessToken)
        };

        if (authResponse.CustomerId.HasValue)
        {
            claims.Add(new Claim("customerId", authResponse.CustomerId.Value.ToString()));
        }

        if (authResponse.EmployeeId.HasValue)
        {
            claims.Add(new Claim("employeeId", authResponse.EmployeeId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Lưu cookie HttpOnly thay vì lưu password hoặc JWT trong localStorage.
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            });

        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Xóa cookie đăng nhập hiện tại.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("Index", "Home")!;
    }
}
