#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmCustomers
{
    private System.ComponentModel.IContainer? components=null;
    private SplitContainer split=null!;
    private DataGridView grid=null!;
    private TextBox txtSearch=null!,txtCode=null!,txtName=null!,txtPhone=null!,txtEmail=null!,txtAddress=null!;
    private DateTimePicker dtBirth=null!;
    private CheckBox chkActive=null!;
    private Button btnRefresh=null!,btnNew=null!,btnSave=null!,btnDelete=null!,btnAccount=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        split=new SplitContainer();grid=new DataGridView();txtSearch=new TextBox();txtCode=new TextBox();txtName=new TextBox();txtPhone=new TextBox();txtEmail=new TextBox();txtAddress=new TextBox();dtBirth=new DateTimePicker();chkActive=new CheckBox();btnRefresh=new Button();btnNew=new Button();btnSave=new Button();btnDelete=new Button();btnAccount=new Button();
        SuspendLayout();
        split.Dock=DockStyle.Fill;
        split.FixedPanel=FixedPanel.Panel2;
        split.SplitterDistance=780;
        split.SplitterWidth=6;
        split.Panel1MinSize=400;
        split.Panel2MinSize=320;
        split.Panel1.Padding=new Padding(12,10,10,12);
        split.Panel1.BackColor=Color.Transparent;
        split.Panel2.Padding=new Padding(20);
        split.Panel2.BackColor=Color.White;
        split.Panel2.AutoScroll=true;

        // Panel1 - Header + Grid
        var top=new Panel{Dock=DockStyle.Top,Height=72,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Khách hàng",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Quản lý hồ sơ, điểm tích lũy và liên hệ",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        txtSearch.PlaceholderText="Tìm mã, tên, SĐT...";txtSearch.Location=new Point(6,8);txtSearch.Width=260;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(104,36);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,txtSearch,btnRefresh});
        top.Resize+=(s,e)=>{
            btnRefresh.Left=top.Width-btnRefresh.Width-4;
            txtSearch.Left=Math.Max(200,btnRefresh.Left-txtSearch.Width-10);
            txtSearch.Top=btnRefresh.Top=18;
        };
        grid.Dock=DockStyle.Fill;
        grid.Margin=new Padding(0,4,0,0);
        split.Panel1.Controls.Add(grid);
        split.Panel1.Controls.Add(top);

        // Panel2 - Form nhập liệu responsive
        var badge=new Label{Text="✎",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(20,20)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thông tin khách hàng",AutoSize=true,Font=new Font("Segoe UI Semibold",13F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(72,22)};
        var sub=new Label{Text="Chọn khách trong danh sách để chỉnh sửa",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(73,48)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(20,78),Height=1,Width=320,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        split.Panel2.Controls.AddRange(new Control[]{badge,title,sub,sep});

        int y=96;
        void AddField(string label, Control c)
        {
            var lb=new Label{Text=label,AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(20,y)};
            c.Location=new Point(20,y+20);c.Width=split.Panel2.ClientSize.Width-40;c.Height=36;c.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
            split.Panel2.Controls.AddRange(new Control[]{lb,c});
            y+=66;
        }
        AddField("Mã khách *",txtCode);
        AddField("Họ tên *",txtName);
        AddField("Số điện thoại",txtPhone);
        AddField("Email",txtEmail);
        dtBirth.Format=DateTimePickerFormat.Short;
        AddField("Ngày sinh",dtBirth);
        AddField("Địa chỉ",txtAddress);
        txtAddress.Height=56;txtAddress.Multiline=true;

        chkActive.Text="  Đang hoạt động";chkActive.AutoSize=true;chkActive.Font=new Font("Segoe UI",9F);chkActive.ForeColor=Color.FromArgb(52,84,105);chkActive.Location=new Point(20,y+4);
        split.Panel2.Controls.Add(chkActive);
        y+=36;

        var pnlButtons=new Panel{Location=new Point(20,y),Size=new Size(320,48),BackColor=Color.Transparent,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        btnSave.Text="✓  Lưu";btnSave.Size=new Size(100,40);btnSave.Location=new Point(0,4);
        btnNew.Text="＋ Mới";btnNew.Size=new Size(80,40);btnNew.Location=new Point(108,4);
        btnDelete.Text="✕  Xóa";btnDelete.Size=new Size(80,40);btnDelete.Location=new Point(196,4);
        pnlButtons.Controls.AddRange(new Control[]{btnSave,btnNew,btnDelete});
        split.Panel2.Controls.Add(pnlButtons);
        y+=56;

        btnAccount.Text="Tạo tài khoản đăng nhập";btnAccount.Location=new Point(20,y);btnAccount.Size=new Size(320,40);btnAccount.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        split.Panel2.Controls.Add(btnAccount);

        // Responsive resize for Panel2 inputs
        split.Panel2.Resize+=(s,e)=>{
            int w=Math.Max(220,split.Panel2.ClientSize.Width-40);
            sep.Width=w;
            foreach(Control c in split.Panel2.Controls)
            {
                if(c is TextBox || c is ComboBox || c is DateTimePicker)
                    c.Width=w;
                if(c is Panel p && p.Controls.OfType<Button>().Any())
                    p.Width=w;
            }
            btnAccount.Width=w;
        };

        Controls.Add(split);
        BackColor=Color.FromArgb(243,249,248);
        ResumeLayout(false);
    }
}
