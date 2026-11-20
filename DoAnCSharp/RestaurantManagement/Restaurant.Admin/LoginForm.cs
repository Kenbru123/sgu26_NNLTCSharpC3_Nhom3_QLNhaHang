using Restaurant.Admin.Services;
using Restaurant.Application.DTOs;

namespace Restaurant.Admin;

public sealed class LoginForm : Form
{
    private readonly AuthApiService _authApiService = new();
    private readonly TextBox _txtUsername = new();
    private readonly TextBox _txtPassword = new();
    private readonly Button _btnLogin = new();

    public LoginForm()
    {
        Text = "Đăng nhập quản lý nhà hàng";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(480, 260);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 2,
            RowCount = 3
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblUsername = new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Tên đăng nhập"
        };
        _txtUsername.Dock = DockStyle.Fill;
        _txtUsername.MaxLength = 80;

        var lblPassword = new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Text = "Mật khẩu"
        };
        _txtPassword.Dock = DockStyle.Fill;
        _txtPassword.MaxLength = 200;
        _txtPassword.UseSystemPasswordChar = true;
        _txtPassword.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _ = LoginAsync();
            }
        };

        _btnLogin.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        _btnLogin.AutoSize = true;
        _btnLogin.MinimumSize = new Size(130, 36);
        _btnLogin.Padding = new Padding(12, 4, 12, 4);
        _btnLogin.Text = "Đăng nhập";
        _btnLogin.Click += async (_, _) => await LoginAsync();

        layout.Controls.Add(lblUsername, 0, 0);
        layout.Controls.Add(_txtUsername, 1, 0);
        layout.Controls.Add(lblPassword, 0, 1);
        layout.Controls.Add(_txtPassword, 1, 1);
        layout.Controls.Add(_btnLogin, 1, 2);
        Controls.Add(layout);
        AcceptButton = _btnLogin;
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(_txtUsername.Text) ||
            string.IsNullOrWhiteSpace(_txtPassword.Text))
        {
            MessageBox.Show(
                "Vui lòng nhập tên đăng nhập và mật khẩu.",
                "Thiếu thông tin",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        _btnLogin.Enabled = false;
        try
        {
            var response = await _authApiService.LoginAsync(new LoginRequest
            {
                TenDangNhap = _txtUsername.Text.Trim(),
                MatKhau = _txtPassword.Text
            });
            ApiSession.Set(response.AccessToken, response.VaiTro);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Không thể đăng nhập",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed)
            {
                _btnLogin.Enabled = true;
            }
        }
    }
}
