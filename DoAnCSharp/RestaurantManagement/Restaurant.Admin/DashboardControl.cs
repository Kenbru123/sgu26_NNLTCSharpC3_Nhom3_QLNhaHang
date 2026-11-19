using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin;

public sealed class DashboardControl : UserControl
{
    private readonly OnlineOrderApiService _onlineOrderApiService = new();
    private readonly DatBanApiService _datBanApiService = new();
    private readonly BanAnApiService _banAnApiService = new();
    private readonly KhoApiService _khoApiService = new();
    private readonly GoiMonApiService _goiMonApiService = new();
    private readonly Label _lblDoanhThu = CreateValueLabel();
    private readonly Label _lblDonHang = CreateValueLabel();
    private readonly Label _lblChoXuLy = CreateValueLabel();
    private readonly Label _lblDatBan = CreateValueLabel();
    private readonly Label _lblBanDangPhucVu = CreateValueLabel();
    private readonly Label _lblTonThap = CreateValueLabel();
    private readonly DataGridView _dgvDonOnline = new();
    private readonly DataGridView _dgvDatBan = new();
    private readonly Button _btnTaiLai = new();
    private readonly Label _lblTrangThai = new();
    private bool _loading;

    public DashboardControl()
    {
        BuildLayout();
        ConfigureGrid(_dgvDonOnline);
        ConfigureGrid(_dgvDatBan);
        ConfigureOnlineGrid();
        ConfigureBookingGrid();
        _btnTaiLai.Click += async (_, _) => await LoadAsync();
    }

    public async Task LoadAsync()
    {
        if (_loading)
        {
            return;
        }

        _loading = true;
        _btnTaiLai.Enabled = false;
        _lblTrangThai.Text = "Đang tải dữ liệu...";
        try
        {
            var onlineTask = TryGetAsync(() => _onlineOrderApiService.GetAllAsync());
            var bookingTask = TryGetAsync(() => _datBanApiService.GetAllAsync());
            var tablesTask = TryGetAsync(() => _banAnApiService.GetAllAsync());
            var inventoryTask = TryGetAsync(() => _khoApiService.GetLowStockIngredientsAsync());
            var ordersTask = TryGetAsync(() => _goiMonApiService.GetOrdersAsync());
            await Task.WhenAll(onlineTask, bookingTask, tablesTask, inventoryTask, ordersTask);

            var online = await onlineTask;
            var bookings = await bookingTask;
            var tables = await tablesTask;
            var lowStock = await inventoryTask;
            var orders = await ordersTask;
            var today = DateTimeOffset.Now.Date;

            var todayOnline = online.Where(item => item.ThoiGianTao.LocalDateTime.Date == today).ToList();
            var todayOrders = orders.Where(item => item.ThoiGianTao.Date == today).ToList();
            _lblDoanhThu.Text = (todayOnline.Sum(item => item.TongTien) + todayOrders.Sum(item => item.TongTien)).ToString("N0") + " đ";
            _lblDonHang.Text = (todayOnline.Count + todayOrders.Count).ToString("N0");
            _lblChoXuLy.Text = online.Count(item => item.TrangThaiDon == "ChoXacNhan").ToString("N0");
            _lblDatBan.Text = bookings.Count(item => item.TrangThai == "ChoXacNhan").ToString("N0");
            _lblBanDangPhucVu.Text = tables.Count(item => item.TrangThaiHoatDong && item.MaBan > 0).ToString("N0");
            _lblTonThap.Text = lowStock.Count.ToString("N0");

            _dgvDonOnline.DataSource = online
                .Where(item => item.TrangThaiDon == "ChoXacNhan" || item.TrangThaiDon == "DangPhucVu")
                .OrderByDescending(item => item.ThoiGianTao)
                .Take(10)
                .ToList();
            _dgvDatBan.DataSource = bookings
                .Where(item => item.TrangThai == "ChoXacNhan")
                .OrderBy(item => item.ThoiGianDen)
                .Take(10)
                .ToList();
            _lblTrangThai.Text = $"Cập nhật lúc {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception exception)
        {
            _lblTrangThai.Text = "Không thể tải đầy đủ dữ liệu: " + exception.Message;
        }
        finally
        {
            _btnTaiLai.Enabled = true;
            _loading = false;
        }
    }

    private void BuildLayout()
    {
        Dock = DockStyle.Fill;
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 122));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        header.Controls.Add(new Label
        {
            Text = "Tổng quan nhà hàng",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        _btnTaiLai.Text = "Tải lại";
        _btnTaiLai.Dock = DockStyle.Fill;
        header.Controls.Add(_btnTaiLai, 1, 0);
        root.Controls.Add(header, 0, 0);

        var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1 };
        for (var index = 0; index < 6; index++)
        {
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6667F));
        }
        cards.Controls.Add(CreateCard("Doanh thu hôm nay", _lblDoanhThu, Color.FromArgb(39, 125, 161)), 0, 0);
        cards.Controls.Add(CreateCard("Đơn hôm nay", _lblDonHang, Color.FromArgb(42, 157, 143)), 1, 0);
        cards.Controls.Add(CreateCard("Đơn chờ xử lý", _lblChoXuLy, Color.FromArgb(231, 111, 81)), 2, 0);
        cards.Controls.Add(CreateCard("Đặt bàn chờ xác nhận", _lblDatBan, Color.FromArgb(244, 162, 97)), 3, 0);
        cards.Controls.Add(CreateCard("Bàn đang hoạt động", _lblBanDangPhucVu, Color.FromArgb(90, 110, 114)), 4, 0);
        cards.Controls.Add(CreateCard("Nguyên liệu tồn thấp", _lblTonThap, Color.FromArgb(214, 40, 40)), 5, 0);
        root.Controls.Add(cards, 0, 1);

        var tables = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(0, 8, 0, 0) };
        tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        tables.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        tables.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tables.Controls.Add(CreateSectionLabel("Đơn online cần xử lý"), 0, 0);
        tables.Controls.Add(CreateSectionLabel("Đặt bàn chờ xác nhận"), 1, 0);
        tables.Controls.Add(_dgvDonOnline, 0, 1);
        tables.Controls.Add(_dgvDatBan, 1, 1);
        root.Controls.Add(tables, 0, 2);
        Controls.Add(root);
        _lblTrangThai.Dock = DockStyle.Bottom;
        _lblTrangThai.Height = 24;
        _lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
        Controls.Add(_lblTrangThai);
    }

    private void ConfigureOnlineGrid()
    {
        AddColumn(_dgvDonOnline, nameof(OnlineOrderResponse.MaDonHang), "Mã đơn", 55);
        AddColumn(_dgvDonOnline, nameof(OnlineOrderResponse.TenNguoiNhan), "Người nhận", 100);
        AddColumn(_dgvDonOnline, nameof(OnlineOrderResponse.TrangThaiDon), "Trạng thái", 100);
        AddColumn(_dgvDonOnline, nameof(OnlineOrderResponse.TongTien), "Tổng tiền", 90, "N0");
    }

    private void ConfigureBookingGrid()
    {
        AddColumn(_dgvDatBan, nameof(DatBanManagementResponse.MaDatBan), "Mã", 50);
        AddColumn(_dgvDatBan, nameof(DatBanManagementResponse.HoTenKhachHang), "Khách hàng", 110);
        AddColumn(_dgvDatBan, nameof(DatBanManagementResponse.ThoiGianDen), "Thời gian", 120, "dd/MM HH:mm");
        AddColumn(_dgvDatBan, nameof(DatBanManagementResponse.SoLuongKhach), "Số khách", 70);
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AutoGenerateColumns = false;
        grid.AllowUserToAddRows = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowTemplate.Height = 30;
        grid.ColumnHeadersHeight = 38;
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
    }

    private static void AddColumn(DataGridView grid, string property, string header, int minimumWidth, string? format = null)
    {
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = property,
            HeaderText = header,
            MinimumWidth = minimumWidth,
            DefaultCellStyle = new DataGridViewCellStyle { Format = format }
        });
    }

    private static Panel CreateCard(string title, Label value, Color color)
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = color, Margin = new Padding(5), Padding = new Padding(10) };
        panel.Controls.Add(new Label { Text = title, Dock = DockStyle.Top, Height = 30, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
        value.Dock = DockStyle.Fill;
        value.ForeColor = Color.White;
        value.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        value.TextAlign = ContentAlignment.MiddleLeft;
        panel.Controls.Add(value);
        return panel;
    }

    private static Label CreateSectionLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI", 11, FontStyle.Bold),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private static Label CreateValueLabel() => new();

    private static async Task<List<T>> TryGetAsync<T>(Func<Task<List<T>>> action)
    {
        try
        {
            return await action();
        }
        catch
        {
            return [];
        }
    }
}
