using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Admin;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Employee;

public partial class FrmEmployeeDashboard : Form
{
    private readonly DashboardCard cBookings = new("Đặt sân hôm nay", "0", "↑ So với hôm qua", AppTheme.Success, "▣");
    private readonly DashboardCard cRevenue = new("Doanh thu hôm nay", "0 đ", "↑ Tổng tiền đã thu", AppTheme.Info, "$");
    private readonly DashboardCard cCustomers = new("Khách hàng mới", "0", "↑ Trong 30 ngày", AppTheme.Purple, "♙");
    private readonly DashboardCard cPending = new("Cần xử lý", "0", "Hóa đơn chờ thanh toán", AppTheme.Warning, "!");

    public FrmEmployeeDashboard()
    {
        InitializeComponent();AppTheme.Upgrade(this);
        lblHello.Text=$"☀  Xin chào, {SessionContext.FullName}!";
        AppTheme.ApplyDecorativeIcon(lblHello);
        foreach (var c in new[] { cBookings, cRevenue, cCustomers, cPending }) flpCards.Controls.Add(c);
        AppTheme.StyleGrid(gridToday);
        BuildQuickActions();
        Shown += (_, __) => LoadData();
    }

    private void BuildQuickActions()
    {
        AddQuick("Đặt sân mới",SportIcon.Booking,AppTheme.Success,()=>OpenDialog(new FrmBooking(),"Đặt sân"),0,0);
        AddQuick("Khách hàng",SportIcon.Customer,AppTheme.Info,()=>OpenDialog(new FrmCustomers(),"Khách hàng"),1,0);
        AddQuick("Hóa đơn",SportIcon.Invoice,AppTheme.Purple,()=>OpenDialog(new FrmInvoices(),"Hóa đơn"),0,1);
        AddQuick("Voucher",SportIcon.Voucher,AppTheme.Warning,()=>OpenDialog(new FrmVouchers(),"Voucher"),1,1);
    }
    private void AddQuick(string text,SportIcon icon,Color color,Action action,int col,int row){var b=new RoundedButton{Dock=DockStyle.Fill,Margin=new Padding(5),Text=text,Image=SportIcons.Get(icon,18,Color.White),TextImageRelation=TextImageRelation.ImageBeforeText,BackColor=color,ForeColor=Color.White,Font=new Font("Segoe UI Semibold",8.4F,FontStyle.Bold),Radius=10,HoverColor=ControlPaint.Dark(color,.08f)};b.Click+=(_,__)=>action();quickGrid.Controls.Add(b,col,row);}
    private void OpenDialog(Form f,string title){f.Text=title;f.StartPosition=FormStartPosition.CenterParent;f.Size=new Size(Math.Min(1100,Width-70),Math.Min(720,Height-70));AppTheme.ApplyToForm(f,"Employee");f.ShowDialogFx(this);LoadData();}

    private void LoadData()
    {
        try
        {
            cBookings.SetValue(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM DatSan WHERE CAST(ThoiGianBatDau AS DATE)=CAST(GETDATE() AS DATE) AND TrangThai<>'Cancelled'") ?? 0).ToString(),"↑ Lịch đặt hôm nay");
            cRevenue.SetValue(Convert.ToDecimal(Db.Scalar("SELECT ISNULL(SUM(SoTien),0) FROM ThanhToan WHERE TrangThai='Confirmed' AND CAST(NgayThanhToan AS DATE)=CAST(GETDATE() AS DATE)") ?? 0).ToString("N0") + " đ","↑ Doanh thu thực thu");
            cCustomers.SetValue(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM KhachHang WHERE NgayTao>=DATEADD(DAY,-30,SYSDATETIME())") ?? 0).ToString(),"↑ Trong 30 ngày");
            cPending.SetValue(Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM HoaDon WHERE TrangThaiThanhToan<>'Paid'") ?? 0).ToString(),"Hóa đơn chờ thanh toán");

            gridToday.DataSource = Db.Query(@"
SELECT CONVERT(varchar(5),b.ThoiGianBatDau,108)+' - '+CONVERT(varchar(5),b.ThoiGianKetThuc,108) [Thời gian],
       f.TenSan [Sân],c.HoTen [Khách hàng],ft.TenLoaiSan [Loại sân],
       CASE b.TrangThai WHEN 'Pending' THEN N'Đang chờ' WHEN 'Confirmed' THEN N'Đã xác nhận' WHEN 'InUse' THEN N'Đang sử dụng' WHEN 'Completed' THEN N'Hoàn thành' WHEN 'Cancelled' THEN N'Đã hủy' ELSE b.TrangThai END [Trạng thái]
FROM DatSan b
JOIN SanTheThao f ON f.SanID=b.SanID
JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID
JOIN KhachHang c ON c.KhachHangID=b.KhachHangID
WHERE CAST(b.ThoiGianBatDau AS DATE)=CAST(GETDATE() AS DATE)
ORDER BY b.ThoiGianBatDau");

            var dt = Db.Query(@"
;WITH d AS(SELECT 0 n UNION ALL SELECT n+1 FROM d WHERE n<6)
SELECT DATEADD(day,-n,CAST(GETDATE() AS date)) TheDate,
       ISNULL((SELECT SUM(p.SoTien) FROM ThanhToan p WHERE p.TrangThai='Confirmed' AND CAST(p.NgayThanhToan AS DATE)=CAST(DATEADD(day,-n,GETDATE()) AS DATE)),0) Value
FROM d OPTION(MAXRECURSION 7)");
            chart.Items = dt.Rows.Cast<System.Data.DataRow>().OrderBy(r=>Convert.ToDateTime(r["TheDate"]))
                .Select(r => (Convert.ToDateTime(r["TheDate"]).ToString("dd/MM"), Convert.ToDecimal(r["Value"]))).ToList();
            chart.Invalidate();
        }
        catch (Exception ex) { UiMsg.Error(ex.Message); }
    }
}
