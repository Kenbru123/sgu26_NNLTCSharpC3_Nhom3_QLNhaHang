using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;
using Restaurant.Application.DTOs.Account;

namespace Restaurant.Admin
{
    public partial class Form1 : Form
    {
        private readonly MonAnApiService _monAnApiService;
        private readonly KhuVucApiService _khuVucApiService;
        private readonly BanAnApiService _banAnApiService;
        private readonly KhoApiService _khoApiService;
        private readonly AccountApiService _accountApiService;
        private readonly ThanhToanApiService _thanhToanApiService;
        private readonly GoiMonControl _goiMonControl;
        private readonly CheBienControl _cheBienControl;
        private readonly DataGridView dgvBanAn = new();
        private readonly Panel pnlDanhSachBanAn = new();
        private readonly Label lblBanAnKhongCoKetQua = new();
        private readonly TextBox txtMaBan = new();
        private readonly TextBox txtTenBan = new();
        private readonly TextBox txtSoCho = new();
        private readonly TextBox txtTimBan = new();
        private readonly ComboBox cmbKhuVucBan = new();
        private readonly ComboBox cmbLocKhuVucBan = new();
        private readonly ComboBox cmbLocTrangThaiBan = new();
        private readonly CheckBox chkBanHoatDong = new();
        private readonly Button btnThemBan = new();
        private readonly Button btnSuaBan = new();
        private readonly Button btnXoaBan = new();
        private readonly Button btnCapNhatTrangThaiBan = new();
        private readonly TextBox txtMaMon = new();
        private readonly TextBox txtTenMon = new();
        private readonly TextBox txtMoTaMon = new();
        private readonly TextBox txtHinhAnhMon = new();
        private readonly NumericUpDown nudDonGiaMon = new();
        private readonly ComboBox cmbDanhMucMon = new();
        private readonly TextBox txtTenDanhMuc = new();
        private readonly TextBox txtMoTaDanhMuc = new();
        private readonly CheckBox chkMonDangBan = new();
        private readonly Button btnThemMon = new();
        private readonly Button btnSuaMon = new();
        private readonly Button btnXoaMon = new();
        private readonly Button btnLamMoiMon = new();
        private readonly Button btnThemDanhMuc = new();
        private readonly DataGridView dgvKhoData = new();
        private readonly DataGridView dgvKhoLowStock = new();
        private readonly DataGridView dgvKhoPhieu = new();
        private readonly TextBox txtKhoTenNguyenLieu = new();
        private readonly TextBox txtKhoDonViTinh = new();
        private readonly NumericUpDown nudKhoMucTonToiThieu = new();
        private readonly Button btnKhoThemNguyenLieu = new();
        private readonly DataGridView dgvThanhToan = new();
        private readonly DataGridView dgvDonOnline = new();
        private readonly OnlineOrderApiService _onlineOrderApiService = new();
        private readonly Button btnTaiLaiDonOnline = new();
        private readonly Button btnXacNhanDonOnline = new();
        private readonly Button btnHuyDonOnline = new();
        private readonly Button btnXemChiTietDonOnline = new();
        private readonly TextBox txtKhoMaNguyenLieuNhap = new();
        private readonly NumericUpDown nudKhoSoLuongNhap = new();
        private readonly NumericUpDown nudKhoDonGiaNhap = new();
        private readonly TextBox txtKhoGhiChuNhap = new();
        private readonly Button btnKhoNhapNhanh = new();
        private readonly TextBox txtKhoMaNguyenLieuXuat = new();
        private readonly NumericUpDown nudKhoSoLuongXuat = new();
        private readonly ComboBox cmbKhoLoaiXuat = new();
        private readonly NumericUpDown nudKhoMaChiTietDonHang = new();
        private readonly NumericUpDown nudKhoMaNhanVienXuat = new();
        private readonly TextBox txtKhoLyDoXuat = new();
        private readonly TextBox txtKhoGhiChuXuat = new();
        private readonly Button btnKhoXuatNhanh = new();
        private readonly ComboBox cmbTrangThaiGiao = new();
        private readonly Button btnCapNhatTrangThaiGiao = new();
        private readonly Button btnKhoXacNhanNhap = new();
        private readonly Button btnKhoHuyNhap = new();
        private readonly Button btnKhoXacNhanXuat = new();
        private readonly Button btnKhoHuyXuat = new();
        private readonly Button btnDangXuat = new();
        private readonly DatBanApiService _datBanApiService;
        private readonly DataGridView dgvDatBan = new();
        private readonly ComboBox cmbLocTrangThaiDatBan = new();
        private readonly Button btnTaiLaiDatBan = new();
        private readonly Button btnXacNhanDatBan = new();
        private readonly Button btnHuyDatBan = new();
        private readonly Button btnDaDenDatBan = new();
        private readonly Button btnHoanTatDatBan = new();
        private readonly Button btnKhongDenDatBan = new();
        private readonly DataGridView dgvTaiKhoanNhanVien = new();
        private readonly TextBox txtTaiKhoanUsername = new();
        private readonly TextBox txtTaiKhoanPassword = new();
        private readonly TextBox txtTaiKhoanHoTen = new();
        private readonly TextBox txtTaiKhoanSoDienThoai = new();
        private readonly DateTimePicker dtpTaiKhoanNgaySinh = new();
        private readonly Button btnTaiKhoanThem = new();
        private bool _loadingBanAn;
        private bool _loadingBanFilters;

        public Form1()
        {
            InitializeComponent();

            btnDangXuat.Text = "Đăng xuất";
            btnDangXuat.Size = new Size(120, 34);
            btnDangXuat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDangXuat.Location = new Point(ClientSize.Width - btnDangXuat.Width - 16, 8);
            btnDangXuat.Click += btnDangXuat_Click;
            Controls.Add(btnDangXuat);
            tabQuanLy.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabQuanLy.Location = new Point(8, 48);
            tabQuanLy.Size = new Size(ClientSize.Width - 16, ClientSize.Height - 56);
            Resize += (_, _) =>
            {
                btnDangXuat.Location = new Point(ClientSize.Width - btnDangXuat.Width - 16, 8);
                tabQuanLy.Size = new Size(ClientSize.Width - 16, ClientSize.Height - 56);
            };
            btnDangXuat.BringToFront();

            _monAnApiService = new MonAnApiService();
            _khuVucApiService = new KhuVucApiService();
            _banAnApiService = new BanAnApiService();
            _khoApiService = new KhoApiService();
            _accountApiService = new AccountApiService();
            _thanhToanApiService = new ThanhToanApiService();
            _datBanApiService = new DatBanApiService();
            _goiMonControl = new GoiMonControl();
            _cheBienControl = new CheBienControl();
            InitializeMonAnTab();
            InitializeKhoTab();
            InitializePaymentTab();
            InitializeOnlineOrdersTab();
            ConfigureKhuVucGrid();
            InitializeBanAnTab();
            InitializeDatBanTab();
            var tabGoiMon = new TabPage("Gọi món")
            {
                Padding = new Padding(8)
            };
            tabGoiMon.Controls.Add(_goiMonControl);
            tabQuanLy.Controls.Add(tabGoiMon);
            var tabCheBien = new TabPage("Quản lý chế biến")
            {
                Padding = new Padding(8)
            };
            tabCheBien.Controls.Add(_cheBienControl);
            tabQuanLy.Controls.Add(tabCheBien);
            btnThemKhuVuc.Enabled = ApiSession.IsManager;
            InitializeAccountTab();
            ConfigureReadableLayout();
        }

        private void ConfigureReadableLayout()
        {
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Dpi;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            tabQuanLy.Multiline = false;
            tabQuanLy.SizeMode = TabSizeMode.Fixed;
            tabQuanLy.ItemSize = new Size(132, 36);
            tabQuanLy.Padding = new Point(8, 4);

            foreach (var control in GetAllControls(this))
            {
                control.Font = new Font("Segoe UI", 10F);

                if (control is Button button)
                {
                    button.MinimumSize = new Size(0, 34);
                    button.Padding = new Padding(10, 5, 10, 5);
                    button.AutoSize = true;
                    button.AutoSizeMode = AutoSizeMode.GrowOnly;
                }
                else if (control is TextBox textBox)
                {
                    textBox.MinimumSize = new Size(0, 30);
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.MinimumSize = new Size(0, 30);
                    comboBox.Margin = new Padding(4, 3, 4, 3);
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.MinimumSize = new Size(0, 30);
                    numericUpDown.Margin = new Padding(4, 3, 4, 3);
                }
                else if (control is DataGridView grid)
                {
                    grid.BackgroundColor = SystemColors.Window;
                    grid.BorderStyle = BorderStyle.FixedSingle;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    grid.RowTemplate.Height = 32;
                    grid.ColumnHeadersHeight = 42;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                    grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
                    grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    grid.RowTemplate.Height = 32;
                    grid.ColumnHeadersHeight = 42;
                    grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                    grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
                    grid.RowHeadersVisible = false;
                    grid.AllowUserToResizeRows = false;
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.Padding = new Padding(10, 24, 10, 10);
                }
            }
        }

        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;

                foreach (var child in GetAllControls(control))
                {
                    yield return child;
                }
            }
        }

        private void InitializeOnlineOrdersTab()
        {
            if (!ApiSession.IsManager)
            {
                return;
            }

            var page = new TabPage("Đơn online") { Padding = new Padding(12) };
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 4, 0, 4)
            };
            btnTaiLaiDonOnline.Text = "Tải lại đơn online";
            btnTaiLaiDonOnline.Size = new Size(150, 34);
            btnTaiLaiDonOnline.Click += async (_, _) => await LoadOnlineOrdersAsync();
            toolbar.Controls.Add(btnTaiLaiDonOnline);
            btnXacNhanDonOnline.Text = "Xác nhận đơn";
            btnXacNhanDonOnline.Size = new Size(130, 34);
            btnXacNhanDonOnline.Click += async (_, _) => await UpdateOnlineOrderStatusAsync("DangPhucVu");
            toolbar.Controls.Add(btnXacNhanDonOnline);
            btnHuyDonOnline.Text = "Hủy đơn";
            btnHuyDonOnline.Size = new Size(100, 34);
            btnHuyDonOnline.Click += async (_, _) => await UpdateOnlineOrderStatusAsync("DaHuy");
            toolbar.Controls.Add(btnHuyDonOnline);
            btnXemChiTietDonOnline.Text = "Xem chi tiết";
            btnXemChiTietDonOnline.Size = new Size(120, 34);
            btnXemChiTietDonOnline.Click += (_, _) => ShowOnlineOrderDetails();
            toolbar.Controls.Add(btnXemChiTietDonOnline);

            cmbTrangThaiGiao.Size = new Size(140, 34);
            cmbTrangThaiGiao.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTrangThaiGiao.Items.AddRange(new object[] { "ChoLayHang", "DangGiao", "DaGiao", "DaNhan", "HoanTat" });
            cmbTrangThaiGiao.SelectedIndex = 0;
            toolbar.Controls.Add(cmbTrangThaiGiao);
            btnCapNhatTrangThaiGiao.Text = "Cập nhật giao hàng";
            btnCapNhatTrangThaiGiao.Size = new Size(160, 34);
            btnCapNhatTrangThaiGiao.Click += async (_, _) => await UpdateSelectedOnlineDeliveryStatusAsync();
            toolbar.Controls.Add(btnCapNhatTrangThaiGiao);
            panel.Controls.Add(toolbar, 0, 0);

            dgvDonOnline.Dock = DockStyle.Fill;
            dgvDonOnline.ReadOnly = true;
            dgvDonOnline.AutoGenerateColumns = false;
            dgvDonOnline.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDonOnline.SelectionChanged += (_, _) => UpdateOnlineOrderActionState();
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.MaDonHang), HeaderText = "Mã đơn", FillWeight = 45, MinimumWidth = 55 });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.LoaiDon), HeaderText = "Loại nhận", FillWeight = 90, MinimumWidth = 100 });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.TenNguoiNhan), HeaderText = "Người nhận", FillWeight = 100, MinimumWidth = 100 });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.SDTNguoiNhan), HeaderText = "Số điện thoại", FillWeight = 90, MinimumWidth = 100 });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.TrangThaiDon), HeaderText = "Trạng thái đơn", FillWeight = 105, MinimumWidth = 115 });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.TrangThaiGiao), HeaderText = "Trạng thái giao", FillWeight = 105, MinimumWidth = 115 });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.TongTien), HeaderText = "Tổng tiền", FillWeight = 75, MinimumWidth = 85, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvDonOnline.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(OnlineOrderResponse.ThoiGianTao), HeaderText = "Thời gian", FillWeight = 95, MinimumWidth = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            panel.Controls.Add(dgvDonOnline, 0, 1);
            page.Controls.Add(panel);
            tabQuanLy.Controls.Add(page);
        }

        private void InitializeDatBanTab()
        {
            var page = new TabPage("Đặt bàn")
            {
                Padding = new Padding(12)
            };
            var filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 4, 0, 4)
            };
            filters.Controls.Add(new Label
            {
                Text = "Trạng thái",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 8, 0)
            });
            cmbLocTrangThaiDatBan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocTrangThaiDatBan.Width = 180;
            cmbLocTrangThaiDatBan.DataSource = new[]
            {
                new { Text = "Tất cả", Value = "TatCa" },
                new { Text = "Chờ xác nhận", Value = "ChoXacNhan" },
                new { Text = "Đã xác nhận", Value = "DaXacNhan" },
                new { Text = "Đã đến", Value = "DaDen" },
                new { Text = "Hoàn tất", Value = "HoanTat" },
                new { Text = "Đã hủy", Value = "DaHuy" },
                new { Text = "Không đến", Value = "KhongDen" }
            };
            cmbLocTrangThaiDatBan.DisplayMember = "Text";
            cmbLocTrangThaiDatBan.ValueMember = "Value";
            cmbLocTrangThaiDatBan.SelectedIndexChanged += async (_, _) =>
                await LoadDatBanAsync();
            filters.Controls.Add(cmbLocTrangThaiDatBan);

            btnTaiLaiDatBan.Text = "Tải lại danh sách";
            btnTaiLaiDatBan.Click += async (_, _) => await LoadDatBanAsync();
            filters.Controls.Add(btnTaiLaiDatBan);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 52,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 4, 0, 4)
            };
            btnXacNhanDatBan.Text = "Xác nhận đặt bàn";
            btnHuyDatBan.Text = "Hủy đặt bàn";
            btnDaDenDatBan.Text = "Khách đã đến";
            btnHoanTatDatBan.Text = "Hoàn tất";
            btnKhongDenDatBan.Text = "Không đến";
            btnXacNhanDatBan.Click += async (_, _) => await UpdateDatBanStatusAsync("DaXacNhan");
            btnHuyDatBan.Click += async (_, _) => await UpdateDatBanStatusAsync("DaHuy");
            btnDaDenDatBan.Click += async (_, _) => await UpdateDatBanStatusAsync("DaDen");
            btnHoanTatDatBan.Click += async (_, _) => await UpdateDatBanStatusAsync("HoanTat");
            btnKhongDenDatBan.Click += async (_, _) => await UpdateDatBanStatusAsync("KhongDen");
            actions.Controls.AddRange(
            [
                btnXacNhanDatBan,
                btnHuyDatBan,
                btnDaDenDatBan,
                btnHoanTatDatBan,
                btnKhongDenDatBan
            ]);

            dgvDatBan.Dock = DockStyle.Fill;
            dgvDatBan.ReadOnly = true;
            dgvDatBan.AllowUserToAddRows = false;
            dgvDatBan.MultiSelect = false;
            dgvDatBan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDatBan.AutoGenerateColumns = false;
            dgvDatBan.Columns.AddRange(
            [
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.MaDatBan),
                    HeaderText = "Mã đặt bàn",
                    Width = 90
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.HoTenKhachHang),
                    HeaderText = "Khách hàng",
                    Width = 170
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.SoDienThoaiKhachHang),
                    HeaderText = "Số điện thoại",
                    Width = 130
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.TenBan),
                    HeaderText = "Bàn",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.TenKhuVuc),
                    HeaderText = "Khu vực",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.ThoiGianDen),
                    HeaderText = "Thời gian đến",
                    Width = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.SoLuongKhach),
                    HeaderText = "Số khách",
                    Width = 90
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.TrangThai),
                    HeaderText = "Trạng thái",
                    Width = 130
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(DatBanManagementResponse.GhiChu),
                    HeaderText = "Ghi chú",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            ]);
            dgvDatBan.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 &&
                    dgvDatBan.Columns[e.ColumnIndex].DataPropertyName ==
                    nameof(DatBanManagementResponse.TrangThai) &&
                    e.Value is string status)
                {
                    e.Value = DatBanStatusLabel(status);
                    e.FormattingApplied = true;
                }
            };
            dgvDatBan.SelectionChanged += (_, _) => UpdateDatBanActionStates();

            page.Controls.Add(dgvDatBan);
            page.Controls.Add(actions);
            page.Controls.Add(filters);
            tabQuanLy.Controls.Add(page);
        }

        private async Task LoadDatBanAsync()
        {
            try
            {
                var status = cmbLocTrangThaiDatBan.SelectedValue?.ToString();
                dgvDatBan.DataSource = await _datBanApiService.GetAllAsync(status);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    "Không thể tải danh sách đặt bàn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                UpdateDatBanActionStates();
            }
        }

        private async Task UpdateDatBanStatusAsync(string status)
        {
            if (dgvDatBan.CurrentRow?.DataBoundItem is not DatBanManagementResponse reservation)
            {
                MessageBox.Show(
                    this,
                    "Hãy chọn một yêu cầu đặt bàn.",
                    "Chưa chọn đặt bàn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _datBanApiService.UpdateStatusAsync(reservation.MaDatBan, status);
                await LoadDatBanAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    "Không thể cập nhật đặt bàn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateDatBanActionStates()
        {
            var status = (dgvDatBan.CurrentRow?.DataBoundItem as DatBanManagementResponse)?.TrangThai;
            btnXacNhanDatBan.Enabled = status == "ChoXacNhan";
            btnHuyDatBan.Enabled = status is "ChoXacNhan" or "DaXacNhan";
            btnDaDenDatBan.Enabled = status == "DaXacNhan";
            btnHoanTatDatBan.Enabled = status == "DaDen";
            btnKhongDenDatBan.Enabled = status == "DaDen";
        }

        private static string DatBanStatusLabel(string status) => status switch
        {
            "ChoXacNhan" => "Chờ xác nhận",
            "DaXacNhan" => "Đã xác nhận",
            "DaDen" => "Đã đến",
            "HoanTat" => "Hoàn tất",
            "DaHuy" => "Đã hủy",
            "KhongDen" => "Không đến",
            _ => status
        };

        private async Task UpdateSelectedOnlineDeliveryStatusAsync()
        {
            if (dgvDonOnline.CurrentRow?.DataBoundItem is not OnlineOrderResponse order ||
                cmbTrangThaiGiao.SelectedItem?.ToString() is not { Length: > 0 } status)
            {
                MessageBox.Show("Vui lòng chọn đơn giao hàng và trạng thái.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (order.LoaiDon != "OnlineGiaoHang")
            {
                MessageBox.Show(
                    "Chỉ đơn giao tận nơi mới có trạng thái giao hàng.",
                    "Không áp dụng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _onlineOrderApiService.UpdateDeliveryStatusAsync(order.MaDonHang, status);
                await LoadOnlineOrdersAsync();
                MessageBox.Show("Đã cập nhật trạng thái giao hàng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Không thể cập nhật giao hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowOnlineOrderDetails()
        {
            if (dgvDonOnline.CurrentRow?.DataBoundItem is not OnlineOrderResponse order)
            {
                MessageBox.Show("Vui lòng chọn một đơn online.", "Chưa chọn đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var items = string.Join(Environment.NewLine, order.MonAn.Select(item =>
                $"- {item.TenMon} x{item.SoLuong}: {item.ThanhTien:N0} đ"));
            MessageBox.Show(
                $"Mã đơn: {order.MaDonHang}\n" +
                $"Người nhận: {order.TenNguoiNhan}\n" +
                $"Số điện thoại: {order.SDTNguoiNhan}\n" +
                $"Địa chỉ: {order.DiaChiGiao ?? "Nhận tại quầy"}\n" +
                $"Trạng thái đơn: {order.TrangThaiDon}\n" +
                $"Trạng thái giao: {order.TrangThaiGiao ?? "Không áp dụng"}\n" +
                $"Tổng tiền: {order.TongTien:N0} đ\n\nMón ăn:\n{items}",
                "Chi tiết đơn online", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task UpdateOnlineOrderStatusAsync(string status)
        {
            if (dgvDonOnline.CurrentRow?.DataBoundItem is not OnlineOrderResponse order)
            {
                MessageBox.Show("Vui lòng chọn một đơn online.", "Chưa chọn đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _onlineOrderApiService.UpdateStatusAsync(order.MaDonHang, status);
                await LoadOnlineOrdersAsync();
                MessageBox.Show(status == "DangPhucVu" ? "Đã xác nhận đơn và chuyển sang chế biến." : "Đã hủy đơn online.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Không thể cập nhật đơn online", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadOnlineOrdersAsync()
        {
            try
            {
                dgvDonOnline.DataSource = await _onlineOrderApiService.GetAllAsync();
                dgvDonOnline.ClearSelection();
                UpdateOnlineOrderActionState();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Không thể tải đơn online", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateOnlineOrderActionState()
        {
            var order = dgvDonOnline.CurrentRow?.DataBoundItem as OnlineOrderResponse;
            var canProcess = order?.TrangThaiDon == "ChoXacNhan";
            var canUpdateDelivery = order?.LoaiDon == "OnlineGiaoHang";
            btnXacNhanDonOnline.Enabled = canProcess;
            btnHuyDonOnline.Enabled = canProcess;
            cmbTrangThaiGiao.Enabled = canUpdateDelivery;
            btnCapNhatTrangThaiGiao.Enabled = canUpdateDelivery;
        }
        private void btnDangXuat_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất không?",
                "Đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            ApiSession.Clear();
            Close();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadMonAnAsync();
            await LoadKhuVucAsync();
            await LoadBanAnAsync();
            await LoadDatBanAsync();
            await LoadKhoAsync();
            await LoadPaymentAsync();
            if (ApiSession.IsManager)
            {
                await LoadOnlineOrdersAsync();
            }
            await _goiMonControl.LoadAsync();
            await _cheBienControl.LoadAsync();
            if (ApiSession.IsManager)
            {
                await LoadEmployeeAccountsAsync();
            }
        }

        private async void btnTaiLai_Click(object sender, EventArgs e)
        {
            await LoadMonAnAsync();
        }

        private async Task LoadMonAnAsync()
        {
            try
            {
                var categoriesTask = _monAnApiService.GetCategoriesAsync();
                var foodsTask = _monAnApiService.GetAllAsync();
                await Task.WhenAll(categoriesTask, foodsTask);
                var foods = await foodsTask;
                var categories = await categoriesTask;

                cmbDanhMucMon.DataSource = categories;
                cmbDanhMucMon.DisplayMember = nameof(FoodCategoryResponse.TenDanhMuc);
                cmbDanhMucMon.ValueMember = nameof(FoodCategoryResponse.MaDanhMuc);
                dgvMonAn.DataSource = foods;
                dgvMonAn.ClearSelection();
                ResetMonAnForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu món ăn.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task LoadKhoAsync()
        {
            try
            {
                var ingredients = await _khoApiService.GetIngredientsAsync();
                var lowStock = await _khoApiService.GetLowStockIngredientsAsync();
                var purchaseReceipts = await _khoApiService.GetPurchaseReceiptsAsync();
                var issueReceipts = await _khoApiService.GetIssueReceiptsAsync();

                if (dgvKhoData is not null)
                {
                    dgvKhoData.DataSource = ingredients;
                    dgvKhoData.ClearSelection();
                }

                if (dgvKhoLowStock is not null)
                {
                    dgvKhoLowStock.DataSource = lowStock;
                    dgvKhoLowStock.ClearSelection();
                }

                if (dgvKhoPhieu is not null)
                {
                    var rows = new List<object>();
                    foreach (var receipt in purchaseReceipts)
                    {
                        rows.Add(new
                        {
                            MaPhieu = receipt.MaPhieuNhap,
                            Loai = "Nhập kho",
                            TrangThai = receipt.TrangThai,
                            ThoiGian = receipt.ThoiGianNhap,
                            GhiChu = receipt.GhiChu ?? string.Empty
                        });
                    }

                    foreach (var receipt in issueReceipts)
                    {
                        rows.Add(new
                        {
                            MaPhieu = receipt.MaPhieuXuat,
                            Loai = "Xuất kho",
                            TrangThai = receipt.TrangThai,
                            ThoiGian = receipt.ThoiGianXuat,
                            GhiChu = receipt.GhiChu ?? string.Empty
                        });
                    }

                    dgvKhoPhieu.DataSource = rows
                        .OrderByDescending(item => (DateTime)item.GetType().GetProperty("ThoiGian")!.GetValue(item)!)
                        .ToList();
                    dgvKhoPhieu.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu kho.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task CreateIngredientAsync()
        {
            var name = txtKhoTenNguyenLieu.Text.Trim();
            var unit = txtKhoDonViTinh.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Vui lòng nhập tên nguyên liệu và đơn vị tính.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _khoApiService.CreateIngredientAsync(new CreateNguyenLieuRequest
                {
                    TenNguyenLieu = name,
                    DonViTinh = unit,
                    MucTonToiThieu = nudKhoMucTonToiThieu.Value,
                    TrangThai = true
                });

                txtKhoTenNguyenLieu.Clear();
                txtKhoDonViTinh.Clear();
                nudKhoMucTonToiThieu.Value = 0;
                await LoadKhoAsync();
                MessageBox.Show("Đã thêm nguyên liệu.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Không thể thêm nguyên liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPaymentAsync()
        {
            try
            {
                if (dgvThanhToan is null)
                {
                    return;
                }

                var orders = await new GoiMonApiService().GetOrdersAsync();
                var rows = new List<object>();
                foreach (var order in orders)
                {
                    var invoice = await _thanhToanApiService.GetInvoiceAsync(order.MaDonHang);
                    rows.Add(new
                    {
                        order.MaDonHang,
                        order.MaBan,
                        order.TenBan,
                        order.TrangThaiDon,
                        order.SoMon,
                        order.TongTien,
                        HoaDon = invoice is null ? "Chưa có" : invoice.TrangThaiThanhToan
                    });
                }

                dgvThanhToan.DataSource = rows;
                dgvThanhToan.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu thanh toán.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InitializeKhoTab()
        {
            var page = new TabPage("Kho");
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var khoTabs = new TabControl
            {
                Dock = DockStyle.Fill
            };
            var overviewPage = new TabPage("Tổng quan tồn kho")
            {
                Padding = new Padding(12)
            };
            var movementPage = new TabPage("Nhập / xuất kho")
            {
                Padding = new Padding(12)
            };
            khoTabs.TabPages.Add(overviewPage);
            khoTabs.TabPages.Add(movementPage);
            panel.Controls.Add(khoTabs);

            var grpThemNguyenLieu = new GroupBox
            {
                Text = "Thêm nguyên liệu",
                Dock = DockStyle.Top,
                Height = 86,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            overviewPage.Controls.Add(grpThemNguyenLieu);

            grpThemNguyenLieu.Controls.Add(new Label { Text = "Tên nguyên liệu", AutoSize = true, Location = new Point(12, 30) });
            txtKhoTenNguyenLieu.Location = new Point(115, 26);
            txtKhoTenNguyenLieu.Size = new Size(220, 26);
            grpThemNguyenLieu.Controls.Add(txtKhoTenNguyenLieu);

            grpThemNguyenLieu.Controls.Add(new Label { Text = "ĐVT", AutoSize = true, Location = new Point(355, 30) });
            txtKhoDonViTinh.Location = new Point(395, 26);
            txtKhoDonViTinh.Size = new Size(90, 26);
            grpThemNguyenLieu.Controls.Add(txtKhoDonViTinh);

            grpThemNguyenLieu.Controls.Add(new Label { Text = "Tồn tối thiểu", AutoSize = true, Location = new Point(505, 30) });
            nudKhoMucTonToiThieu.Location = new Point(595, 26);
            nudKhoMucTonToiThieu.Size = new Size(100, 26);
            nudKhoMucTonToiThieu.DecimalPlaces = 2;
            nudKhoMucTonToiThieu.Maximum = 999999999;
            grpThemNguyenLieu.Controls.Add(nudKhoMucTonToiThieu);

            btnKhoThemNguyenLieu.Text = "Thêm nguyên liệu";
            btnKhoThemNguyenLieu.Size = new Size(145, 32);
            btnKhoThemNguyenLieu.Location = new Point(720, 24);
            btnKhoThemNguyenLieu.Click += async (_, _) => await CreateIngredientAsync();
            grpThemNguyenLieu.Controls.Add(btnKhoThemNguyenLieu);

            var lblKho = new Label
            {
                Text = "Tồn kho nguyên liệu",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 32,
                Padding = new Padding(0, 4, 0, 0)
            };
            overviewPage.Controls.Add(lblKho);

            dgvKhoData.Dock = DockStyle.Top;
            dgvKhoData.Height = 190;
            dgvKhoData.ReadOnly = true;
            dgvKhoData.AutoGenerateColumns = false;
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MaNguyenLieu), HeaderText = "Mã", Width = 60 });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TenNguyenLieu), HeaderText = "Tên nguyên liệu", Width = 220 });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.DonViTinh), HeaderText = "ĐVT", Width = 80 });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MucTonToiThieu), HeaderText = "Tối thiểu", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TonKho), HeaderText = "Tồn kho", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvKhoData.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TrangThai), HeaderText = "Hoạt động", Width = 100 });
            dgvKhoData.SelectionChanged += (_, _) =>
            {
                if (dgvKhoData.CurrentRow?.DataBoundItem is not NguyenLieuDto ingredient)
                {
                    return;
                }

                txtKhoMaNguyenLieuNhap.Text = ingredient.MaNguyenLieu.ToString();
                txtKhoMaNguyenLieuXuat.Text = ingredient.MaNguyenLieu.ToString();
            };
            overviewPage.Controls.Add(dgvKhoData);

            var lblTonThap = new Label
            {
                Text = "Nguyên liệu dưới mức tối thiểu",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 32,
                Padding = new Padding(0, 4, 0, 0)
            };
            overviewPage.Controls.Add(lblTonThap);

            dgvKhoLowStock.Dock = DockStyle.Top;
            dgvKhoLowStock.Height = 175;
            dgvKhoLowStock.ReadOnly = true;
            dgvKhoLowStock.AutoGenerateColumns = false;
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MaNguyenLieu), HeaderText = "Mã", Width = 60 });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TenNguyenLieu), HeaderText = "Tên nguyên liệu", Width = 220 });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.DonViTinh), HeaderText = "ĐVT", Width = 80 });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MucTonToiThieu), HeaderText = "Tối thiểu", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TonKho), HeaderText = "Tồn kho", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            overviewPage.Controls.Add(dgvKhoLowStock);

            var grpNhapNhanh = new GroupBox
            {
                Text = "Nhập kho nhanh",
                Location = new Point(12, 12),
                Size = new Size(450, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            movementPage.Controls.Add(grpNhapNhanh);

            var lblKhoMaNhap = new Label { Text = "Mã NL", AutoSize = true, Location = new Point(12, 28) };
            txtKhoMaNguyenLieuNhap.Location = new Point(90, 24);
            txtKhoMaNguyenLieuNhap.Size = new Size(80, 26);
            grpNhapNhanh.Controls.Add(lblKhoMaNhap);
            grpNhapNhanh.Controls.Add(txtKhoMaNguyenLieuNhap);

            var lblKhoSoLuongNhap = new Label { Text = "Số lượng", AutoSize = true, Location = new Point(180, 28) };
            nudKhoSoLuongNhap.Location = new Point(260, 24);
            nudKhoSoLuongNhap.Size = new Size(80, 26);
            nudKhoSoLuongNhap.DecimalPlaces = 2;
            nudKhoSoLuongNhap.Minimum = 1;
            nudKhoSoLuongNhap.Maximum = 999999;
            nudKhoSoLuongNhap.Value = 1;
            grpNhapNhanh.Controls.Add(lblKhoSoLuongNhap);
            grpNhapNhanh.Controls.Add(nudKhoSoLuongNhap);

            var lblKhoDonGiaNhap = new Label { Text = "Đơn giá", AutoSize = true, Location = new Point(12, 64) };
            nudKhoDonGiaNhap.Location = new Point(90, 60);
            nudKhoDonGiaNhap.Size = new Size(120, 26);
            nudKhoDonGiaNhap.DecimalPlaces = 2;
            nudKhoDonGiaNhap.Minimum = 0;
            nudKhoDonGiaNhap.Maximum = 999999999;
            nudKhoDonGiaNhap.Value = 0;
            grpNhapNhanh.Controls.Add(lblKhoDonGiaNhap);
            grpNhapNhanh.Controls.Add(nudKhoDonGiaNhap);

            var lblKhoGhiChuNhap = new Label { Text = "Ghi chú", AutoSize = true, Location = new Point(220, 64) };
            txtKhoGhiChuNhap.Location = new Point(285, 60);
            txtKhoGhiChuNhap.Size = new Size(150, 26);
            grpNhapNhanh.Controls.Add(lblKhoGhiChuNhap);
            grpNhapNhanh.Controls.Add(txtKhoGhiChuNhap);

            var btnKhoRefresh = new Button
            {
                Text = "Làm mới",
                Size = new Size(90, 32),
                Location = new Point(305, 88)
            };
            btnKhoRefresh.Click += async (_, _) => await LoadKhoAsync();
            grpNhapNhanh.Controls.Add(btnKhoRefresh);

            btnKhoNhapNhanh.Text = "Tạo phiếu nhập";
            btnKhoNhapNhanh.Size = new Size(130, 32);
            btnKhoNhapNhanh.Location = new Point(305, 48);
            btnKhoNhapNhanh.Click += async (_, _) => await CreateQuickPurchaseReceiptAsync();
            grpNhapNhanh.Controls.Add(btnKhoNhapNhanh);

            var grpXuatNhanh = new GroupBox
            {
                Text = "Xuất kho nhanh",
                Location = new Point(474, 12),
                Size = new Size(468, 120),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            movementPage.Controls.Add(grpXuatNhanh);

            var lblKhoMaXuat = new Label { Text = "Mã NL", AutoSize = true, Location = new Point(12, 28) };
            txtKhoMaNguyenLieuXuat.Location = new Point(90, 24);
            txtKhoMaNguyenLieuXuat.Size = new Size(70, 26);
            grpXuatNhanh.Controls.Add(lblKhoMaXuat);
            grpXuatNhanh.Controls.Add(txtKhoMaNguyenLieuXuat);

            var lblKhoSoLuongXuat = new Label { Text = "Số lượng", AutoSize = true, Location = new Point(170, 28) };
            nudKhoSoLuongXuat.Location = new Point(250, 24);
            nudKhoSoLuongXuat.Size = new Size(80, 26);
            nudKhoSoLuongXuat.DecimalPlaces = 2;
            nudKhoSoLuongXuat.Minimum = 1;
            nudKhoSoLuongXuat.Maximum = 999999;
            nudKhoSoLuongXuat.Value = 1;
            grpXuatNhanh.Controls.Add(lblKhoSoLuongXuat);
            grpXuatNhanh.Controls.Add(nudKhoSoLuongXuat);

            var lblKhoLoaiXuat = new Label { Text = "Loại", AutoSize = true, Location = new Point(340, 28) };
            cmbKhoLoaiXuat.Location = new Point(385, 24);
            cmbKhoLoaiXuat.Size = new Size(70, 26);
            cmbKhoLoaiXuat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKhoLoaiXuat.Items.Add("CheBien");
            cmbKhoLoaiXuat.Items.Add("ThuCong");
            cmbKhoLoaiXuat.SelectedIndex = 0;
            grpXuatNhanh.Controls.Add(lblKhoLoaiXuat);
            grpXuatNhanh.Controls.Add(cmbKhoLoaiXuat);

            var lblKhoMaDonHang = new Label { Text = "Mã ĐH", AutoSize = true, Location = new Point(12, 64) };
            nudKhoMaChiTietDonHang.Location = new Point(90, 60);
            nudKhoMaChiTietDonHang.Size = new Size(70, 26);
            nudKhoMaChiTietDonHang.Minimum = 0;
            nudKhoMaChiTietDonHang.Maximum = 999999;
            nudKhoMaChiTietDonHang.Value = 0;
            grpXuatNhanh.Controls.Add(lblKhoMaDonHang);
            grpXuatNhanh.Controls.Add(nudKhoMaChiTietDonHang);

            var lblKhoMaNvXuat = new Label { Text = "Mã NV", AutoSize = true, Location = new Point(170, 64) };
            nudKhoMaNhanVienXuat.Location = new Point(250, 60);
            nudKhoMaNhanVienXuat.Size = new Size(70, 26);
            nudKhoMaNhanVienXuat.Minimum = 0;
            nudKhoMaNhanVienXuat.Maximum = 999999;
            nudKhoMaNhanVienXuat.Value = 0;
            grpXuatNhanh.Controls.Add(lblKhoMaNvXuat);
            grpXuatNhanh.Controls.Add(nudKhoMaNhanVienXuat);

            var lblKhoLyDoXuat = new Label { Text = "Lý do", AutoSize = true, Location = new Point(330, 64) };
            txtKhoLyDoXuat.Location = new Point(385, 60);
            txtKhoLyDoXuat.Size = new Size(70, 26);
            grpXuatNhanh.Controls.Add(lblKhoLyDoXuat);
            grpXuatNhanh.Controls.Add(txtKhoLyDoXuat);

            var lblKhoGhiChuXuat = new Label { Text = "Ghi chú", AutoSize = true, Location = new Point(12, 96) };
            txtKhoGhiChuXuat.Location = new Point(90, 92);
            txtKhoGhiChuXuat.Size = new Size(220, 26);
            grpXuatNhanh.Controls.Add(lblKhoGhiChuXuat);
            grpXuatNhanh.Controls.Add(txtKhoGhiChuXuat);

            btnKhoXuatNhanh.Text = "Tạo phiếu xuất";
            btnKhoXuatNhanh.Size = new Size(130, 32);
            btnKhoXuatNhanh.Location = new Point(320, 88);
            btnKhoXuatNhanh.Click += async (_, _) => await CreateQuickIssueReceiptAsync();
            grpXuatNhanh.Controls.Add(btnKhoXuatNhanh);

            var lblPhieuKho = new Label
            {
                Text = "Phiếu nhập/xuất kho gần nhất",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(12, 155)
            };
            movementPage.Controls.Add(lblPhieuKho);

            dgvKhoPhieu.Location = new Point(12, 183);
            dgvKhoPhieu.Size = new Size(930, 150);
            dgvKhoPhieu.ReadOnly = true;
            dgvKhoPhieu.AutoGenerateColumns = false;
            dgvKhoPhieu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaPhieu", HeaderText = "Mã phiếu", Width = 90 });
            dgvKhoPhieu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Loai", HeaderText = "Loại", Width = 120 });
            dgvKhoPhieu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThai", HeaderText = "Trạng thái", Width = 130 });
            dgvKhoPhieu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ThoiGian", HeaderText = "Thời gian", Width = 180, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            dgvKhoPhieu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GhiChu", HeaderText = "Ghi chú", Width = 320 });
            movementPage.Controls.Add(dgvKhoPhieu);

            btnKhoXacNhanNhap.Text = "Xác nhận nhập";
            btnKhoXacNhanNhap.Size = new Size(130, 32);
            btnKhoXacNhanNhap.Location = new Point(12, 345);
            btnKhoXacNhanNhap.Click += async (_, _) => await UpdateSelectedWarehouseReceiptAsync(isPurchase: true, confirmed: true);
            movementPage.Controls.Add(btnKhoXacNhanNhap);

            btnKhoHuyNhap.Text = "Hủy nhập";
            btnKhoHuyNhap.Size = new Size(120, 32);
            btnKhoHuyNhap.Location = new Point(150, 345);
            btnKhoHuyNhap.Click += async (_, _) => await UpdateSelectedWarehouseReceiptAsync(isPurchase: true, confirmed: false);
            movementPage.Controls.Add(btnKhoHuyNhap);

            btnKhoXacNhanXuat.Text = "Xác nhận xuất";
            btnKhoXacNhanXuat.Size = new Size(130, 32);
            btnKhoXacNhanXuat.Location = new Point(282, 345);
            btnKhoXacNhanXuat.Click += async (_, _) => await UpdateSelectedWarehouseReceiptAsync(isPurchase: false, confirmed: true);
            movementPage.Controls.Add(btnKhoXacNhanXuat);

            btnKhoHuyXuat.Text = "Hủy xuất";
            btnKhoHuyXuat.Size = new Size(120, 32);
            btnKhoHuyXuat.Location = new Point(420, 345);
            btnKhoHuyXuat.Click += async (_, _) => await UpdateSelectedWarehouseReceiptAsync(isPurchase: false, confirmed: false);
            movementPage.Controls.Add(btnKhoHuyXuat);

            page.Controls.Add(panel);
            tabQuanLy.Controls.Add(page);
        }

        private void InitializeAccountTab()
        {
            if (!ApiSession.IsManager)
            {
                return;
            }

            var page = new TabPage("Tài khoản nhân viên") { Padding = new Padding(12) };
            var panel = new Panel { Dock = DockStyle.Fill };
            var editor = new GroupBox
            {
                Text = "Tạo tài khoản nhân viên",
                Dock = DockStyle.Top,
                Height = 142,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            AddAccountField(editor, "Tên đăng nhập", txtTaiKhoanUsername, 12, 28, 150);
            AddAccountField(editor, "Mật khẩu", txtTaiKhoanPassword, 185, 28, 150);
            txtTaiKhoanPassword.UseSystemPasswordChar = true;
            AddAccountField(editor, "Họ tên", txtTaiKhoanHoTen, 358, 28, 190);
            AddAccountField(editor, "Số điện thoại", txtTaiKhoanSoDienThoai, 556, 28, 130);
            editor.Controls.Add(new Label { Text = "Ngày sinh", AutoSize = true, Location = new Point(12, 72) });
            dtpTaiKhoanNgaySinh.Location = new Point(85, 68);
            dtpTaiKhoanNgaySinh.Size = new Size(150, 26);
            dtpTaiKhoanNgaySinh.Format = DateTimePickerFormat.Short;
            dtpTaiKhoanNgaySinh.ShowCheckBox = true;
            editor.Controls.Add(dtpTaiKhoanNgaySinh);
            btnTaiKhoanThem.Text = "Tạo tài khoản";
            btnTaiKhoanThem.Size = new Size(140, 32);
            btnTaiKhoanThem.Location = new Point(255, 66);
            btnTaiKhoanThem.Click += async (_, _) => await CreateEmployeeAccountAsync();
            editor.Controls.Add(btnTaiKhoanThem);
            editor.Controls.Add(new Label { Text = "Phân quyền mặc định: Nhân viên", AutoSize = true, Location = new Point(410, 74) });
            panel.Controls.Add(editor);

            dgvTaiKhoanNhanVien.Dock = DockStyle.Fill;
            dgvTaiKhoanNhanVien.ReadOnly = true;
            dgvTaiKhoanNhanVien.AutoGenerateColumns = false;
            dgvTaiKhoanNhanVien.AllowUserToAddRows = false;
            dgvTaiKhoanNhanVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTaiKhoanNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(EmployeeAccountResponse.EmployeeId), HeaderText = "Mã NV", FillWeight = 55, MinimumWidth = 65 });
            dgvTaiKhoanNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(EmployeeAccountResponse.UserName), HeaderText = "Tài khoản", FillWeight = 100, MinimumWidth = 110 });
            dgvTaiKhoanNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(EmployeeAccountResponse.FullName), HeaderText = "Họ tên", FillWeight = 135, MinimumWidth = 130 });
            dgvTaiKhoanNhanVien.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(EmployeeAccountResponse.RoleName), HeaderText = "Vai trò", FillWeight = 90, MinimumWidth = 100 });
            dgvTaiKhoanNhanVien.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(EmployeeAccountResponse.IsActive), HeaderText = "Hoạt động", FillWeight = 80, MinimumWidth = 90 });
            panel.Controls.Add(dgvTaiKhoanNhanVien);
            dgvTaiKhoanNhanVien.BringToFront();
            page.Controls.Add(panel);
            tabQuanLy.Controls.Add(page);
        }

        private static void AddAccountField(Control parent, string labelText, TextBox textBox, int x, int y, int width)
        {
            parent.Controls.Add(new Label { Text = labelText, AutoSize = true, Location = new Point(x, y + 4) });
            textBox.Location = new Point(x + 78, y);
            textBox.Size = new Size(width, 26);
            parent.Controls.Add(textBox);
        }

        private async Task LoadEmployeeAccountsAsync()
        {
            try
            {
                dgvTaiKhoanNhanVien.DataSource = await _accountApiService.GetEmployeesAsync();
                dgvTaiKhoanNhanVien.ClearSelection();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Không thể tải tài khoản nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CreateEmployeeAccountAsync()
        {
            if (string.IsNullOrWhiteSpace(txtTaiKhoanUsername.Text) ||
                string.IsNullOrWhiteSpace(txtTaiKhoanPassword.Text) ||
                string.IsNullOrWhiteSpace(txtTaiKhoanHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập, mật khẩu và họ tên.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _accountApiService.CreateEmployeeAsync(new CreateEmployeeAccountRequest
                {
                    UserName = txtTaiKhoanUsername.Text.Trim(),
                    Password = txtTaiKhoanPassword.Text,
                    FullName = txtTaiKhoanHoTen.Text.Trim(),
                    PhoneNumber = string.IsNullOrWhiteSpace(txtTaiKhoanSoDienThoai.Text) ? null : txtTaiKhoanSoDienThoai.Text.Trim(),
                    BirthDate = dtpTaiKhoanNgaySinh.Checked ? DateOnly.FromDateTime(dtpTaiKhoanNgaySinh.Value) : null,
                    RoleName = ApiSession.EmployeeRole
                });

                txtTaiKhoanUsername.Clear();
                txtTaiKhoanPassword.Clear();
                txtTaiKhoanHoTen.Clear();
                txtTaiKhoanSoDienThoai.Clear();
                dtpTaiKhoanNgaySinh.Checked = false;
                await LoadEmployeeAccountsAsync();
                MessageBox.Show("Đã tạo tài khoản nhân viên và liên kết hồ sơ nhân viên.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Không thể tạo tài khoản nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializePaymentTab()
        {
            var page = new TabPage("Thanh toán");
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 1,
                RowCount = 3
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            var lblTitle = new Label
            {
                Text = "Tình trạng hóa đơn và trạng thái thanh toán",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(lblTitle, 0, 0);

            dgvThanhToan.Dock = DockStyle.Fill;
            dgvThanhToan.ReadOnly = true;
            dgvThanhToan.AutoGenerateColumns = false;
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaDonHang", DataPropertyName = "MaDonHang", HeaderText = "Mã đơn", Width = 80 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaBan", HeaderText = "Mã bàn", Width = 80 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenBan", HeaderText = "Tên bàn", Width = 150 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThaiDon", HeaderText = "Trạng thái đơn", Width = 160 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoMon", HeaderText = "Số món", Width = 80 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TongTien", HeaderText = "Tổng tiền", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoaDon", HeaderText = "Hóa đơn", Width = 130 });
            panel.Controls.Add(dgvThanhToan, 0, 1);

            var btnReload = new Button
            {
                Text = "Tải lại",
                Size = new Size(120, 32),
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left
            };
            btnReload.Click += async (_, _) => await LoadPaymentAsync();
            panel.Controls.Add(btnReload, 0, 2);

            page.Controls.Add(panel);
            tabQuanLy.Controls.Add(page);
        }

        private async Task CreateQuickPurchaseReceiptAsync()
        {
            if (!int.TryParse(txtKhoMaNguyenLieuNhap.Text.Trim(), out var materialId) || materialId <= 0)
            {
                MessageBox.Show("Vui lòng nhập mã nguyên liệu hợp lệ.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nudKhoDonGiaNhap.Value < 0)
            {
                MessageBox.Show("Đơn giá nhập không hợp lệ.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var request = new CreatePhieuNhapRequest
                {
                    ChiTiet = new[]
                    {
                        new NguyenLieuItemRequest(materialId, nudKhoSoLuongNhap.Value, nudKhoDonGiaNhap.Value)
                    },
                    GhiChu = string.IsNullOrWhiteSpace(txtKhoGhiChuNhap.Text)
                        ? null
                        : txtKhoGhiChuNhap.Text.Trim()
                };

                await _khoApiService.CreatePurchaseReceiptAsync(request);
                await LoadKhoAsync();
                MessageBox.Show("Đã tạo phiếu nhập kho thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không thể tạo phiếu nhập kho",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CreateQuickIssueReceiptAsync()
        {
            if (!int.TryParse(txtKhoMaNguyenLieuXuat.Text.Trim(), out var materialId) || materialId <= 0)
            {
                MessageBox.Show("Vui lòng nhập mã nguyên liệu hợp lệ.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var type = cmbKhoLoaiXuat.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(type) || type is not ("CheBien" or "ThuCong"))
            {
                MessageBox.Show("Vui lòng chọn loại xuất kho hợp lệ.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (type == "CheBien" && nudKhoMaChiTietDonHang.Value <= 0)
            {
                MessageBox.Show("Xuất chế biến phải điền mã chi tiết đơn hàng.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (type == "ThuCong" && nudKhoMaNhanVienXuat.Value <= 0)
            {
                MessageBox.Show("Xuất thủ công phải điền mã nhân viên thực hiện.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var request = new CreatePhieuXuatRequest
                {
                    LoaiXuat = type,
                    ChiTiet = new[]
                    {
                        new NguyenLieuItemRequest(materialId, nudKhoSoLuongXuat.Value, 0m)
                    },
                    MaChiTietDonHang = type == "CheBien" ? (int)nudKhoMaChiTietDonHang.Value : null,
                    MaNhanVien = type == "ThuCong" ? (int)nudKhoMaNhanVienXuat.Value : null,
                    LyDo = string.IsNullOrWhiteSpace(txtKhoLyDoXuat.Text)
                        ? null
                        : txtKhoLyDoXuat.Text.Trim(),
                    GhiChu = string.IsNullOrWhiteSpace(txtKhoGhiChuXuat.Text)
                        ? null
                        : txtKhoGhiChuXuat.Text.Trim()
                };

                await _khoApiService.CreateIssueReceiptAsync(request);
                await LoadKhoAsync();
                MessageBox.Show("Đã tạo phiếu xuất kho thành công.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không thể tạo phiếu xuất kho",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateSelectedWarehouseReceiptAsync(bool isPurchase, bool confirmed)
        {
            if (dgvKhoPhieu.CurrentRow is null || dgvKhoPhieu.CurrentRow.DataBoundItem is null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu kho để cập nhật.", "Chưa chọn phiếu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = dgvKhoPhieu.CurrentRow.DataBoundItem;
            var id = (int)item.GetType().GetProperty("MaPhieu")!.GetValue(item)!;
            var receiptType = item.GetType().GetProperty("Loai")?.GetValue(item)?.ToString();
            var expectedType = isPurchase ? "Nhập kho" : "Xuất kho";
            if (!string.Equals(receiptType, expectedType, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    $"Bạn đang chọn {receiptType ?? "phiếu không xác định"}. Hãy chọn phiếu {expectedType} để thực hiện thao tác này.",
                    "Sai loại phiếu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var status = confirmed ? "XacNhan" : "Huy";

            try
            {
                if (isPurchase)
                {
                    await _khoApiService.UpdatePurchaseReceiptStatusAsync(id, status);
                }
                else
                {
                    await _khoApiService.UpdateIssueReceiptStatusAsync(id, status);
                }

                await LoadKhoAsync();
                MessageBox.Show("Đã cập nhật trạng thái phiếu kho.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không thể cập nhật phiếu kho",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateSelectedOrderDeliveryStatusAsync()
        {
            if (dgvThanhToan.CurrentRow is null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần cập nhật trạng thái giao.", "Chưa chọn đơn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var orderId = Convert.ToInt32(dgvThanhToan.CurrentRow.Cells["MaDonHang"].Value);
            var status = cmbTrangThaiGiao.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("Vui lòng chọn trạng thái giao hàng.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _thanhToanApiService.UpdateDeliveryStatusAsync(orderId, status);
                await LoadPaymentAsync();
                MessageBox.Show($"Đã cập nhật trạng thái giao hàng thành {status}.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Không thể cập nhật giao hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeMonAnTab()
        {
            var editor = new Panel
            {
                Location = new Point(12, 12),
                Size = new Size(968, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            AddFoodLabel(editor, "Mã món", 8, 8);
            txtMaMon.Location = new Point(95, 5);
            txtMaMon.Size = new Size(80, 27);
            txtMaMon.ReadOnly = true;
            editor.Controls.Add(txtMaMon);

            AddFoodLabel(editor, "Tên món", 190, 8);
            txtTenMon.Location = new Point(270, 5);
            txtTenMon.Size = new Size(270, 27);
            txtTenMon.MaxLength = 150;
            editor.Controls.Add(txtTenMon);

            AddFoodLabel(editor, "Đơn giá", 555, 8);
            nudDonGiaMon.Location = new Point(625, 5);
            nudDonGiaMon.Size = new Size(135, 27);
            nudDonGiaMon.DecimalPlaces = 2;
            nudDonGiaMon.Minimum = 0.01m;
            nudDonGiaMon.Maximum = 999999999m;
            editor.Controls.Add(nudDonGiaMon);

            AddFoodLabel(editor, "Danh mục", 775, 8);
            cmbDanhMucMon.Location = new Point(850, 5);
            cmbDanhMucMon.Size = new Size(110, 28);
            cmbDanhMucMon.DropDownStyle = ComboBoxStyle.DropDownList;
            editor.Controls.Add(cmbDanhMucMon);

            AddFoodLabel(editor, "Mô tả", 8, 44);
            txtMoTaMon.Location = new Point(95, 41);
            txtMoTaMon.Size = new Size(445, 27);
            txtMoTaMon.MaxLength = 500;
            editor.Controls.Add(txtMoTaMon);

            AddFoodLabel(editor, "Hình ảnh (URL)", 555, 44);
            txtHinhAnhMon.Location = new Point(655, 41);
            txtHinhAnhMon.Size = new Size(305, 27);
            txtHinhAnhMon.MaxLength = 500;
            editor.Controls.Add(txtHinhAnhMon);

            chkMonDangBan.Location = new Point(95, 78);
            chkMonDangBan.AutoSize = true;
            chkMonDangBan.Text = "Đang bán";
            chkMonDangBan.Checked = true;
            editor.Controls.Add(chkMonDangBan);

            AddFoodLabel(editor, "Danh mục mới", 8, 112);
            txtTenDanhMuc.Location = new Point(105, 109);
            txtTenDanhMuc.Size = new Size(190, 27);
            txtTenDanhMuc.MaxLength = 100;
            editor.Controls.Add(txtTenDanhMuc);

            AddFoodLabel(editor, "Mô tả danh mục", 310, 112);
            txtMoTaDanhMuc.Location = new Point(415, 109);
            txtMoTaDanhMuc.Size = new Size(300, 27);
            txtMoTaDanhMuc.MaxLength = 500;
            editor.Controls.Add(txtMoTaDanhMuc);

            btnThemDanhMuc.Location = new Point(735, 107);
            btnThemDanhMuc.Size = new Size(125, 32);
            btnThemDanhMuc.Text = "Thêm danh mục";
            btnThemDanhMuc.Click += async (_, _) => await btnThemDanhMuc_Click();
            btnThemDanhMuc.Enabled = ApiSession.IsManager;
            editor.Controls.Add(btnThemDanhMuc);

            ConfigureFoodButton(btnThemMon, "Thêm món", 220);
            btnThemMon.Click += btnThemMon_Click;
            editor.Controls.Add(btnThemMon);
            ConfigureFoodButton(btnSuaMon, "Cập nhật món", 330);
            btnSuaMon.Click += btnSuaMon_Click;
            editor.Controls.Add(btnSuaMon);
            ConfigureFoodButton(btnXoaMon, "Ngừng bán", 455);
            btnXoaMon.Click += btnXoaMon_Click;
            editor.Controls.Add(btnXoaMon);
            ConfigureFoodButton(btnLamMoiMon, "Làm mới", 575);
            btnLamMoiMon.Click += (_, _) => ResetMonAnForm();
            editor.Controls.Add(btnLamMoiMon);

            dgvMonAn.Location = new Point(12, 170);
            dgvMonAn.Size = new Size(968, 373);
            dgvMonAn.AutoGenerateColumns = false;
            dgvMonAn.Columns.Clear();
            dgvMonAn.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(MonAnDto.MaMon),
                HeaderText = "Mã món",
                Width = 75
            });
            dgvMonAn.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(MonAnDto.TenMon),
                HeaderText = "Tên món",
                Width = 210
            });
            dgvMonAn.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(MonAnDto.TenDanhMuc),
                HeaderText = "Danh mục",
                Width = 150
            });
            dgvMonAn.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(MonAnDto.DonGia),
                HeaderText = "Đơn giá",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0"
                }
            });
            dgvMonAn.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(MonAnDto.TrangThai),
                HeaderText = "Đang bán",
                Width = 100
            });
            dgvMonAn.SelectionChanged += dgvMonAn_SelectionChanged;
            editor.BringToFront();
            tabMonAn.Controls.Add(editor);
            editor.BringToFront();
            dgvMonAn.BringToFront();
            editor.BringToFront();

            btnSuaMon.Enabled = ApiSession.IsManager;
            btnXoaMon.Enabled = ApiSession.IsManager;
            btnThemMon.Enabled = ApiSession.IsManager;
        }

        private async Task btnThemDanhMuc_Click()
        {
            var name = txtTenDanhMuc.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên danh mục.",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _monAnApiService.CreateCategoryAsync(new CreateFoodCategoryRequest
                {
                    TenDanhMuc = name,
                    MoTa = string.IsNullOrWhiteSpace(txtMoTaDanhMuc.Text)
                        ? null
                        : txtMoTaDanhMuc.Text.Trim()
                });

                txtTenDanhMuc.Clear();
                txtMoTaDanhMuc.Clear();
                await LoadMonAnAsync();
                MessageBox.Show(
                    "Đã thêm danh mục.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "Không thể thêm danh mục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void AddFoodLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(x, y + 4),
                Text = text
            });
        }

        private static void ConfigureFoodButton(Button button, string text, int x)
        {
            button.Location = new Point(x, 76);
            button.Size = new Size(110, 32);
            button.Text = text;
        }

        private void dgvMonAn_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvMonAn.CurrentRow?.DataBoundItem is not MonAnDto food)
            {
                ResetMonAnForm();
                return;
            }

            txtMaMon.Text = food.MaMon.ToString();
            txtTenMon.Text = food.TenMon;
            txtMoTaMon.Text = food.MoTa ?? string.Empty;
            txtHinhAnhMon.Text = food.HinhAnh ?? string.Empty;
            nudDonGiaMon.Value = Math.Clamp(food.DonGia, nudDonGiaMon.Minimum, nudDonGiaMon.Maximum);
            cmbDanhMucMon.SelectedValue = food.MaDanhMuc;
            chkMonDangBan.Checked = food.TrangThai;
            btnSuaMon.Enabled = ApiSession.IsManager;
            btnXoaMon.Enabled = ApiSession.IsManager && food.TrangThai;
        }

        private void ResetMonAnForm()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            txtMoTaMon.Clear();
            txtHinhAnhMon.Clear();
            nudDonGiaMon.Value = nudDonGiaMon.Minimum;
            chkMonDangBan.Checked = true;
            if (cmbDanhMucMon.Items.Count > 0)
            {
                cmbDanhMucMon.SelectedIndex = 0;
            }

            btnThemMon.Enabled = ApiSession.IsManager;
            btnSuaMon.Enabled = false;
            btnXoaMon.Enabled = false;
        }

        private bool ValidateMonAnInput()
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Vui lòng nhập tên món.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMon.Focus();
                return false;
            }

            if (cmbDanhMucMon.SelectedValue is not int)
            {
                MessageBox.Show("Vui lòng chọn danh mục món ăn.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private CreateFoodRequest BuildCreateFoodRequest() =>
            new()
            {
                TenMon = txtTenMon.Text.Trim(),
                MoTa = txtMoTaMon.Text.Trim(),
                DonGia = nudDonGiaMon.Value,
                HinhAnh = txtHinhAnhMon.Text.Trim(),
                MaDanhMuc = (int)cmbDanhMucMon.SelectedValue!,
                TrangThai = chkMonDangBan.Checked
            };

        private UpdateFoodRequest BuildUpdateFoodRequest() =>
            new()
            {
                TenMon = txtTenMon.Text.Trim(),
                MoTa = txtMoTaMon.Text.Trim(),
                DonGia = nudDonGiaMon.Value,
                HinhAnh = txtHinhAnhMon.Text.Trim(),
                MaDanhMuc = (int)cmbDanhMucMon.SelectedValue!,
                TrangThai = chkMonDangBan.Checked
            };

        private async void btnThemMon_Click(object? sender, EventArgs e)
        {
            if (!ValidateMonAnInput())
            {
                return;
            }

            try
            {
                await _monAnApiService.CreateAsync(BuildCreateFoodRequest());
                await LoadMonAnAsync();
                MessageBox.Show("Đã thêm món ăn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowMonAnError(exception);
            }
        }

        private async void btnSuaMon_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaMon.Text, out var id) || !ValidateMonAnInput())
            {
                return;
            }

            try
            {
                await _monAnApiService.UpdateAsync(id, BuildUpdateFoodRequest());
                await LoadMonAnAsync();
                MessageBox.Show("Đã cập nhật món ăn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowMonAnError(exception);
            }
        }

        private async void btnXoaMon_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaMon.Text, out var id))
            {
                MessageBox.Show("Vui lòng chọn món cần ngừng bán.", "Chưa chọn món",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                    "Món sẽ được ẩn khỏi thực đơn nhưng vẫn giữ trong lịch sử đơn. Tiếp tục?",
                    "Xác nhận ngừng bán",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                await _monAnApiService.DeleteAsync(id);
                await LoadMonAnAsync();
                MessageBox.Show("Đã ngừng bán món ăn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowMonAnError(exception);
            }
        }

        private static void ShowMonAnError(Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Không thể xử lý món ăn",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private async void btnTaiKhuVuc_Click(object sender, EventArgs e)
        {
            await LoadKhuVucAsync();
        }

        private async void btnThemKhuVuc_Click(object sender, EventArgs e)
        {
            if (!ValidateKhuVucInput())
            {
                    return;
            }

            try
            {
                    await _khuVucApiService.CreateAsync(new CreateKhuVucRequest
                    {
                        TenKhuVuc = txtTenKhuVuc.Text.Trim(),
                        MoTa = txtMoTaKhuVuc.Text.Trim()
                    });
                    await LoadKhuVucAsync();
                    MessageBox.Show("Đã thêm khu vực.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                    ShowKhuVucError(ex);
            }
        }

        private async void btnSuaKhuVuc_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMaKhuVuc.Text, out var id))
            {
                    MessageBox.Show("Vui lòng chọn khu vực cần sửa.", "Chưa chọn khu vực",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            if (!ValidateKhuVucInput())
            {
                    return;
            }

            try
            {
                    await _khuVucApiService.UpdateAsync(id, new UpdateKhuVucRequest
                    {
                        TenKhuVuc = txtTenKhuVuc.Text.Trim(),
                        MoTa = txtMoTaKhuVuc.Text.Trim()
                    });
                    await LoadKhuVucAsync();
                    MessageBox.Show("Đã cập nhật khu vực.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                    ShowKhuVucError(ex);
            }
        }

        private async void btnXoaKhuVuc_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMaKhuVuc.Text, out var id))
            {
                    MessageBox.Show("Vui lòng chọn khu vực cần xóa.", "Chưa chọn khu vực",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            if (MessageBox.Show(
                        "Bạn có chắc muốn xóa khu vực đã chọn?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                    return;
            }

            try
            {
                    await _khuVucApiService.DeleteAsync(id);
                    await LoadKhuVucAsync();
                    MessageBox.Show("Đã xóa khu vực.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                    ShowKhuVucError(ex);
            }
        }

        private async Task LoadKhuVucAsync()
        {
            try
            {
                    var areas = await _khuVucApiService.GetAllAsync();
                    dgvKhuVuc.DataSource = areas;
                    dgvKhuVuc.ClearSelection();
                    ResetKhuVucForm();
                    LoadBanAnAreas(areas);
            }
            catch (Exception ex)
            {
                    ShowKhuVucError(ex);
            }
        }

        private void ConfigureKhuVucGrid()
        {
            dgvKhuVuc.AutoGenerateColumns = false;
            dgvKhuVuc.Columns.Add(new DataGridViewTextBoxColumn
            {
                    DataPropertyName = nameof(KhuVucDto.MaKhuVuc),
                    HeaderText = "Mã khu vực",
                    Name = "MaKhuVuc",
                    Width = 120
            });
            dgvKhuVuc.Columns.Add(new DataGridViewTextBoxColumn
            {
                    DataPropertyName = nameof(KhuVucDto.TenKhuVuc),
                    HeaderText = "Tên khu vực",
                    Name = "TenKhuVuc",
                    Width = 220
            });
            dgvKhuVuc.Columns.Add(new DataGridViewTextBoxColumn
            {
                    DataPropertyName = nameof(KhuVucDto.MoTa),
                    HeaderText = "Mô tả",
                    Name = "MoTa",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvKhuVuc.SelectionChanged += dgvKhuVuc_SelectionChanged;
        }

        private void dgvKhuVuc_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvKhuVuc.SelectedRows.Count == 0 ||
                dgvKhuVuc.CurrentRow?.DataBoundItem is not KhuVucDto khuVuc)
            {
                    ResetKhuVucForm();
                    return;
            }

            txtMaKhuVuc.Text = khuVuc.MaKhuVuc.ToString();
            txtTenKhuVuc.Text = khuVuc.TenKhuVuc;
            txtMoTaKhuVuc.Text = khuVuc.MoTa ?? string.Empty;
            btnSuaKhuVuc.Enabled = ApiSession.IsManager;
            btnXoaKhuVuc.Enabled = ApiSession.IsManager;
        }

        private void ResetKhuVucForm()
        {
            txtMaKhuVuc.Clear();
            txtTenKhuVuc.Clear();
            txtMoTaKhuVuc.Clear();
            btnSuaKhuVuc.Enabled = false;
            btnXoaKhuVuc.Enabled = false;
        }

        private bool ValidateKhuVucInput()
        {
            if (string.IsNullOrWhiteSpace(txtTenKhuVuc.Text))
            {
                    MessageBox.Show("Tên khu vực không được để trống.", "Dữ liệu chưa hợp lệ",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenKhuVuc.Focus();
                    return false;
            }

            if (txtTenKhuVuc.Text.Trim().Length > 100)
            {
                    MessageBox.Show("Tên khu vực không được vượt quá 100 ký tự.",
                        "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenKhuVuc.Focus();
                    return false;
            }

            if (txtMoTaKhuVuc.Text.Trim().Length > 255)
            {
                    MessageBox.Show("Mô tả không được vượt quá 255 ký tự.",
                        "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMoTaKhuVuc.Focus();
                    return false;
            }

            return true;
        }

        private static void ShowKhuVucError(Exception exception)
        {
            MessageBox.Show(
                    "Không thể thực hiện thao tác với khu vực.\n" + exception.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }

        private void InitializeBanAnTab()
        {
            var tabBanAn = new TabPage("Bàn ăn")
            {
                Padding = new Padding(12),
                UseVisualStyleBackColor = true
            };
            var filters = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46
            };
            var editor = new Panel
            {
                Dock = DockStyle.Top,
                Height = 126
            };

            var lblTimBan = new Label
            {
                AutoSize = true,
                Location = new Point(8, 12),
                Text = "Tìm mã/tên bàn"
            };
            txtTimBan.Location = new Point(125, 7);
            txtTimBan.Size = new Size(160, 27);
            txtTimBan.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtTimBan.KeyDown += txtTimBan_KeyDown;

            var btnTimBan = new Button
            {
                Location = new Point(294, 6),
                Size = new Size(75, 30),
                Text = "Tìm"
            };
            btnTimBan.Click += async (_, _) => await LoadBanAnAsync();

            var lblLocKhuVuc = new Label
            {
                AutoSize = true,
                Location = new Point(382, 12),
                Text = "Khu vực"
            };
            cmbLocKhuVucBan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocKhuVucBan.Location = new Point(445, 7);
            cmbLocKhuVucBan.Size = new Size(180, 28);
            cmbLocKhuVucBan.SelectedIndexChanged += cmbLocBan_SelectedIndexChanged;

            var lblLocTrangThai = new Label
            {
                AutoSize = true,
                Location = new Point(638, 12),
                Text = "Trạng thái"
            };
            cmbLocTrangThaiBan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLocTrangThaiBan.Location = new Point(715, 7);
            cmbLocTrangThaiBan.Size = new Size(150, 28);
            cmbLocTrangThaiBan.Items.AddRange(
                ["Tất cả", "Đang hoạt động", "Ngừng hoạt động"]);
            cmbLocTrangThaiBan.SelectedIndex = 0;
            cmbLocTrangThaiBan.SelectedIndexChanged += cmbLocBan_SelectedIndexChanged;

            var btnTaiLaiBan = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(875, 6),
                Size = new Size(97, 30),
                Text = "Tải lại"
            };
            btnTaiLaiBan.Click += async (_, _) => await LoadBanAnAsync();

            filters.Controls.AddRange(
            [
                lblTimBan, txtTimBan, btnTimBan,
                lblLocKhuVuc, cmbLocKhuVucBan,
                lblLocTrangThai, cmbLocTrangThaiBan,
                btnTaiLaiBan
            ]);

            AddEditorField(editor, "Mã bàn", txtMaBan, 8, 100, true);
            AddEditorField(editor, "Tên bàn", txtTenBan, 125, 185, false);
            AddEditorField(editor, "Số chỗ", txtSoCho, 328, 90, false);
            var lblKhuVuc = new Label
            {
                AutoSize = true,
                Location = new Point(432, 10),
                Text = "Khu vực *"
            };
            cmbKhuVucBan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKhuVucBan.Location = new Point(432, 37);
            cmbKhuVucBan.Size = new Size(240, 28);
            chkBanHoatDong.AutoSize = true;
            chkBanHoatDong.Checked = true;
            chkBanHoatDong.Location = new Point(690, 39);
            chkBanHoatDong.Text = "Đang hoạt động";
            if (!ApiSession.IsManager)
            {
                txtTenBan.ReadOnly = true;
                txtSoCho.ReadOnly = true;
                cmbKhuVucBan.Enabled = false;
            }

            btnThemBan.Text = "Thêm bàn";
            btnThemBan.Location = new Point(8, 82);
            btnThemBan.Size = new Size(105, 32);
            btnThemBan.Click += btnThemBan_Click;
            btnSuaBan.Text = "Cập nhật";
            btnSuaBan.Location = new Point(120, 82);
            btnSuaBan.Size = new Size(105, 32);
            btnSuaBan.Enabled = false;
            btnSuaBan.Click += btnSuaBan_Click;
            btnXoaBan.Text = "Xóa";
            btnXoaBan.Location = new Point(232, 82);
            btnXoaBan.Size = new Size(90, 32);
            btnXoaBan.Enabled = false;
            btnXoaBan.Click += btnXoaBan_Click;
            btnCapNhatTrangThaiBan.Text = "Cập nhật trạng thái";
            btnCapNhatTrangThaiBan.Location = new Point(330, 82);
            btnCapNhatTrangThaiBan.Size = new Size(160, 32);
            btnCapNhatTrangThaiBan.Enabled = false;
            btnCapNhatTrangThaiBan.Click += btnCapNhatTrangThaiBan_Click;
            var btnBanMoi = new Button
            {
                Location = new Point(498, 82),
                Size = new Size(90, 32),
                Text = "Làm mới"
            };
            btnBanMoi.Click += (_, _) => ResetBanAnForm();
            var btnXemChiTietBan = new Button
            {
                Location = new Point(596, 82),
                Size = new Size(120, 32),
                Text = "Xem chi tiết"
            };
            btnXemChiTietBan.Click += btnXemChiTietBan_Click;

            editor.Controls.AddRange(
            [
                lblKhuVuc, cmbKhuVucBan, chkBanHoatDong,
                btnThemBan, btnSuaBan, btnXoaBan, btnCapNhatTrangThaiBan,
                btnBanMoi, btnXemChiTietBan
            ]);

            dgvBanAn.Dock = DockStyle.Fill;
            dgvBanAn.AutoGenerateColumns = false;
            dgvBanAn.AllowUserToAddRows = false;
            dgvBanAn.MultiSelect = false;
            dgvBanAn.ReadOnly = true;
            dgvBanAn.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBanAn.RowHeadersWidth = 51;
            dgvBanAn.Columns.AddRange(
            [
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(BanAnDto.MaBan),
                    HeaderText = "Mã bàn",
                    Width = 90
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(BanAnDto.TenBan),
                    HeaderText = "Tên bàn",
                    Width = 180
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(BanAnDto.TenKhuVuc),
                    HeaderText = "Khu vực",
                    Width = 230
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(BanAnDto.SoCho),
                    HeaderText = "Số chỗ",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = nameof(BanAnDto.TrangThaiHoatDong),
                    HeaderText = "Trạng thái",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            ]);
            dgvBanAn.CellFormatting += dgvBanAn_CellFormatting;
            dgvBanAn.SelectionChanged += dgvBanAn_SelectionChanged;
            pnlDanhSachBanAn.Dock = DockStyle.Fill;
            pnlDanhSachBanAn.Controls.Add(dgvBanAn);
            lblBanAnKhongCoKetQua.Dock = DockStyle.Fill;
            lblBanAnKhongCoKetQua.Text = "Không tìm thấy bàn ăn phù hợp.";
            lblBanAnKhongCoKetQua.TextAlign = ContentAlignment.MiddleCenter;
            lblBanAnKhongCoKetQua.Visible = false;
            pnlDanhSachBanAn.Controls.Add(lblBanAnKhongCoKetQua);
            lblBanAnKhongCoKetQua.BringToFront();

            tabBanAn.Controls.Add(pnlDanhSachBanAn);
            tabBanAn.Controls.Add(editor);
            tabBanAn.Controls.Add(filters);
            tabQuanLy.Controls.Add(tabBanAn);
        }

        private static void AddEditorField(
            Control parent,
            string label,
            TextBox textBox,
            int x,
            int width,
            bool readOnly)
        {
            parent.Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(x, 10),
                Text = label
            });
            textBox.Location = new Point(x, 37);
            textBox.Size = new Size(width, 27);
            textBox.ReadOnly = readOnly;
            parent.Controls.Add(textBox);
        }

        private async Task LoadBanAnAsync()
        {
            try
            {
                var areaFilter = cmbLocKhuVucBan.SelectedItem as AreaFilterOption;
                bool? activeFilter = cmbLocTrangThaiBan.SelectedIndex switch
                {
                    1 => true,
                    2 => false,
                    _ => null
                };
                var tables = await _banAnApiService.GetAllAsync(
                    areaFilter?.MaKhuVuc,
                    activeFilter,
                    txtTimBan.Text);

                _loadingBanAn = true;
                dgvBanAn.DataSource = tables;
                dgvBanAn.ClearSelection();
                lblBanAnKhongCoKetQua.Visible = tables.Count == 0;
                ResetBanAnForm();
            }
            catch (Exception ex)
            {
                ShowBanAnError(ex);
            }
            finally
            {
                _loadingBanAn = false;
            }
        }

        private void LoadBanAnAreas(IReadOnlyList<KhuVucDto> areas)
        {
            _loadingBanFilters = true;
            try
            {
                cmbKhuVucBan.DataSource = null;
                cmbKhuVucBan.DisplayMember = nameof(KhuVucDto.TenKhuVuc);
                cmbKhuVucBan.ValueMember = nameof(KhuVucDto.MaKhuVuc);
                cmbKhuVucBan.DataSource = areas;
                cmbKhuVucBan.SelectedIndex = -1;
                btnThemBan.Enabled = ApiSession.IsManager && areas.Count > 0;

                cmbLocKhuVucBan.Items.Clear();
                cmbLocKhuVucBan.Items.Add(new AreaFilterOption(null, "Tất cả khu vực"));
                foreach (var area in areas)
                {
                    cmbLocKhuVucBan.Items.Add(
                        new AreaFilterOption(area.MaKhuVuc, area.TenKhuVuc));
                }

                cmbLocKhuVucBan.DisplayMember = nameof(AreaFilterOption.TenKhuVuc);
                cmbLocKhuVucBan.SelectedIndex = 0;
            }
            finally
            {
                _loadingBanFilters = false;
            }

        }

        private async void btnThemBan_Click(object? sender, EventArgs e)
        {
            if (!TryReadBanAnForm(out var areaId, out var name, out var seats))
            {
                return;
            }

            try
            {
                await _banAnApiService.CreateAsync(new CreateBanAnRequest
                {
                    MaKhuVuc = areaId,
                    TenBan = name,
                    SoCho = seats
                });
                await LoadBanAnAsync();
                MessageBox.Show("Đã thêm bàn ăn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowBanAnError(ex);
            }
        }

        private async void btnSuaBan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaBan.Text, out var id))
            {
                MessageBox.Show("Vui lòng chọn bàn cần sửa.", "Chưa chọn bàn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryReadBanAnForm(out var areaId, out var name, out var seats))
            {
                return;
            }

            try
            {
                await _banAnApiService.UpdateAsync(id, new UpdateBanAnRequest
                {
                    MaKhuVuc = areaId,
                    TenBan = name,
                    SoCho = seats
                });
                await LoadBanAnAsync();
                MessageBox.Show("Đã cập nhật bàn ăn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowBanAnError(ex);
            }
        }

        private async void btnCapNhatTrangThaiBan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaBan.Text, out var id))
            {
                MessageBox.Show("Vui lòng chọn bàn cần cập nhật trạng thái.", "Chưa chọn bàn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var updatedTable = await _banAnApiService.UpdateStatusAsync(id, new UpdateBanAnStatusRequest
                {
                    TrangThaiHoatDong = chkBanHoatDong.Checked
                });
                await LoadBanAnAsync();
                MessageBox.Show(
                    $"Đã cập nhật trạng thái bàn: {(updatedTable.TrangThaiHoatDong ? "Đang hoạt động" : "Ngừng hoạt động")}.",
                    "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowBanAnError(ex);
            }
        }

        private async void btnXemChiTietBan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaBan.Text, out var id))
            {
                MessageBox.Show("Vui lòng chọn bàn cần xem.", "Chưa chọn bàn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var banAn = await _banAnApiService.GetByIdAsync(id);
                MessageBox.Show(
                    $"Mã bàn: {banAn.MaBan}\n" +
                    $"Tên bàn: {banAn.TenBan}\n" +
                    $"Khu vực: {banAn.TenKhuVuc}\n" +
                    $"Số chỗ: {banAn.SoCho}\n" +
                    $"Trạng thái: {(banAn.TrangThaiHoatDong ? "Đang hoạt động" : "Ngừng hoạt động")}",
                    "Chi tiết bàn ăn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowBanAnError(ex);
            }
        }

        private async void btnXoaBan_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaBan.Text, out var id))
            {
                MessageBox.Show("Vui lòng chọn bàn cần xóa.", "Chưa chọn bàn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                    "Bạn có chắc muốn xóa bàn đã chọn?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                await _banAnApiService.DeleteAsync(id);
                await LoadBanAnAsync();
                MessageBox.Show("Đã xóa bàn ăn.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowBanAnError(ex);
            }
        }

        private bool TryReadBanAnForm(out int areaId, out string name, out int seats)
        {
            areaId = 0;
            name = txtTenBan.Text.Trim();
            seats = 0;

            if (cmbKhuVucBan.SelectedValue is not int selectedAreaId ||
                selectedAreaId <= 0)
            {
                MessageBox.Show(
                    "Chưa có khu vực nào. Hãy thêm khu vực trước khi thêm bàn.",
                    "Chưa chọn khu vực",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Tên bàn không được để trống.", "Dữ liệu chưa hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenBan.Focus();
                return false;
            }

            if (name.Length > 40)
            {
                MessageBox.Show("Tên bàn không được vượt quá 40 ký tự.",
                    "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenBan.Focus();
                return false;
            }

            if (!int.TryParse(txtSoCho.Text, out seats) || seats <= 0)
            {
                MessageBox.Show("Số chỗ phải là số nguyên lớn hơn 0.",
                    "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoCho.Focus();
                return false;
            }

            areaId = selectedAreaId;
            return true;
        }

        private void dgvBanAn_SelectionChanged(object? sender, EventArgs e)
        {
            if (_loadingBanAn)
            {
                return;
            }

            if (dgvBanAn.CurrentRow?.DataBoundItem is not BanAnDto banAn)
            {
                ResetBanAnForm();
                return;
            }

            txtMaBan.Text = banAn.MaBan.ToString();
            txtTenBan.Text = banAn.TenBan;
            txtSoCho.Text = banAn.SoCho.ToString();
            cmbKhuVucBan.SelectedValue = banAn.MaKhuVuc;
            chkBanHoatDong.Checked = banAn.TrangThaiHoatDong;
            btnThemBan.Enabled = ApiSession.IsManager && cmbKhuVucBan.Items.Count > 0;
            btnSuaBan.Enabled = ApiSession.IsManager;
            btnXoaBan.Enabled = ApiSession.IsManager;
            btnCapNhatTrangThaiBan.Enabled = true;
        }

        private void ResetBanAnForm()
        {
            txtMaBan.Clear();
            txtTenBan.Clear();
            txtSoCho.Text = "4";
            chkBanHoatDong.Checked = true;
            cmbKhuVucBan.SelectedIndex = -1;

            btnThemBan.Enabled = ApiSession.IsManager && cmbKhuVucBan.Items.Count > 0;
            btnSuaBan.Enabled = false;
            btnXoaBan.Enabled = false;
            btnCapNhatTrangThaiBan.Enabled = false;
        }

        private void dgvBanAn_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (dgvBanAn.Columns[e.ColumnIndex].DataPropertyName ==
                nameof(BanAnDto.TrangThaiHoatDong) && e.Value is bool isActive)
            {
                e.Value = isActive ? "Đang hoạt động" : "Ngừng hoạt động";
                e.FormattingApplied = true;
            }
        }

        private async void cmbLocBan_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_loadingBanFilters)
            {
                await LoadBanAnAsync();
            }
        }

        private async void txtTimBan_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoadBanAnAsync();
            }
        }

        private static void ShowBanAnError(Exception exception)
        {
            MessageBox.Show(
                "Không thể thực hiện thao tác với bàn ăn.\n" + exception.Message,
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private sealed record AreaFilterOption(int? MaKhuVuc, string TenKhuVuc)
        {
            public override string ToString() => TenKhuVuc;
        }
    }
}
