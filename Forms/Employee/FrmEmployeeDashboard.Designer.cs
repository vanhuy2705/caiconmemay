#nullable enable
namespace QuanLyThueSanTheThao.Forms.Employee;
partial class FrmEmployeeDashboard
{
    private System.ComponentModel.IContainer? components=null;
    private Panel greeting=null!; private Label lblHello=null!; private Label lblSub=null!;
    private FlowLayoutPanel flpCards=null!; private TableLayoutPanel mainLayout=null!; private TableLayoutPanel centerLayout=null!; private TableLayoutPanel sideLayout=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlToday=null!; private Label lblToday=null!; private DataGridView gridToday=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlChart=null!; private Label lblChart=null!; private QuanLyThueSanTheThao.Forms.Common.SimpleBarChart chart=null!;
    private QuanLyThueSanTheThao.Forms.Common.MiniCalendarPanel calendar=null!; private QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel motivation=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel pnlQuick=null!; private Label lblQuick=null!; private TableLayoutPanel quickGrid=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        greeting=new Panel();lblHello=new Label();lblSub=new Label();flpCards=new FlowLayoutPanel();mainLayout=new TableLayoutPanel();centerLayout=new TableLayoutPanel();sideLayout=new TableLayoutPanel();
        pnlToday=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();lblToday=new Label();gridToday=new DataGridView();pnlChart=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();lblChart=new Label();chart=new QuanLyThueSanTheThao.Forms.Common.SimpleBarChart();calendar=new QuanLyThueSanTheThao.Forms.Common.MiniCalendarPanel();motivation=new QuanLyThueSanTheThao.Forms.Common.SportsHeroPanel();pnlQuick=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();lblQuick=new Label();quickGrid=new TableLayoutPanel();SuspendLayout();
        BackColor=Color.FromArgb(238,248,252);Padding=new Padding(0);AutoScaleMode=AutoScaleMode.Dpi;
        greeting.Dock=DockStyle.Top;greeting.Height=60;greeting.BackColor=Color.Transparent;lblHello.AutoSize=true;lblHello.Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold);lblHello.ForeColor=Color.FromArgb(18,53,78);lblHello.Location=new Point(4,2);lblSub.Text="Chúc bạn một ngày làm việc hiệu quả!";lblSub.AutoSize=true;lblSub.Font=new Font("Segoe UI",8.5F);lblSub.ForeColor=Color.FromArgb(96,120,137);lblSub.Location=new Point(38,34);greeting.Controls.AddRange(new Control[]{lblHello,lblSub});
        flpCards.Dock=DockStyle.Top;flpCards.Height=108;flpCards.WrapContents=false;flpCards.AutoScroll=true;flpCards.BackColor=Color.Transparent;
        mainLayout.Dock=DockStyle.Fill;mainLayout.ColumnCount=2;mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,78F));mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,22F));mainLayout.Padding=new Padding(0,4,0,0);
        centerLayout.Dock=DockStyle.Fill;centerLayout.ColumnCount=2;centerLayout.RowCount=2;centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,68F));centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,32F));centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent,58F));centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent,42F));centerLayout.Margin=new Padding(0,0,8,0);
        sideLayout.Dock=DockStyle.Fill;sideLayout.ColumnCount=1;sideLayout.RowCount=2;sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent,52F));sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent,48F));
        pnlToday.Dock=DockStyle.Fill;pnlToday.Radius=14;pnlToday.BorderColor=Color.FromArgb(220,233,240);pnlToday.Padding=new Padding(12,8,12,10);pnlToday.Margin=new Padding(0,0,8,8);lblToday.Text="▣  Lịch đặt sân hôm nay";lblToday.Dock=DockStyle.Top;lblToday.Height=32;lblToday.Font=new Font("Segoe UI Semibold",10.5F,FontStyle.Bold);lblToday.ForeColor=Color.FromArgb(18,53,78);gridToday.Dock=DockStyle.Fill;pnlToday.Controls.Add(gridToday);pnlToday.Controls.Add(lblToday);
        pnlChart.Dock=DockStyle.Fill;pnlChart.Radius=14;pnlChart.BorderColor=Color.FromArgb(220,233,240);pnlChart.Padding=new Padding(10,8,10,8);pnlChart.Margin=new Padding(0,0,0,8);lblChart.Text="▣  Doanh thu 7 ngày gần đây";lblChart.Dock=DockStyle.Top;lblChart.Height=32;lblChart.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold);lblChart.ForeColor=Color.FromArgb(18,53,78);chart.Dock=DockStyle.Fill;pnlChart.Controls.Add(chart);pnlChart.Controls.Add(lblChart);
        pnlQuick.Dock=DockStyle.Fill;pnlQuick.Radius=14;pnlQuick.BorderColor=Color.FromArgb(220,233,240);pnlQuick.Padding=new Padding(10,8,10,10);pnlQuick.Margin=new Padding(0,0,0,0);lblQuick.Text="⌁  Thao tác nhanh";lblQuick.Dock=DockStyle.Top;lblQuick.Height=32;lblQuick.Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold);lblQuick.ForeColor=Color.FromArgb(18,53,78);quickGrid.Dock=DockStyle.Fill;quickGrid.ColumnCount=2;quickGrid.RowCount=2;quickGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));quickGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));quickGrid.RowStyles.Add(new RowStyle(SizeType.Percent,50));quickGrid.RowStyles.Add(new RowStyle(SizeType.Percent,50));pnlQuick.Controls.Add(quickGrid);pnlQuick.Controls.Add(lblQuick);
        calendar.Dock=DockStyle.Fill;calendar.Margin=new Padding(0,0,0,8);calendar.SelectedDate=DateTime.Today;
        motivation.Dock=DockStyle.Fill;motivation.Compact=true;motivation.Heading="Làm việc hết mình!";motivation.Subheading="Quản lý chuyên nghiệp · Phát triển bền vững";motivation.Margin=new Padding(0);motivation.Paint += (_,__)=>{};
        centerLayout.Controls.Add(pnlToday,0,0);centerLayout.SetColumnSpan(pnlToday,1);centerLayout.Controls.Add(pnlChart,1,0);centerLayout.Controls.Add(pnlQuick,0,1);centerLayout.SetColumnSpan(pnlQuick,2);
        sideLayout.Controls.Add(calendar,0,0);sideLayout.Controls.Add(motivation,0,1);
        mainLayout.Controls.Add(centerLayout,0,0);mainLayout.Controls.Add(sideLayout,1,0);
        Controls.Add(mainLayout);Controls.Add(flpCards);Controls.Add(greeting);ResumeLayout(false);
    }
}
