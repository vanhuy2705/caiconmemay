using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Auth;

public partial class FrmLogin : Form
{
    private readonly AuthService _auth = new();
    internal bool uFocus, uHover, pFocus, pHover;

    public FrmLogin()
    {
        InitializeComponent();ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.Smooth(this);
        Opacity = 0;
        Shown += (_,__) =>
        {
            Fx.FadeIn(this, 220);
            var target = loginCard.Top;
            loginCard.Top = target + 18;
            var t = new System.Windows.Forms.Timer { Interval = 16 };
            int i = 0;
            t.Tick += (_, _) =>
            {
                i++;
                float k = Math.Min(1f, i / 12f);
                float e = 1f - (1f - k) * (1f - k);
                loginCard.Top = target + (int)Math.Round(18 * (1 - e));
                if (k >= 1) { t.Stop(); t.Dispose(); }
            };
            t.Start();
        };
        btnLogin.Click += BtnLogin_Click;
        btnEye.Click += (_,__) => txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        lnkForgot.Click += (_,__) => UiMsg.Info("Vui lòng liên hệ quản trị viên để đặt lại mật khẩu tài khoản.", "Quên mật khẩu");
        lnkRegister.Click += (_,__) =>
        {
            using var register = new FrmRegister();
            if (register.ShowDialogFx(this) == DialogResult.OK)
            {
                txtUsername.Text = register.RegisteredUsername;
                txtPassword.Clear();
                txtPassword.Focus();
            }
        };
        txtPassword.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(this, EventArgs.Empty); };
        txtUsername.GotFocus += (_,__) => { uFocus = true; pnlUsername.Invalidate(); };
        txtUsername.LostFocus += (_,__) => { uFocus = false; pnlUsername.Invalidate(); };
        txtPassword.GotFocus += (_,__) => { pFocus = true; pnlPassword.Invalidate(); };
        txtPassword.LostFocus += (_,__) => { pFocus = false; pnlPassword.Invalidate(); };
        pnlUsername.MouseEnter += (_,__) => { uHover = true; pnlUsername.Invalidate(); };
        pnlUsername.MouseLeave += (_,__) => { uHover = false; pnlUsername.Invalidate(); };
        pnlPassword.MouseEnter += (_,__) => { pHover = true; pnlPassword.Invalidate(); };
        pnlPassword.MouseLeave += (_,__) => { pHover = false; pnlPassword.Invalidate(); };
        var remembered = RememberMeStore.Load();
        txtUsername.Text = remembered.username;
        txtPassword.Text = remembered.password;
        chkRemember.Checked = remembered.enabled;
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        try
        {
            btnLogin.Enabled = false;
            var result = _auth.Login(txtUsername.Text, txtPassword.Text);
            if (!result.Success)
            {
                UiMsg.Warn(result.Message, "Đăng nhập");
                return;
            }

            SessionContext.UserId = result.UserId;
            SessionContext.CustomerId = result.CustomerId;
            SessionContext.EmployeeId = result.EmployeeId;
            SessionContext.Username = result.Username;
            SessionContext.FullName = result.FullName;
            SessionContext.Role = result.Role;
            RememberMeStore.Save(txtUsername.Text.Trim(), txtPassword.Text, chkRemember.Checked);

            Hide();
            using var main = new FrmMainShell();
            main.ShowDialog();
            SessionContext.Clear();
            Show();
            txtPassword.Focus();
        }
        catch (Exception ex)
        {
            UiMsg.Error("Không thể kết nối CSDL. Hãy chạy file SQL của QuanLySanTheThaoDB và kiểm tra Data\\DatabaseConfig.cs.\n\n" + ex.Message, "Lỗi kết nối");
        }
        finally { btnLogin.Enabled = true; }
    }
}
