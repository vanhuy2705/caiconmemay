#nullable enable
namespace QuanLyThueSanTheThao.Forms.Auth;

partial class FrmRegister
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel hero = null!;
    private Panel right = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel card = null!;
    private TextBox txtFullName = null!, txtPhone = null!, txtEmail = null!, txtUsername = null!, txtPassword = null!, txtConfirm = null!;
    private CheckBox chkTerms = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnRegister = null!, btnBack = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        layout = new TableLayoutPanel(); hero = new QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel();
        right = new Panel(); card = new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();
        txtFullName = new TextBox(); txtPhone = new TextBox(); txtEmail = new TextBox(); txtUsername = new TextBox();
        txtPassword = new TextBox(); txtConfirm = new TextBox(); chkTerms = new CheckBox();
        btnRegister = new QuanLyThueSanTheThao.Forms.Common.RoundedButton(); btnBack = new QuanLyThueSanTheThao.Forms.Common.RoundedButton();
        SuspendLayout();
        BackColor = Color.FromArgb(4, 28, 38); AutoScaleMode = AutoScaleMode.Dpi;
        layout.Dock = DockStyle.Fill; layout.ColumnCount = 2; layout.RowCount = 1;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56));
        hero.Dock = DockStyle.Fill; hero.Margin = new Padding(0); hero.Heading = "Gia nhập SportField"; hero.Subheading = "Tạo tài khoản · Đặt sân trong vài phút";
        right.Dock = DockStyle.Fill; right.BackColor = Color.FromArgb(239, 248, 253); right.AutoScroll = true;
        card.Size = new Size(520, 620); card.BackColor = Color.White; card.BorderColor = Color.FromArgb(214, 230, 238); card.Radius = 18; card.Padding = new Padding(34);
        var badge = new QuanLyThueSanTheThao.Forms.Common.IconBadge { IconImage = QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.User, 20, Color.White), AccentColor = Color.FromArgb(19, 198, 119), Size = new Size(44,44), Location = new Point(34,25) };
        var title = new Label { Text = "Tạo tài khoản khách hàng", AutoSize = true, Font = new Font("Segoe UI Semibold",18F,FontStyle.Bold), ForeColor = Color.FromArgb(18,53,78), Location = new Point(88,22) };
        var sub = new Label { Text = "Thông tin sẽ được lưu trực tiếp vào cơ sở dữ liệu.", AutoSize = true, Font = new Font("Segoe UI",8.8F), ForeColor = Color.FromArgb(103,126,145), Location = new Point(90,55) };
        card.Controls.AddRange(new Control[] { badge, title, sub });

        int y = 96;
        void AddField(string label, TextBox box, string placeholder, bool password = false)
        {
            var l = new Label { Text = label, AutoSize = true, Font = new Font("Segoe UI Semibold",8.5F), ForeColor = Color.FromArgb(52,84,105), Location = new Point(34,y) };
            box.Location = new Point(34,y+22); box.Size = new Size(452,37); box.PlaceholderText = placeholder; box.UseSystemPasswordChar = password;
            card.Controls.AddRange(new Control[] { l, box }); y += 74;
        }
        AddField("Họ và tên *", txtFullName, "Ví dụ: Nguyễn Văn An");
        AddField("Số điện thoại *", txtPhone, "09xxxxxxxx");
        AddField("Email", txtEmail, "ten@email.com (không bắt buộc)");
        AddField("Tên đăng nhập *", txtUsername, "Từ 4 ký tự, không có khoảng trắng");
        AddField("Mật khẩu *", txtPassword, "Tối thiểu 6 ký tự", true);
        AddField("Nhập lại mật khẩu *", txtConfirm, "Nhập lại chính xác mật khẩu", true);
        chkTerms.Text = "Tôi xác nhận thông tin đăng ký là chính xác"; chkTerms.AutoSize = true; chkTerms.Font = new Font("Segoe UI",8.5F); chkTerms.ForeColor = Color.FromArgb(52,84,105); chkTerms.Location = new Point(34,y-2);
        btnBack.Text = "Quay lại"; btnBack.Size = new Size(128,44); btnBack.Location = new Point(34,y+34);
        btnRegister.Text = "Đăng ký tài khoản"; btnRegister.Size = new Size(310,44); btnRegister.Location = new Point(176,y+34);
        card.Controls.AddRange(new Control[] { chkTerms, btnBack, btnRegister });
        right.Controls.Add(card);
        right.Resize += (_, _) => { card.Left = Math.Max(20,(right.ClientSize.Width-card.Width)/2); card.Top = Math.Max(18,(right.ClientSize.Height-card.Height)/2); };
        layout.Controls.Add(hero,0,0); layout.Controls.Add(right,1,0); Controls.Add(layout);
        ClientSize = new Size(1120,720); MinimumSize = new Size(940,680); StartPosition = FormStartPosition.CenterParent;
        Text = "SportField - Đăng ký khách hàng"; ResumeLayout(false);
    }
}
