#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmStatistics
{
    private System.ComponentModel.IContainer? components=null;private FlowLayoutPanel cards=null!;private TableLayoutPanel layout=null!;private QuanLyThueSanTheThao.Forms.Common.SimpleBarChart chart=null!;private DataGridView grid=null!;private DateTimePicker dtFrom=null!,dtTo=null!;private Button btnLoad=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){cards=new FlowLayoutPanel();layout=new TableLayoutPanel();chart=new QuanLyThueSanTheThao.Forms.Common.SimpleBarChart();grid=new DataGridView();dtFrom=new DateTimePicker();dtTo=new DateTimePicker();btnLoad=new Button();SuspendLayout();
        var top=new Panel{Dock=DockStyle.Top,Height=92,BackColor=Color.White};
        var pageTitle=new Label{Text="Thống kê",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(16,10)};
        var pageSub=new Label{Text="Doanh thu, lượt đặt và báo cáo theo khoảng thời gian",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,38)};
        var lblFrom=new Label{Text="Từ",AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(16,58)};
        var lblTo=new Label{Text="Đến",AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(216,58)};
        dtFrom.Format=dtTo.Format=DateTimePickerFormat.Short;dtFrom.Location=new Point(38,54);dtFrom.Width=160;dtTo.Location=new Point(248,54);dtTo.Width=160;
        btnLoad.Text="⚡  Xem báo cáo";btnLoad.Size=new Size(140,38);btnLoad.Location=new Point(420,54);
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,lblFrom,lblTo,dtFrom,dtTo,btnLoad});
        cards.Dock=DockStyle.Top;cards.Height=118;cards.WrapContents=false;cards.AutoScroll=true;cards.Padding=new Padding(12,10,12,4);
        layout.Dock=DockStyle.Fill;layout.ColumnCount=2;layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,40));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,60));
        chart.Dock=DockStyle.Fill;chart.BackColor=Color.White;grid.Dock=DockStyle.Fill;
        layout.Controls.Add(chart,0,0);layout.Controls.Add(grid,1,0);
        Controls.Add(layout);Controls.Add(cards);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);}
}
