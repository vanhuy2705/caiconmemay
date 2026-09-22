using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Forms.Common;

namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerVouchers:Form
{
    public FrmCustomerVouchers(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            flow.Controls.Clear();
            var dt=Db.Query(@"SELECT v.MaPhieuGiamGia,v.TenPhieuGiamGia,v.LoaiGiamGia,v.GiaTriGiam,v.GiaTriDonHangToiThieu,v.GiaTriGiamToiDa,cv.DaSuDung,v.NgayKetThuc FROM PhieuGiamGiaKhachHang cv JOIN PhieuGiamGia v ON v.PhieuGiamGiaID=cv.PhieuGiamGiaID WHERE cv.KhachHangID=@c ORDER BY cv.DaSuDung, v.NgayKetThuc", new SqlParameter("@c",SessionContext.CustomerId??0));
            if(dt.Rows.Count==0){
                var empty=new Label{Text="Bạn chưa có voucher nào. Đặt sân thường xuyên để nhận ưu đãi!",AutoSize=false,Size=new Size(400,60),Font=new Font("Segoe UI",9F),ForeColor=AppTheme.Muted,TextAlign=ContentAlignment.MiddleLeft,Padding=new Padding(12,8,0,0)};
                flow.Controls.Add(empty);
                return;
            }
            foreach(System.Data.DataRow r in dt.Rows){
                var ma=Convert.ToString(r["MaPhieuGiamGia"]) ?? "";
                var ten=Convert.ToString(r["TenPhieuGiamGia"]) ?? "";
                var loai=Convert.ToString(r["LoaiGiamGia"]) ?? "";
                var giaTri= r["GiaTriGiam"]==DBNull.Value ? 0 : Convert.ToDecimal(r["GiaTriGiam"]);
                var min= r["GiaTriDonHangToiThieu"]==DBNull.Value ? 0 : Convert.ToDecimal(r["GiaTriDonHangToiThieu"]);
                var max= r["GiaTriGiamToiDa"]==DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["GiaTriGiamToiDa"]);
                var daDung= r["DaSuDung"]!=DBNull.Value && Convert.ToBoolean(r["DaSuDung"]);
                var hetHan= r["NgayKetThuc"]==DBNull.Value ? DateTime.MaxValue : Convert.ToDateTime(r["NgayKetThuc"]);
                var card=new RoundedPanel{Width=380,Height=110,Radius=12,BorderColor= daDung ? Color.FromArgb(220,220,220) : AppTheme.Success,BorderThickness=1,Margin=new Padding(8),BackColor= daDung ? Color.FromArgb(245,245,245) : Color.White};
                var lblMa=new Label{Text=ma,AutoSize=true,Font=new Font("Segoe UI Semibold",10F,FontStyle.Bold),ForeColor=AppTheme.Text,Location=new Point(12,10)};
                var lblTen=new Label{Text=ten,AutoSize=false,Width=340,Height=32,Font=new Font("Segoe UI",8.5F),ForeColor=AppTheme.Muted,Location=new Point(12,30)};
                var gtText= loai=="Percent" ? $"{giaTri:0}%": $"{giaTri:N0}đ";
                if(max!=null) gtText+=$" (tối đa {max.Value:N0}đ)";
                var lblGt=new Label{Text=$"Giảm: {gtText} • ĐH tối thiểu: {min:N0}đ",AutoSize=false,Width=340,Height=20,Font=new Font("Segoe UI",7.8F),ForeColor=AppTheme.Text,Location=new Point(12,62)};
                var lblStatus=new Label{Text= daDung ? "Đã dùng" : (hetHan<DateTime.Now ? "Hết hạn" : $"HSD: {hetHan:dd/MM/yyyy}"),AutoSize=true,Font=new Font("Segoe UI Semibold",8F,FontStyle.Bold),ForeColor= daDung ? Color.Gray : (hetHan<DateTime.Now ? Color.Red : AppTheme.Success),Location=new Point(12,82)};
                card.Controls.AddRange(new Control[]{lblMa,lblTen,lblGt,lblStatus});
                flow.Controls.Add(card);
            }
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải voucher");}
    }
}
