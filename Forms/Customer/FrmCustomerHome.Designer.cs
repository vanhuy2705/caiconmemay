#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerHome
{
    private System.ComponentModel.IContainer? components=null;
    private TableLayoutPanel rootLayout=null!; private TableLayoutPanel leftLayout=null!; private TableLayoutPanel rightLayout=null!;
    private QuanLyThueSanTheThao.Forms.Common.CustomerBannerPanel hero=null!; private FlowLayoutPanel categoryFlow=null!;
    private Panel fieldsHeader=null!; private Label lblFields=null!; private FlowLayoutPanel flpFields=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlProfile=null!; private Label lblWelcome=null!; private FlowLayoutPanel flpStats=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlVoucher=null!; private Label lblVoucherTitle=null!; private Label lblVoucher=null!;
    private QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel promoBanner=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        rootLayout=new TableLayoutPanel();leftLayout=new TableLayoutPanel();rightLayout=new TableLayoutPanel();hero=new QuanLyThueSanTheThao.Forms.Common.CustomerBannerPanel();categoryFlow=new FlowLayoutPanel();fieldsHeader=new Panel();lblFields=new Label();flpFields=new FlowLayoutPanel();pnlProfile=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();lblWelcome=new Label();flpStats=new FlowLayoutPanel();pnlVoucher=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();lblVoucherTitle=new Label();lblVoucher=new Label();promoBanner=new QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel();SuspendLayout();
        BackColor=Color.FromArgb(239,248,253);AutoScaleMode=AutoScaleMode.Dpi;
        rootLayout.Dock=DockStyle.Fill;rootLayout.ColumnCount=2;rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,76F));rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,24F));rootLayout.RowCount=1;rootLayout.Padding=new Padding(0);
        leftLayout.Dock=DockStyle.Fill;leftLayout.ColumnCount=1;leftLayout.RowCount=4;leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,166));leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,66));leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,38));leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100));leftLayout.Margin=new Padding(0,0,8,0);
        rightLayout.Dock=DockStyle.Fill;rightLayout.ColumnCount=1;rightLayout.RowCount=3;rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,158));rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,126));rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100));rightLayout.Margin=new Padding(8,0,0,0);
        hero.Dock=DockStyle.Fill;hero.Margin=new Padding(0,0,0,8);
        categoryFlow.Dock=DockStyle.Fill;categoryFlow.WrapContents=false;categoryFlow.AutoScroll=true;categoryFlow.Padding=new Padding(0,4,0,4);categoryFlow.BackColor=Color.Transparent;
        fieldsHeader.Dock=DockStyle.Fill;fieldsHeader.BackColor=Color.Transparent;lblFields.Text="★  Sân nổi bật";lblFields.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold);lblFields.ForeColor=Color.FromArgb(18,53,78);lblFields.AutoSize=true;lblFields.Location=new Point(8,9);fieldsHeader.Controls.Add(lblFields);
        flpFields.Dock=DockStyle.Fill;flpFields.AutoScroll=true;flpFields.WrapContents=true;flpFields.BackColor=Color.Transparent;flpFields.Padding=new Padding(0,0,0,4);
        pnlProfile.Dock=DockStyle.Fill;pnlProfile.Radius=14;pnlProfile.BorderColor=Color.FromArgb(218,232,240);pnlProfile.Padding=new Padding(12);pnlProfile.Margin=new Padding(0,0,0,8);lblWelcome.Text="Xin chào!";lblWelcome.AutoSize=true;lblWelcome.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold);lblWelcome.ForeColor=Color.FromArgb(18,53,78);lblWelcome.Location=new Point(14,12);flpStats.Location=new Point(12,47);flpStats.Size=new Size(340,92);flpStats.Anchor=AnchorStyles.Left|AnchorStyles.Right|AnchorStyles.Top;flpStats.WrapContents=false;flpStats.AutoScroll=true;flpStats.BackColor=Color.Transparent;pnlProfile.Controls.AddRange(new Control[]{lblWelcome,flpStats});pnlProfile.Resize+=(_,__)=>flpStats.Width=pnlProfile.Width-24;
        pnlVoucher.Dock=DockStyle.Fill;pnlVoucher.Radius=14;pnlVoucher.BorderColor=Color.FromArgb(218,232,240);pnlVoucher.Padding=new Padding(12);pnlVoucher.Margin=new Padding(0,0,0,8);lblVoucherTitle.Text="Voucher của bạn";lblVoucherTitle.AutoSize=true;lblVoucherTitle.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold);lblVoucherTitle.ForeColor=Color.FromArgb(18,53,78);lblVoucherTitle.Location=new Point(14,12);lblVoucher.Text="GIẢM 20%\nCho đơn đủ điều kiện\nHSD: --/--/----";lblVoucher.AutoSize=false;lblVoucher.TextAlign=ContentAlignment.MiddleLeft;lblVoucher.Padding=new Padding(18,0,0,0);lblVoucher.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold);lblVoucher.ForeColor=Color.White;lblVoucher.BackColor=Color.FromArgb(14,196,118);lblVoucher.Location=new Point(12,42);lblVoucher.Size=new Size(320,68);pnlVoucher.Controls.AddRange(new Control[]{lblVoucherTitle,lblVoucher});pnlVoucher.Resize+=(_,__)=>lblVoucher.Width=pnlVoucher.Width-24;
        promoBanner.Dock=DockStyle.Fill;promoBanner.Compact=true;promoBanner.Margin=new Padding(0);promoBanner.Subheading="Đặt sân hôm nay · Nhận ngay ưu đãi!";
        leftLayout.Controls.Add(hero,0,0);leftLayout.Controls.Add(categoryFlow,0,1);leftLayout.Controls.Add(fieldsHeader,0,2);leftLayout.Controls.Add(flpFields,0,3);
        rightLayout.Controls.Add(pnlProfile,0,0);rightLayout.Controls.Add(pnlVoucher,0,1);rightLayout.Controls.Add(promoBanner,0,2);
        rootLayout.Controls.Add(leftLayout,0,0);rootLayout.Controls.Add(rightLayout,1,0);Controls.Add(rootLayout);ResumeLayout(false);
    }
}
