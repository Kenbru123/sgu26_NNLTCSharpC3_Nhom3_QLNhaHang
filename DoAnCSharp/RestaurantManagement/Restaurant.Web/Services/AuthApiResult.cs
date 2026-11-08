using Restaurant.Application.DTOs.Auth;

namespace Restaurant.Web.Services;

public sealed class AuthApiResult
{
    public bool Succeeded { get; init; }
    public string? Error { get; init; }
    public AuthResponse? Response { get; init; }

    public static AuthApiResult Failed(string error) => new()
    {
        Error = error
    };

    public static AuthApiResult Success(AuthResponse response) => new()
    {
        Succeeded = true,
        Response = response
    };
}
