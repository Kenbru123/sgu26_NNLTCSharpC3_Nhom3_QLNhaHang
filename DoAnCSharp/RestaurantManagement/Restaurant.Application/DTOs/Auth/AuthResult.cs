namespace Restaurant.Application.DTOs.Auth;

public sealed class AuthResult
{
    // Cho biết nghiệp vụ xác thực có thành công hay không.
    public bool Succeeded { get; init; }
    // Nội dung lỗi khi đăng ký hoặc đăng nhập thất bại.
    public string? Error { get; init; }
    // Dữ liệu trả về khi xác thực thành công.
    public AuthResponse? Response { get; init; }

    // Tạo kết quả thất bại.
    public static AuthResult Failed(string error) => new()
    {
        Error = error
    };

    // Tạo kết quả thành công.
    public static AuthResult Success(AuthResponse response) => new()
    {
        Succeeded = true,
        Response = response
    };
}
