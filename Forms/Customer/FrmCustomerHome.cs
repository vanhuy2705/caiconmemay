using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Admin;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerHome:Form
{
    private int? _filterLoaiSanId = null;
    private string _filterLoaiSanName = "";
    public FrmCustomerHome()
    {
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        lblWelcome.Text=$"Xin chào, {SessionContext.FullName}!";
        hero.Cursor=Cursors.Hand;
        hero.Click+=(_,__)=>OpenBooking();
        BuildCategories();
        Shown+=(_,__)=>LoadData();
    }

    private void BuildCategories()
    {
        categoryFlow.Controls.Clear();
        try{
            var dt=Db.Query("SELECT LoaiSanID,TenLoaiSan FROM LoaiSan WHERE DangHoatDong=1 ORDER BY TenLoaiSan");
            AddCategory(null,"Tất cả sân","Hiển thị toàn bộ",AppTheme.Success,true);
            foreach(System.Data.DataRow r in dt.Rows){
                var id= r["LoaiSanID"]==DBNull.Value ? (int?)null : Convert.ToInt32(r["LoaiSanID"]);
                if(id==null) continue;
                var name=Convert.ToString(r["TenLoaiSan"]) ?? "";
                var icon=name.Contains("bóng đá",StringComparison.OrdinalIgnoreCase)?SportIcon.Football:
                         name.Contains("cầu lông",StringComparison.OrdinalIgnoreCase)?SportIcon.Badminton:
                         name.Contains("chuyền",StringComparison.OrdinalIgnoreCase)?SportIcon.Volleyball:SportIcon.Field;
                var color=icon==SportIcon.Football?AppTheme.Info:icon==SportIcon.Badminton?AppTheme.Warning:AppTheme.Purple;
                AddCategory(id,name,name,color,false);
            }
        }catch{
            AddCategory(SportIcon.Football,"Sân bóng đá","Phổ biến, linh hoạt",AppTheme.Info);
            AddCategory(SportIcon.Badminton,"Sân cầu lông","Rèn luyện sức khỏe",AppTheme.Warning);
            AddCategory(SportIcon.Volleyball,"Sân bóng chuyền","Thi đấu & tập luyện",AppTheme.Purple);
        }
    }
    private void AddCategory(int? loaiId,string title,string sub,Color accent,bool isAll=false){
        var icon=isAll?SportIcon.Category:(title.Contains("bóng đá")?SportIcon.Football:title.Contains("cầu lông")?SportIcon.Badminton:SportIcon.Volleyball);
        AddCategory(icon,title,sub,accent,loaiId);
    }
    private void AddCategory(SportIcon icon,string title,string sub,Color accent,int? loaiId=null)
    {
        bool isSelected=_filterLoaiSanId==loaiId;
        var p=new RoundedPanel{
            Width=240,Height=54,Radius=14,
            BorderColor=isSelected?accent:Color.FromArgb(225,235,241),
            BorderThickness=isSelected?2:1,
            Margin=new Padding(4),
            BackColor=isSelected?Color.FromArgb(240,255,247):Color.White,
            Cursor=Cursors.Hand
        };
        var i=new IconBadge{IconImage=SportIcons.Get(icon,18,accent),AccentColor=Color.FromArgb(28,accent),Filled=false,Size=new Size(38,38),Location=new Point(8,8)};
        var t=new Label{Text=title,AutoSize=true,Font=new Font("Segoe UI Semibold",8.6F,FontStyle.Bold),ForeColor=AppTheme.Text,Location=new Point(50,8)};
        var s=new Label{Text=sub,AutoSize=true,Font=new Font("Segoe UI",7.2F),ForeColor=AppTheme.Muted,Location=new Point(50,28)};
        p.Controls.AddRange(new Control[]{i,t,s});

        void SelectCategory(){
            _filterLoaiSanId=loaiId;
            _filterLoaiSanName=title;
            lblFields.Text=loaiId==null?"★  Sân nổi bật":$"★  {title}";
            BuildCategories();
            LoadFields();
        }

        p.Click+=(_,__)=>SelectCategory();
        foreach(Control c in p.Controls){
            c.Cursor=Cursors.Hand;
            c.Click+=(_,__)=>SelectCategory();
        }
        categoryFlow.Controls.Add(p);
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
            if(voucher.Rows.Count>0){
                var r=voucher.Rows[0];
                var loai=Convert.ToString(r["LoaiGiamGia"]) ?? "";
                var gt= r["GiaTriGiam"]==DBNull.Value ? 0 : Convert.ToDecimal(r["GiaTriGiam"]);
                var amount=loai=="Percent"?$"GIẢM {gt:0}%":$"GIẢM {gt:N0}đ";
                var ten=Convert.ToString(r["TenPhieuGiamGia"]) ?? "";
                var hsd= r["NgayKetThuc"]==DBNull.Value ? "" : Convert.ToDateTime(r["NgayKetThuc"]).ToString("dd/MM/yyyy");
                lblVoucher.Text=$"{amount}\n{ten}\nHSD: {hsd}";
            }else lblVoucher.Text="CHƯA CÓ VOUCHER\nTheo dõi ưu đãi mới\nđể nhận khuyến mãi hấp dẫn";

            LoadFields();
        }
        catch(Exception ex){UiMsg.Warn(ex.Message, "Trang chủ");}
    }

    private void LoadFields(){
        try{
            var sql=@"SELECT TOP 12 f.SanID,f.TenSan,ft.TenLoaiSan,f.ViTri,f.GiaMoiGio,f.TrangThai FROM SanTheThao f JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID WHERE f.DangHoatDong=1 AND ft.DangHoatDong=1 AND f.TrangThai='Available' AND (@LoaiID IS NULL OR f.LoaiSanID=@LoaiID) ORDER BY ft.TenLoaiSan,f.TenSan";
            var dt=Db.Query(sql, new SqlParameter("@LoaiID",(object?)_filterLoaiSanId ?? DBNull.Value));
            flpFields.Controls.Clear();
            foreach(System.Data.DataRow r in dt.Rows)
            {
                var tenSan=Convert.ToString(r["TenSan"]) ?? "Sân";
                var tenLoai=Convert.ToString(r["TenLoaiSan"]) ?? "";
                var viTri=Convert.ToString(r["ViTri"]) ?? "";
                var gia= r["GiaMoiGio"]==DBNull.Value ? 0 : Convert.ToDecimal(r["GiaMoiGio"]);
                var card=new FieldCardControl(tenSan,tenLoai,viTri,gia);
                var sanId= r["SanID"]==DBNull.Value ? 0 : Convert.ToInt32(r["SanID"]);
                card.BookClicked+=(_,__)=>OpenBooking(sanId);
                flpFields.Controls.Add(card);
            }
            if(flpFields.Controls.Count==0){
                var empty=new Label{
                    Text=$"Không có sân trống cho '{_filterLoaiSanName}'.\nThử chọn loại khác hoặc xem tất cả.",
                    AutoSize=false,Size=new Size(400,60),
                    Font=new Font("Segoe UI",9F),ForeColor=AppTheme.Muted,
                    TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(12,0,0,0)
                };
                flpFields.Controls.Add(empty);
            }
        }catch(Exception ex){UiMsg.Warn(ex.Message,"Tải sân");}
    }

    private Control MiniStat(SportIcon icon,string title,string value,Color color)
    {
        var p=new RoundedPanel{Width=104,Height=72,Radius=12,BorderColor=Color.FromArgb(229,237,242),Margin=new Padding(3),BackColor=Color.FromArgb(250,253,254)};
        var i=new IconBadge{IconImage=SportIcons.Get(icon,15,Color.White),AccentColor=color,Size=new Size(30,30),Location=new Point(7,20)};
        var t=new Label{Text=title,AutoSize=true,Font=new Font("Segoe UI",6.8F),ForeColor=AppTheme.Muted,Location=new Point(42,12)};
        var v=new Label{Text=value,AutoSize=false,Width=59,Height=34,Font=new Font("Segoe UI Semibold",8.6F,FontStyle.Bold),ForeColor=AppTheme.Text,Location=new Point(42,28),TextAlign=ContentAlignment.MiddleLeft};
        p.Controls.AddRange(new Control[]{i,t,v});return p;
    }

    private void OpenBooking(int? preselectedFieldId=null)
    {
        using var f=new FrmBooking(customerMode:true){Text="Đặt sân",StartPosition=FormStartPosition.CenterParent,Size=new Size(Math.Min(1060,Width-60),Math.Min(720,Height-60))};
        AppTheme.ApplyToForm(f,"Customer");
        f.ShowDialogFx(this);
        LoadData();
    }
}
