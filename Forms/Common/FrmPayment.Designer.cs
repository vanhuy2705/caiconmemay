#nullable enable
namespace QuanLyThueSanTheThao.Forms.Common;
partial class FrmPayment
{
    private System.ComponentModel.IContainer? components=null;
    private TableLayoutPanel layout=null!;
    private RoundedPanel leftCard=null!;
    private RoundedPanel pnlQr=null!;
    private Label lblTitle=null!,lblBooking=null!,lblAmountCaption=null!,lblAmount=null!,lblBank=null!,lblAccount=null!,lblAccountName=null!,lblContent=null!,lblQrTitle=null!,lblQrSub=null!;
    private ComboBox cboMethod=null!;private PictureBox picQr=null!;private TextBox txtReference=null!;private RoundedButton btnConfirm=null!,btnClose=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        layout=new TableLayoutPanel();leftCard=new RoundedPanel();pnlQr=new RoundedPanel();lblTitle=new Label();lblBooking=new Label();lblAmountCaption=new Label();lblAmount=new Label();lblBank=new Label();lblAccount=new Label();lblAccountName=new Label();lblContent=new Label();lblQrTitle=new Label();lblQrSub=new Label();cboMethod=new ComboBox();picQr=new PictureBox();txtReference=new TextBox();btnConfirm=new RoundedButton();btnClose=new RoundedButton();SuspendLayout();
        BackColor=Color.FromArgb(239,248,252);ClientSize=new Size(900,620);MinimumSize=new Size(820,570);StartPosition=FormStartPosition.CenterParent;Text="Thanh toán";Padding=new Padding(18);
        layout.Dock=DockStyle.Fill;layout.ColumnCount=2;layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,45));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,55));layout.RowCount=1;
        leftCard.Dock=DockStyle.Fill;leftCard.Margin=new Padding(0,0,9,0);leftCard.Padding=new Padding(24);leftCard.Radius=16;leftCard.BorderColor=Color.FromArgb(218,232,240);leftCard.BackColor=Color.White;
        var payBadge=new Label{Text="$",AutoSize=false,Size=new Size(42,42),Font=new Font("Segoe UI Symbol",14F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(24,22)};
        payBadge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,41,41),13));
        lblTitle.Text="Thanh toán đơn đặt sân";lblTitle.Font=new Font("Segoe UI Semibold",16.5F,FontStyle.Bold);lblTitle.ForeColor=Color.FromArgb(18,53,78);lblTitle.AutoSize=true;lblTitle.Location=new Point(78,26);
        var paySub=new Label{Text="Kiểm tra kỹ thông tin trước khi xác nhận",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(80,52)};
        lblBooking.AutoSize=false;lblBooking.Size=new Size(330,34);lblBooking.Location=new Point(26,90);lblBooking.ForeColor=Color.FromArgb(95,119,138);lblBooking.Font=new Font("Segoe UI",8.6F);
        leftCard.Controls.Add(payBadge);leftCard.Controls.Add(paySub);
        var amountBox=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{BackColor=Color.FromArgb(243,253,248),BorderColor=Color.FromArgb(183,228,208),Radius=12,Location=new Point(24,146),Size=new Size(334,76)};
        lblAmountCaption.Text="SỐ TIỀN CẦN THANH TOÁN";lblAmountCaption.AutoSize=true;lblAmountCaption.Location=new Point(16,11);lblAmountCaption.Font=new Font("Segoe UI Semibold",8F,FontStyle.Bold);lblAmountCaption.ForeColor=Color.FromArgb(84,140,120);
        lblAmount.AutoSize=true;lblAmount.Location=new Point(14,28);lblAmount.Font=new Font("Segoe UI Semibold",22F,FontStyle.Bold);lblAmount.ForeColor=Color.FromArgb(0,160,103);
        amountBox.Controls.AddRange(new Control[]{lblAmountCaption,lblAmount});
        var lm=new Label{Text="Phương thức thanh toán",AutoSize=true,Font=new Font("Segoe UI Semibold",8.7F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(26,236)};
        cboMethod.Location=new Point(26,261);cboMethod.Size=new Size(330,32);cboMethod.DropDownStyle=ComboBoxStyle.DropDownList;cboMethod.FlatStyle=FlatStyle.Flat;
        var lr=new Label{Text="Mã giao dịch / ghi chú",AutoSize=true,Font=new Font("Segoe UI Semibold",8.7F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(26,316)};
        txtReference.Location=new Point(26,341);txtReference.Size=new Size(330,31);txtReference.PlaceholderText="Nhập mã giao dịch nếu có";
        var note=new Label{Text="• Tiền mặt: nhân viên có thể xác nhận ngay.\n• QR/chuyển khoản: chờ nhân viên xác nhận tiền về.",AutoSize=false,Size=new Size(334,58),Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(100,123,141),Location=new Point(26,394)};
        btnConfirm.Text="Xác nhận thanh toán";btnConfirm.Location=new Point(26,478);btnConfirm.Size=new Size(190,44);btnConfirm.BackColor=Color.FromArgb(19,198,119);btnConfirm.ForeColor=Color.White;btnConfirm.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold);btnConfirm.Radius=10;btnConfirm.HoverColor=Color.FromArgb(8,158,89);
        btnClose.Text="Đóng";btnClose.Location=new Point(226,478);btnClose.Size=new Size(130,44);btnClose.BackColor=Color.FromArgb(239,247,249);btnClose.ForeColor=Color.FromArgb(18,53,78);btnClose.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold);btnClose.Radius=10;btnClose.HoverColor=Color.FromArgb(225,239,243);
        lm.Location=new Point(26,244);lm.Font=new Font("Segoe UI Semibold",8.7F,FontStyle.Bold);lm.ForeColor=Color.FromArgb(18,53,78);cboMethod.Location=new Point(26,268);cboMethod.Size=new Size(330,36);lr.Location=new Point(26,322);lr.Font=new Font("Segoe UI Semibold",8.7F,FontStyle.Bold);lr.ForeColor=Color.FromArgb(18,53,78);txtReference.Location=new Point(26,346);txtReference.Size=new Size(330,36);note.Location=new Point(26,398);btnConfirm.Location=new Point(26,470);btnClose.Location=new Point(226,470);leftCard.Controls.AddRange(new Control[]{lblTitle,lblBooking,amountBox,lm,cboMethod,lr,txtReference,note,btnConfirm,btnClose});
        pnlQr.Dock=DockStyle.Fill;pnlQr.Margin=new Padding(9,0,0,0);pnlQr.Padding=new Padding(20);pnlQr.Radius=16;pnlQr.BorderColor=Color.FromArgb(218,232,240);pnlQr.BackColor=Color.White;
        lblQrTitle.Text="Quét mã để chuyển khoản";lblQrTitle.AutoSize=true;lblQrTitle.Font=new Font("Segoe UI Semibold",14F,FontStyle.Bold);lblQrTitle.ForeColor=Color.FromArgb(18,53,78);lblQrTitle.Location=new Point(22,18);
        lblQrSub.Text="Mã QR đã chứa sẵn số tiền và nội dung chuyển khoản.";lblQrSub.AutoSize=false;lblQrSub.Size=new Size(410,40);lblQrSub.Font=new Font("Segoe UI",8.2F);lblQrSub.ForeColor=Color.FromArgb(102,125,143);lblQrSub.Location=new Point(24,50);
        picQr.SizeMode=PictureBoxSizeMode.Zoom;picQr.Location=new Point(86,92);picQr.Size=new Size(270,270);picQr.BackColor=Color.White;
        lblBank.AutoSize=true;lblBank.Font=new Font("Segoe UI Semibold",8.5F,FontStyle.Bold);lblBank.ForeColor=Color.FromArgb(18,53,78);lblBank.Location=new Point(34,388);
        lblAccount.AutoSize=true;lblAccount.Font=new Font("Segoe UI",8.4F);lblAccount.ForeColor=Color.FromArgb(66,92,112);lblAccount.Location=new Point(34,417);
        lblAccountName.AutoSize=true;lblAccountName.Font=new Font("Segoe UI",8.4F);lblAccountName.ForeColor=Color.FromArgb(66,92,112);lblAccountName.Location=new Point(34,444);
        lblContent.AutoSize=false;lblContent.Size=new Size(390,54);lblContent.Font=new Font("Segoe UI Semibold",8.4F,FontStyle.Bold);lblContent.ForeColor=Color.FromArgb(0,145,95);lblContent.Location=new Point(34,474);
        pnlQr.Controls.AddRange(new Control[]{lblQrTitle,lblQrSub,picQr,lblBank,lblAccount,lblAccountName,lblContent});
        layout.Controls.Add(leftCard,0,0);layout.Controls.Add(pnlQr,1,0);Controls.Add(layout);ResumeLayout(false);
    }
}
