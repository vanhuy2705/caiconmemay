#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmAccounts
{
    private System.ComponentModel.IContainer? components=null;private Panel top=null!;private DataGridView grid=null!;private TextBox txtSearch=null!;private Button btnRefresh=null!,btnToggle=null!,btnReset=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){top=new Panel();grid=new DataGridView();txtSearch=new TextBox();btnRefresh=new Button();btnToggle=new Button();btnReset=new Button();SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=92;top.BackColor=Color.White;
        var pageTitle=new Label{Text="Tài khoản",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(16,10)};
        var pageSub=new Label{Text="Quản lý đăng nhập, phân quyền và khóa tài khoản",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,38)};
        txtSearch.PlaceholderText="Tìm tài khoản / họ tên...";txtSearch.Location=new Point(16,48);txtSearch.Width=260;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(104,38);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnToggle.Text="⏻  Khóa / Mở";btnToggle.Size=new Size(118,38);btnToggle.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnReset.Text="🔑  Đặt lại mật khẩu";btnReset.Size=new Size(170,38);btnReset.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,btnRefresh,btnToggle,btnReset,txtSearch});
        top.Resize+=(_,__)=>{
            btnReset.Left=top.Width-btnReset.Width-16;
            btnToggle.Left=btnReset.Left-btnToggle.Width-8;
            btnRefresh.Left=btnToggle.Left-btnRefresh.Width-8;
            txtSearch.Width=Math.Max(200,btnRefresh.Left-txtSearch.Left-14);
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);}
}
