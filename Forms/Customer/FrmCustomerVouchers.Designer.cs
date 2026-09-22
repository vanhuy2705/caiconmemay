#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerVouchers
{
    private System.ComponentModel.IContainer? components=null;private FlowLayoutPanel flow=null!;private Label title=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        flow=new FlowLayoutPanel();title=new Label();
        SuspendLayout();
        var top=new Panel{Dock=DockStyle.Top,Height=72,BackColor=Color.White,Padding=new Padding(16,10,16,8)};
        title.Text="Voucher của bạn";title.AutoSize=true;title.Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold);title.ForeColor=Color.FromArgb(18,53,78);title.Location=new Point(16,10);
        var sub=new Label{Text="Dùng voucher khi đặt sân để được giảm giá trực tiếp",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,38)};
        top.Controls.AddRange(new Control[]{title,sub});
        flow.Dock=DockStyle.Fill;flow.AutoScroll=true;flow.Padding=new Padding(12,8,12,8);flow.WrapContents=true;flow.BackColor=Color.Transparent;
        flow.Resize+=(s,e)=>{
            foreach(Control c in flow.Controls){
                if(c.Width!=flow.ClientSize.Width-32 && flow.ClientSize.Width>400)
                    c.Width=Math.Min(420,flow.ClientSize.Width-32);
            }
        };
        Controls.Add(flow);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
