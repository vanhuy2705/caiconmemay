#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerVouchers
{
    private System.ComponentModel.IContainer? components=null;private FlowLayoutPanel flow=null!;private Label title=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){flow=new FlowLayoutPanel();title=new Label();SuspendLayout();title.Text="Voucher của bạn";title.Dock=DockStyle.Top;title.Height=66;title.Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold);title.ForeColor=Color.FromArgb(18,53,78);title.TextAlign=ContentAlignment.MiddleLeft;
        var sub=new Label{Text="Dùng voucher khi đặt sân để được giảm giá trực tiếp",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,40)};
        Controls.Add(sub);flow.Dock=DockStyle.Fill;flow.AutoScroll=true;flow.Padding=new Padding(0,8,0,0);Controls.Add(flow);Controls.Add(title);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);}
}
