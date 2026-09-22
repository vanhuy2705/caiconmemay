#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmBooking
{
    private System.ComponentModel.IContainer? components=null;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel card=null!;
    private TableLayoutPanel mainLayout=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel left=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel right=null!;
    private ComboBox cboCustomer=null!,cboType=null!,cboField=null!,cboVoucher=null!;
    private DateTimePicker dtDate=null!,dtStart=null!,dtEnd=null!;private TextBox txtNote=null!;
    private Label lblPrice=null!,lblSubtotal=null!,lblDiscount=null!,lblTotal=null!,lblAvailability=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnCheck=null!,btnBook=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        card=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();
        mainLayout=new TableLayoutPanel();
        left=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();
        right=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();
        cboCustomer=new ComboBox();cboType=new ComboBox();cboField=new ComboBox();cboVoucher=new ComboBox();
        dtDate=new DateTimePicker();dtStart=new DateTimePicker();dtEnd=new DateTimePicker();txtNote=new TextBox();
        lblPrice=new Label();lblSubtotal=new Label();lblDiscount=new Label();lblTotal=new Label();lblAvailability=new Label();
        btnCheck=new QuanLyThueSanTheThao.Forms.Common.RoundedButton();btnBook=new QuanLyThueSanTheThao.Forms.Common.RoundedButton();
        SuspendLayout();

        BackColor=Color.FromArgb(239,248,252);Padding=new Padding(12);AutoScaleMode=AutoScaleMode.Dpi;AutoScroll=true;

        // Card chính
        card.BackColor=Color.White;card.Dock=DockStyle.Fill;card.Padding=new Padding(20);card.Radius=16;card.BorderColor=Color.FromArgb(218,232,240);
        Controls.Add(card);

        // Header
        var header=new Panel{Dock=DockStyle.Top,Height=88,BackColor=Color.White};
        var badge=new QuanLyThueSanTheThao.Forms.Common.IconBadge{IconImage=QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Booking,20,Color.White),Size=new Size(44,44),AccentColor=Color.FromArgb(19,170,157),Location=new Point(8,12)};
        var title=new Label{Text="Đặt sân thể thao",Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),AutoSize=true,Location=new Point(62,14)};
        var sub=new Label{Text="Chọn sân và khung giờ — hệ thống tự kiểm tra trùng lịch và tính tiền",Font=new Font("Segoe UI",8.5F),ForeColor=Color.FromArgb(103,126,145),AutoSize=true,Location=new Point(64,42)};
        var sep=new Panel{BackColor=Color.FromArgb(232,239,243),Dock=DockStyle.Bottom,Height=1};
        header.Controls.AddRange(new Control[]{badge,title,sub,sep});
        card.Controls.Add(header);

        // Main layout 2 cột responsive
        mainLayout.Dock=DockStyle.Fill;mainLayout.Padding=new Padding(0,8,0,0);
        mainLayout.ColumnCount=2;mainLayout.RowCount=1;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,48F));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,52F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
        card.Controls.Add(mainLayout);

        // Left panel
        left.BackColor=Color.FromArgb(250,253,254);left.BorderColor=Color.FromArgb(229,237,242);left.Radius=14;left.Dock=DockStyle.Fill;left.Padding=new Padding(16);left.Margin=new Padding(0,0,6,0);
        left.AutoScroll=true;
        var ltitle=new Label{Text="THÔNG TIN ĐẶT SÂN",AutoSize=true,Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold),ForeColor=Color.FromArgb(52,84,105),Dock=DockStyle.Top,Height=28};
        left.Controls.Add(ltitle);

        var leftFlow=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,AutoScroll=false,Padding=new Padding(0,4,0,0)};
        leftFlow.RowCount=5;
        for(int i=0;i<5;i++) leftFlow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        left.Controls.Add(leftFlow);

        // Helper tạo field
        Control MakeField(string labelText, Control input)
        {
            var p=new Panel{Dock=DockStyle.Top,Height=68,BackColor=Color.Transparent,Padding=new Padding(0,4,0,4)};
            var lb=new Label{Text=labelText,AutoSize=true,Font=new Font("Segoe UI Semibold",8.4F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(2,2)};
            input.Location=new Point(2,22);input.Width=p.Width-4;input.Height=36;input.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
            p.Controls.AddRange(new Control[]{lb,input});
            p.Resize+=(s,e)=>{input.Width=p.Width-4;};
            return p;
        }

        leftFlow.Controls.Add(MakeField("Khách hàng *",cboCustomer),0,0);
        leftFlow.Controls.Add(MakeField("Loại sân",cboType),0,1);
        leftFlow.Controls.Add(MakeField("Sân trống *",cboField),0,2);
        leftFlow.Controls.Add(MakeField("Voucher giảm giá",cboVoucher),0,3);
        var notePanel=new Panel{Dock=DockStyle.Top,Height=90,BackColor=Color.Transparent,Padding=new Padding(0,4,0,0)};
        var noteLb=new Label{Text="Ghi chú",AutoSize=true,Font=new Font("Segoe UI Semibold",8.4F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(2,2)};
        txtNote.Multiline=true;txtNote.Location=new Point(2,22);txtNote.Height=58;txtNote.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        txtNote.Width=notePanel.Width-4;
        notePanel.Controls.AddRange(new Control[]{noteLb,txtNote});
        notePanel.Resize+=(s,e)=>{txtNote.Width=notePanel.Width-4;};
        leftFlow.Controls.Add(notePanel,0,4);

        // Right panel
        right.BackColor=Color.FromArgb(250,253,254);right.BorderColor=Color.FromArgb(229,237,242);right.Radius=14;right.Dock=DockStyle.Fill;right.Padding=new Padding(16);right.Margin=new Padding(6,0,0,0);
        right.AutoScroll=true;
        var rtitle=new Label{Text="THỜI GIAN & THANH TOÁN",AutoSize=true,Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold),ForeColor=Color.FromArgb(52,84,105),Dock=DockStyle.Top,Height=28};
        right.Controls.Add(rtitle);

        var rightFlow=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,Padding=new Padding(0,4,0,0)};
        rightFlow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        rightFlow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        rightFlow.RowCount=6;
        for(int i=0;i<6;i++) rightFlow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.Controls.Add(rightFlow);

        // Time pickers
        var dateP=new Panel{Dock=DockStyle.Fill,Height=68,BackColor=Color.Transparent,Padding=new Padding(0,4,4,4)};
        var dateLb=new Label{Text="Ngày đặt *",AutoSize=true,Font=new Font("Segoe UI Semibold",8.4F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(2,2)};
        dtDate.Location=new Point(2,22);dtDate.Height=34;dtDate.Dock=DockStyle.Bottom;dtDate.Anchor=AnchorStyles.Left|AnchorStyles.Right|AnchorStyles.Bottom;
        dateP.Controls.AddRange(new Control[]{dateLb,dtDate});
        rightFlow.Controls.Add(dateP,0,0);
        rightFlow.SetColumnSpan(dateP,2);

        var startP=new Panel{Dock=DockStyle.Fill,Height=68,BackColor=Color.Transparent,Padding=new Padding(0,4,4,4)};
        var startLb=new Label{Text="Giờ bắt đầu *",AutoSize=true,Font=new Font("Segoe UI Semibold",8.4F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(2,2)};
        dtStart.Location=new Point(2,22);dtStart.Height=34;dtStart.Dock=DockStyle.Bottom;
        startP.Controls.AddRange(new Control[]{startLb,dtStart});
        rightFlow.Controls.Add(startP,0,1);

        var endP=new Panel{Dock=DockStyle.Fill,Height=68,BackColor=Color.Transparent,Padding=new Padding(4,4,0,4)};
        var endLb=new Label{Text="Giờ kết thúc *",AutoSize=true,Font=new Font("Segoe UI Semibold",8.4F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(2,2)};
        dtEnd.Location=new Point(2,22);dtEnd.Height=34;dtEnd.Dock=DockStyle.Bottom;
        endP.Controls.AddRange(new Control[]{endLb,dtEnd});
        rightFlow.Controls.Add(endP,1,1);

        // Check button + availability
        var checkPanel=new Panel{Dock=DockStyle.Fill,Height=46,BackColor=Color.Transparent,Padding=new Padding(0,6,0,0)};
        btnCheck.Text="⟳  Kiểm tra sân trống";btnCheck.Dock=DockStyle.Fill;btnCheck.BackColor=Color.FromArgb(236,248,246);btnCheck.ForeColor=Color.FromArgb(0,135,100);btnCheck.Radius=10;btnCheck.HoverColor=Color.FromArgb(220,242,236);btnCheck.Font=new Font("Segoe UI Semibold",8.8F,FontStyle.Bold);
        checkPanel.Controls.Add(btnCheck);
        rightFlow.Controls.Add(checkPanel,0,2);
        rightFlow.SetColumnSpan(checkPanel,2);

        lblAvailability.AutoSize=false;lblAvailability.Dock=DockStyle.Fill;lblAvailability.Height=32;lblAvailability.ForeColor=Color.FromArgb(0,145,95);lblAvailability.Font=new Font("Segoe UI Semibold",8.6F);lblAvailability.TextAlign=ContentAlignment.MiddleLeft;
        lblAvailability.Padding=new Padding(4,0,0,0);
        rightFlow.Controls.Add(lblAvailability,0,3);
        rightFlow.SetColumnSpan(lblAvailability,2);

        // Summary box
        var summary=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{BackColor=Color.FromArgb(243,253,248),BorderColor=Color.FromArgb(183,228,208),Radius=12,Dock=DockStyle.Fill,Height=130,Margin=new Padding(0,8,0,8),Padding=new Padding(14)};
        lblPrice.AutoSize=true;lblPrice.ForeColor=Color.FromArgb(91,113,130);lblPrice.Font=new Font("Segoe UI",8.6F);lblPrice.Location=new Point(14,12);
        lblSubtotal.AutoSize=true;lblSubtotal.ForeColor=Color.FromArgb(52,84,105);lblSubtotal.Font=new Font("Segoe UI Semibold",9F);lblSubtotal.Location=new Point(14,34);
        lblDiscount.AutoSize=true;lblDiscount.ForeColor=Color.FromArgb(214,106,22);lblDiscount.Font=new Font("Segoe UI Semibold",9F);lblDiscount.Location=new Point(14,58);
        lblTotal.AutoSize=true;lblTotal.Font=new Font("Segoe UI Semibold",14F,FontStyle.Bold);lblTotal.ForeColor=Color.FromArgb(0,150,100);lblTotal.Location=new Point(14,82);
        summary.Controls.AddRange(new Control[]{lblPrice,lblSubtotal,lblDiscount,lblTotal});
        rightFlow.Controls.Add(summary,0,4);
        rightFlow.SetColumnSpan(summary,2);

        // Book button
        var bookPanel=new Panel{Dock=DockStyle.Fill,Height=56,BackColor=Color.Transparent,Padding=new Padding(0,6,0,0)};
        btnBook.Text="✓  XÁC NHẬN ĐẶT SÂN";btnBook.Dock=DockStyle.Fill;btnBook.BackColor=Color.FromArgb(19,198,119);btnBook.ForeColor=Color.White;btnBook.Radius=11;btnBook.HoverColor=Color.FromArgb(8,158,89);btnBook.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold);
        bookPanel.Controls.Add(btnBook);
        rightFlow.Controls.Add(bookPanel,0,5);
        rightFlow.SetColumnSpan(bookPanel,2);

        var note=new Label{Text="Lịch được xác nhận ngay sau khi tạo; trạng thái thanh toán được theo dõi riêng.",AutoSize=false,Dock=DockStyle.Bottom,Height=36,Font=new Font("Segoe UI",7.8F),ForeColor=Color.FromArgb(150,163,175),TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(4,4,0,0)};
        right.Controls.Add(note);

        mainLayout.Controls.Add(left,0,0);
        mainLayout.Controls.Add(right,1,0);

        // Responsive handler
        mainLayout.Resize+=(s,e)=>{
            bool narrow = mainLayout.Width < 900;
            if(narrow && mainLayout.ColumnCount==2)
            {
                mainLayout.SuspendLayout();
                mainLayout.ColumnCount=1;mainLayout.RowCount=2;
                mainLayout.ColumnStyles.Clear();mainLayout.RowStyles.Clear();
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent,52F));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent,48F));
                mainLayout.SetCellPosition(left,new TableLayoutPanelCellPosition(0,0));
                mainLayout.SetCellPosition(right,new TableLayoutPanelCellPosition(0,1));
                left.Margin=new Padding(0,0,0,6);
                right.Margin=new Padding(0,6,0,0);
                mainLayout.ResumeLayout(true);
            }
            else if(!narrow && mainLayout.ColumnCount==1)
            {
                mainLayout.SuspendLayout();
                mainLayout.ColumnCount=2;mainLayout.RowCount=1;
                mainLayout.ColumnStyles.Clear();mainLayout.RowStyles.Clear();
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,48F));
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,52F));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
                mainLayout.SetCellPosition(left,new TableLayoutPanelCellPosition(0,0));
                mainLayout.SetCellPosition(right,new TableLayoutPanelCellPosition(1,0));
                left.Margin=new Padding(0,0,6,0);
                right.Margin=new Padding(6,0,0,0);
                mainLayout.ResumeLayout(true);
            }
        };

        ResumeLayout(false);
    }
}
