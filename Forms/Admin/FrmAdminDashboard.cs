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
        lblGreeting.Text=$"☀  Xin chào, {SessionContext.FullName}!";
        lblGreetingSub.Text="Chúc bạn một ngày làm việc hiệu quả!";
        foreach(var c in new[]{cRevenue,cBookings,cPending,cFields}) flpCards.Controls.Add(c);
        AppTheme.StyleGrid(gridRecent);
        AppTheme.StylePrimary(btnNewBooking);
        btnNewBooking.Click+=(_,__)=>{using var f=new FrmBooking();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);LoadData();};
        BuildQuickActions();
        Shown+=(_,__)=>LoadData();
    }

    private void BuildQuickActions(){
        quickGrid.Controls.Clear();
        AddQuick("Đặt sân mới",SportIcon.Booking,AppTheme.Success,()=>{using var f=new FrmBooking();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);LoadData();},0,0);
        AddQuick("Khách hàng",SportIcon.Customer,AppTheme.Info,()=>{using var f=new FrmCustomers();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);},1,0);
        AddQuick("Hóa đơn",SportIcon.Invoice,AppTheme.Purple,()=>{using var f=new FrmInvoices();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);LoadData();},0,1);
        AddQuick("Voucher",SportIcon.Voucher,AppTheme.Warning,()=>{using var f=new FrmVouchers();AppTheme.ApplyToForm(f,"Admin");f.ShowDialogFx(this);},1,1);
    }
    private void AddQuick(string text,SportIcon icon,Color color,Action action,int col,int row){
        var b=new RoundedButton{Dock=DockStyle.Fill,Margin=new Padding(5),Text=text,Image=SportIcons.Get(icon,18,Color.White),TextImageRelation=TextImageRelation.ImageBeforeText,BackColor=color,ForeColor=Color.White,Font=new Font("Segoe UI Semibold",8.4F,FontStyle.Bold),Radius=10,HoverColor=ControlPaint.Dark(color,.08f)};
        b.Click+=(_,__)=>action();
        quickGrid.Controls.Add(b,col,row);
    }

    private void LoadData(){
        try{
            var today=DateTime.Today;
            var tomorrow=today.AddDays(1);
            lblDate.Text=DateTime.Now.ToString("dddd, dd/MM/yyyy - HH:mm");

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

            // Timeline - map to BookingTimelineControl.Item
            var todayData=Db.Query(@"SELECT f.TenSan [Sân],b.ThoiGianBatDau [Bắt đầu],b.ThoiGianKetThuc [Kết thúc],c.HoTen [Khách],CASE b.TrangThai WHEN 'Pending' THEN N'Chờ' WHEN 'Confirmed' THEN N'Đã xác nhận' WHEN 'InUse' THEN N'Đang dùng' WHEN 'Completed' THEN N'Hoàn thành' WHEN 'Cancelled' THEN N'Đã hủy' ELSE b.TrangThai END [TT] FROM DatSan b JOIN KhachHang c ON c.KhachHangID=b.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID WHERE CAST(b.ThoiGianBatDau AS DATE)=@d ORDER BY b.ThoiGianBatDau",new SqlParameter("@d",today));
            var items=new List<BookingTimelineControl.Item>();
            foreach(System.Data.DataRow r in todayData.Rows){
                var field=Convert.ToString(r["Sân"]) ?? "Sân";
                var start= r["Bắt đầu"]==DBNull.Value ? today.AddHours(8) : Convert.ToDateTime(r["Bắt đầu"]);
                var end= r["Kết thúc"]==DBNull.Value ? start.AddHours(1) : Convert.ToDateTime(r["Kết thúc"]);
                var cust=Convert.ToString(r["Khách"]) ?? "";
                var status=Convert.ToString(r["TT"]) ?? "";
                items.Add(new BookingTimelineControl.Item(field,start,end,cust,status));
            }
            timeline.Items=items;
            timeline.Invalidate();

            gridRecent.DataSource=Db.Query(@"SELECT TOP 8 b.MaDatSan [Mã đơn],c.HoTen [Khách],f.TenSan [Sân],b.TongTien [Tổng],CASE b.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa TT' WHEN 'PartiallyPaid' THEN N'1 phần' WHEN 'Paid' THEN N'Đã TT' ELSE b.TrangThaiThanhToan END [Thanh toán],b.NgayTao [Ngày tạo] FROM DatSan b JOIN KhachHang c ON c.KhachHangID=b.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID ORDER BY b.NgayTao DESC");

            // Donut - Available/Busy/Maintenance
            donut.Available=Math.Max(0,totalFields-busyFields-maintenance);
            donut.Busy=busyFields;
            donut.Maintenance=maintenance;
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
