using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Admin;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerHome:Form
{
    public FrmCustomerHome()
    {
        InitializeComponent();AppTheme.Upgrade(this);
        lblWelcome.Text=$"Xin chào, {SessionContext.FullName}!";
        hero.Cursor=Cursors.Hand;
        hero.Click+=(_,__)=>OpenBooking();
        BuildCategories();
        Shown+=(_,__)=>LoadData();
    }

    private void BuildCategories()
    {
        AddCategory(SportIcon.Football,"Sân bóng đá","Phổ biến, linh hoạt",AppTheme.Info);
        AddCategory(SportIcon.Badminton,"Sân cầu lông","Rèn luyện sức khỏe",AppTheme.Warning);
        AddCategory(SportIcon.Volleyball,"Sân bóng chuyền","Thi đấu & tập luyện",AppTheme.Purple);
    }
    private void AddCategory(SportIcon icon,string title,string sub,Color accent)
    {
        var p=new RoundedPanel{Width=240,Height=54,Radius=14,BorderColor=Color.FromArgb(225,235,241),Margin=new Padding(4),BackColor=Color.White};
        var i=new IconBadge{IconImage=SportIcons.Get(icon,18,accent),AccentColor=Color.FromArgb(28,accent),Filled=false,Size=new Size(38,38),Location=new Point(8,8)};
        var t=new Label{Text=title,AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F,FontStyle.Bold),ForeColor=AppTheme.Text,Location=new Point(50,8)};
        var s=new Label{Text=sub,AutoSize=true,Font=new Font("Segoe UI",7.2F),ForeColor=AppTheme.Muted,Location=new Point(50,28)};
        p.Controls.AddRange(new Control[]{i,t,s});categoryFlow.Controls.Add(p);
    }

    private void LoadData()
    {
        try
        {
            var cid=SessionContext.CustomerId??0;
            var booking=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM DatSan WHERE KhachHangID=@c",new SqlParameter("@c",cid))??0);
            var vouchers=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM PhieuGiamGiaKhachHang WHERE KhachHangID=@c AND DaSuDung=0",new SqlParameter("@c",cid))??0);
            var spent=Convert.ToDecimal(Db.Scalar("SELECT ISNULL(SUM(TongTien),0) FROM HoaDon WHERE KhachHangID=@c AND TrangThaiThanhToan='Paid'",new SqlParameter("@c",cid))??0);
            flpStats.Controls.Clear();
            flpStats.Controls.Add(MiniStat(SportIcon.Booking,"Đơn đặt sân",booking.ToString(),AppTheme.Info));
            flpStats.Controls.Add(MiniStat(SportIcon.Voucher,"Voucher",vouchers.ToString(),AppTheme.Purple));
            flpStats.Controls.Add(MiniStat(SportIcon.Pay,"Tổng chi tiêu",spent.ToString("N0")+"đ",AppTheme.Warning));

            var voucher=Db.Query(@"SELECT TOP 1 v.TenPhieuGiamGia,v.LoaiGiamGia,v.GiaTriGiam,v.NgayKetThuc FROM PhieuGiamGiaKhachHang cv JOIN PhieuGiamGia v ON v.PhieuGiamGiaID=cv.PhieuGiamGiaID WHERE cv.KhachHangID=@c AND cv.DaSuDung=0 AND v.DangHoatDong=1 AND SYSDATETIME() BETWEEN v.NgayBatDau AND v.NgayKetThuc ORDER BY v.NgayKetThuc",new SqlParameter("@c",cid));
            if(voucher.Rows.Count>0){var r=voucher.Rows[0];var amount=Convert.ToString(r["LoaiGiamGia"])=="Percent"?$"GIẢM {Convert.ToDecimal(r["GiaTriGiam"]):0}%":$"GIẢM {Convert.ToDecimal(r["GiaTriGiam"]):N0}đ";lblVoucher.Text=$"{amount}\n{Convert.ToString(r["TenPhieuGiamGia"])}\nHSD: {Convert.ToDateTime(r["NgayKetThuc"]):dd/MM/yyyy}";}else lblVoucher.Text="CHƯA CÓ VOUCHER\nTheo dõi ưu đãi mới\nđể nhận khuyến mãi hấp dẫn";

            var dt=Db.Query("SELECT TOP 12 f.TenSan,ft.TenLoaiSan,f.ViTri,f.GiaMoiGio,f.TrangThai FROM SanTheThao f JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID WHERE f.DangHoatDong=1 AND ft.DangHoatDong=1 AND f.TrangThai='Available' ORDER BY ft.TenLoaiSan,f.TenSan");
            flpFields.Controls.Clear();
            foreach(System.Data.DataRow r in dt.Rows)
            {
                var card=new FieldCardControl(Convert.ToString(r["TenSan"])??"Sân",Convert.ToString(r["TenLoaiSan"])??"",Convert.ToString(r["ViTri"])??"",Convert.ToDecimal(r["GiaMoiGio"]));
                card.BookClicked+=(_,__)=>OpenBooking();
                flpFields.Controls.Add(card);
            }
        }
        catch(Exception ex){UiMsg.Warn(ex.Message, "Trang chủ");}
    }

    private Control MiniStat(SportIcon icon,string title,string value,Color color)
    {
        var p=new RoundedPanel{Width=104,Height=72,Radius=12,BorderColor=Color.FromArgb(229,237,242),Margin=new Padding(3),BackColor=Color.FromArgb(250,253,254)};
        var i=new IconBadge{IconImage=SportIcons.Get(icon,15,Color.White),AccentColor=color,Size=new Size(30,30),Location=new Point(7,20)};
        var t=new Label{Text=title,AutoSize=true,Font=new Font("Segoe UI",6.8F),ForeColor=AppTheme.Muted,Location=new Point(42,12)};
        var v=new Label{Text=value,AutoSize=false,Width=59,Height=34,Font=new Font("Segoe UI Semibold",8.6F,FontStyle.Bold),ForeColor=AppTheme.Text,Location=new Point(42,28),TextAlign=ContentAlignment.MiddleLeft};
        p.Controls.AddRange(new Control[]{i,t,v});return p;
    }

    private void OpenBooking()
    {
        using var f=new FrmBooking(customerMode:true){Text="Đặt sân",StartPosition=FormStartPosition.CenterParent,Size=new Size(Math.Min(1060,Width-60),Math.Min(720,Height-60))};
        AppTheme.ApplyToForm(f,"Customer");f.ShowDialogFx(this);LoadData();
    }
}
