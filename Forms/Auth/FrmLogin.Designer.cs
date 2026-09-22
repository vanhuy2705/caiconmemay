#nullable enable
namespace QuanLyThueSanTheThao.Forms.Auth;

partial class FrmLogin
{
    private System.ComponentModel.IContainer? components = null;
    private TableLayoutPanel layout = null!;
    private QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel hero = null!;
    private Panel right = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel loginCard = null!;
    private Label lblBrand = null!;
    private Label lblTitle = null!;
    private Label lblSub = null!;
    private Label lblUsername = null!;
    private Panel pnlUsername = null!;
    private Label icoUsername = null!;
    private TextBox txtUsername = null!;
    private Label lblPassword = null!;
    private Panel pnlPassword = null!;
    private Label icoPassword = null!;
    private TextBox txtPassword = null!;
    private Button btnEye = null!;
    private CheckBox chkRemember = null!;
    private LinkLabel lnkForgot = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnLogin = null!;
    private Label lblDivider = null!;
    private Label lblDemo = null!;
    private LinkLabel lnkRegister = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        layout=new TableLayoutPanel();hero=new QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel();right=new Panel();loginCard=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();lblBrand=new Label();lblTitle=new Label();lblSub=new Label();lblUsername=new Label();pnlUsername=new Panel();icoUsername=new Label();txtUsername=new TextBox();lblPassword=new Label();pnlPassword=new Panel();icoPassword=new Label();txtPassword=new TextBox();btnEye=new Button();chkRemember=new CheckBox();lnkForgot=new LinkLabel();btnLogin=new QuanLyThueSanTheThao.Forms.Common.RoundedButton();lblDivider=new Label();lblDemo=new Label();lnkRegister=new LinkLabel();SuspendLayout();
        BackColor=Color.FromArgb(4,28,38);AutoScaleMode=AutoScaleMode.Dpi;
        layout.Dock=DockStyle.Fill;layout.ColumnCount=2;layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,54));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,46));layout.RowCount=1;
        hero.Dock=DockStyle.Fill;hero.Margin=new Padding(0);hero.Heading="Chào mừng trở lại!";hero.Subheading="Đặt sân dễ dàng · Chơi hết mình";
        right.Dock=DockStyle.Fill;right.BackColor=Color.FromArgb(4,29,39);right.Padding=new Padding(42,38,42,38);
        right.Paint+=(_,e)=>{if(right.Width<=1||right.Height<=1)return;using var br=new System.Drawing.Drawing2D.LinearGradientBrush(right.ClientRectangle,Color.FromArgb(4,29,39),Color.FromArgb(8,67,64),90f);e.Graphics.FillRectangle(br,right.ClientRectangle);};
        loginCard.Size=new Size(460,590);loginCard.Anchor=AnchorStyles.None;loginCard.BackColor=Color.FromArgb(9,40,49);loginCard.BorderColor=Color.FromArgb(30,151,118);loginCard.BorderThickness=1;loginCard.Radius=18;loginCard.Padding=new Padding(28);
        lblBrand.Text="⚽  SportField";lblBrand.AutoSize=true;lblBrand.Font=new Font("Segoe UI Semibold",16F,FontStyle.Bold);lblBrand.ForeColor=Color.FromArgb(32,220,123);lblBrand.Location=new Point(30,24);
        lblTitle.Text="Đăng nhập";lblTitle.AutoSize=true;lblTitle.Font=new Font("Segoe UI Semibold",22F,FontStyle.Bold);lblTitle.ForeColor=Color.White;lblTitle.Location=new Point(30,74);
        lblSub.Text="Đăng nhập để tiếp tục quản lý và đặt sân.";lblSub.AutoSize=true;lblSub.Font=new Font("Segoe UI",9F);lblSub.ForeColor=Color.FromArgb(180,211,216);lblSub.Location=new Point(31,116);
        lblUsername.Text="Tên đăng nhập";lblUsername.AutoSize=true;lblUsername.Font=new Font("Segoe UI Semibold",8.5F);lblUsername.ForeColor=Color.FromArgb(218,235,237);lblUsername.Location=new Point(30,158);
        pnlUsername.BackColor=Color.FromArgb(8,50,59);pnlUsername.Location=new Point(30,182);pnlUsername.Size=new Size(398,48);pnlUsername.Padding=new Padding(12,8,12,8);
        pnlUsername.Region=QuanLyThueSanTheThao.Forms.Common.RoundedPanel.RegionFor(pnlUsername,10);
        pnlUsername.Paint+=(_,e)=>{using var p=new Pen(uFocus?Color.FromArgb(32,220,123):Color.FromArgb(uHover?92:57,100,110),uFocus?1.8f:1f);e.Graphics.DrawRectangle(p,0,0,pnlUsername.Width-1,pnlUsername.Height-1);};
        icoUsername.Text="";icoUsername.Image=QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Email,18,Color.White);icoUsername.ImageAlign=ContentAlignment.MiddleCenter;icoUsername.AutoSize=false;icoUsername.Size=new Size(32,30);icoUsername.ForeColor=Color.White;icoUsername.TextAlign=ContentAlignment.MiddleCenter;icoUsername.Location=new Point(8,8);
        txtUsername.BorderStyle=BorderStyle.None;txtUsername.BackColor=Color.FromArgb(8,50,59);txtUsername.ForeColor=Color.White;txtUsername.Font=new Font("Segoe UI",10.5F);txtUsername.Location=new Point(46,13);txtUsername.Width=330;txtUsername.PlaceholderText="Email, số điện thoại hoặc tên đăng nhập";
        pnlUsername.Controls.AddRange(new Control[]{icoUsername,txtUsername});
        lblPassword.Text="Mật khẩu";lblPassword.AutoSize=true;lblPassword.Font=new Font("Segoe UI Semibold",8.5F);lblPassword.ForeColor=Color.FromArgb(218,235,237);lblPassword.Location=new Point(30,248);
        pnlPassword.BackColor=Color.FromArgb(8,50,59);pnlPassword.Location=new Point(30,272);pnlPassword.Size=new Size(398,48);pnlPassword.Padding=new Padding(12,8,12,8);
        pnlPassword.Region=QuanLyThueSanTheThao.Forms.Common.RoundedPanel.RegionFor(pnlPassword,10);
        pnlPassword.Paint+=(_,e)=>{using var p=new Pen(pFocus?Color.FromArgb(32,220,123):Color.FromArgb(pHover?92:57,100,110),pFocus?1.8f:1f);e.Graphics.DrawRectangle(p,0,0,pnlPassword.Width-1,pnlPassword.Height-1);};
        icoPassword.Text="";icoPassword.Image=QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Password,18,Color.White);icoPassword.ImageAlign=ContentAlignment.MiddleCenter;icoPassword.AutoSize=false;icoPassword.Size=new Size(32,30);icoPassword.ForeColor=Color.White;icoPassword.TextAlign=ContentAlignment.MiddleCenter;icoPassword.Location=new Point(8,8);
        txtPassword.BorderStyle=BorderStyle.None;txtPassword.BackColor=Color.FromArgb(8,50,59);txtPassword.ForeColor=Color.White;txtPassword.Font=new Font("Segoe UI",10.5F);txtPassword.Location=new Point(46,13);txtPassword.Width=292;txtPassword.UseSystemPasswordChar=true;txtPassword.PlaceholderText="Mật khẩu";
        btnEye.Text="";btnEye.Image=QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Eye,18,Color.FromArgb(190,215,219));btnEye.FlatStyle=FlatStyle.Flat;btnEye.FlatAppearance.BorderSize=0;btnEye.BackColor=Color.FromArgb(8,50,59);btnEye.ForeColor=Color.FromArgb(190,215,219);btnEye.Size=new Size(38,32);btnEye.Location=new Point(352,8);btnEye.Cursor=Cursors.Hand;
        pnlPassword.Controls.AddRange(new Control[]{icoPassword,txtPassword,btnEye});
        chkRemember.Text="Nhớ tài khoản";chkRemember.AutoSize=true;chkRemember.Font=new Font("Segoe UI",8.7F);chkRemember.ForeColor=Color.White;chkRemember.Location=new Point(30,340);
        lnkForgot.Text="Quên mật khẩu?";lnkForgot.AutoSize=true;lnkForgot.Font=new Font("Segoe UI Semibold",8.5F);lnkForgot.LinkColor=Color.FromArgb(40,218,126);lnkForgot.ActiveLinkColor=Color.FromArgb(77,239,155);lnkForgot.Location=new Point(320,341);
        btnLogin.Text="Đăng nhập";btnLogin.Image=QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Lock,19,Color.White);btnLogin.TextImageRelation=TextImageRelation.ImageBeforeText;btnLogin.Location=new Point(30,382);btnLogin.Size=new Size(398,48);btnLogin.BackColor=Color.FromArgb(20,202,116);btnLogin.ForeColor=Color.White;btnLogin.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold);btnLogin.Radius=12;btnLogin.HoverColor=Color.FromArgb(11,170,94);
        lblDivider.Text="────────────  Tài khoản demo  ────────────";lblDivider.AutoSize=true;lblDivider.Font=new Font("Segoe UI",7.8F);lblDivider.ForeColor=Color.FromArgb(138,180,185);lblDivider.Location=new Point(74,450);
        lblDemo.Text="Admin: admin / admin123     •     Nhân viên: staff / staff123\nKhách hàng: customer / customer123";lblDemo.AutoSize=false;lblDemo.Size=new Size(398,52);lblDemo.Font=new Font("Segoe UI",8F);lblDemo.ForeColor=Color.FromArgb(190,216,219);lblDemo.TextAlign=ContentAlignment.MiddleCenter;lblDemo.Location=new Point(30,478);
        lnkRegister.Text="Chưa có tài khoản?  Đăng ký khách hàng";lnkRegister.AutoSize=true;lnkRegister.Font=new Font("Segoe UI Semibold",8.7F);lnkRegister.LinkColor=Color.FromArgb(40,218,126);lnkRegister.ActiveLinkColor=Color.FromArgb(77,239,155);lnkRegister.Location=new Point(112,544);
        loginCard.Controls.AddRange(new Control[]{lblBrand,lblTitle,lblSub,lblUsername,pnlUsername,lblPassword,pnlPassword,chkRemember,lnkForgot,btnLogin,lblDivider,lblDemo,lnkRegister});
        right.Controls.Add(loginCard);right.Resize+=(_,__)=>{loginCard.Left=(right.Width-loginCard.Width)/2;loginCard.Top=Math.Max(18,(right.Height-loginCard.Height)/2);};
        layout.Controls.Add(hero,0,0);layout.Controls.Add(right,1,0);Controls.Add(layout);
        ClientSize=new Size(1200,720);MinimumSize=new Size(980,620);StartPosition=FormStartPosition.CenterScreen;Text="SportField - Đăng nhập";ResumeLayout(false);
    }
}
