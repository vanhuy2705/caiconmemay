#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmStatistics
{
    private System.ComponentModel.IContainer? components=null;private FlowLayoutPanel cards=null!;private TableLayoutPanel layout=null!;private QuanLyThueSanTheThao.Forms.Common.SimpleBarChart chart=null!;private DataGridView grid=null!;private DateTimePicker dtFrom=null!,dtTo=null!;private Button btnLoad=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        cards=new FlowLayoutPanel();layout=new TableLayoutPanel();chart=new QuanLyThueSanTheThao.Forms.Common.SimpleBarChart();grid=new DataGridView();dtFrom=new DateTimePicker();dtTo=new DateTimePicker();btnLoad=new Button();
        SuspendLayout();
        var top=new Panel{Dock=DockStyle.Top,Height=88,BackColor=Color.White,Padding=new Padding(12,8,12,8)};
        var pageTitle=new Label{Text="Thống kê",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(12,8)};
        var pageSub=new Label{Text="Doanh thu, lượt đặt và báo cáo theo khoảng thời gian",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(14,34)};
        var lblFrom=new Label{Text="Từ",AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(12,58)};
        var lblTo=new Label{Text="Đến",AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(210,58)};
        dtFrom.Format=dtTo.Format=DateTimePickerFormat.Short;dtFrom.Location=new Point(36,56);dtFrom.Width=150;dtTo.Location=new Point(244,56);dtTo.Width=150;
        btnLoad.Text="⚡  Xem báo cáo";btnLoad.Size=new Size(130,36);btnLoad.Location=new Point(410,54);
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,lblFrom,lblTo,dtFrom,dtTo,btnLoad});
        cards.Dock=DockStyle.Top;cards.Height=116;cards.WrapContents=false;cards.AutoScroll=true;cards.Padding=new Padding(12,8,12,4);cards.BackColor=Color.Transparent;
        layout.Dock=DockStyle.Fill;layout.ColumnCount=2;layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,42F));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,58F));layout.RowCount=1;
        chart.Dock=DockStyle.Fill;chart.BackColor=Color.White;chart.Margin=new Padding(0,0,6,0);
        grid.Dock=DockStyle.Fill;grid.Margin=new Padding(6,0,0,0);
        layout.Controls.Add(chart,0,0);layout.Controls.Add(grid,1,0);
        layout.Resize+=(s,e)=>{
            bool narrow=layout.Width<800;
            if(narrow && layout.ColumnCount==2){
                layout.ColumnCount=1;layout.RowCount=2;
                layout.ColumnStyles.Clear();layout.RowStyles.Clear();
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent,45F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent,55F));
                layout.SetCellPosition(chart,new TableLayoutPanelCellPosition(0,0));
                layout.SetCellPosition(grid,new TableLayoutPanelCellPosition(0,1));
                chart.Margin=new Padding(0,0,0,6);
                grid.Margin=new Padding(0,6,0,0);
            }else if(!narrow && layout.ColumnCount==1){
                layout.ColumnCount=2;layout.RowCount=1;
                layout.ColumnStyles.Clear();layout.RowStyles.Clear();
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,42F));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,58F));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent,100F));
                layout.SetCellPosition(chart,new TableLayoutPanelCellPosition(0,0));
                layout.SetCellPosition(grid,new TableLayoutPanelCellPosition(1,0));
                chart.Margin=new Padding(0,0,6,0);
                grid.Margin=new Padding(6,0,0,0);
            }
        };
        Controls.Add(layout);Controls.Add(cards);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
