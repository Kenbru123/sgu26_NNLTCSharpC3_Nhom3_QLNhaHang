using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Authentication;
using Restaurant.API.Commands;
using Restaurant.Infrastructure.Data;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Services;

if (args.Any(argument => argument.Equals("--bootstrap-admin", StringComparison.OrdinalIgnoreCase)))
{
    if (args.Length != 1)
    {
        throw new ArgumentException("--bootstrap-admin cannot be combined with other application arguments.");
    }

    await AdminBootstrapCommand.RunAsync();
    return;
}

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Missing connection string 'DefaultConnection'. Configure it with user-secrets or the ConnectionStrings__DefaultConnection environment variable.");
}

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(jwtSigningKey) || Encoding.UTF8.GetByteCount(jwtSigningKey) < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:SigningKey with at least 32 bytes using user-secrets or the Jwt__SigningKey environment variable.");
}

var jwtOptions = new JwtOptions
{
    SigningKey = jwtSigningKey,
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "RestaurantManagement",
    Audience = builder.Configuration["Jwt:Audience"] ?? "RestaurantManagement.Admin"
};
builder.Services.Configure<JwtOptions>(options =>
{
    options.SigningKey = jwtOptions.SigningKey;
    options.Issuer = jwtOptions.Issuer;
    options.Audience = jwtOptions.Audience;
});

builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IMonAnService, MonAnService>();
builder.Services.AddScoped<IKhuVucService, KhuVucService>();
builder.Services.AddScoped<IBanAnService, BanAnService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
