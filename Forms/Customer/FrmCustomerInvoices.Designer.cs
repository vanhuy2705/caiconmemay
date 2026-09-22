#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerInvoices
{
    private System.ComponentModel.IContainer? components=null;private DataGridView grid=null!;private Panel top=null!;private Button btnRefresh=null!,btnPay=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        grid=new DataGridView();top=new Panel();btnRefresh=new Button();btnPay=new Button();
        SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=88;top.BackColor=Color.White;top.Padding=new Padding(12,8,12,8);
        var pageTitle=new Label{Text="Hóa đơn của tôi",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(12,8)};
        var pageSub=new Label{Text="Lịch sử thanh toán các lượt đặt sân của bạn",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(14,34)};
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(96,36);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnPay.Text="⚡  Thanh toán";btnPay.Size=new Size(116,36);btnPay.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,btnRefresh,btnPay});
        top.Resize+=(s,e)=>{
            btnPay.Left=top.Width-btnPay.Width-12;
            btnRefresh.Left=btnPay.Left-btnRefresh.Width-8;
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
