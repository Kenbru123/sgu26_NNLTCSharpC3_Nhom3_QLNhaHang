using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Restaurant.Infrastructure.Data;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext kết nối tới SQL Server.
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các service nghiệp vụ.
builder.Services.AddScoped<IMonAnService, MonAnService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Đọc cấu hình JWT từ appsettings.json hoặc biến môi trường.
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("Thiếu cấu hình Jwt:SecretKey.");

// Cấu hình xác thực JWT Bearer cho các API yêu cầu đăng nhập.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// Chỉ quản lý hoặc admin được truy cập API quản trị.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagementOnly", policy =>
        policy.RequireRole("QuanLy", "Admin"));
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed tài khoản Admin ban đầu nếu AdminSeed:Enabled=true.
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    await DbInitializer.SeedAdminAsync(dbContext, app.Configuration);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Xác thực token trước khi kiểm tra quyền truy cập endpoint.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
