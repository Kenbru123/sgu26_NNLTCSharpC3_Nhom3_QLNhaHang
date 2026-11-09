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
        ClientSize = new Size(420, 220);

        var lblUsername = new Label
        {
            AutoSize = true,
            Location = new Point(24, 28),
            Text = "Tên đăng nhập"
        };
        _txtUsername.Location = new Point(160, 24);
        _txtUsername.Size = new Size(230, 27);
        _txtUsername.MaxLength = 80;

        var lblPassword = new Label
        {
            AutoSize = true,
            Location = new Point(24, 76),
            Text = "Mật khẩu"
        };
        _txtPassword.Location = new Point(160, 72);
        _txtPassword.Size = new Size(230, 27);
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

        _btnLogin.Location = new Point(160, 125);
        _btnLogin.Size = new Size(120, 36);
        _btnLogin.Text = "Đăng nhập";
        _btnLogin.Click += async (_, _) => await LoginAsync();

        Controls.AddRange([lblUsername, _txtUsername, lblPassword, _txtPassword, _btnLogin]);
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
