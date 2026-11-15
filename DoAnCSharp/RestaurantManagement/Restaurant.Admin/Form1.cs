using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin
{
    public partial class Form1 : Form
    {
        private readonly MonAnApiService _monAnApiService;
        private readonly KhuVucApiService _khuVucApiService;
        private readonly BanAnApiService _banAnApiService;
        private readonly KhoApiService _khoApiService;
        private readonly ThanhToanApiService _thanhToanApiService;
        private readonly GoiMonControl _goiMonControl;
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
        private readonly CheckBox chkMonDangBan = new();
        private readonly Button btnThemMon = new();
        private readonly Button btnSuaMon = new();
        private readonly Button btnXoaMon = new();
        private readonly Button btnLamMoiMon = new();
        private readonly DataGridView dgvKhoData = new();
        private readonly DataGridView dgvKhoLowStock = new();
        private readonly DataGridView dgvThanhToan = new();
        private bool _loadingBanAn;
        private bool _loadingBanFilters;

        public Form1()
        {
            InitializeComponent();

            _monAnApiService = new MonAnApiService();
            _khuVucApiService = new KhuVucApiService();
            _banAnApiService = new BanAnApiService();
            _khoApiService = new KhoApiService();
            _thanhToanApiService = new ThanhToanApiService();
            _goiMonControl = new GoiMonControl();
            InitializeMonAnTab();
            InitializeKhoTab();
            InitializePaymentTab();
            ConfigureKhuVucGrid();
            InitializeBanAnTab();
            var tabGoiMon = new TabPage("Gọi món")
            {
                Padding = new Padding(8)
            };
            tabGoiMon.Controls.Add(_goiMonControl);
            tabQuanLy.Controls.Add(tabGoiMon);
            btnThemKhuVuc.Enabled = ApiSession.IsManager;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadMonAnAsync();
            await LoadKhuVucAsync();
            await LoadBanAnAsync();
            await LoadKhoAsync();
            await LoadPaymentAsync();
            await _goiMonControl.LoadAsync();
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

            var lblKho = new Label
            {
                Text = "Tồn kho nguyên liệu",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(12, 12)
            };
            panel.Controls.Add(lblKho);

            dgvKhoData.Location = new Point(12, 44);
            dgvKhoData.Size = new Size(930, 230);
            dgvKhoData.ReadOnly = true;
            dgvKhoData.AutoGenerateColumns = false;
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MaNguyenLieu), HeaderText = "Mã", Width = 60 });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TenNguyenLieu), HeaderText = "Tên nguyên liệu", Width = 220 });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.DonViTinh), HeaderText = "ĐVT", Width = 80 });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MucTonToiThieu), HeaderText = "Tối thiểu", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvKhoData.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TonKho), HeaderText = "Tồn kho", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvKhoData.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TrangThai), HeaderText = "Hoạt động", Width = 100 });
            panel.Controls.Add(dgvKhoData);

            var lblTonThap = new Label
            {
                Text = "Nguyên liệu dưới mức tối thiểu",
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(12, 292)
            };
            panel.Controls.Add(lblTonThap);

            dgvKhoLowStock.Location = new Point(12, 320);
            dgvKhoLowStock.Size = new Size(930, 200);
            dgvKhoLowStock.ReadOnly = true;
            dgvKhoLowStock.AutoGenerateColumns = false;
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MaNguyenLieu), HeaderText = "Mã", Width = 60 });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TenNguyenLieu), HeaderText = "Tên nguyên liệu", Width = 220 });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.DonViTinh), HeaderText = "ĐVT", Width = 80 });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.MucTonToiThieu), HeaderText = "Tối thiểu", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvKhoLowStock.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(NguyenLieuDto.TonKho), HeaderText = "Tồn kho", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            panel.Controls.Add(dgvKhoLowStock);

            page.Controls.Add(panel);
            tabQuanLy.Controls.Add(page);
        }

        private void InitializePaymentTab()
        {
            var page = new TabPage("Thanh toán");
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var lblTitle = new Label
            {
                Text = "Tình trạng hóa đơn và trạng thái thanh toán",
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(12, 12)
            };
            panel.Controls.Add(lblTitle);

            dgvThanhToan.Location = new Point(12, 44);
            dgvThanhToan.Size = new Size(930, 470);
            dgvThanhToan.ReadOnly = true;
            dgvThanhToan.AutoGenerateColumns = false;
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaDonHang", HeaderText = "Mã đơn", Width = 80 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaBan", HeaderText = "Mã bàn", Width = 80 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenBan", HeaderText = "Tên bàn", Width = 150 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThaiDon", HeaderText = "Trạng thái đơn", Width = 160 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoMon", HeaderText = "Số món", Width = 80 });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TongTien", HeaderText = "Tổng tiền", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvThanhToan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoaDon", HeaderText = "Hóa đơn", Width = 130 });
            panel.Controls.Add(dgvThanhToan);

            var btnReload = new Button
            {
                Text = "Tải lại",
                Size = new Size(120, 32),
                Location = new Point(822, 530)
            };
            btnReload.Click += async (_, _) => await LoadPaymentAsync();
            panel.Controls.Add(btnReload);

            page.Controls.Add(panel);
            tabQuanLy.Controls.Add(page);
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