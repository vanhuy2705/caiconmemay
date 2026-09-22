using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;

public partial class FrmAdminDashboard : Form
{
    private readonly DashboardCard cRevenue = new("Doanh thu hôm nay","0 đ","↑ Tổng tiền đã thu", AppTheme.Success, "$");
    private readonly DashboardCard cBooking = new("Booking hôm nay","0 lượt","↑ Lịch đặt trong ngày", AppTheme.Info, "▣");
    private readonly DashboardCard cInvoice = new("Hóa đơn chưa thu","0 hóa đơn","Cần xử lý", AppTheme.Warning, "▤");
    private readonly DashboardCard cField = new("Tình trạng sân","0/0 trống","Theo dữ liệu hiện tại", AppTheme.Purple, "◎");

    public FrmAdminDashboard()
    {
        InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        lblGreeting.Text = $"☀  Xin chào, {SessionContext.FullName}!";
        AppTheme.ApplyDecorativeIcon(lblGreeting);
        lblDate.Text = DateTime.Today.ToString("dddd, dd/MM/yyyy");
        foreach(var c in new[]{cRevenue,cBooking,cInvoice,cField}) flpCards.Controls.Add(c);
        AppTheme.StyleGrid(gridRecent);
        BuildQuickActions();
        btnNewBooking.Click += (_,__) => OpenDialog(new FrmBooking(), "Đặt sân");
        Shown += (_,__) => LoadDashboard();
    }

    private void BuildQuickActions()
    {
        AddQuick("Đặt sân mới", SportIcon.Booking, AppTheme.Accent2, () => OpenDialog(new FrmBooking(), "Đặt sân"), 0, 0);
        AddQuick("Khách hàng", SportIcon.Customer, AppTheme.Info, () => OpenDialog(new FrmCustomers(), "Khách hàng"), 1, 0);
        AddQuick("Hóa đơn", SportIcon.Invoice, AppTheme.Purple, () => OpenDialog(new FrmInvoices(), "Hóa đơn"), 0, 1);
        AddQuick("Voucher", SportIcon.Voucher, AppTheme.Warning, () => OpenDialog(new FrmVouchers(), "Voucher"), 1, 1);
    }

    private void AddQuick(string text, SportIcon icon, Color color, Action action, int col, int row)
    {
        var b = new RoundedButton { Dock=DockStyle.Fill, Margin=new Padding(4), Text=text, Image=SportIcons.Get(icon,18,Color.White), TextImageRelation=TextImageRelation.ImageBeforeText, BackColor=color, ForeColor=Color.White, Font=new Font("Segoe UI Semibold",8.5F,FontStyle.Bold), Radius=9, HoverColor=ControlPaint.Dark(color, .08f) };
        b.Click += (_,__) => action();
        quickGrid.Controls.Add(b,col,row);
    }

    private void OpenDialog(Form form, string title)
    {
        form.Text = title;
        form.StartPosition = FormStartPosition.CenterParent;
        form.Size = new Size(Math.Min(1180, Width-80), Math.Min(760, Height-80));
        AppTheme.ApplyToForm(form,"Admin");
        form.ShowDialogFx(this);
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        try
        {
            var revenue = Convert.ToDecimal(Db.Scalar(@"SELECT ISNULL(SUM(p.SoTien),0) FROM ThanhToan p WHERE p.TrangThai='Confirmed' AND CAST(p.NgayThanhToan AS DATE)=CAST(GETDATE() AS DATE)") ?? 0);
            var bookings = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM DatSan WHERE CAST(ThoiGianBatDau AS DATE)=CAST(GETDATE() AS DATE) AND TrangThai<>'Cancelled'") ?? 0);
            var unpaid = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM HoaDon WHERE TrangThaiThanhToan<>'Paid'") ?? 0);
            var totalFields = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM SanTheThao WHERE DangHoatDong=1") ?? 0);
            var busyFields = Convert.ToInt32(Db.Scalar(@"SELECT COUNT(DISTINCT SanID) FROM DatSan WHERE GETDATE()>=ThoiGianBatDau AND GETDATE()<ThoiGianKetThuc AND TrangThai NOT IN ('Cancelled','Completed')") ?? 0);
            var maintenance = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM SanTheThao WHERE DangHoatDong=1 AND TrangThai='Maintenance'") ?? 0);
            var available = Math.Max(0,totalFields-busyFields-maintenance);
            cRevenue.SetValue(revenue.ToString("N0")+" đ", "↑ Doanh thu thực thu");
            cBooking.SetValue(bookings+" lượt", "↑ Lịch đặt trong ngày");
            cInvoice.SetValue(unpaid+" hóa đơn", unpaid > 0 ? "Cần xử lý thanh toán" : "Đã xử lý hết");
            cField.SetValue(available+"/"+totalFields+" trống", $"Trống: {available} | Thuê: {busyFields} | Bảo trì: {maintenance}");

            var schedule = Db.Query(@"SELECT f.TenSan,b.ThoiGianBatDau,b.ThoiGianKetThuc,c.HoTen,b.TrangThai FROM DatSan b JOIN SanTheThao f ON f.SanID=b.SanID JOIN KhachHang c ON c.KhachHangID=b.KhachHangID WHERE CAST(b.ThoiGianBatDau AS DATE)=CAST(GETDATE() AS DATE) AND b.TrangThai<>'Cancelled' ORDER BY f.TenSan,b.ThoiGianBatDau");
            timeline.Items = schedule.Rows.Cast<System.Data.DataRow>().Select(r => new BookingTimelineControl.Item(Convert.ToString(r["TenSan"])??"Sân", Convert.ToDateTime(r["ThoiGianBatDau"]), Convert.ToDateTime(r["ThoiGianKetThuc"]), Convert.ToString(r["HoTen"])??"Khách", Convert.ToString(r["TrangThai"])??"")).ToList();
            timeline.Invalidate();

            gridRecent.DataSource = Db.Query(@"SELECT TOP 8 b.MaDatSan AS [Mã],c.HoTen AS [Khách hàng],f.TenSan AS [Sân],CONVERT(varchar(5),b.ThoiGianBatDau,108) AS [Bắt đầu],CONVERT(varchar(5),b.ThoiGianKetThuc,108) AS [Kết thúc],FORMAT(b.TongTien,'N0','vi-VN') AS [Tiền sân],CASE b.TrangThai WHEN 'Pending' THEN N'Chờ xác nhận' WHEN 'Confirmed' THEN N'Đã xác nhận' WHEN 'InUse' THEN N'Đang sử dụng' WHEN 'Completed' THEN N'Hoàn thành' WHEN 'Cancelled' THEN N'Đã hủy' ELSE b.TrangThai END AS [Trạng thái] FROM DatSan b JOIN KhachHang c ON c.KhachHangID=b.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID ORDER BY b.NgayTao DESC");

            donut.Available=available; donut.Busy=busyFields; donut.Maintenance=maintenance; donut.Invalidate();
            var dt = Db.Query(@";WITH d AS (SELECT 0 n UNION ALL SELECT n+1 FROM d WHERE n<6) SELECT DATEADD(day,-n,CAST(GETDATE() AS date)) TheDate,ISNULL((SELECT SUM(p.SoTien) FROM ThanhToan p WHERE p.TrangThai='Confirmed' AND CAST(p.NgayThanhToan AS DATE)=CAST(DATEADD(day,-n,GETDATE()) AS DATE)),0) Value FROM d OPTION(MAXRECURSION 7)");
            chart.Items = dt.Rows.Cast<System.Data.DataRow>().OrderBy(r=>Convert.ToDateTime(r["TheDate"])).Select(r=>(Convert.ToDateTime(r["TheDate"]).ToString("dd/MM"),Convert.ToDecimal(r["Value"]))).ToList();
            chart.Invalidate();
        }
        catch(Exception ex){ UiMsg.Warn(ex.Message, "Dashboard"); }
    }
}
