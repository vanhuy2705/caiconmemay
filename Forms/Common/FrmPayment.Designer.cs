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
        layout=new TableLayoutPanel();leftCard=new RoundedPanel();pnlQr=new RoundedPanel();lblTitle=new Label();lblBooking=new Label();lblAmountCaption=new Label();lblAmount=new Label();lblBank=new Label();lblAccount=new Label();lblAccountName=new Label();lblContent=new Label();lblQrTitle=new Label();lblQrSub=new Label();cboMethod=new ComboBox();picQr=new PictureBox();txtReference=new TextBox();btnConfirm=new RoundedButton();btnClose=new RoundedButton();
        SuspendLayout();
        BackColor=Color.FromArgb(239,248,252);ClientSize=new Size(960,640);MinimumSize=new Size(860,580);StartPosition=FormStartPosition.CenterParent;Text="Thanh toán";Padding=new Padding(16);AutoScaleMode=AutoScaleMode.Dpi;
        layout.Dock=DockStyle.Fill;layout.ColumnCount=2;layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,44F));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,56F));layout.RowCount=1;
        leftCard.Dock=DockStyle.Fill;leftCard.Margin=new Padding(0,0,8,0);leftCard.Padding=new Padding(20);leftCard.Radius=16;leftCard.BorderColor=Color.FromArgb(218,232,240);leftCard.BackColor=Color.White;leftCard.AutoScroll=true;
        var payBadge=new Label{Text="$",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(20,18)};
        payBadge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        lblTitle.Text="Thanh toán đơn đặt sân";lblTitle.Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold);lblTitle.ForeColor=Color.FromArgb(18,53,78);lblTitle.AutoSize=true;lblTitle.Location=new Point(70,20);
        var paySub=new Label{Text="Kiểm tra kỹ thông tin trước khi xác nhận",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(72,46)};
        lblBooking.AutoSize=false;lblBooking.Size=new Size(320,36);lblBooking.Location=new Point(20,84);lblBooking.ForeColor=Color.FromArgb(95,119,138);lblBooking.Font=new Font("Segoe UI",8.6F);
        var amountBox=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{BackColor=Color.FromArgb(243,253,248),BorderColor=Color.FromArgb(183,228,208),Radius=12,Location=new Point(20,132),Size=new Size(320,76),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        lblAmountCaption.Text="SỐ TIỀN CẦN THANH TOÁN";lblAmountCaption.AutoSize=true;lblAmountCaption.Location=new Point(14,10);lblAmountCaption.Font=new Font("Segoe UI Semibold",8F,FontStyle.Bold);lblAmountCaption.ForeColor=Color.FromArgb(84,140,120);
        lblAmount.AutoSize=true;lblAmount.Location=new Point(12,26);lblAmount.Font=new Font("Segoe UI Semibold",20F,FontStyle.Bold);lblAmount.ForeColor=Color.FromArgb(0,160,103);
        amountBox.Controls.AddRange(new Control[]{lblAmountCaption,lblAmount});
        var lm=new Label{Text="Phương thức thanh toán",AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(20,224)};
        cboMethod.Location=new Point(20,248);cboMethod.Size=new Size(320,34);cboMethod.DropDownStyle=ComboBoxStyle.DropDownList;cboMethod.FlatStyle=FlatStyle.Flat;cboMethod.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        var lr=new Label{Text="Mã giao dịch / ghi chú",AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(20,300)};
        txtReference.Location=new Point(20,324);txtReference.Size=new Size(320,34);txtReference.PlaceholderText="Nhập mã giao dịch nếu có";txtReference.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        var note=new Label{Text="• Tiền mặt: nhân viên có thể xác nhận ngay.\n• QR/chuyển khoản: chờ nhân viên xác nhận tiền về.",AutoSize=false,Size=new Size(320,48),Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(100,123,141),Location=new Point(20,376),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        btnConfirm.Text="Xác nhận thanh toán";btnConfirm.Location=new Point(20,440);btnConfirm.Size=new Size(180,44);btnConfirm.BackColor=Color.FromArgb(19,198,119);btnConfirm.ForeColor=Color.White;btnConfirm.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold);btnConfirm.Radius=10;btnConfirm.HoverColor=Color.FromArgb(8,158,89);
        btnClose.Text="Đóng";btnClose.Location=new Point(210,440);btnClose.Size=new Size(110,44);btnClose.BackColor=Color.FromArgb(239,247,249);btnClose.ForeColor=Color.FromArgb(18,53,78);btnClose.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold);btnClose.Radius=10;btnClose.HoverColor=Color.FromArgb(225,239,243);
        leftCard.Controls.AddRange(new Control[]{payBadge,lblTitle,paySub,lblBooking,amountBox,lm,cboMethod,lr,txtReference,note,btnConfirm,btnClose});
        leftCard.Resize+=(s,e)=>{
            int w=leftCard.ClientSize.Width-40;
            amountBox.Width=w;lblBooking.Width=w;cboMethod.Width=w;txtReference.Width=w;note.Width=w;
        };

        pnlQr.Dock=DockStyle.Fill;pnlQr.Margin=new Padding(8,0,0,0);pnlQr.Padding=new Padding(20);pnlQr.Radius=16;pnlQr.BorderColor=Color.FromArgb(218,232,240);pnlQr.BackColor=Color.White;
        lblQrTitle.Text="Quét mã để chuyển khoản";lblQrTitle.AutoSize=true;lblQrTitle.Font=new Font("Segoe UI Semibold",13F,FontStyle.Bold);lblQrTitle.ForeColor=Color.FromArgb(18,53,78);lblQrTitle.Location=new Point(20,16);
        lblQrSub.Text="Mã QR đã chứa sẵn số tiền và nội dung chuyển khoản.";lblQrSub.AutoSize=false;lblQrSub.Size=new Size(400,36);lblQrSub.Font=new Font("Segoe UI",8.2F);lblQrSub.ForeColor=Color.FromArgb(102,125,143);lblQrSub.Location=new Point(20,44);
        picQr.SizeMode=PictureBoxSizeMode.Zoom;picQr.Location=new Point(60,84);picQr.Size=new Size(280,280);picQr.BackColor=Color.White;picQr.Anchor=AnchorStyles.Top;
        lblBank.AutoSize=true;lblBank.Font=new Font("Segoe UI Semibold",8.5F,FontStyle.Bold);lblBank.ForeColor=Color.FromArgb(18,53,78);lblBank.Location=new Point(20,380);
        lblAccount.AutoSize=true;lblAccount.Font=new Font("Segoe UI",8.4F);lblAccount.ForeColor=Color.FromArgb(66,92,112);lblAccount.Location=new Point(20,404);
        lblAccountName.AutoSize=true;lblAccountName.Font=new Font("Segoe UI",8.4F);lblAccountName.ForeColor=Color.FromArgb(66,92,112);lblAccountName.Location=new Point(20,428);
        lblContent.AutoSize=false;lblContent.Size=new Size(380,50);lblContent.Font=new Font("Segoe UI Semibold",8.4F,FontStyle.Bold);lblContent.ForeColor=Color.FromArgb(0,145,95);lblContent.Location=new Point(20,456);
        pnlQr.Controls.AddRange(new Control[]{lblQrTitle,lblQrSub,picQr,lblBank,lblAccount,lblAccountName,lblContent});
        pnlQr.Resize+=(s,e)=>{
            picQr.Left=(pnlQr.Width-picQr.Width)/2;
        };

        layout.Controls.Add(leftCard,0,0);layout.Controls.Add(pnlQr,1,0);
        layout.Resize+=(s,e)=>{
            bool narrow=layout.Width<800;
            if(narrow && layout.ColumnCount==2){
                layout.ColumnCount=1;layout.RowCount=2;
                layout.ColumnStyles.Clear();layout.RowStyles.Clear();
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent,55F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent,45F));
                layout.SetCellPosition(leftCard,new TableLayoutPanelCellPosition(0,0));
                layout.SetCellPosition(pnlQr,new TableLayoutPanelCellPosition(0,1));
                leftCard.Margin=new Padding(0,0,0,8);
                pnlQr.Margin=new Padding(0,8,0,0);
            }else if(!narrow && layout.ColumnCount==1){
                layout.ColumnCount=2;layout.RowCount=1;
                layout.ColumnStyles.Clear();layout.RowStyles.Clear();
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,44F));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,56F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
                layout.SetCellPosition(leftCard,new TableLayoutPanelCellPosition(0,0));
                layout.SetCellPosition(pnlQr,new TableLayoutPanelCellPosition(1,0));
                leftCard.Margin=new Padding(0,0,8,0);
                pnlQr.Margin=new Padding(8,0,0,0);
            }
        };

        Controls.Add(layout);ResumeLayout(false);
    }
}
