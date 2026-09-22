#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmVouchers
{
    private System.ComponentModel.IContainer? components=null;private SplitContainer split=null!;private DataGridView grid=null!;private TextBox txtSearch=null!,txtCode=null!,txtName=null!,txtValue=null!,txtMax=null!,txtMin=null!,txtQty=null!;private ComboBox cboType=null!;private DateTimePicker dtStart=null!,dtEnd=null!;private CheckBox chkActive=null!;private Button btnRefresh=null!,btnNew=null!,btnSave=null!,btnDelete=null!,btnAssign=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){split=new SplitContainer();grid=new DataGridView();txtSearch=new TextBox();txtCode=new TextBox();txtName=new TextBox();txtValue=new TextBox();txtMax=new TextBox();txtMin=new TextBox();txtQty=new TextBox();cboType=new ComboBox();dtStart=new DateTimePicker();dtEnd=new DateTimePicker();chkActive=new CheckBox();btnRefresh=new Button();btnNew=new Button();btnSave=new Button();btnDelete=new Button();btnAssign=new Button();SuspendLayout();
        split.Dock=DockStyle.Fill;split.Size=new Size(1200,760);split.FixedPanel=FixedPanel.Panel2;split.SplitterDistance=810;split.SplitterWidth=1;split.Panel1.Padding=new Padding(12,10,10,14);split.Panel2.Padding=new Padding(22,18,22,18);split.Panel2.BackColor=Color.White;split.Panel2.AutoScroll=true;
        var top=new Panel{Dock=DockStyle.Top,Height=64,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Voucher",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Mã giảm giá dành cho khách hàng thân thiết",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        txtSearch.PlaceholderText="Tìm voucher...";txtSearch.Location=new Point(0,14);txtSearch.Width=250;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(104,38);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnAssign.Text="🎁  Cấp voucher";btnAssign.Size=new Size(126,38);btnAssign.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,txtSearch,btnRefresh,btnAssign});
        top.Resize+=(_,__)=>{btnAssign.Left=top.Width-btnAssign.Width-4;btnRefresh.Left=btnAssign.Left-btnRefresh.Width-8;txtSearch.Left=btnRefresh.Left-txtSearch.Width-10;};
        grid.Dock=DockStyle.Fill;split.Panel1.Controls.Add(grid);split.Panel1.Controls.Add(top);
        var badge=new Label{Text="▱",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(22,20)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thông tin voucher",AutoSize=true,Font=new Font("Segoe UI Semibold",13.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(74,22)};
        var sub=new Label{Text="Chọn voucher trong danh sách để chỉnh sửa",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(75,48)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(22,78),Size=new Size(312,1)};
        split.Panel2.Controls.AddRange(new Control[]{badge,title,sub,sep});
        int y=98;void Add(string l,Control c){var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.4F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(22,y)};c.Location=new Point(22,y+20);c.Width=312;c.Height=36;y+=60;split.Panel2.Controls.AddRange(new Control[]{lb,c});}
        Add("Mã voucher",txtCode);Add("Tên voucher",txtName);Add("Loại giảm",cboType);Add("Giá trị giảm",txtValue);Add("Giảm tối đa",txtMax);Add("Đơn tối thiểu",txtMin);Add("Số lượng",txtQty);Add("Bắt đầu",dtStart);Add("Kết thúc",dtEnd);dtStart.Format=dtEnd.Format=DateTimePickerFormat.Custom;dtStart.CustomFormat=dtEnd.CustomFormat="dd/MM/yyyy HH:mm";
        chkActive.Text="  Đang hoạt động";chkActive.AutoSize=true;chkActive.Font=new Font("Segoe UI",9F);chkActive.ForeColor=Color.FromArgb(52,84,105);chkActive.Location=new Point(22,y+2);y+=34;
        btnSave.Text="✓  Lưu";btnNew.Text="＋ Mới";btnDelete.Text="✕  Xóa";btnSave.Size=new Size(116,40);btnNew.Size=new Size(92,40);btnDelete.Size=new Size(92,40);
        btnSave.Location=new Point(22,y);btnNew.Location=new Point(146,y);btnDelete.Location=new Point(246,y);
        split.Panel2.Controls.AddRange(new Control[]{chkActive,btnSave,btnNew,btnDelete});
        Controls.Add(split);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);}
}
