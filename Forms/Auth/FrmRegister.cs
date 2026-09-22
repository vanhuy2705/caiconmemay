using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Auth;

public partial class FrmRegister : Form
{
    private readonly AuthService _auth = new();
    public string RegisteredUsername { get; private set; } = "";

    public FrmRegister()
    {
        InitializeComponent();
        AppTheme.Upgrade(this);
        AppTheme.StylePrimary(btnRegister);
        AppTheme.StyleSecondary(btnBack);
        btnRegister.Click += (_, _) => Register();
        btnBack.Click += (_, _) => Close();
        txtConfirm.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) Register(); };
        Shown += (_, _) => txtFullName.Focus();
    }

    private void Register()
    {
        if (!chkTerms.Checked)
        {
            UiMsg.Warn("Vui lòng xác nhận thông tin đăng ký là chính xác.", "Đăng ký");
            return;
        }

        try
        {
            btnRegister.Enabled = false;
            _auth.RegisterCustomer(txtFullName.Text, txtPhone.Text, txtEmail.Text,
                txtUsername.Text, txtPassword.Text, txtConfirm.Text);
            RegisteredUsername = txtUsername.Text.Trim();
            UiMsg.Success("Tạo tài khoản khách hàng thành công. Bạn có thể đăng nhập ngay.", "Đăng ký thành công");
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            UiMsg.Warn(ex.Message, "Không thể đăng ký");
        }
        finally { btnRegister.Enabled = true; }
    }
}
