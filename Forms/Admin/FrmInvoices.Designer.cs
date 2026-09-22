#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmInvoices
{
    private System.ComponentModel.IContainer? components=null;private Panel top=null!;private TextBox txtSearch=null!;private ComboBox cboStatus=null!;private Button btnRefresh=null!,btnPay=null!,btnConfirmTransfer=null!;private DataGridView grid=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){top=new Panel();txtSearch=new TextBox();cboStatus=new ComboBox();btnRefresh=new Button();btnPay=new Button();btnConfirmTransfer=new Button();grid=new DataGridView();SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=92;top.BackColor=Color.White;
        var pageTitle=new Label{Text="Hóa đơn",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(16,10)};
        var pageSub=new Label{Text="Theo dõi thanh toán, xác nhận chuyển khoản và xuất hóa đơn",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,38)};
        txtSearch.PlaceholderText="Tìm hóa đơn / mã đơn / khách hàng...";txtSearch.Location=new Point(16,48);txtSearch.Width=260;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        cboStatus.Location=new Point(0,48);cboStatus.Width=170;cboStatus.DropDownStyle=ComboBoxStyle.DropDownList;cboStatus.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(104,38);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnPay.Text="⚡  Thanh toán";btnPay.Size=new Size(126,38);btnPay.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnConfirmTransfer.Text="✓  Xác nhận chuyển khoản";btnConfirmTransfer.Size=new Size(196,38);btnConfirmTransfer.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,btnRefresh,btnPay,btnConfirmTransfer,cboStatus,txtSearch});
        top.Resize+=(_,__)=>{
            btnConfirmTransfer.Left=top.Width-btnConfirmTransfer.Width-16;
            btnPay.Left=btnConfirmTransfer.Left-btnPay.Width-8;
            btnRefresh.Left=btnPay.Left-btnRefresh.Width-8;
            cboStatus.Left=btnRefresh.Left-cboStatus.Width-14;
            txtSearch.Width=Math.Max(200,cboStatus.Left-txtSearch.Left-14);
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
