#nullable enable
namespace QuanLyThueSanTheThao.Forms.Employee;

partial class FrmEmployeeAccount
{
    private System.ComponentModel.IContainer? components = null;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel card = null!;
    private TextBox txtCode = null!, txtName = null!, txtPhone = null!, txtEmail = null!, txtPosition = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnSave = null!, btnPassword = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        card = new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); txtCode = new TextBox(); txtName = new TextBox();
        txtPhone = new TextBox(); txtEmail = new TextBox(); txtPosition = new TextBox();
        btnSave = new QuanLyThueSanTheThao.Forms.Common.RoundedButton(); btnPassword = new QuanLyThueSanTheThao.Forms.Common.RoundedButton();
        SuspendLayout(); BackColor = QuanLyThueSanTheThao.Helpers.AppTheme.Background; AutoScaleMode = AutoScaleMode.Dpi; AutoScroll = true;
        card.Dock = DockStyle.Top; card.Height = 430; card.Radius = 16; card.BorderColor = QuanLyThueSanTheThao.Helpers.AppTheme.Border; card.Padding = new Padding(26); card.Margin = new Padding(6);
        var badge = new QuanLyThueSanTheThao.Forms.Common.IconBadge { IconImage = QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Employee,20,Color.White), AccentColor = QuanLyThueSanTheThao.Helpers.AppTheme.Accent2, Size = new Size(40,40), Location = new Point(26,22) };
        var title = new Label { Text="Thông tin cá nhân",AutoSize=true,Font=new Font("Segoe UI Semibold",13.5F,FontStyle.Bold),ForeColor=QuanLyThueSanTheThao.Helpers.AppTheme.Text,Location=new Point(78,26) };
        var sub = new Label { Text="Cập nhật thông tin cá nhân và bảo mật tài khoản nhân viên.",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=QuanLyThueSanTheThao.Helpers.AppTheme.Muted,Location=new Point(79,52) };
        var sep = new Panel { BackColor=Color.FromArgb(238,244,248),Location=new Point(26,84),Width=760,Height=1,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right };
        card.Controls.AddRange(new Control[] { badge,title,sub,sep });
        int y=104;
        void AddField(string label,Control control,int x,ref int top)
        {
            var l=new Label{Text=label,AutoSize=true,Font=new Font("Segoe UI Semibold",8.7F),ForeColor=QuanLyThueSanTheThao.Helpers.AppTheme.Text,Location=new Point(x,top)};
            control.Location=new Point(x,top+24);control.Size=new Size(330,32);card.Controls.AddRange(new Control[]{l,control});top+=72;
        }
        AddField("Mã nhân viên",txtCode,28,ref y);txtCode.ReadOnly=true;
        AddField("Họ tên",txtName,28,ref y);AddField("Số điện thoại",txtPhone,28,ref y);
        int y2=104;AddField("Email",txtEmail,410,ref y2);AddField("Chức vụ",txtPosition,410,ref y2);txtPosition.ReadOnly=true;
        btnSave.Text="Lưu thay đổi";btnSave.Location=new Point(410,260);btnSave.Size=new Size(150,42);
        btnPassword.Text="Đổi mật khẩu";btnPassword.Location=new Point(570,260);btnPassword.Size=new Size(160,42);
        card.Controls.AddRange(new Control[]{btnSave,btnPassword});Controls.Add(card);ResumeLayout(false);
    }
}
