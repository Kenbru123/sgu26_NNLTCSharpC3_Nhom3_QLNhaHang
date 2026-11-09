
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Authentication;
using Restaurant.API.Commands;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Services;

// Lệnh khởi tạo tài khoản Admin thủ công
if (args.Any(argument =>
    argument.Equals(
        "--bootstrap-admin",
        StringComparison.OrdinalIgnoreCase)))
{
    if (args.Length != 1)
    {
        throw new ArgumentException(
            "--bootstrap-admin cannot be combined " +
            "with other application arguments.");
    }

    await AdminBootstrapCommand.RunAsync();
    return;
}

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1. CẤU HÌNH KẾT NỐI SQL SERVER
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Thiếu cấu hình ConnectionStrings:DefaultConnection.");
}

builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(connectionString));

// =====================================================
// 2. ĐĂNG KÝ CÁC SERVICE NGHIỆP VỤ
// =====================================================

// Quản lý thực đơn
builder.Services.AddScoped<IMonAnService, MonAnService>();

// Quản lý khu vực và bàn ăn
builder.Services.AddScoped<IKhuVucService, KhuVucService>();
builder.Services.AddScoped<IBanAnService, BanAnService>();

// Đăng ký, đăng nhập và quản lý tài khoản
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// =====================================================
// 3. CẤU HÌNH JWT
// =====================================================

// Dùng thống nhất Jwt:SigningKey từ main.
// Khóa thật phải nằm trong User Secrets hoặc biến môi trường.
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];

if (string.IsNullOrWhiteSpace(jwtSigningKey) ||
    Encoding.UTF8.GetByteCount(jwtSigningKey) < 32)
{
    throw new InvalidOperationException(
        "Thiếu Jwt:SigningKey hoặc khóa JWT ngắn hơn 32 bytes.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Thiếu cấu hình Jwt:Issuer hoặc Jwt:Audience.");
}

// Đăng ký JwtOptions để AuthController sử dụng.
builder.Services.Configure<JwtOptions>(options =>
{
    options.SigningKey = jwtSigningKey;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
});

// Chỉ đăng ký JWT Bearer MỘT LẦN.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSigningKey)),

                ValidateLifetime = true,

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,

                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });

// =====================================================
// 4. PHÂN QUYỀN
// =====================================================

builder.Services.AddAuthorization(options =>
{
    // Chỉ quản lý hoặc Admin được truy cập
    // các API có policy ManagementOnly.
    options.AddPolicy("ManagementOnly", policy =>
        policy.RequireRole("QuanLy", "Admin"));
});

// =====================================================
// 5. GIỚI HẠN SỐ LẦN GỌI API ĐĂNG NHẬP
// =====================================================

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

// =====================================================
// 6. CONTROLLERS VÀ OPENAPI
// =====================================================

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// =====================================================
// 7. KHỞI TẠO ADMIN (NẾU ĐƯỢC BẬT TRONG CẤU HÌNH)
// =====================================================

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<RestaurantDbContext>();

    await DbInitializer.SeedAdminAsync(
        dbContext,
        app.Configuration);
}

// =====================================================
// 8. HTTP REQUEST PIPELINE
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

// Giới hạn request trước khi xử lý đăng nhập
app.UseRateLimiter();

// Xác thực người dùng
app.UseAuthentication();

// Kiểm tra quyền truy cập
app.UseAuthorization();

app.MapControllers();

app.Run();
