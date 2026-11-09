# Restaurant Management: SQL Server Docker, API và Admin

## 1. Tạo cấu hình môi trường và khởi động SQL Server

Mở PowerShell tại thư mục `DoAnCSharp\RestaurantManagement`:

```powershell
Copy-Item .env.example .env
notepad .env
```

Đổi `REPLACE_ME` thành mật khẩu SA mạnh. File `.env` được Git bỏ qua; không commit mật khẩu.

```powershell
docker compose up -d
docker compose ps
```

Chờ trạng thái container là `healthy`. Kết nối SSMS bằng:

- **Server name:** `localhost,<MSSQL_PORT>` (giá trị trong `.env`; trên máy hiện tại là `localhost,14330`)
- **Authentication:** SQL Server Authentication
- **Login:** `sa`
- **Password:** giá trị `MSSQL_SA_PASSWORD` trong `.env`
- Nếu SSMS hỏi về mã hóa/chứng chỉ, bật **Trust server certificate**.

## 2. Cấu hình database và chạy API (BE)

Các lệnh sau đọc mật khẩu và cổng từ `.env`, sau đó lưu connection string trong .NET User Secrets:

```powershell
$settings = Get-Content .\.env
$password = (($settings | Where-Object { $_ -match '^MSSQL_SA_PASSWORD=' } | Select-Object -First 1) -replace '^MSSQL_SA_PASSWORD=', '').Trim().Trim('"').Trim("'")
$port = (($settings | Where-Object { $_ -match '^MSSQL_PORT=' } | Select-Object -First 1) -replace '^MSSQL_PORT=', '').Trim()
$connection = "Server=localhost,$port;Database=RestaurantDB;User Id=sa;Password=$password;Encrypt=True;TrustServerCertificate=True"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connection --project .\Restaurant.API\Restaurant.API.csproj
Remove-Variable password, connection
```

Tạo khóa ký JWT ngẫu nhiên và lưu vào User Secrets (không commit khóa này):

```powershell
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 48
$rng.GetBytes($bytes)
$jwtKey = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "Jwt:SigningKey" $jwtKey --project .\Restaurant.API\Restaurant.API.csproj
$rng.Dispose()
Remove-Variable jwtKey, bytes
```

Tạo tài khoản Quản lý ban đầu một lần. Lệnh yêu cầu nhập tên đăng nhập, họ tên và mật khẩu (mật khẩu không hiển thị khi gõ); tài khoản và vai trò được lưu vào database, mật khẩu được băm bằng PBKDF2:

```powershell
dotnet run --project .\Restaurant.API\Restaurant.API.csproj -- --bootstrap-admin
```

Lệnh bootstrap chỉ chạy khi database chưa có tài khoản. API đăng nhập tại `POST /api/auth/login`, phát JWT có thời hạn 1 giờ và giới hạn 5 lần thử mỗi phút trên mỗi IP. Vai trò `QuanLy` có quyền quản lý bàn/khu vực; `NhanVien` có thể xem, tìm kiếm và cập nhật trạng thái bàn nhưng không thêm, sửa hoặc xóa.

Áp dụng migration để tạo schema cho database, rồi khởi động API:

```powershell
dotnet ef database update --project .\Restaurant.Infrastructure\Restaurant.Infrastructure.csproj --startup-project .\Restaurant.API\Restaurant.API.csproj
dotnet run --project .\Restaurant.API\Restaurant.API.csproj --launch-profile https
```

API chạy tại `https://localhost:7053` và `http://localhost:5297`. Giữ cửa sổ này mở khi sử dụng ứng dụng Admin.

## 3. Chạy WinForms Admin (FE)

Mở cửa sổ PowerShell thứ hai tại `DoAnCSharp\RestaurantManagement`:

```powershell
dotnet run --project .\Restaurant.Admin\Restaurant.Admin.csproj
```

Khi mở ứng dụng Admin, đăng nhập bằng tài khoản Quản lý vừa tạo. Trong ứng dụng:

- Tab **Khu vực** cho phép xem, thêm, cập nhật và xóa khu vực. Tên khu vực là duy nhất; khu vực đang có bàn không thể bị xóa.
- Tab **Bàn ăn** cho phép thêm, xem chi tiết, cập nhật, tìm kiếm theo mã/tên, lọc theo khu vực/trạng thái, cập nhật trạng thái và xóa bàn. Khi thêm hoặc sửa bàn, phải chọn một khu vực đã tồn tại trong danh sách; nếu chưa có khu vực thì cần tạo khu vực trước. Tên bàn không được trùng trong cùng một khu vực và số chỗ phải lớn hơn 0. Không thể ngừng hoạt động bàn đang có đơn hàng/lượt đặt còn hiệu lực; bàn còn liên kết với dữ liệu đơn hàng/đặt bàn không thể bị xóa, kể cả dữ liệu lịch sử.
- Tab **Gọi món** dành cho Quản lý và Nhân viên: tạo/xóa phiếu tại bàn, tìm món theo tên/danh mục, thêm/xóa món, xem phiếu/chi tiết, cập nhật ghi chú và số lượng, cập nhật trạng thái phiếu (`Đang phục vụ`/`Đã thanh toán`) và trạng thái chế biến món (`Chờ chế biến` → `Đang chế biến` → `Đã chế biến`). Chi tiết món hiển thị ghi chú, trạng thái chế biến, thời gian bắt đầu và hoàn thành, đơn giá đã chốt theo phiếu. Mỗi bàn chỉ có một phiếu đang phục vụ; nếu bàn đã có phiếu thì mở phiếu đó. Không thể sửa món đã chuyển chế biến; không thể xóa phiếu có món đang chế biến/đã chế biến hoặc có hóa đơn.
- Khi thanh toán, nhân viên chọn phương thức (`Tiền mặt`, `Chuyển khoản`, `Thẻ`) và VAT (0–100%). Hệ thống tính tiền hàng/VAT, tạo hoặc cập nhật hóa đơn và đánh dấu phiếu đã thanh toán trong cùng transaction.

Các API quản lý khu vực: `GET/POST /api/khu-vuc`, `GET/PUT/DELETE /api/khu-vuc/{id}`.
Các API quản lý bàn ăn: `GET/POST /api/ban-an`, `GET/PUT/DELETE /api/ban-an/{id}`, `PATCH /api/ban-an/{id}/trang-thai`. API chỉ chấp nhận mã khu vực đã tồn tại.
Các API gọi món (chỉ Quản lý/Nhân viên): `GET /api/goi-mon`, `GET/DELETE /api/goi-mon/{id}`, `POST /api/goi-mon`, `PUT /api/goi-mon/{id}`, `PATCH /api/goi-mon/{id}/trang-thai`, `POST /api/goi-mon/{id}/thanh-toan`, `GET /api/goi-mon/ban-an`, `GET /api/goi-mon/danh-muc`, `GET /api/goi-mon/mon-an`, `POST /api/goi-mon/{id}/mon-an`, `PUT/DELETE /api/goi-mon/{id}/mon-an/{detailId}`, `PATCH /api/goi-mon/{id}/mon-an/{detailId}/trang-thai`.

## Quản lý container

```powershell
docker compose logs -f sqlserver
docker compose down
```

`docker compose down` giữ dữ liệu trong volume `restaurant-sql-data`. Không chạy `docker compose down -v` trừ khi muốn xóa hẳn dữ liệu database.
