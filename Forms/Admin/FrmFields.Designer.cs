#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmFields
{
    private System.ComponentModel.IContainer? components=null;private SplitContainer split=null!;private DataGridView grid=null!;private TextBox txtSearch=null!,txtCode=null!,txtName=null!,txtDesc=null!,txtLocation=null!,txtPrice=null!;private ComboBox cboType=null!,cboStatus=null!;private CheckBox chkActive=null!;private Button btnRefresh=null!,btnNew=null!,btnSave=null!,btnDelete=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        split=new SplitContainer();grid=new DataGridView();txtSearch=new TextBox();txtCode=new TextBox();txtName=new TextBox();txtDesc=new TextBox();txtLocation=new TextBox();txtPrice=new TextBox();cboType=new ComboBox();cboStatus=new ComboBox();chkActive=new CheckBox();btnRefresh=new Button();btnNew=new Button();btnSave=new Button();btnDelete=new Button();
        SuspendLayout();
        split.Dock=DockStyle.Fill;split.FixedPanel=FixedPanel.Panel2;split.SplitterDistance=780;split.SplitterWidth=6;split.Panel1MinSize=400;split.Panel2MinSize=320;
        split.Panel1.Padding=new Padding(12,10,10,12);split.Panel2.Padding=new Padding(20);split.Panel2.BackColor=Color.White;split.Panel2.AutoScroll=true;
        var top=new Panel{Dock=DockStyle.Top,Height=72,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Quản lý sân",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Danh sách sân, giá giờ và trạng thái",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        txtSearch.PlaceholderText="Tìm mã, tên sân...";txtSearch.Location=new Point(6,8);txtSearch.Width=260;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnRefresh=new Button{Text="↻  Làm mới",Size=new Size(104,36),Anchor=AnchorStyles.Top|AnchorStyles.Right};
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,txtSearch,btnRefresh});
        top.Resize+=(s,e)=>{btnRefresh.Left=top.Width-btnRefresh.Width-4;txtSearch.Left=Math.Max(200,btnRefresh.Left-txtSearch.Width-10);txtSearch.Top=btnRefresh.Top=18;};
        grid.Dock=DockStyle.Fill;split.Panel1.Controls.Add(grid);split.Panel1.Controls.Add(top);
        var badge=new Label{Text="⚽",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(20,20)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thông tin sân",AutoSize=true,Font=new Font("Segoe UI Semibold",13F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(72,22)};
        var sub=new Label{Text="Chọn sân trong danh sách để chỉnh sửa",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(73,48)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(20,78),Height=1,Width=320,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        split.Panel2.Controls.AddRange(new Control[]{badge,title,sub,sep});
        int y=96;
        void Add(string l,Control c){
            var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(20,y)};
            c.Location=new Point(20,y+20);c.Width=split.Panel2.ClientSize.Width-40;c.Height=36;c.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
            split.Panel2.Controls.AddRange(new Control[]{lb,c});y+=66;
        }
        Add("Mã sân *",txtCode);Add("Tên sân *",txtName);Add("Loại sân *",cboType);Add("Vị trí",txtLocation);Add("Giá / giờ *",txtPrice);Add("Trạng thái",cboStatus);
        var lbDesc=new Label{Text="Mô tả",AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(20,y)};
        txtDesc.Location=new Point(20,y+20);txtDesc.Width=split.Panel2.ClientSize.Width-40;txtDesc.Height=60;txtDesc.Multiline=true;txtDesc.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        split.Panel2.Controls.AddRange(new Control[]{lbDesc,txtDesc});y+=88;
        chkActive.Text="  Đang hoạt động";chkActive.AutoSize=true;chkActive.Font=new Font("Segoe UI",9F);chkActive.ForeColor=Color.FromArgb(52,84,105);chkActive.Location=new Point(20,y+4);
        split.Panel2.Controls.Add(chkActive);y+=36;
        var pnl=new Panel{Location=new Point(20,y),Size=new Size(320,48),BackColor=Color.Transparent,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        btnSave.Text="✓  Lưu";btnSave.Size=new Size(110,40);btnSave.Location=new Point(0,4);
        btnNew.Text="＋ Mới";btnNew.Size=new Size(80,40);btnNew.Location=new Point(118,4);
        btnDelete.Text="✕  Xóa";btnDelete.Size=new Size(80,40);btnDelete.Location=new Point(206,4);
        pnl.Controls.AddRange(new Control[]{btnSave,btnNew,btnDelete});
        split.Panel2.Controls.Add(pnl);
        split.Panel2.Resize+=(s,e)=>{
            int w=Math.Max(220,split.Panel2.ClientSize.Width-40);
            sep.Width=w;
            foreach(Control c in split.Panel2.Controls){if(c is TextBox || c is ComboBox || c is DateTimePicker) c.Width=w; if(c is Panel p && p.Controls.OfType<Button>().Any()) p.Width=w;}
        };
        Controls.Add(split);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
