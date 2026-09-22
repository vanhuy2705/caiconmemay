#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerProfile
{
    private System.ComponentModel.IContainer? components=null;private TextBox txtCode=null!,txtName=null!,txtPhone=null!,txtEmail=null!,txtAddress=null!;private DateTimePicker dtBirth=null!;private Label lblPoints=null!;private Button btnSave=null!,btnChangePassword=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){txtCode=new TextBox();txtName=new TextBox();txtPhone=new TextBox();txtEmail=new TextBox();txtAddress=new TextBox();dtBirth=new DateTimePicker();lblPoints=new Label();btnSave=new Button();btnChangePassword=new Button();SuspendLayout();
        BackColor=Color.FromArgb(239,248,253);
        var header=new Panel{Dock=DockStyle.Top,Height=60,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Hồ sơ của tôi",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Cập nhật thông tin cá nhân và bảo mật tài khoản",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        header.Controls.AddRange(new Control[]{pageTitle,pageSub});
        var banner=new Panel{Dock=DockStyle.Top,Height=104,BackColor=Color.White,Padding=new Padding(22)};
        var avatar=new Label{Text="♙",AutoSize=false,Size=new Size(56,56),Font=new Font("Segoe UI Symbol",20F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(22,24)};
        avatar.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,55,55),16));
        var hello=new Label{Text="Tài khoản khách hàng",AutoSize=true,Font=new Font("Segoe UI Semibold",12.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(94,30)};
        var helloSub=new Label{Text="Giữ thông tin chính xác để nhận ưu đãi nhanh nhất",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(95,54)};
        lblPoints.AutoSize=true;lblPoints.Font=new Font("Segoe UI Semibold",10.5F,FontStyle.Bold);lblPoints.ForeColor=Color.FromArgb(9,140,100);lblPoints.Anchor=AnchorStyles.Top|AnchorStyles.Right;lblPoints.Location=new Point(700,40);
        banner.Controls.AddRange(new Control[]{avatar,hello,helloSub,lblPoints});
        banner.Resize+=(_,__)=>lblPoints.Left=banner.Width-lblPoints.Width-40;
        var card=new Panel{Dock=DockStyle.Top,Height=330,BackColor=Color.White,Padding=new Padding(24)};
        var y=26;void Field(string l,Control c,int x){var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(x,y)};c.Location=new Point(x,y+21);c.Width=320;c.Height=38;card.Controls.AddRange(new Control[]{lb,c});}
        Field("Mã khách hàng",txtCode,28);txtCode.ReadOnly=true;Field("Email",txtEmail,392);y+=66;
        Field("Họ tên",txtName,28);Field("Ngày sinh",dtBirth,392);dtBirth.Format=DateTimePickerFormat.Short;y+=66;
        Field("Số điện thoại",txtPhone,28);Field("Địa chỉ",txtAddress,392);y+=74;
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(28,y),Size=new Size(684,1)};card.Controls.Add(sep);y+=20;
        btnSave.Text="✓  Lưu thay đổi";btnSave.Location=new Point(28,y);btnSave.Size=new Size(160,44);
        btnChangePassword.Text="🔒  Đổi mật khẩu";btnChangePassword.Location=new Point(200,y);btnChangePassword.Size=new Size(160,44);
        card.Controls.AddRange(new Control[]{btnSave,btnChangePassword});
        Controls.Add(card);Controls.Add(banner);Controls.Add(header);ResumeLayout(false);}
}
