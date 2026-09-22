#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmSettings
{
    private System.ComponentModel.IContainer? components=null;private TextBox txtCompany=null!,txtBankName=null!,txtBankBin=null!,txtAccount=null!,txtAccountName=null!,txtPrefix=null!,txtOpen=null!,txtClose=null!;private Button btnSave=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){txtCompany=new TextBox();txtBankName=new TextBox();txtBankBin=new TextBox();txtAccount=new TextBox();txtAccountName=new TextBox();txtPrefix=new TextBox();txtOpen=new TextBox();txtClose=new TextBox();btnSave=new Button();SuspendLayout();
        BackColor=Color.FromArgb(243,249,248);AutoScroll=true;
        var header=new Panel{Dock=DockStyle.Top,Height=64,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Cấu hình hệ thống",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Thông tin đơn vị, tài khoản nhận tiền VietQR và giờ hoạt động",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        header.Controls.AddRange(new Control[]{pageTitle,pageSub});
        var card=new Panel{Dock=DockStyle.Top,Height=690,BackColor=Color.White,Padding=new Padding(28)};
        var badge=new Label{Text="⚙",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(28,22)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thiết lập chung & thanh toán",AutoSize=true,Font=new Font("Segoe UI Semibold",13.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(80,26)};
        var sub=new Label{Text="Các thiết lập áp dụng cho toàn hệ thống và mã QR thanh toán",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(81,52)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(28,84),Size=new Size(644,1)};
        card.Controls.AddRange(new Control[]{badge,title,sub,sep});
        int y=104;void Section(string t){var mark=new Panel{BackColor=Color.FromArgb(19,198,119),Location=new Point(28,y+3),Size=new Size(4,15)};var l=new Label{Text=t,AutoSize=true,Font=new Font("Segoe UI Semibold",10.3F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(40,y)};card.Controls.AddRange(new Control[]{mark,l});y+=34;}
        void Field(string l,Control c,int x,int w){var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(x,y)};c.Location=new Point(x,y+21);c.Width=w;c.Height=38;card.Controls.AddRange(new Control[]{lb,c});y+=60;}
        Section("Thông tin chung");
        Field("Tên hệ thống / công ty",txtCompany,28,300);
        y+=6;Section("Thanh toán VietQR — tiền về tài khoản của bạn");
        Field("Tên ngân hàng",txtBankName,28,300);Field("Bank BIN (VietQR)",txtBankBin,28,300);Field("Số tài khoản",txtAccount,28,300);
        int yMark=y;Field("Chủ tài khoản",txtAccountName,372,300);Field("Tiền tố nội dung CK",txtPrefix,372,300);
        var hint2=new Label{Text="VD: THANHTOAN DS000123 — tiền tố đứng trước\r\nmã đơn khi khách quét QR.",AutoSize=false,Size=new Size(284,40),Font=new Font("Segoe UI",7.8F),ForeColor=Color.FromArgb(140,155,168),Location=new Point(372,yMark+122)};
        card.Controls.Add(hint2);
        y+=6;Section("Giờ hoạt động");
        Field("Giờ mở cửa (HH:mm)",txtOpen,28,150);int ySaved=y;Field("Giờ đóng cửa (HH:mm)",txtClose,200,150);
        y+=8;
        var banner=new Panel{Location=new Point(28,y),Size=new Size(644,56),BackColor=Color.FromArgb(236,249,243)};
        banner.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,643,55),10));
        var note=new Label{Text="ⓘ  Mã QR tạo động theo số tiền còn phải trả của từng đơn. Giao dịch chuyển khoản ở trạng thái chờ\r\n     cho đến khi nhân viên xác nhận tiền về.",AutoSize=false,Size=new Size(600,44),Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(16,120,84),Location=new Point(16,7)};
        banner.Controls.Add(note);card.Controls.Add(banner);
        y+=70;
        btnSave.Text="✓  Lưu cấu hình";btnSave.Location=new Point(28,y);btnSave.Size=new Size(176,44);
        var hint=new Label{Text="Áp dụng cho QR thanh toán và kiểm tra giờ đặt sân.",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(140,155,168),Location=new Point(216,y+13)};
        card.Controls.AddRange(new Control[]{btnSave,hint});
        Controls.Add(card);Controls.Add(header);ResumeLayout(false);}
}
