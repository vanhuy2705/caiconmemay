using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmAdminDashboard:Form
{
    private readonly DashboardCard cRevenue=new("Doanh thu hôm nay","0 đ","Từ thanh toán đã xác nhận");
    private readonly DashboardCard cBookings=new("Lịch đặt hôm nay","0","Đã tạo trong ngày");
    private readonly DashboardCard cPending=new("Hóa đơn chờ","0","Chưa/1 phần thanh toán");
    private readonly DashboardCard cFields=new("Sân hoạt động","0/0","Khả dụng / Tổng");
    public FrmAdminDashboard(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        foreach(var c in new[]{cRevenue,cBookings,cPending,cFields}) cards.Controls.Add(c);
        AppTheme.StyleGrid(gridToday);
        AppTheme.StyleGrid(gridRecent);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StylePrimary(btnNewBooking);
        AppTheme.StyleSecondary(btnManageCustomers);
        AppTheme.StyleSecondary(btnViewInvoices);
        AppTheme.StyleSecondary(btnManageVouchers);
        btnRefresh.Click+=(_,__)=>LoadData();
        btnNewBooking.Click+=(_,__)=>{using var f=new FrmBooking();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);LoadData();};
        btnManageCustomers.Click+=(_,__)=>{using var f=new FrmCustomers();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);};
        btnViewInvoices.Click+=(_,__)=>{using var f=new FrmInvoices();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);LoadData();};
        btnManageVouchers.Click+=(_,__)=>{using var f=new FrmVouchers();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);};
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            var today=DateTime.Today;
            var tomorrow=today.AddDays(1);
            var revenue=Convert.ToDecimal(Db.Scalar("SELECT ISNULL(SUM(SoTien),0) FROM ThanhToan WHERE TrangThai='Confirmed' AND NgayThanhToan>=@f AND NgayThanhToan<@t",new SqlParameter("@f",today),new SqlParameter("@t",tomorrow))??0);
            var bookings=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM DatSan WHERE CAST(ThoiGianBatDau AS DATE)=@d",new SqlParameter("@d",today))??0);
            var pending=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM HoaDon WHERE TrangThaiThanhToan<>'Paid'")??0);
            var totalFields=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM SanTheThao WHERE DangHoatDong=1")??0);
            var busyFields=Convert.ToInt32(Db.Scalar("SELECT COUNT(DISTINCT SanID) FROM DatSan WHERE CAST(ThoiGianBatDau AS DATE)=@d AND TrangThai<>'Cancelled'",new SqlParameter("@d",today))??0);
            var maintenance=Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM SanTheThao WHERE TrangThai='Maintenance' AND DangHoatDong=1")??0);

            cRevenue.SetValue(revenue.ToString("N0")+" đ");
            cBookings.SetValue(bookings.ToString());
            cPending.SetValue(pending.ToString());
            cFields.SetValue($"{Math.Max(0,totalFields-busyFields-maintenance)}/{totalFields}");

            lblFieldStatus.Text=$"Bận: {busyFields} • Bảo trì: {maintenance} • Rảnh: {Math.Max(0,totalFields-busyFields-maintenance)}";

            gridToday.DataSource=Db.Query(@"SELECT b.MaDatSan [Mã đơn],c.HoTen [Khách],f.TenSan [Sân],b.ThoiGianBatDau [Bắt đầu],b.ThoiGianKetThuc [Kết thúc],CASE b.TrangThai WHEN 'Pending' THEN N'Chờ' WHEN 'Confirmed' THEN N'Đã xác nhận' WHEN 'InUse' THEN N'Đang dùng' WHEN 'Completed' THEN N'Hoàn thành' WHEN 'Cancelled' THEN N'Đã hủy' ELSE b.TrangThai END [TT] FROM DatSan b JOIN KhachHang c ON c.KhachHangID=b.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID WHERE CAST(b.ThoiGianBatDau AS DATE)=@d ORDER BY b.ThoiGianBatDau",new SqlParameter("@d",today));

            gridRecent.DataSource=Db.Query(@"SELECT TOP 8 b.MaDatSan [Mã đơn],c.HoTen [Khách],f.TenSan [Sân],b.TongTien [Tổng],CASE b.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa TT' WHEN 'PartiallyPaid' THEN N'1 phần' WHEN 'Paid' THEN N'Đã TT' ELSE b.TrangThaiThanhToan END [Thanh toán],b.NgayTao [Ngày tạo] FROM DatSan b JOIN KhachHang c ON c.KhachHangID=b.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID ORDER BY b.NgayTao DESC");

            var typeStats=Db.Query(@"SELECT ft.TenLoaiSan Label, COUNT(DISTINCT f.SanID) Total, COUNT(DISTINCT CASE WHEN f.TrangThai='Available' THEN f.SanID END) Avail FROM LoaiSan ft LEFT JOIN SanTheThao f ON f.LoaiSanID=ft.LoaiSanID AND f.DangHoatDong=1 GROUP BY ft.TenLoaiSan");
            donut.Items=typeStats.Rows.Cast<System.Data.DataRow>().Select(r=>{
                var label=Convert.ToString(r["Label"]) ?? "";
                var total= r["Total"]==DBNull.Value ? 0 : Convert.ToInt32(r["Total"]);
                return (label,(decimal)total);
            }).ToList();
            donut.Invalidate();

            var rev7=Db.Query(@"SELECT FORMAT(CAST(NgayThanhToan AS DATE),'dd/MM') Label, SUM(SoTien) Value FROM ThanhToan WHERE TrangThai='Confirmed' AND NgayThanhToan>=@f GROUP BY CAST(NgayThanhToan AS DATE) ORDER BY CAST(NgayThanhToan AS DATE)",new SqlParameter("@f",today.AddDays(-6)));
            chart.Items=rev7.Rows.Cast<System.Data.DataRow>().Select(r=>{
                var label=Convert.ToString(r["Label"]) ?? "";
                var val= r["Value"]==DBNull.Value ? 0 : Convert.ToDecimal(r["Value"]);
                return (label,val);
            }).ToList();
            chart.Invalidate();
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải dashboard");}
    }
}
