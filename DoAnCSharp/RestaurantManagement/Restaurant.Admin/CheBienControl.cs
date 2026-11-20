using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin;

public sealed class CheBienControl : UserControl
{
    private readonly GoiMonApiService _apiService = new();
    private readonly DataGridView _dgvMonCho = new();
    private readonly Button _btnTiepNhan = new();
    private readonly Button _btnHoanTat = new();
    private readonly Button _btnTaiLai = new();
    private readonly System.Windows.Forms.Timer _refreshTimer = new()
    {
        Interval = 10000
    };
    private bool _loading;

    public CheBienControl()
    {
        BuildLayout();
        ConfigureGrid();
        WireEvents();
        UpdateActionStates();
    }

    public async Task LoadAsync()
    {
        await RefreshAsync();
        _refreshTimer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshTimer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void BuildLayout()
    {
        Dock = DockStyle.Fill;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(new Label
        {
            Text = "Món chờ chế biến và đang chế biến",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
        }, 0, 0);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true
        };
        _btnTiepNhan.Text = "Tiếp nhận món";
        _btnHoanTat.Text = "Hoàn tất món";
        _btnTaiLai.Text = "Tải lại";
        actions.Controls.Add(_btnTiepNhan);
        actions.Controls.Add(_btnHoanTat);
        actions.Controls.Add(_btnTaiLai);
        root.Controls.Add(actions, 0, 1);
        root.Controls.Add(_dgvMonCho, 0, 2);

        Controls.Add(root);
    }

    private void ConfigureGrid()
    {
        _dgvMonCho.Dock = DockStyle.Fill;
        _dgvMonCho.AutoGenerateColumns = false;
        _dgvMonCho.ReadOnly = true;
        _dgvMonCho.AllowUserToAddRows = false;
        _dgvMonCho.AllowUserToDeleteRows = false;
        _dgvMonCho.MultiSelect = false;
        _dgvMonCho.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _dgvMonCho.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _dgvMonCho.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        _dgvMonCho.RowTemplate.Height = 32;
        _dgvMonCho.ColumnHeadersHeight = 42;
        _dgvMonCho.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        _dgvMonCho.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        _dgvMonCho.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _dgvMonCho.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
        _dgvMonCho.RowHeadersVisible = false;
        _dgvMonCho.AllowUserToResizeRows = false;

        AddTextColumn(nameof(GoiMonCheBienDto.MaDonHang), "Mã phiếu", "N0");
        AddTextColumn(nameof(GoiMonCheBienDto.TenBan), "Bàn");
        AddTextColumn(
            nameof(GoiMonCheBienDto.ThoiGianTao),
            "Thời gian gọi",
            "dd/MM/yyyy HH:mm");
        AddTextColumn(nameof(GoiMonCheBienDto.TenMon), "Món ăn");
        AddTextColumn(nameof(GoiMonCheBienDto.SoLuong), "Số lượng", "N0");
        AddTextColumn(
            nameof(GoiMonCheBienDto.TrangThaiCheBien),
            "Trạng thái");
        AddTextColumn(
            nameof(GoiMonCheBienDto.ThoiGianBatDau),
            "Bắt đầu chế biến",
            "dd/MM/yyyy HH:mm");
        AddTextColumn(nameof(GoiMonCheBienDto.GhiChuMon), "Ghi chú");

        _dgvMonCho.CellFormatting += (_, e) =>
        {
            if (_dgvMonCho.Columns[e.ColumnIndex].DataPropertyName ==
                    nameof(GoiMonCheBienDto.TrangThaiCheBien) &&
                e.Value is string status)
            {
                e.Value = FoodStatusLabel(status);
                e.FormattingApplied = true;
            }
        };
    }

    private void WireEvents()
    {
        _btnTiepNhan.Click += async (_, _) =>
            await UpdateSelectedStatusAsync("DangCheBien");
        _btnHoanTat.Click += async (_, _) =>
            await UpdateSelectedStatusAsync("HoanThanh");
        _btnTaiLai.Click += async (_, _) => await RefreshAsync();
        _dgvMonCho.SelectionChanged += (_, _) => UpdateActionStates();
        _refreshTimer.Tick += async (_, _) =>
        {
            if (Visible && !_loading)
            {
                await RefreshAsync();
            }
        };
    }

    private async Task RefreshAsync()
    {
        if (_loading)
        {
            return;
        }

        _loading = true;
        var selectedDetailId = SelectedDish?.MaChiTietDonHang;
        try
        {
            var queue = await _apiService.GetKitchenQueueAsync();
            _dgvMonCho.DataSource = queue;
            var rowToSelect = queue.FindIndex(
                item => item.MaChiTietDonHang == selectedDetailId);
            if (rowToSelect < 0 && queue.Count > 0)
            {
                rowToSelect = 0;
            }

            if (rowToSelect >= 0)
            {
                _dgvMonCho.Rows[rowToSelect].Selected = true;
                _dgvMonCho.CurrentCell = _dgvMonCho.Rows[rowToSelect].Cells[0];
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Không thể tải danh sách chế biến",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _loading = false;
            UpdateActionStates();
        }
    }

    private async Task UpdateSelectedStatusAsync(string nextStatus)
    {
        if (SelectedDish is not { } dish)
        {
            MessageBox.Show(
                this,
                "Hãy chọn món cần cập nhật.",
                "Chưa chọn món",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        try
        {
            await _apiService.UpdateFoodStatusAsync(
                dish.MaDonHang,
                dish.MaChiTietDonHang,
                new CapNhatTrangThaiMonRequest(nextStatus));
            await RefreshAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "Không thể cập nhật món",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void UpdateActionStates()
    {
        var dish = SelectedDish;
        _btnTiepNhan.Enabled =
            !_loading && dish is { TrangThaiCheBien: "ChoCheBien" };
        _btnHoanTat.Enabled =
            !_loading && dish is { TrangThaiCheBien: "DangCheBien" };
        _btnTaiLai.Enabled = !_loading;
    }

    private GoiMonCheBienDto? SelectedDish =>
        _dgvMonCho.CurrentRow?.DataBoundItem as GoiMonCheBienDto;

    private void AddTextColumn(
        string propertyName,
        string header,
        string? format = null)
    {
        _dgvMonCho.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = header,
            Name = propertyName,
            MinimumWidth = 45,
            SortMode = DataGridViewColumnSortMode.Automatic,
            DefaultCellStyle = new DataGridViewCellStyle { Format = format }
        });
    }

    private static string FoodStatusLabel(string status) =>
        status switch
        {
            "ChoCheBien" => "Chờ chế biến",
            "DangCheBien" => "Đang chế biến",
            "HoanThanh" => "Hoàn thành",
            "DaHuy" => "Đã hủy",
            _ => status
        };
}
