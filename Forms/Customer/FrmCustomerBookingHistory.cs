using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerBookingHistory:Form
{
    private readonly BookingService _booking = new();
    public FrmCustomerBookingHistory(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StylePrimary(btnPay);
        AppTheme.StyleDanger(btnCancel);
        cboStatus.Items.AddRange(new object[]{"Tất cả","Đã xác nhận","Đang sử dụng","Hoàn thành","Đã hủy"});
        cboStatus.SelectedIndex=0;
        cboStatus.SelectedIndexChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnPay.Click+=(_,__)=>Pay();
        btnCancel.Click+=(_,__)=>CancelBooking();
        Shown+=(_,__)=>LoadData();
    }
    private int? BookingId(){
        if(grid.CurrentRow==null) return null;
        var v=grid.CurrentRow.Cells["DatSanID"].Value;
        if(v==null || v==DBNull.Value) return null;
        return Convert.ToInt32(v);
    }
    private string StatusCode(){
        var txt=Convert.ToString(cboStatus.SelectedItem) ?? "";
        return txt switch{"Đã xác nhận"=>"Confirmed","Đang sử dụng"=>"InUse","Hoàn thành"=>"Completed","Đã hủy"=>"Cancelled",_=>""};
    }
    private void LoadData(){
        try{
            var st=StatusCode();
            grid.DataSource=Db.Query(@"SELECT b.DatSanID,b.MaDatSan [Mã đơn],f.TenSan [Sân],ft.TenLoaiSan [Loại sân],b.ThoiGianBatDau [Bắt đầu],b.ThoiGianKetThuc [Kết thúc],b.TongTien [Tổng tiền],CASE b.TrangThai WHEN 'Pending' THEN N'Chờ xác nhận' WHEN 'Confirmed' THEN N'Đã xác nhận' WHEN 'InUse' THEN N'Đang sử dụng' WHEN 'Completed' THEN N'Hoàn thành' WHEN 'Cancelled' THEN N'Đã hủy' ELSE b.TrangThai END [Trạng thái],CASE b.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa thanh toán' WHEN 'PartiallyPaid' THEN N'Thanh toán một phần' WHEN 'Paid' THEN N'Đã thanh toán' ELSE b.TrangThaiThanhToan END [Thanh toán] FROM DatSan b JOIN SanTheThao f ON f.SanID=b.SanID JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID WHERE b.KhachHangID=@c AND (@s='' OR b.TrangThai=@s) ORDER BY b.ThoiGianBatDau DESC",new SqlParameter("@c",SessionContext.CustomerId??0),new SqlParameter("@s",st));
            if(grid.Columns.Contains("DatSanID"))grid.Columns["DatSanID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải lịch sử");}
    }
    private void Pay(){
        var id=BookingId();if(id==null){UiMsg.Warn("Chọn lịch cần thanh toán.");return;}
        using var f=new FrmPayment(id.Value);f.ShowDialogFx(this);LoadData();
    }
    private void CancelBooking(){
        var id=BookingId();if(id==null){UiMsg.Warn("Chọn lịch cần hủy.");return;}
        var row=grid.CurrentRow;if(row==null) return;
        var statusCell=row.Cells["Trạng thái"].Value;
        var status= statusCell==null ? "" : Convert.ToString(statusCell) ?? "";
        if(status=="Đã hủy"||status=="Hoàn thành"){UiMsg.Warn("Đơn này không thể hủy.");return;}
        if(UiMsg.Ask("Bạn chắc chắn muốn hủy lịch đặt? Đơn đã nhận tiền cần liên hệ nhân viên để xử lý.","Xác nhận")!=DialogResult.Yes)return;
        try{_booking.SetStatus(id.Value,"Cancelled",SessionContext.CustomerId??0);LoadData();Toast.Success("Đã hủy lịch đặt.");}
        catch(Exception ex){UiMsg.Warn(ex.Message,"Hủy lịch");}
    }
}
