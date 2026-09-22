#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmCustomers
{
    private System.ComponentModel.IContainer? components=null;private SplitContainer split=null!;private DataGridView grid=null!;private TextBox txtSearch=null!,txtCode=null!,txtName=null!,txtPhone=null!,txtEmail=null!,txtAddress=null!;private DateTimePicker dtBirth=null!;private CheckBox chkActive=null!;private Button btnRefresh=null!,btnNew=null!,btnSave=null!,btnDelete=null!,btnAccount=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){split=new SplitContainer();grid=new DataGridView();txtSearch=new TextBox();txtCode=new TextBox();txtName=new TextBox();txtPhone=new TextBox();txtEmail=new TextBox();txtAddress=new TextBox();dtBirth=new DateTimePicker();chkActive=new CheckBox();btnRefresh=new Button();btnNew=new Button();btnSave=new Button();btnDelete=new Button();btnAccount=new Button();SuspendLayout();split.Dock=DockStyle.Fill;split.Size=new Size(1180,720);split.FixedPanel=FixedPanel.Panel2;split.SplitterDistance=830;split.SplitterWidth=1;split.Panel1.Padding=new Padding(12,10,10,14);split.Panel1.BackColor=Color.Transparent;split.Panel2.Padding=new Padding(24,20,24,20);split.Panel2.BackColor=Color.White;split.Panel2.AutoScroll=true;

        // ===== Cột trái: tiêu đề + thanh công cụ + lưới =====
        var top=new Panel{Dock=DockStyle.Top,Height=64,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Khách hàng",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Quản lý hồ sơ, điểm tích lũy và liên hệ của khách",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        txtSearch.PlaceholderText="Tìm theo mã, tên hoặc SĐT...";txtSearch.Location=new Point(0,10);txtSearch.Width=280;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(104,38);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,txtSearch,btnRefresh});
        top.Resize+=(_,__)=>{btnRefresh.Left=top.Width-btnRefresh.Width-4;txtSearch.Left=btnRefresh.Left-txtSearch.Width-10;};
        grid.Dock=DockStyle.Fill;
        split.Panel1.Controls.Add(grid);split.Panel1.Controls.Add(top);

        // ===== Cột phải: thẻ nhập liệu =====
        var badge=new Label{Text="✎",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(24,24)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thông tin khách hàng",AutoSize=true,Font=new Font("Segoe UI Semibold",13.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(76,26)};
        var sub=new Label{Text="Chọn khách trong danh sách để chỉnh sửa",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(77,52)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(24,84),Size=new Size(308,1)};
        split.Panel2.Controls.AddRange(new Control[]{badge,title,sub,sep});

        int y=108;void Add(string label,Control c){var l=new Label{Text=label,AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(24,y)};c.Location=new Point(24,y+21);c.Width=308;c.Height=38;y+=70;split.Panel2.Controls.AddRange(new Control[]{l,c});}
        Add("Mã khách",txtCode);Add("Họ tên",txtName);Add("Số điện thoại",txtPhone);Add("Email",txtEmail);Add("Ngày sinh",dtBirth);dtBirth.Format=DateTimePickerFormat.Short;Add("Địa chỉ",txtAddress);
        chkActive.Text="  Đang hoạt động";chkActive.AutoSize=true;chkActive.Font=new Font("Segoe UI",9F);chkActive.ForeColor=Color.FromArgb(52,84,105);chkActive.Location=new Point(24,y+2);y+=40;
        var spacer=new Panel{Height=1,Width=1,Location=new Point(0,y)};split.Panel2.Controls.Add(spacer);
        btnNew.Text="＋ Mới";btnSave.Text="✓  Lưu";btnDelete.Text="✕  Xóa";btnNew.Size=new Size(94,42);btnSave.Size=new Size(120,42);btnDelete.Size=new Size(94,42);
        btnSave.Location=new Point(24,y);btnNew.Location=new Point(152,y);btnDelete.Location=new Point(254,y);
        btnAccount.Text="Tạo tài khoản đăng nhập";btnAccount.Location=new Point(24,y+50);btnAccount.Size=new Size(324,40);
        split.Panel2.Controls.AddRange(new Control[]{chkActive,btnSave,btnNew,btnDelete,btnAccount});
        Controls.Add(split);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
