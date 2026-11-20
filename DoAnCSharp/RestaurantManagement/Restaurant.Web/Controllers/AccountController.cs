using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs;
using Restaurant.Application.DTOs.Auth;
using AuthLoginRequest = Restaurant.Application.DTOs.Auth.LoginRequest;
using Restaurant.Web.Models;
using Restaurant.Web.Services;

namespace Restaurant.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly ICustomerAccountApiService _customerAccountApiService;

    public AccountController(
        IAuthApiService authApiService,
        ICustomerAccountApiService customerAccountApiService)
    {
        _authApiService = authApiService;
        _customerAccountApiService = customerAccountApiService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        // Hiển thị form đăng ký khách hàng.
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Gửi thông tin đăng ký đến Auth API.
        var result = await _authApiService.RegisterAsync(
            new RegisterRequest
            {
                UserName = model.UserName,
                Password = model.Password,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Đăng ký thất bại.");
            return View(model);
        }

        // Đăng ký xong thì chuyển người dùng sang trang đăng nhập.
        TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
        return RedirectToAction(nameof(Login));
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
            new AuthLoginRequest
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
            new Claim(ClaimTypes.Name, authResponse.UserName),
            new Claim(ClaimTypes.Role, authResponse.Role),
            new Claim("accountId", authResponse.AccountId.ToString()),
            new Claim("access_token", authResponse.AccessToken)
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
                AllowRefresh = false,
                ExpiresUtc = new DateTimeOffset(
                    DateTime.SpecifyKind(
                        authResponse.ExpiresAtUtc,
                        DateTimeKind.Utc))
            });

        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _customerAccountApiService.GetProfileAsync(cancellationToken);
            if (TempData.TryGetValue("AccountSuccess", out var successMessage))
            {
                ViewData["SuccessMessage"] = successMessage;
            }

            return View(new CustomerProfileViewModel
            {
                CustomerId = profile.CustomerId,
                UserName = profile.UserName,
                HoTen = profile.HoTen,
                SDT = profile.SDT,
                Email = profile.Email
            });
        }
        catch (HttpRequestException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(new CustomerProfileViewModel());
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(
        CustomerProfileViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Profile", model);
        }

        try
        {
            await _customerAccountApiService.UpdateProfileAsync(
                new UpdateCustomerProfileRequest
                {
                    HoTen = model.HoTen,
                    SDT = model.SDT,
                    Email = model.Email
                },
                cancellationToken);
            TempData["AccountSuccess"] = "Đã cập nhật thông tin tài khoản.";
            return RedirectToAction(nameof(Profile));
        }
        catch (HttpRequestException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Profile", model);
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _customerAccountApiService.ChangePasswordAsync(
                new ChangeCustomerPasswordRequest
                {
                    MatKhauHienTai = model.MatKhauHienTai,
                    MatKhauMoi = model.MatKhauMoi
                },
                cancellationToken);
            TempData["AccountSuccess"] = "Đã đổi mật khẩu thành công.";
            return RedirectToAction(nameof(Profile));
        }
        catch (HttpRequestException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
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
