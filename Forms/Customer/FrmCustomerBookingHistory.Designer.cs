#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerBookingHistory
{
    private System.ComponentModel.IContainer? components=null;private Panel top=null!;private ComboBox cboStatus=null!;private Button btnRefresh=null!,btnPay=null!,btnCancel=null!;private DataGridView grid=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){top=new Panel();cboStatus=new ComboBox();btnRefresh=new Button();btnPay=new Button();btnCancel=new Button();grid=new DataGridView();SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=92;top.BackColor=Color.White;
        var pageTitle=new Label{Text="Lịch sử đặt sân",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(16,10)};
        var pageSub=new Label{Text="Theo dõi trạng thái, thanh toán hoặc hủy các lượt đặt của bạn",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,38)};
        cboStatus.Location=new Point(16,48);cboStatus.Width=180;cboStatus.DropDownStyle=ComboBoxStyle.DropDownList;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(104,38);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnPay.Text="⚡  Thanh toán";btnPay.Size=new Size(126,38);btnPay.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnCancel.Text="✕  Hủy đặt sân";btnCancel.Size=new Size(122,38);btnCancel.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,cboStatus,btnRefresh,btnPay,btnCancel});
        top.Resize+=(_,__)=>{
            btnCancel.Left=top.Width-btnCancel.Width-16;
            btnPay.Left=btnCancel.Left-btnPay.Width-8;
            btnRefresh.Left=btnPay.Left-btnRefresh.Width-8;
            cboStatus.Width=Math.Max(140,btnRefresh.Left-cboStatus.Left-14);
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(239,248,253);ResumeLayout(false);}
}
