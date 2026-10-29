using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoRongHeThong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "DanhMucMon",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "KhuVuc",
                columns: table => new
                {
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhuVuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuVuc", x => x.MaKhuVuc);
                });

            migrationBuilder.CreateTable(
                name: "NguyenLieu",
                columns: table => new
                {
                    MaNguyenLieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNguyenLieu = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MucTonToiThieu = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguyenLieu", x => x.MaNguyenLieu);
                    table.CheckConstraint("CK_NguyenLieu_TonToiThieu", "[MucTonToiThieu] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThaiLamViec = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "GioHang",
                columns: table => new
                {
                    MaGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHang", x => x.MaGioHang);
                    table.ForeignKey(
                        name: "FK_GioHang_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang");
                });

            migrationBuilder.CreateTable(
                name: "BanAn",
                columns: table => new
                {
                    MaBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false),
                    TenBan = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SoCho = table.Column<int>(type: "int", nullable: false),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanAn", x => x.MaBan);
                    table.CheckConstraint("CK_BanAn_SoCho", "[SoCho] > 0");
                    table.ForeignKey(
                        name: "FK_BanAn_KhuVuc_MaKhuVuc",
                        column: x => x.MaKhuVuc,
                        principalTable: "KhuVuc",
                        principalColumn: "MaKhuVuc");
                });

            migrationBuilder.CreateTable(
                name: "CongThucMon",
                columns: table => new
                {
                    MaMon = table.Column<int>(type: "int", nullable: false),
                    MaNguyenLieu = table.Column<int>(type: "int", nullable: false),
                    DinhLuong = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongThucMon", x => new { x.MaMon, x.MaNguyenLieu });
                    table.CheckConstraint("CK_CongThucMon_DinhLuong", "[DinhLuong] > 0");
                    table.ForeignKey(
                        name: "FK_CongThucMon_MonAn_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonAn",
                        principalColumn: "MaMon");
                    table.ForeignKey(
                        name: "FK_CongThucMon_NguyenLieu_MaNguyenLieu",
                        column: x => x.MaNguyenLieu,
                        principalTable: "NguyenLieu",
                        principalColumn: "MaNguyenLieu");
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhapKho",
                columns: table => new
                {
                    MaPhieuNhap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    ThoiGianNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Nhap"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhapKho", x => x.MaPhieuNhap);
                    table.CheckConstraint("CK_PhieuNhapKho_TrangThai", "[TrangThai] IN (N'Nhap',N'XacNhan',N'Huy')");
                    table.ForeignKey(
                        name: "FK_PhieuNhapKho_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                    table.CheckConstraint("CK_TaiKhoan_ChuThe", "([MaNhanVien] IS NOT NULL AND [MaKhachHang] IS NULL) OR ([MaNhanVien] IS NULL AND [MaKhachHang] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_TaiKhoan_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang");
                    table.ForeignKey(
                        name: "FK_TaiKhoan_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_TaiKhoan_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietGioHang",
                columns: table => new
                {
                    MaChiTietGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGioHang = table.Column<int>(type: "int", nullable: false),
                    MaMon = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietGioHang", x => x.MaChiTietGioHang);
                    table.CheckConstraint("CK_ChiTietGioHang_SoLuong", "[SoLuong] > 0");
                    table.ForeignKey(
                        name: "FK_ChiTietGioHang_GioHang_MaGioHang",
                        column: x => x.MaGioHang,
                        principalTable: "GioHang",
                        principalColumn: "MaGioHang");
                    table.ForeignKey(
                        name: "FK_ChiTietGioHang_MonAn_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonAn",
                        principalColumn: "MaMon");
                });

            migrationBuilder.CreateTable(
                name: "DatBan",
                columns: table => new
                {
                    MaDatBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaBan = table.Column<int>(type: "int", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThucDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongKhach = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValue: "ChoXacNhan"),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBan", x => x.MaDatBan);
                    table.CheckConstraint("CK_DatBan_SoLuongKhach", "[SoLuongKhach] > 0");
                    table.CheckConstraint("CK_DatBan_ThoiGian", "[ThoiGianKetThucDuKien] > [ThoiGianDen]");
                    table.CheckConstraint("CK_DatBan_TrangThai", "[TrangThai] IN (N'ChoXacNhan',N'DaXacNhan',N'DaDen',N'HoanTat',N'DaHuy',N'KhongDen')");
                    table.ForeignKey(
                        name: "FK_DatBan_BanAn_MaBan",
                        column: x => x.MaBan,
                        principalTable: "BanAn",
                        principalColumn: "MaBan");
                    table.ForeignKey(
                        name: "FK_DatBan_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang");
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    MaBan = table.Column<int>(type: "int", nullable: true),
                    MaNhanVienPhucVu = table.Column<int>(type: "int", nullable: true),
                    LoaiDon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SDTNguoiNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChiGiao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhiGiao = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ThoiGianNhanDuKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiDon = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValue: "ChoXacNhan"),
                    TrangThaiGiao = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.MaDonHang);
                    table.CheckConstraint("CK_DonHang_DiaChiGiao", "[LoaiDon] <> N'OnlineGiaoHang' OR ([DiaChiGiao] IS NOT NULL AND LEN(LTRIM(RTRIM([DiaChiGiao]))) > 0)");
                    table.CheckConstraint("CK_DonHang_LoaiDon", "[LoaiDon] IN (N'TaiBan',N'OnlineNhanTaiQuay',N'OnlineGiaoHang')");
                    table.CheckConstraint("CK_DonHang_PhiGiao", "[PhiGiao] >= 0");
                    table.CheckConstraint("CK_DonHang_ThongTinNhan", "([LoaiDon] = N'TaiBan' AND [MaBan] IS NOT NULL) OR ([LoaiDon] <> N'TaiBan' AND [MaBan] IS NULL AND [TenNguoiNhan] IS NOT NULL AND LEN(LTRIM(RTRIM([TenNguoiNhan]))) > 0 AND [SDTNguoiNhan] IS NOT NULL AND LEN(LTRIM(RTRIM([SDTNguoiNhan]))) > 0)");
                    table.CheckConstraint("CK_DonHang_TrangThaiGiao", "[LoaiDon] = N'OnlineGiaoHang' OR [TrangThaiGiao] IS NULL");
                    table.ForeignKey(
                        name: "FK_DonHang_BanAn_MaBan",
                        column: x => x.MaBan,
                        principalTable: "BanAn",
                        principalColumn: "MaBan");
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang");
                    table.ForeignKey(
                        name: "FK_DonHang_NhanVien_MaNhanVienPhucVu",
                        column: x => x.MaNhanVienPhucVu,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhap",
                columns: table => new
                {
                    MaChiTietNhap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieuNhap = table.Column<int>(type: "int", nullable: false),
                    MaNguyenLieu = table.Column<int>(type: "int", nullable: false),
                    SoLuongNhap = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhap", x => x.MaChiTietNhap);
                    table.CheckConstraint("CK_ChiTietPhieuNhap_DonGiaNhap", "[DonGiaNhap] >= 0");
                    table.CheckConstraint("CK_ChiTietPhieuNhap_SoLuongNhap", "[SoLuongNhap] > 0");
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_NguyenLieu_MaNguyenLieu",
                        column: x => x.MaNguyenLieu,
                        principalTable: "NguyenLieu",
                        principalColumn: "MaNguyenLieu");
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_PhieuNhapKho_MaPhieuNhap",
                        column: x => x.MaPhieuNhap,
                        principalTable: "PhieuNhapKho",
                        principalColumn: "MaPhieuNhap");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaChiTietDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    MaMon = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChuMon = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThaiCheBien = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValue: "ChoCheBien"),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHang", x => x.MaChiTietDonHang);
                    table.CheckConstraint("CK_ChiTietDonHang_DonGia", "[DonGia] >= 0");
                    table.CheckConstraint("CK_ChiTietDonHang_SoLuong", "[SoLuong] > 0");
                    table.CheckConstraint("CK_ChiTietDonHang_TrangThaiCheBien", "[TrangThaiCheBien] IN (N'ChoCheBien',N'DangCheBien',N'HoanThanh',N'DaHuy')");
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_MonAn_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonAn",
                        principalColumn: "MaMon");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    MaNhanVienLap = table.Column<int>(type: "int", nullable: false),
                    ThoiGianLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianThanhToan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TienHang = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhiGiaoChot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TyLeVAT = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TienVAT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongThanhToan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValue: "ChuaThanhToan")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                    table.CheckConstraint("CK_HoaDon_SoTien", "[TienHang] >= 0 AND [PhiGiaoChot] >= 0 AND [TienVAT] >= 0 AND [TongThanhToan] >= 0");
                    table.CheckConstraint("CK_HoaDon_TrangThaiThanhToan", "[TrangThaiThanhToan] IN (N'ChuaThanhToan',N'DaThanhToan',N'DaHuy')");
                    table.CheckConstraint("CK_HoaDon_TyLeVAT", "[TyLeVAT] >= 0 AND [TyLeVAT] <= 100");
                    table.ForeignKey(
                        name: "FK_HoaDon_DonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang");
                    table.ForeignKey(
                        name: "FK_HoaDon_NhanVien_MaNhanVienLap",
                        column: x => x.MaNhanVienLap,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "PhieuXuatKho",
                columns: table => new
                {
                    MaPhieuXuat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    MaChiTietDonHang = table.Column<int>(type: "int", nullable: true),
                    ThoiGianXuat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiXuat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Nhap"),
                    LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuXuatKho", x => x.MaPhieuXuat);
                    table.CheckConstraint("CK_PhieuXuatKho_LoaiXuat", "[LoaiXuat] IN (N'CheBien',N'ThuCong')");
                    table.CheckConstraint("CK_PhieuXuatKho_NguonXuat", "([LoaiXuat] = N'CheBien' AND [MaChiTietDonHang] IS NOT NULL) OR ([LoaiXuat] = N'ThuCong' AND [MaNhanVien] IS NOT NULL AND [LyDo] IS NOT NULL AND LEN(LTRIM(RTRIM([LyDo]))) > 0)");
                    table.CheckConstraint("CK_PhieuXuatKho_TrangThai", "[TrangThai] IN (N'Nhap',N'XacNhan',N'Huy')");
                    table.ForeignKey(
                        name: "FK_PhieuXuatKho_ChiTietDonHang_MaChiTietDonHang",
                        column: x => x.MaChiTietDonHang,
                        principalTable: "ChiTietDonHang",
                        principalColumn: "MaChiTietDonHang");
                    table.ForeignKey(
                        name: "FK_PhieuXuatKho_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuXuat",
                columns: table => new
                {
                    MaChiTietXuat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieuXuat = table.Column<int>(type: "int", nullable: false),
                    MaNguyenLieu = table.Column<int>(type: "int", nullable: false),
                    SoLuongXuat = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuXuat", x => x.MaChiTietXuat);
                    table.CheckConstraint("CK_ChiTietPhieuXuat_SoLuongXuat", "[SoLuongXuat] > 0");
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuXuat_NguyenLieu_MaNguyenLieu",
                        column: x => x.MaNguyenLieu,
                        principalTable: "NguyenLieu",
                        principalColumn: "MaNguyenLieu");
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuXuat_PhieuXuatKho_MaPhieuXuat",
                        column: x => x.MaPhieuXuat,
                        principalTable: "PhieuXuatKho",
                        principalColumn: "MaPhieuXuat");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanAn_MaKhuVuc_TenBan",
                table: "BanAn",
                columns: new[] { "MaKhuVuc", "TenBan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDonHang",
                table: "ChiTietDonHang",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaMon",
                table: "ChiTietDonHang",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHang_MaGioHang_MaMon",
                table: "ChiTietGioHang",
                columns: new[] { "MaGioHang", "MaMon" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHang_MaMon",
                table: "ChiTietGioHang",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_MaNguyenLieu",
                table: "ChiTietPhieuNhap",
                column: "MaNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_MaPhieuNhap",
                table: "ChiTietPhieuNhap",
                column: "MaPhieuNhap");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuXuat_MaNguyenLieu",
                table: "ChiTietPhieuXuat",
                column: "MaNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuXuat_MaPhieuXuat",
                table: "ChiTietPhieuXuat",
                column: "MaPhieuXuat");

            migrationBuilder.CreateIndex(
                name: "IX_CongThucMon_MaNguyenLieu",
                table: "CongThucMon",
                column: "MaNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_MaBan",
                table: "DatBan",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_MaKhachHang",
                table: "DatBan",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaBan",
                table: "DonHang",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKhachHang",
                table: "DonHang",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaNhanVienPhucVu",
                table: "DonHang",
                column: "MaNhanVienPhucVu");

            migrationBuilder.CreateIndex(
                name: "IX_GioHang_MaKhachHang",
                table: "GioHang",
                column: "MaKhachHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaDonHang",
                table: "HoaDon",
                column: "MaDonHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaNhanVienLap",
                table: "HoaDon",
                column: "MaNhanVienLap");

            migrationBuilder.CreateIndex(
                name: "IX_KhuVuc_TenKhuVuc",
                table: "KhuVuc",
                column: "TenKhuVuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapKho_MaNhanVien",
                table: "PhieuNhapKho",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuXuatKho_MaChiTietDonHang",
                table: "PhieuXuatKho",
                column: "MaChiTietDonHang",
                unique: true,
                filter: "[MaChiTietDonHang] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuXuatKho_MaNhanVien",
                table: "PhieuXuatKho",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaKhachHang",
                table: "TaiKhoan",
                column: "MaKhachHang",
                unique: true,
                filter: "[MaKhachHang] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaNhanVien",
                table: "TaiKhoan",
                column: "MaNhanVien",
                unique: true,
                filter: "[MaNhanVien] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_MaVaiTro",
                table: "TaiKhoan",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_TenVaiTro",
                table: "VaiTro",
                column: "TenVaiTro",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietGioHang");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuXuat");

            migrationBuilder.DropTable(
                name: "CongThucMon");

            migrationBuilder.DropTable(
                name: "DatBan");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "GioHang");

            migrationBuilder.DropTable(
                name: "PhieuNhapKho");

            migrationBuilder.DropTable(
                name: "PhieuXuatKho");

            migrationBuilder.DropTable(
                name: "NguyenLieu");

            migrationBuilder.DropTable(
                name: "VaiTro");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "BanAn");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "KhuVuc");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "DanhMucMon");
        }
    }
}
