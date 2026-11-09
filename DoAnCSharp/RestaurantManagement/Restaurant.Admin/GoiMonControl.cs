using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin;

public sealed class GoiMonControl : UserControl
{
    private readonly GoiMonApiService _apiService = new();
    private readonly ComboBox _cmbBan = new();
    private readonly TextBox _txtGhiChu = new();
    private readonly TextBox _txtTimMon = new();
    private readonly ComboBox _cmbDanhMuc = new();
    private readonly DataGridView _dgvPhieu = new();
    private readonly DataGridView _dgvMonAn = new();
    private readonly DataGridView _dgvChiTiet = new();
    private readonly NumericUpDown _numSoLuong = new();
    private readonly Button _btnTaoPhieu = new();
    private readonly Button _btnCapNhatPhieu = new();
    private readonly Button _btnXoaPhieu = new();
    private readonly Button _btnDangPhucVu = new();
    private readonly Button _btnThanhToan = new();
    private readonly Button _btnThemMon = new();
    private readonly Button _btnCapNhatSoLuong = new();
    private readonly Button _btnXoaMon = new();
    private readonly Button _btnCapNhatTrangThaiMon = new();
    private readonly Button _btnTaiLai = new();
    private bool _loading;

    public GoiMonControl()
    {
        BuildLayout();
        ConfigureGrids();
        WireEvents();
        UpdateActionStates();
    }

    public async Task LoadAsync()
    {
        await RefreshAsync();
    }

    private void BuildLayout()
    {
        Dock = DockStyle.Fill;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(8)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var orderActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            AutoScroll = true,
            Padding = new Padding(0, 4, 0, 4)
        };
        orderActions.Controls.Add(CreateLabel("Bàn ăn"));
        _cmbBan.Width = 240;
        _cmbBan.DropDownStyle = ComboBoxStyle.DropDownList;
        orderActions.Controls.Add(_cmbBan);
        _btnTaoPhieu.Text = "Tạo phiếu gọi món";
        orderActions.Controls.Add(_btnTaoPhieu);
        orderActions.Controls.Add(CreateLabel("Ghi chú phiếu"));
        _txtGhiChu.Width = 270;
        _txtGhiChu.MaxLength = 500;
        orderActions.Controls.Add(_txtGhiChu);
        _btnCapNhatPhieu.Text = "Lưu ghi chú";
        orderActions.Controls.Add(_btnCapNhatPhieu);
        _btnXoaPhieu.Text = "Xóa phiếu";
        orderActions.Controls.Add(_btnXoaPhieu);
        _btnDangPhucVu.Text = "Đang phục vụ";
        orderActions.Controls.Add(_btnDangPhucVu);
        _btnThanhToan.Text = "Thanh toán";
        orderActions.Controls.Add(_btnThanhToan);
        _btnTaiLai.Text = "Tải lại";
        orderActions.Controls.Add(_btnTaiLai);

        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0, 4, 0, 0)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var orderPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        orderPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        orderPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        orderPanel.Controls.Add(
            CreateSectionHeader("Phiếu gọi món"),
            0,
            0);
        orderPanel.Controls.Add(_dgvPhieu, 0, 1);
        body.Controls.Add(orderPanel, 0, 0);

        var workPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        workPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
        workPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 48));

        var menuPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        menuPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        menuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true
        };
        searchPanel.Controls.Add(CreateLabel("Tìm món"));
        _txtTimMon.Width = 200;
        _txtTimMon.PlaceholderText = "Tên món hoặc danh mục";
        searchPanel.Controls.Add(_txtTimMon);
        _cmbDanhMuc.Width = 180;
        _cmbDanhMuc.DropDownStyle = ComboBoxStyle.DropDownList;
        searchPanel.Controls.Add(_cmbDanhMuc);
        var btnTimMon = new Button { Text = "Tìm kiếm" };
        btnTimMon.Click += async (_, _) => await LoadFoodsAsync();
        searchPanel.Controls.Add(btnTimMon);
        menuPanel.Controls.Add(searchPanel, 0, 0);
        menuPanel.Controls.Add(_dgvMonAn, 0, 1);
        workPanel.Controls.Add(menuPanel, 0, 0);

        var detailsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        detailsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var detailActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            AutoScroll = true
        };
        detailActions.Controls.Add(CreateLabel("Số lượng"));
        _numSoLuong.Minimum = 1;
        _numSoLuong.Maximum = int.MaxValue;
        _numSoLuong.Value = 1;
        _numSoLuong.Width = 90;
        detailActions.Controls.Add(_numSoLuong);
        _btnThemMon.Text = "Thêm món";
        detailActions.Controls.Add(_btnThemMon);
        _btnCapNhatSoLuong.Text = "Cập nhật số lượng";
        detailActions.Controls.Add(_btnCapNhatSoLuong);
        _btnXoaMon.Text = "Xóa món";
        detailActions.Controls.Add(_btnXoaMon);
        _btnCapNhatTrangThaiMon.Text = "Cập nhật trạng thái món";
        detailActions.Controls.Add(_btnCapNhatTrangThaiMon);
        detailsPanel.Controls.Add(detailActions, 0, 0);
        detailsPanel.Controls.Add(_dgvChiTiet, 0, 1);
        workPanel.Controls.Add(detailsPanel, 0, 1);

        root.Controls.Add(orderActions, 0, 0);
        body.Controls.Add(workPanel, 1, 0);
        root.Controls.Add(body, 0, 1);
    }

    private void ConfigureGrids()
    {
        ConfigureGrid(_dgvPhieu);
        AddTextColumn(_dgvPhieu, nameof(GoiMonPhieuDto.MaDonHang), "Mã phiếu", 75);
        AddTextColumn(_dgvPhieu, nameof(GoiMonPhieuDto.TenBan), "Bàn", 90);
        AddTextColumn(_dgvPhieu, nameof(GoiMonPhieuDto.ThoiGianTao), "Thời gian", 145);
        AddTextColumn(_dgvPhieu, nameof(GoiMonPhieuDto.TrangThaiDon), "Trạng thái", 100);
        AddTextColumn(_dgvPhieu, nameof(GoiMonPhieuDto.SoMon), "SL món", 65);
        AddTextColumn(_dgvPhieu, nameof(GoiMonPhieuDto.TongTien), "Tổng tiền", 105);

        ConfigureGrid(_dgvMonAn);
        AddTextColumn(_dgvMonAn, nameof(GoiMonMonAnDto.TenMon), "Tên món", 220);
        AddTextColumn(_dgvMonAn, nameof(GoiMonMonAnDto.TenDanhMuc), "Danh mục", 140);
        AddTextColumn(_dgvMonAn, nameof(GoiMonMonAnDto.DonGia), "Đơn giá", 100);

        ConfigureGrid(_dgvChiTiet);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.TenMon), "Tên món", 190);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.SoLuong), "Số lượng", 75);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.DonGia), "Đơn giá", 100);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.ThanhTien), "Thành tiền", 110);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.TrangThaiCheBien), "Chế biến", 110);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.ThoiGianBatDau), "Bắt đầu chế biến", 145);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.ThoiGianHoanThanh), "Hoàn thành chế biến", 145);
        AddTextColumn(_dgvChiTiet, nameof(GoiMonChiTietDto.GhiChuMon), "Ghi chú", 150);
        _dgvPhieu.CellFormatting += (_, e) =>
        {
            if (_dgvPhieu.Columns[e.ColumnIndex].DataPropertyName ==
                    nameof(GoiMonPhieuDto.TrangThaiDon) &&
                e.Value is string status)
            {
                e.Value = OrderStatusLabel(status);
                e.FormattingApplied = true;
            }
        };
        _dgvChiTiet.CellFormatting += (_, e) =>
        {
            if (_dgvChiTiet.Columns[e.ColumnIndex].DataPropertyName ==
                    nameof(GoiMonChiTietDto.TrangThaiCheBien) &&
                e.Value is string status)
            {
                e.Value = FoodStatusLabel(status);
                e.FormattingApplied = true;
            }
        };
    }

    private void WireEvents()
    {
        _btnTaoPhieu.Click += async (_, _) => await CreateOrderAsync();
        _btnCapNhatPhieu.Click += async (_, _) => await UpdateOrderAsync();
        _btnXoaPhieu.Click += async (_, _) => await DeleteOrderAsync();
        _btnDangPhucVu.Click += async (_, _) => await SetOrderInServiceAsync();
        _btnThanhToan.Click += async (_, _) => await PayOrderAsync();
        _btnThemMon.Click += async (_, _) => await AddFoodAsync();
        _btnCapNhatSoLuong.Click += async (_, _) => await UpdateFoodQuantityAsync();
        _btnXoaMon.Click += async (_, _) => await RemoveFoodAsync();
        _btnCapNhatTrangThaiMon.Click += async (_, _) =>
            await AdvanceFoodStatusAsync();
        _dgvPhieu.SelectionChanged += async (_, _) =>
        {
            if (!_loading)
            {
                await LoadSelectedOrderAsync();
            }
        };
        _dgvChiTiet.SelectionChanged += (_, _) => UpdateActionStates();
        _dgvMonAn.SelectionChanged += (_, _) => UpdateActionStates();
        _cmbBan.SelectedIndexChanged += (_, _) => UpdateActionStates();
        _txtTimMon.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoadFoodsAsync();
            }
        };
        _btnTaiLai.Click += async (_, _) =>
            await RefreshAsync(SelectedOrder?.MaDonHang);
    }

    private async Task RefreshAsync(int? preferredOrderId = null)
    {
        _loading = true;
        try
        {
            var tablesTask = _apiService.GetTablesAsync();
            var ordersTask = _apiService.GetOrdersAsync();
            var categoriesTask = _apiService.GetCategoriesAsync();
            var foodsTask = _apiService.SearchFoodsAsync(null, null);
            await Task.WhenAll(tablesTask, ordersTask, categoriesTask, foodsTask);

            _cmbBan.DataSource = tablesTask.Result;
            _cmbBan.DisplayMember = nameof(GoiMonBanDto.TenHienThi);
            _cmbBan.SelectedIndex = tablesTask.Result.Count == 0 ? -1 : 0;

            _dgvPhieu.DataSource = ordersTask.Result;

            var categories = new List<CategoryFilterOption>
            {
                new(null, "Tất cả danh mục")
            };
            categories.AddRange(categoriesTask.Result.Select(
                category => new CategoryFilterOption(
                    category.MaDanhMuc,
                    category.TenDanhMuc)));
            _cmbDanhMuc.DataSource = categories;
            _cmbDanhMuc.DisplayMember = nameof(CategoryFilterOption.Name);
            _cmbDanhMuc.SelectedIndex = 0;

            _dgvMonAn.DataSource = foodsTask.Result;
            _dgvChiTiet.DataSource = null;
            _txtGhiChu.Clear();

            var preferredRow = ordersTask.Result.FindIndex(
                order => order.MaDonHang == preferredOrderId);
            if (preferredRow >= 0)
            {
                _dgvPhieu.Rows[preferredRow].Selected = true;
                _dgvPhieu.CurrentCell = _dgvPhieu.Rows[preferredRow].Cells[0];
            }
            else if (_dgvPhieu.Rows.Count > 0)
            {
                _dgvPhieu.Rows[0].Selected = true;
                _dgvPhieu.CurrentCell = _dgvPhieu.Rows[0].Cells[0];
            }
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            _loading = false;
        }

        await LoadSelectedOrderAsync();
        UpdateActionStates();
    }

    private async Task LoadFoodsAsync()
    {
        try
        {
            var category = _cmbDanhMuc.SelectedItem as CategoryFilterOption;
            _dgvMonAn.DataSource = await _apiService.SearchFoodsAsync(
                _txtTimMon.Text,
                category?.MaDanhMuc);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }

        UpdateActionStates();
    }

    private async Task LoadSelectedOrderAsync()
    {
        var order = SelectedOrder;
        if (order is null)
        {
            _dgvChiTiet.DataSource = null;
            _txtGhiChu.Clear();
            UpdateActionStates();
            return;
        }

        try
        {
            var details = await _apiService.GetOrderAsync(order.MaDonHang);
            _dgvChiTiet.DataSource = details.MonAn.ToList();
            _txtGhiChu.Text = details.GhiChu ?? string.Empty;
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }

        UpdateActionStates();
    }

    private async Task CreateOrderAsync()
    {
        if (_cmbBan.SelectedItem is not GoiMonBanDto table)
        {
            ShowWarning("Hãy chọn bàn đang hoạt động.");
            return;
        }

        if (table.MaDonHang.HasValue)
        {
            SelectOrder(table.MaDonHang.Value);
            ShowWarning(
                $"Bàn đã có phiếu #{table.MaDonHang.Value} ({table.TrangThaiPhieu}). " +
                "Đã mở phiếu hiện có.");
            return;
        }

        try
        {
            var order = await _apiService.CreateOrderAsync(
                new TaoPhieuGoiMonRequest(table.MaBan, null));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess($"Đã tạo phiếu gọi món #{order.MaDonHang}.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task UpdateOrderAsync()
    {
        if (SelectedOrder is not { } order)
        {
            ShowWarning("Hãy chọn phiếu cần cập nhật.");
            return;
        }

        try
        {
            await _apiService.UpdateOrderAsync(
                order.MaDonHang,
                new CapNhatPhieuGoiMonRequest(EmptyToNull(_txtGhiChu.Text)));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess("Đã cập nhật ghi chú phiếu.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task DeleteOrderAsync()
    {
        if (SelectedOrder is not { } order)
        {
            ShowWarning("Hãy chọn phiếu cần xóa.");
            return;
        }

        if (MessageBox.Show(
                $"Xóa phiếu gọi món #{order.MaDonHang} của bàn {order.TenBan}?",
                "Xác nhận xóa phiếu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await _apiService.DeleteOrderAsync(order.MaDonHang);
            await RefreshAsync();
            ShowSuccess("Đã xóa phiếu gọi món.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task SetOrderInServiceAsync()
    {
        if (SelectedOrder is not { } order)
        {
            ShowWarning("Hãy chọn phiếu cần cập nhật.");
            return;
        }

        try
        {
            await _apiService.UpdateOrderStatusAsync(
                order.MaDonHang,
                new CapNhatTrangThaiPhieuRequest("DangPhucVu"));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess("Trạng thái phiếu: Đang phục vụ.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task PayOrderAsync()
    {
        if (SelectedOrder is not { } order)
        {
            ShowWarning("Hãy chọn phiếu cần thanh toán.");
            return;
        }

        using var dialog = new PaymentDialog(order.TongTien);
        if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
        {
            return;
        }

        if (MessageBox.Show(
                $"Xác nhận thanh toán phiếu #{order.MaDonHang}?\n" +
                $"Tiền hàng: {order.TongTien:N0}\n" +
                $"VAT: {dialog.VatRate:N2}%\n" +
                $"Tổng thanh toán: {dialog.TotalDue:N0}\n" +
                $"Phương thức: {dialog.PaymentMethodLabel}",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await _apiService.PayOrderAsync(
                order.MaDonHang,
                new ThanhToanPhieuRequest(
                    dialog.PaymentMethod,
                    dialog.VatRate));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess("Đã tạo hóa đơn và cập nhật trạng thái Đã thanh toán.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task AddFoodAsync()
    {
        if (SelectedOrder is not { } order ||
            _dgvMonAn.CurrentRow?.DataBoundItem is not GoiMonMonAnDto food)
        {
            ShowWarning("Hãy chọn phiếu gọi món và món cần thêm.");
            return;
        }

        try
        {
            await _apiService.AddFoodAsync(
                order.MaDonHang,
                new ThemMonVaoPhieuRequest(
                    food.MaMon,
                    decimal.ToInt32(_numSoLuong.Value),
                    null));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess($"Đã thêm {food.TenMon} vào phiếu.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task UpdateFoodQuantityAsync()
    {
        if (SelectedOrder is not { } order ||
            SelectedDetail is not { } detail)
        {
            ShowWarning("Hãy chọn món cần cập nhật.");
            return;
        }

        try
        {
            await _apiService.UpdateFoodQuantityAsync(
                order.MaDonHang,
                detail.MaChiTietDonHang,
                new CapNhatSoLuongMonRequest(
                    decimal.ToInt32(_numSoLuong.Value)));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess("Đã cập nhật số lượng món.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task RemoveFoodAsync()
    {
        if (SelectedOrder is not { } order ||
            SelectedDetail is not { } detail)
        {
            ShowWarning("Hãy chọn món cần xóa khỏi phiếu.");
            return;
        }

        if (MessageBox.Show(
                $"Xóa món \"{detail.TenMon}\" khỏi phiếu #{order.MaDonHang}?",
                "Xác nhận xóa món",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            await _apiService.RemoveFoodAsync(
                order.MaDonHang,
                detail.MaChiTietDonHang);
            await RefreshAsync(order.MaDonHang);
            ShowSuccess("Đã xóa món khỏi phiếu.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private async Task AdvanceFoodStatusAsync()
    {
        if (SelectedOrder is not { } order ||
            SelectedDetail is not { } detail)
        {
            ShowWarning("Hãy chọn món cần cập nhật trạng thái.");
            return;
        }

        var nextStatus = detail.TrangThaiCheBien switch
        {
            "ChoCheBien" => "DangCheBien",
            "DangCheBien" => "HoanThanh",
            _ => null
        };

        if (nextStatus is null)
        {
            ShowWarning("Món đã chế biến xong.");
            return;
        }

        try
        {
            await _apiService.UpdateFoodStatusAsync(
                order.MaDonHang,
                detail.MaChiTietDonHang,
                new CapNhatTrangThaiMonRequest(nextStatus));
            await RefreshAsync(order.MaDonHang);
            ShowSuccess($"Trạng thái món: {FoodStatusLabel(nextStatus)}.");
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void SelectOrder(int orderId)
    {
        for (var index = 0; index < _dgvPhieu.Rows.Count; index++)
        {
            if (_dgvPhieu.Rows[index].DataBoundItem is GoiMonPhieuDto order &&
                order.MaDonHang == orderId)
            {
                _dgvPhieu.Rows[index].Selected = true;
                _dgvPhieu.CurrentCell = _dgvPhieu.Rows[index].Cells[0];
                return;
            }
        }
    }

    private void UpdateActionStates()
    {
        var order = SelectedOrder;
        var isOpen = order is not null &&
                     order.TrangThaiDon is not "HoanThanh" and not "DaHuy";
        _btnCapNhatPhieu.Enabled = isOpen;
        _btnXoaPhieu.Enabled = isOpen;
        _btnDangPhucVu.Enabled =
            isOpen && order!.TrangThaiDon != "DangPhucVu";
        _btnThanhToan.Enabled = isOpen && order!.SoMon > 0;
        _btnThemMon.Enabled =
            isOpen && _dgvMonAn.CurrentRow?.DataBoundItem is GoiMonMonAnDto;

        var canChangeSelectedFood =
            isOpen &&
            SelectedDetail is { TrangThaiCheBien: "ChoCheBien" };
        _btnCapNhatSoLuong.Enabled = canChangeSelectedFood;
        _btnXoaMon.Enabled = canChangeSelectedFood;
        _btnCapNhatTrangThaiMon.Enabled =
            isOpen &&
            SelectedDetail is { TrangThaiCheBien: "ChoCheBien" or "DangCheBien" };
        _btnTaoPhieu.Enabled = _cmbBan.SelectedItem is GoiMonBanDto;
        if (SelectedDetail is { } detail)
        {
            _numSoLuong.Value = Math.Clamp(
                detail.SoLuong,
                decimal.ToInt32(_numSoLuong.Minimum),
                decimal.ToInt32(_numSoLuong.Maximum));
        }
    }

    private GoiMonPhieuDto? SelectedOrder =>
        _dgvPhieu.CurrentRow?.DataBoundItem as GoiMonPhieuDto;

    private GoiMonChiTietDto? SelectedDetail =>
        _dgvChiTiet.CurrentRow?.DataBoundItem as GoiMonChiTietDto;

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.AutoGenerateColumns = false;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowHeadersVisible = false;
    }

    private static void AddTextColumn(
        DataGridView grid,
        string propertyName,
        string header,
        int width)
    {
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = header,
            Name = propertyName,
            Width = width,
            MinimumWidth = 45,
            SortMode = DataGridViewColumnSortMode.Automatic,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = propertyName is nameof(GoiMonPhieuDto.TongTien) or
                    nameof(GoiMonChiTietDto.DonGia) or
                    nameof(GoiMonChiTietDto.ThanhTien) or
                    nameof(GoiMonMonAnDto.DonGia)
                    ? "N0"
                    : propertyName is nameof(GoiMonPhieuDto.ThoiGianTao) or
                        nameof(GoiMonChiTietDto.ThoiGianBatDau) or
                        nameof(GoiMonChiTietDto.ThoiGianHoanThanh)
                        ? "dd/MM/yyyy HH:mm"
                        : null
            }
        });
    }

    private static Label CreateLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Margin = new Padding(8, 8, 4, 4)
    };

    private static Label CreateSectionHeader(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
    };

    private static string? EmptyToNull(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string OrderStatusLabel(string status) => status switch
    {
        "ChoXacNhan" => "Chờ xác nhận",
        "DangPhucVu" => "Đang phục vụ",
        "DaThanhToan" => "Đã thanh toán",
        "HoanThanh" => "Hoàn thành",
        "DaHuy" => "Đã hủy",
        _ => status
    };

    private static string FoodStatusLabel(string status) => status switch
    {
        "ChoCheBien" => "Chờ chế biến",
        "DangCheBien" => "Đang chế biến",
        "HoanThanh" => "Đã chế biến",
        "DaHuy" => "Đã hủy",
        _ => status
    };

    private static void ShowError(Exception exception)
    {
        MessageBox.Show(
            exception.Message,
            "Không thể thực hiện thao tác gọi món",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(
            message,
            "Chưa đủ thông tin",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private static void ShowSuccess(string message)
    {
        MessageBox.Show(
            message,
            "Thành công",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private sealed record CategoryFilterOption(
        int? MaDanhMuc,
        string Name);

    private sealed class PaymentDialog : Form
    {
        private readonly ComboBox _cmbPhuongThuc = new();
        private readonly NumericUpDown _numVat = new();
        private readonly decimal _subtotal;

        public PaymentDialog(decimal subtotal)
        {
            _subtotal = subtotal;
            Text = "Thanh toán phiếu gọi món";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(390, 200);

            var lblSubtotal = new Label
            {
                Text = $"Tiền hàng: {subtotal:N0} VNĐ",
                AutoSize = true,
                Location = new Point(20, 20)
            };
            var lblMethod = new Label
            {
                Text = "Phương thức thanh toán",
                AutoSize = true,
                Location = new Point(20, 62)
            };
            _cmbPhuongThuc.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPhuongThuc.Location = new Point(190, 58);
            _cmbPhuongThuc.Size = new Size(180, 28);
            _cmbPhuongThuc.DisplayMember = nameof(PaymentOption.Label);
            _cmbPhuongThuc.ValueMember = nameof(PaymentOption.Code);
            _cmbPhuongThuc.DataSource = new List<PaymentOption>
            {
                new("TienMat", "Tiền mặt"),
                new("ChuyenKhoan", "Chuyển khoản"),
                new("The", "Thẻ")
            };

            var lblVat = new Label
            {
                Text = "VAT (%)",
                AutoSize = true,
                Location = new Point(20, 104)
            };
            _numVat.DecimalPlaces = 2;
            _numVat.Minimum = 0;
            _numVat.Maximum = 100;
            _numVat.Increment = 0.5m;
            _numVat.Location = new Point(190, 100);
            _numVat.Size = new Size(100, 28);

            var btnPay = new Button
            {
                Text = "Thanh toán",
                DialogResult = DialogResult.OK,
                Location = new Point(190, 148),
                Size = new Size(100, 32)
            };
            var btnCancel = new Button
            {
                Text = "Hủy",
                DialogResult = DialogResult.Cancel,
                Location = new Point(300, 148),
                Size = new Size(70, 32)
            };

            Controls.AddRange(
            [
                lblSubtotal,
                lblMethod,
                _cmbPhuongThuc,
                lblVat,
                _numVat,
                btnPay,
                btnCancel
            ]);
            AcceptButton = btnPay;
            CancelButton = btnCancel;
        }

        public string PaymentMethod =>
            ((PaymentOption)_cmbPhuongThuc.SelectedItem!).Code;

        public string PaymentMethodLabel =>
            ((PaymentOption)_cmbPhuongThuc.SelectedItem!).Label;

        public decimal VatRate => _numVat.Value;

        public decimal TotalDue => decimal.Round(
            _subtotal + _subtotal * VatRate / 100m,
            2,
            MidpointRounding.AwayFromZero);

        private sealed record PaymentOption(string Code, string Label);
    }
}
