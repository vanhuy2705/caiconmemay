using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmStatistics:Form
{
    private readonly DashboardCard cRevenue=new("Doanh thu","0 đ","Trong khoảng chọn");
    private readonly DashboardCard cBookings=new("Lượt đặt","0","Không tính đơn hủy");
    private readonly DashboardCard cPaid=new("Đã thanh toán","0","Hóa đơn");
    private readonly DashboardCard cCustomers=new("Khách hàng","0","Có giao dịch");
    public FrmStatistics(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        foreach(var c in new[]{cRevenue,cBookings,cPaid,cCustomers}) cards.Controls.Add(c);
        AppTheme.StyleGrid(grid);
        AppTheme.StylePrimary(btnLoad);
        dtFrom.Value=DateTime.Today.AddDays(-6);
        dtTo.Value=DateTime.Today;
        btnLoad.Click+=(_,__)=>LoadReport();
        Shown+=(_,__)=>LoadReport();
    }
    private void LoadReport(){
        if(dtFrom.Value.Date>dtTo.Value.Date){UiMsg.Warn("Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.","Thống kê");return;}
        try{
            var from=dtFrom.Value.Date;
            var to=dtTo.Value.Date.AddDays(1);
            var revenue=Convert.ToDecimal(Db.Scalar("SELECT ISNULL(SUM(SoTien),0) FROM ThanhToan WHERE TrangThai='Confirmed' AND NgayThanhToan>=@f AND NgayThanhToan<@t",new SqlParameter("@f",from),new SqlParameter("@t",to))??0);
            var bookings=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM DatSan WHERE ThoiGianBatDau>=@f AND ThoiGianBatDau<@t AND TrangThai<>'Cancelled'",new SqlParameter("@f",from),new SqlParameter("@t",to))??0);
            var paid=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM HoaDon WHERE NgayTao>=@f AND NgayTao<@t AND TrangThaiThanhToan='Paid'",new SqlParameter("@f",from),new SqlParameter("@t",to))??0);
            var cust=Convert.ToInt32(Db.Scalar("SELECT COUNT(DISTINCT KhachHangID) FROM DatSan WHERE ThoiGianBatDau>=@f AND ThoiGianBatDau<@t AND TrangThai<>'Cancelled'",new SqlParameter("@f",from),new SqlParameter("@t",to))??0);
            cRevenue.SetValue(revenue.ToString("N0")+" đ");
            cBookings.SetValue(bookings.ToString());
            cPaid.SetValue(paid.ToString());
            cCustomers.SetValue(cust.ToString());
            grid.DataSource=Db.Query(@"SELECT ft.TenLoaiSan [Loại sân],COUNT(b.DatSanID) [Lượt đặt],ISNULL(SUM(CASE WHEN b.TrangThai<>'Cancelled' THEN b.TongTien ELSE 0 END),0) [Giá trị đặt] FROM LoaiSan ft LEFT JOIN SanTheThao f ON f.LoaiSanID=ft.LoaiSanID LEFT JOIN DatSan b ON b.SanID=f.SanID AND b.ThoiGianBatDau>=@f AND b.ThoiGianBatDau<@t GROUP BY ft.TenLoaiSan ORDER BY [Lượt đặt] DESC",new SqlParameter("@f",from),new SqlParameter("@t",to));
            var dt=Db.Query(@"SELECT FORMAT(CAST(NgayThanhToan AS date),'dd/MM') Label,SUM(SoTien) Value FROM ThanhToan WHERE TrangThai='Confirmed' AND NgayThanhToan>=@f AND NgayThanhToan<@t GROUP BY CAST(NgayThanhToan AS date) ORDER BY CAST(NgayThanhToan AS date)",new SqlParameter("@f",from),new SqlParameter("@t",to));
            chart.Items=dt.Rows.Cast<System.Data.DataRow>().Select(r=>{
                var label=Convert.ToString(r["Label"]) ?? "";
                var val= r["Value"]==DBNull.Value ? 0 : Convert.ToDecimal(r["Value"]);
                return (label,val);
            }).ToList();
            chart.Invalidate();
        }catch(Exception ex){UiMsg.Error(ex.Message,"Thống kê");}
    }
}
