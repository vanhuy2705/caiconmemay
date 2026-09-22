#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmInvoices
{
    private System.ComponentModel.IContainer? components=null;private Panel top=null!;private TextBox txtSearch=null!;private ComboBox cboStatus=null!;private Button btnRefresh=null!,btnPay=null!,btnConfirmTransfer=null!;private DataGridView grid=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        top=new Panel();txtSearch=new TextBox();cboStatus=new ComboBox();btnRefresh=new Button();btnPay=new Button();btnConfirmTransfer=new Button();grid=new DataGridView();
        SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=96;top.BackColor=Color.White;top.Padding=new Padding(12,8,12,8);
        var pageTitle=new Label{Text="Hóa đơn",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(12,8)};
        var pageSub=new Label{Text="Theo dõi thanh toán, xác nhận chuyển khoản",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(14,34)};
        txtSearch.PlaceholderText="Tìm hóa đơn / mã đơn / khách...";txtSearch.Location=new Point(12,58);txtSearch.Width=240;txtSearch.Height=32;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        cboStatus.Width=160;cboStatus.Height=32;cboStatus.DropDownStyle=ComboBoxStyle.DropDownList;cboStatus.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnRefresh.Text="↻  Làm mới";btnRefresh.Size=new Size(96,36);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnPay.Text="⚡  Thanh toán";btnPay.Size=new Size(116,36);btnPay.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnConfirmTransfer.Text="✓  Xác nhận CK";btnConfirmTransfer.Size=new Size(140,36);btnConfirmTransfer.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,btnRefresh,btnPay,btnConfirmTransfer,cboStatus,txtSearch});
        top.Resize+=(s,e)=>{
            btnConfirmTransfer.Left=top.Width-btnConfirmTransfer.Width-12;
            btnPay.Left=btnConfirmTransfer.Left-btnPay.Width-8;
            btnRefresh.Left=btnPay.Left-btnRefresh.Width-8;
            cboStatus.Left=btnRefresh.Left-cboStatus.Width-12;
            txtSearch.Width=Math.Max(180,cboStatus.Left-txtSearch.Left-16);
            txtSearch.Top=cboStatus.Top=btnRefresh.Top=58;
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
