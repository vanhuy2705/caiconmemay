#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmPromotions
{
    private System.ComponentModel.IContainer? components=null;private SplitContainer split=null!;private DataGridView grid=null!;private TextBox txtCode=null!,txtName=null!,txtDesc=null!,txtPercent=null!;private DateTimePicker dtStart=null!,dtEnd=null!;private CheckBox chkActive=null!;private Button btnNew=null!,btnSave=null!,btnDelete=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){split=new SplitContainer();grid=new DataGridView();txtCode=new TextBox();txtName=new TextBox();txtDesc=new TextBox();txtPercent=new TextBox();dtStart=new DateTimePicker();dtEnd=new DateTimePicker();chkActive=new CheckBox();btnNew=new Button();btnSave=new Button();btnDelete=new Button();SuspendLayout();
        split.Dock=DockStyle.Fill;split.Size=new Size(1180,720);split.FixedPanel=FixedPanel.Panel2;split.SplitterDistance=830;split.SplitterWidth=1;split.Panel1.Padding=new Padding(12,10,10,14);split.Panel2.Padding=new Padding(24,20,24,20);split.Panel2.BackColor=Color.White;split.Panel2.AutoScroll=true;
        var top=new Panel{Dock=DockStyle.Top,Height=64,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Khuyến mãi",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Chương trình giảm giá áp dụng tự động theo thời gian",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        top.Controls.AddRange(new Control[]{pageTitle,pageSub});
        grid.Dock=DockStyle.Fill;split.Panel1.Controls.Add(grid);split.Panel1.Controls.Add(top);
        var badge=new Label{Text="✦",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(24,24)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thông tin khuyến mãi",AutoSize=true,Font=new Font("Segoe UI Semibold",13.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(76,26)};
        var sub=new Label{Text="Chọn chương trình trong danh sách để chỉnh sửa",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(77,52)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(24,84),Size=new Size(308,1)};
        split.Panel2.Controls.AddRange(new Control[]{badge,title,sub,sep});
        int y=108;void Add(string l,Control c){var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(24,y)};c.Location=new Point(24,y+21);c.Width=308;c.Height=38;y+=64;split.Panel2.Controls.AddRange(new Control[]{lb,c});}
        Add("Mã khuyến mãi",txtCode);Add("Tên chương trình",txtName);Add("Mô tả",txtDesc);Add("Giảm (%)",txtPercent);Add("Bắt đầu",dtStart);Add("Kết thúc",dtEnd);
        chkActive.Text="  Đang hoạt động";chkActive.AutoSize=true;chkActive.Font=new Font("Segoe UI",9F);chkActive.ForeColor=Color.FromArgb(52,84,105);chkActive.Location=new Point(24,y+4);y+=40;
        btnSave.Text="✓  Lưu";btnNew.Text="＋ Mới";btnDelete.Text="✕  Xóa";btnSave.Size=new Size(120,42);btnNew.Size=new Size(94,42);btnDelete.Size=new Size(94,42);
        btnSave.Location=new Point(24,y);btnNew.Location=new Point(152,y);btnDelete.Location=new Point(254,y);
        split.Panel2.Controls.AddRange(new Control[]{chkActive,btnSave,btnNew,btnDelete});
        Controls.Add(split);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);}
}
