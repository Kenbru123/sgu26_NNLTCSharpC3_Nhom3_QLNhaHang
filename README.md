# Hệ thống quản lý nhà hàng — Nhóm 3

Đồ án xây dựng hệ thống quản lý hoạt động nhà hàng, gồm API nghiệp vụ, website dành cho khách hàng và ứng dụng desktop dành cho nhân viên/quản lý.

## Thành viên

- Nguyễn Xuân Hậu Thành
- Lưu Lương Mạnh Dũng
- Nguyễn Thành Đại
- Lê Đăng Tiến

## Thành phần

| Project | Vai trò |
| --- | --- |
| `Restaurant.API` | REST API, xác thực JWT và nghiệp vụ nhà hàng |
| `Restaurant.Web` | Website khách hàng dùng ASP.NET Core MVC (Controllers/Views) |
| `Restaurant.Admin` | Ứng dụng quản trị Windows Forms |
| `Restaurant.Application` | DTO, interface và hợp đồng nghiệp vụ |
| `Restaurant.Domain` | Entity và mô hình miền |
| `Restaurant.Infrastructure` | EF Core, SQL Server và triển khai dịch vụ |

Các luồng hiện có gồm đăng ký/đăng nhập khách hàng, thực đơn, giỏ hàng và đặt món online, đặt bàn, quản lý bàn/khu vực, xử lý đơn hàng, kho, tài khoản và ghi nhận thanh toán tại nhà hàng.

## Yêu cầu môi trường

- Windows 10/11.
- .NET 10 SDK.
- Visual Studio 2026 hoặc PowerShell.
- SQL Server 2022 cài tại máy, hoặc Docker Desktop để chạy SQL Server bằng Compose.
- Công cụ `dotnet-ef` phiên bản 10 nếu cần cập nhật database bằng dòng lệnh.

## Cài đặt database

Mở PowerShell từ thư mục gốc repository rồi chuyển vào thư mục solution:

```powershell
cd .\DoAnCSharp\RestaurantManagement
```

### Dùng SQL Server trong Docker

```powershell
Copy-Item .env.example .env
```

Mở `.env`, thay mật khẩu SQL Server mẫu bằng mật khẩu mạnh riêng của máy trước khi chạy:

```powershell
docker compose up -d sqlserver
```

Đặt cùng mật khẩu vào User Secrets của API, cùng connection string trỏ đến SQL Server trong Docker. Thay `<SQL_PASSWORD>` bằng mật khẩu vừa đặt trong `.env`:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=RestaurantDB;User Id=sa;Password=<SQL_PASSWORD>;Encrypt=True;TrustServerCertificate=True" --project .\Restaurant.API\Restaurant.API.csproj
```

### Dùng SQL Server cài tại máy

Nếu dùng Windows Authentication và instance mặc định `(local)`, có thể dùng connection string trong `Restaurant.API/appsettings.json`. Nếu SQL Server dùng instance/tài khoản khác, lưu connection string riêng bằng User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<YOUR_SQL_SERVER_CONNECTION_STRING>" --project .\Restaurant.API\Restaurant.API.csproj
```

### Cấu hình khóa JWT và tạo schema

Không dùng khóa JWT mặc định trong `appsettings.json` khi chia sẻ hoặc triển khai. Tạo khóa ngẫu nhiên và lưu bằng User Secrets:

```powershell
$jwtKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
dotnet user-secrets set "Jwt:SigningKey" $jwtKey --project .\Restaurant.API\Restaurant.API.csproj
```

Nếu chưa cài `dotnet-ef`, cài công cụ cùng phiên bản EF Core của dự án:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

Đặt môi trường Development cho lệnh EF để API đọc User Secrets, sau đó cập nhật schema theo các migration hiện có:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project .\Restaurant.Infrastructure\Restaurant.Infrastructure.csproj --startup-project .\Restaurant.API\Restaurant.API.csproj
```

API hiện không tự chạy migration khi khởi động, vì vậy cần thực hiện bước cập nhật database trước khi chạy ứng dụng.

## Tạo tài khoản quản lý

Sau khi database đã có schema, chạy lệnh sau và nhập tên đăng nhập, họ tên, mật khẩu theo lời nhắc. Mật khẩu quản lý cần ít nhất 12 ký tự. Lệnh chỉ dùng để tạo tài khoản ban đầu khi database chưa có tài khoản nào:

```powershell
dotnet run --project .\Restaurant.API\Restaurant.API.csproj -- --bootstrap-admin
```

Khách hàng có thể tự đăng ký tại trang `/Account/Register` của website.

## Chạy ứng dụng

Khởi động API trước. Mở một PowerShell tại thư mục solution:

```powershell
dotnet run --project .\Restaurant.API\Restaurant.API.csproj --launch-profile https
```

API HTTPS: `https://localhost:7053`  
Tài liệu OpenAPI (Development): `https://localhost:7053/openapi/v1.json`

Mở PowerShell thứ hai để chạy website:

```powershell
dotnet run --project .\Restaurant.Web\Restaurant.Web.csproj --launch-profile http
```

Website: `http://localhost:5240`

Chạy ứng dụng desktop trên Windows khi API đang hoạt động:

```powershell
dotnet run --project .\Restaurant.Admin\Restaurant.Admin.csproj
```

Desktop gọi API tại `https://localhost:7053`. Nếu chứng chỉ HTTPS phát triển chưa được tin cậy, có thể cần chạy `dotnet dev-certs https --trust`.

## Build và test

```powershell
dotnet restore .\RestaurantManagement.slnx
dotnet build .\RestaurantManagement.slnx
dotnet test .\RestaurantManagement.slnx
```

Hiện solution chưa có project test tự động; `dotnet test` không thay thế cho việc kiểm thử thủ công các luồng đăng nhập, đặt món, đặt bàn và thao tác quản trị.

## Lưu ý và giới hạn hiện tại

- Database mới chỉ được tạo schema từ migrations, chưa có dữ liệu danh mục/món mẫu được seed tự động.
- `SQLQuery1.sql` hiện có chuỗi tiếng Việt lỗi encoding; không nên dùng làm dữ liệu mẫu cho đến khi sửa nội dung/encoding.
- Website hiện chưa tích hợp cổng thanh toán online. Đơn hàng web được tạo để nhân viên xác nhận; thanh toán được ghi nhận qua luồng quản trị tại nhà hàng.
- `Restaurant.API` còn endpoint mẫu `WeatherForecast`; trang Privacy của website vẫn là nội dung scaffold mặc định.
- JWT hết hạn theo cấu hình API (mặc định 60 phút); khi phiên đăng nhập hết hạn, đăng nhập lại để tiếp tục thao tác.

## Cấu hình an toàn

Không commit `.env`, mật khẩu SQL Server hoặc khóa JWT thật. Dùng User Secrets cho phát triển cục bộ và biến môi trường/secret store của môi trường triển khai cho production. Không tái sử dụng mật khẩu mẫu trong `.env.example`.
