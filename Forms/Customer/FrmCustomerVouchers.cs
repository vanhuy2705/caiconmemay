using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerVouchers:Form
{
    public FrmCustomerVouchers(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        btnRefresh.Click+=(_,__)=>LoadData();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query(@"SELECT v.MaPhieuGiamGia [Mã voucher],v.TenPhieuGiamGia [Tên voucher],CASE v.LoaiGiamGia WHEN 'Percent' THEN N'Phần trăm' ELSE N'Số tiền' END [Loại],v.GiaTriGiam [Giá trị],v.GiaTriDonHangToiThieu [ĐH tối thiểu],v.GiaTriGiamToiDa [Giảm tối đa],CASE cv.DaSuDung WHEN 1 THEN N'Đã dùng' ELSE N'Chưa dùng' END [Trạng thái],v.NgayKetThuc [Hết hạn] FROM PhieuGiamGiaKhachHang cv JOIN PhieuGiamGia v ON v.PhieuGiamGiaID=cv.PhieuGiamGiaID WHERE cv.KhachHangID=@c ORDER BY cv.DaSuDung, v.NgayKetThuc", new SqlParameter("@c",SessionContext.CustomerId??0));
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải voucher");}
    }
}
