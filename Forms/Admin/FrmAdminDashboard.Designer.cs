#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmAdminDashboard
{
    private System.ComponentModel.IContainer? components = null;
    private Panel header = null!;
    private Label lblGreeting = null!;
    private Label lblGreetingSub = null!;
    private Label lblDate = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnNewBooking = null!;
    private FlowLayoutPanel flpCards = null!;
    private TableLayoutPanel mainLayout = null!;
    private TableLayoutPanel leftLayout = null!;
    private TableLayoutPanel rightLayout = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlSchedule = null!;
    private Label lblSchedule = null!;
    private QuanLyThueSanTheThao.Forms.Common.BookingTimelineControl timeline = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlRecent = null!;
    private Label lblRecent = null!;
    private DataGridView gridRecent = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlStatus = null!;
    private Label lblStatus = null!;
    private QuanLyThueSanTheThao.Forms.Common.DonutStatusChart donut = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlRevenue = null!;
    private Label lblRevenue = null!;
    private QuanLyThueSanTheThao.Forms.Common.SimpleBarChart chart = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlQuick = null!;
    private Label lblQuick = null!;
    private TableLayoutPanel quickGrid = null!;

    protected override void Dispose(bool disposing){ if(disposing&&components!=null)components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        header=new Panel(); lblGreeting=new Label(); lblGreetingSub=new Label(); lblDate=new Label(); btnNewBooking=new QuanLyThueSanTheThao.Forms.Common.RoundedButton();
        flpCards=new FlowLayoutPanel(); mainLayout=new TableLayoutPanel(); leftLayout=new TableLayoutPanel(); rightLayout=new TableLayoutPanel();
        pnlSchedule=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); lblSchedule=new Label(); timeline=new QuanLyThueSanTheThao.Forms.Common.BookingTimelineControl();
        pnlRecent=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); lblRecent=new Label(); gridRecent=new DataGridView();
        pnlStatus=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); lblStatus=new Label(); donut=new QuanLyThueSanTheThao.Forms.Common.DonutStatusChart();
        pnlRevenue=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); lblRevenue=new Label(); chart=new QuanLyThueSanTheThao.Forms.Common.SimpleBarChart();
        pnlQuick=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); lblQuick=new Label(); quickGrid=new TableLayoutPanel();
        SuspendLayout();

        BackColor=Color.FromArgb(244,250,252); Padding=new Padding(0); AutoScaleMode=AutoScaleMode.Dpi;

        header.Dock=DockStyle.Top; header.Height=66; header.BackColor=Color.Transparent;
        lblGreeting.Text="☀  Xin chào!"; lblGreeting.AutoSize=true; lblGreeting.Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold); lblGreeting.ForeColor=Color.FromArgb(18,53,78); lblGreeting.Location=new Point(4,4);
        lblGreetingSub.Text="Chúc bạn một ngày làm việc hiệu quả!"; lblGreetingSub.AutoSize=true; lblGreetingSub.Font=new Font("Segoe UI",8.5F); lblGreetingSub.ForeColor=Color.FromArgb(93,119,143); lblGreetingSub.Location=new Point(38,36);
        lblDate.AutoSize=false; lblDate.Width=220; lblDate.Height=34; lblDate.Anchor=AnchorStyles.Top|AnchorStyles.Right; lblDate.TextAlign=ContentAlignment.MiddleRight; lblDate.Font=new Font("Segoe UI",8.7F); lblDate.ForeColor=Color.FromArgb(80,116,154); lblDate.Location=new Point(850,8);
        btnNewBooking.Text="＋  Đặt sân"; btnNewBooking.Size=new Size(112,38); btnNewBooking.Anchor=AnchorStyles.Top|AnchorStyles.Right; btnNewBooking.BackColor=Color.FromArgb(19,198,119); btnNewBooking.ForeColor=Color.White; btnNewBooking.Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold); btnNewBooking.HoverColor=Color.FromArgb(8,158,89); btnNewBooking.Radius=9; btnNewBooking.Location=new Point(1080,7);
        header.Controls.AddRange(new Control[]{lblGreeting,lblGreetingSub,lblDate,btnNewBooking});
        header.Resize += (_,__) => { btnNewBooking.Left=header.Width-btnNewBooking.Width-4; lblDate.Left=btnNewBooking.Left-lblDate.Width-14; };

        flpCards.Dock=DockStyle.Top; flpCards.Height=112; flpCards.Padding=new Padding(0,1,0,3); flpCards.WrapContents=false; flpCards.AutoScroll=true; flpCards.BackColor=Color.Transparent;

        mainLayout.Dock=DockStyle.Fill; mainLayout.ColumnCount=2; mainLayout.RowCount=1; mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,70F)); mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,30F)); mainLayout.Padding=new Padding(0,4,0,0);
        leftLayout.Dock=DockStyle.Fill; leftLayout.RowCount=2; leftLayout.ColumnCount=1; leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent,56F)); leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent,44F)); leftLayout.Margin=new Padding(0,0,8,0);
        rightLayout.Dock=DockStyle.Fill; rightLayout.RowCount=3; rightLayout.ColumnCount=1; rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent,33F)); rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent,39F)); rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent,28F)); rightLayout.Margin=new Padding(0);

        pnlSchedule.Dock=DockStyle.Fill; pnlSchedule.Radius=13; pnlSchedule.BorderColor=Color.FromArgb(218,232,240); pnlSchedule.Margin=new Padding(0,0,0,8); pnlSchedule.Padding=new Padding(12,8,12,10);
        lblSchedule.Text="▣  Lịch sân hôm nay"; lblSchedule.Dock=DockStyle.Top; lblSchedule.Height=32; lblSchedule.Font=new Font("Segoe UI Semibold",10.5F,FontStyle.Bold); lblSchedule.ForeColor=Color.FromArgb(18,53,78); lblSchedule.TextAlign=ContentAlignment.MiddleLeft;
        timeline.Dock=DockStyle.Fill; timeline.BackColor=Color.White; pnlSchedule.Controls.Add(timeline); pnlSchedule.Controls.Add(lblSchedule);

        pnlRecent.Dock=DockStyle.Fill; pnlRecent.Radius=13; pnlRecent.BorderColor=Color.FromArgb(218,232,240); pnlRecent.Padding=new Padding(12,8,12,10); pnlRecent.Margin=new Padding(0);
        lblRecent.Text="◷  Đặt sân gần đây"; lblRecent.Dock=DockStyle.Top; lblRecent.Height=32; lblRecent.Font=new Font("Segoe UI Semibold",10.5F,FontStyle.Bold); lblRecent.ForeColor=Color.FromArgb(18,53,78); lblRecent.TextAlign=ContentAlignment.MiddleLeft;
        gridRecent.Dock=DockStyle.Fill; pnlRecent.Controls.Add(gridRecent); pnlRecent.Controls.Add(lblRecent);

        pnlStatus.Dock=DockStyle.Fill; pnlStatus.Radius=13; pnlStatus.BorderColor=Color.FromArgb(218,232,240); pnlStatus.Padding=new Padding(10,8,10,8); pnlStatus.Margin=new Padding(0,0,0,8);
        lblStatus.Text="▣  Trạng thái sân"; lblStatus.Dock=DockStyle.Top; lblStatus.Height=30; lblStatus.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold); lblStatus.ForeColor=Color.FromArgb(18,53,78);
        donut.Dock=DockStyle.Fill; pnlStatus.Controls.Add(donut); pnlStatus.Controls.Add(lblStatus);

        pnlRevenue.Dock=DockStyle.Fill; pnlRevenue.Radius=13; pnlRevenue.BorderColor=Color.FromArgb(218,232,240); pnlRevenue.Padding=new Padding(10,8,10,8); pnlRevenue.Margin=new Padding(0,0,0,8);
        lblRevenue.Text="▣  Doanh thu 7 ngày gần đây"; lblRevenue.Dock=DockStyle.Top; lblRevenue.Height=30; lblRevenue.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold); lblRevenue.ForeColor=Color.FromArgb(18,53,78);
        chart.Dock=DockStyle.Fill; pnlRevenue.Controls.Add(chart); pnlRevenue.Controls.Add(lblRevenue);

        pnlQuick.Dock=DockStyle.Fill; pnlQuick.Radius=13; pnlQuick.BorderColor=Color.FromArgb(218,232,240); pnlQuick.Padding=new Padding(10,8,10,10); pnlQuick.Margin=new Padding(0);
        lblQuick.Text="⌁  Thao tác nhanh"; lblQuick.Dock=DockStyle.Top; lblQuick.Height=30; lblQuick.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold); lblQuick.ForeColor=Color.FromArgb(18,53,78);
        quickGrid.Dock=DockStyle.Fill; quickGrid.ColumnCount=2; quickGrid.RowCount=2; quickGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50)); quickGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50)); quickGrid.RowStyles.Add(new RowStyle(SizeType.Percent,50)); quickGrid.RowStyles.Add(new RowStyle(SizeType.Percent,50)); quickGrid.Padding=new Padding(0,2,0,0);
        pnlQuick.Controls.Add(quickGrid); pnlQuick.Controls.Add(lblQuick);

        leftLayout.Controls.Add(pnlSchedule,0,0); leftLayout.Controls.Add(pnlRecent,0,1);
        rightLayout.Controls.Add(pnlStatus,0,0); rightLayout.Controls.Add(pnlRevenue,0,1); rightLayout.Controls.Add(pnlQuick,0,2);
        mainLayout.Controls.Add(leftLayout,0,0); mainLayout.Controls.Add(rightLayout,1,0);

        Controls.Add(mainLayout); Controls.Add(flpCards); Controls.Add(header);
        ResumeLayout(false);
    }
}
