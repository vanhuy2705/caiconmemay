#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmBooking
{
    private System.ComponentModel.IContainer? components=null;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel card=null!;
    private ComboBox cboCustomer=null!,cboType=null!,cboField=null!,cboVoucher=null!;
    private DateTimePicker dtDate=null!,dtStart=null!,dtEnd=null!;private TextBox txtNote=null!;
    private Label lblPrice=null!,lblSubtotal=null!,lblDiscount=null!,lblTotal=null!,lblAvailability=null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnCheck=null!,btnBook=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent()
    {
        card=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel();cboCustomer=new ComboBox();cboType=new ComboBox();cboField=new ComboBox();cboVoucher=new ComboBox();dtDate=new DateTimePicker();dtStart=new DateTimePicker();dtEnd=new DateTimePicker();txtNote=new TextBox();lblPrice=new Label();lblSubtotal=new Label();lblDiscount=new Label();lblTotal=new Label();lblAvailability=new Label();btnCheck=new QuanLyThueSanTheThao.Forms.Common.RoundedButton();btnBook=new QuanLyThueSanTheThao.Forms.Common.RoundedButton();SuspendLayout();
        BackColor=Color.FromArgb(239,248,252);Padding=new Padding(10);AutoScaleMode=AutoScaleMode.Dpi;AutoScroll=true;
        card.BackColor=Color.White;card.Dock=DockStyle.Top;card.Height=640;card.Padding=new Padding(26);card.Radius=16;card.BorderColor=Color.FromArgb(218,232,240);Controls.Add(card);
        var badge=new QuanLyThueSanTheThao.Forms.Common.IconBadge{IconImage=QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Booking,20,Color.White),Size=new Size(42,42),AccentColor=Color.FromArgb(19,170,157),Location=new Point(28,20)};
        var title=new Label{Text="Đặt sân thể thao",Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),AutoSize=true,Location=new Point(82,22)};
        var sub=new Label{Text="Chọn sân và khung giờ — hệ thống tự kiểm tra trùng lịch và tính tiền",Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),AutoSize=true,Location=new Point(84,50)};
        var sep=new Panel{BackColor=Color.FromArgb(232,239,243),Location=new Point(28,84),Height=1,Width=870,Anchor=AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right};
        card.Controls.AddRange(new Control[]{badge,title,sub,sep});
        var left=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{BackColor=Color.FromArgb(250,253,254),BorderColor=Color.FromArgb(229,237,242),Radius=13,Location=new Point(28,104),Size=new Size(410,500),Padding=new Padding(18)};
        var right=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{BackColor=Color.FromArgb(250,253,254),BorderColor=Color.FromArgb(229,237,242),Radius=13,Location=new Point(456,104),Size=new Size(442,505),Padding=new Padding(18)};
        card.Controls.AddRange(new Control[]{left,right});
        var ltitle=new Label{Text="THÔNG TIN ĐẶT SÂN",AutoSize=true,Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold),ForeColor=Color.FromArgb(52,84,105),Location=new Point(18,14)};left.Controls.Add(ltitle);
        var rtitle=new Label{Text="THỜI GIAN & THANH TOÁN",AutoSize=true,Font=new Font("Segoe UI Semibold",9F,FontStyle.Bold),ForeColor=Color.FromArgb(52,84,105),Location=new Point(18,14)};right.Controls.Add(rtitle);
        var y=48;void AddL(string label,Control c){var lb=new Label{Text=label,AutoSize=true,Font=new Font("Segoe UI Semibold",8.3F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(18,y)};c.Location=new Point(18,y+21);c.Width=360;c.Height=34;left.Controls.AddRange(new Control[]{lb,c});y+=64;}
        AddL("Khách hàng",cboCustomer);AddL("Loại sân",cboType);AddL("Sân",cboField);AddL("Voucher",cboVoucher);AddL("Ghi chú",txtNote);txtNote.Multiline=true;txtNote.Height=52;
        var y2=48;void AddR(string label,Control c){var lb=new Label{Text=label,AutoSize=true,Font=new Font("Segoe UI Semibold",8.3F),ForeColor=Color.FromArgb(39,67,88),Location=new Point(18,y2)};c.Location=new Point(18,y2+21);c.Width=200;c.Height=34;right.Controls.AddRange(new Control[]{lb,c});y2+=56;}
        AddR("Ngày đặt",dtDate);AddR("Giờ bắt đầu",dtStart);AddR("Giờ kết thúc",dtEnd);
        btnCheck.Text="⟳  Kiểm tra sân trống";btnCheck.Location=new Point(236,110);btnCheck.Size=new Size(170,55);btnCheck.BackColor=Color.FromArgb(236,248,246);btnCheck.ForeColor=Color.FromArgb(0,135,100);btnCheck.Radius=10;btnCheck.HoverColor=Color.FromArgb(220,242,236);btnCheck.Font=new Font("Segoe UI Semibold",8.5F,FontStyle.Bold);right.Controls.Add(btnCheck);
        lblAvailability.AutoSize=false;lblAvailability.Size=new Size(390,30);lblAvailability.Location=new Point(18,232);lblAvailability.ForeColor=Color.FromArgb(0,145,95);lblAvailability.Font=new Font("Segoe UI Semibold",8.5F);lblAvailability.TextAlign=ContentAlignment.MiddleLeft;right.Controls.Add(lblAvailability);
        var summary=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{BackColor=Color.FromArgb(243,253,248),BorderColor=Color.FromArgb(183,228,208),Radius=12,Location=new Point(18,270),Size=new Size(390,124)};
        lblPrice.Location=new Point(16,12);lblPrice.AutoSize=true;lblPrice.ForeColor=Color.FromArgb(91,113,130);lblPrice.Font=new Font("Segoe UI",8.4F);
        lblSubtotal.Location=new Point(16,34);lblSubtotal.AutoSize=true;lblSubtotal.ForeColor=Color.FromArgb(52,84,105);lblSubtotal.Font=new Font("Segoe UI Semibold",8.8F);
        lblDiscount.Location=new Point(16,58);lblDiscount.AutoSize=true;lblDiscount.ForeColor=Color.FromArgb(214,106,22);lblDiscount.Font=new Font("Segoe UI Semibold",8.8F);
        lblTotal.Location=new Point(16,84);lblTotal.AutoSize=true;lblTotal.Font=new Font("Segoe UI Semibold",14.5F,FontStyle.Bold);lblTotal.ForeColor=Color.FromArgb(0,150,100);
        summary.Controls.AddRange(new Control[]{lblPrice,lblSubtotal,lblDiscount,lblTotal});
        right.Controls.Add(summary);
        btnBook.Text="✓  XÁC NHẬN ĐẶT SÂN";btnBook.Location=new Point(18,406);btnBook.Size=new Size(390,46);btnBook.BackColor=Color.FromArgb(19,198,119);btnBook.ForeColor=Color.White;btnBook.Radius=10;btnBook.HoverColor=Color.FromArgb(8,158,89);btnBook.Font=new Font("Segoe UI Semibold",9.5F,FontStyle.Bold);right.Controls.Add(btnBook);
        var note=new Label{Text="Lịch được xác nhận ngay sau khi tạo; trạng thái thanh toán được theo dõi riêng.",AutoSize=true,Font=new Font("Segoe UI",7.8F),ForeColor=Color.FromArgb(150,163,175),Location=new Point(18,462)};right.Controls.Add(note);
        card.Resize+=(_,__)=>{sep.Width=card.Width-56;right.Width=Math.Max(400,card.Width-right.Left-28);};
        ResumeLayout(false);
    }
}
