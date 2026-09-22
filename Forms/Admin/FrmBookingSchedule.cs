using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmBookingSchedule:Form
{
    private readonly BookingService _booking = new();
    public FrmBookingSchedule(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StylePrimary(btnStart);
        AppTheme.StyleSecondary(btnComplete);
        AppTheme.StyleDanger(btnCancel);
        AppTheme.StyleSecondary(btnHistory);
        cboStatus.Items.AddRange(new object[]{"Tất cả","Đã xác nhận","Đang sử dụng","Hoàn thành","Đã hủy"});
        cboStatus.SelectedIndex=0;
        dtDate.ValueChanged+=(_,__)=>LoadData();
        cboStatus.SelectedIndexChanged+=(_,__)=>LoadData();
        txtSearch.TextChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnStart.Click+=(_,__)=>SetStatus("InUse");
        btnComplete.Click+=(_,__)=>SetStatus("Completed");
        btnCancel.Click+=(_,__)=>SetStatus("Cancelled");
        btnHistory.Click+=(_,__)=>ShowHistory();
        grid.CellDoubleClick+=(_,__)=>ShowHistory();
        Shown+=(_,__)=>LoadData();
    }
    private int? SelectedId(){
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
            var status=StatusCode();
            grid.DataSource=Db.Query(@"SELECT b.DatSanID,b.MaDatSan [Mã đơn],c.HoTen [Khách hàng],f.TenSan [Sân],ft.TenLoaiSan [Loại sân],b.ThoiGianBatDau [Bắt đầu],b.ThoiGianKetThuc [Kết thúc],b.TongTien [Tổng tiền],CASE b.TrangThai WHEN 'Pending' THEN N'Chờ xác nhận' WHEN 'Confirmed' THEN N'Đã xác nhận' WHEN 'InUse' THEN N'Đang sử dụng' WHEN 'Completed' THEN N'Hoàn thành' WHEN 'Cancelled' THEN N'Đã hủy' ELSE b.TrangThai END [Trạng thái],CASE b.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa thanh toán' WHEN 'PartiallyPaid' THEN N'Thanh toán một phần' WHEN 'Paid' THEN N'Đã thanh toán' ELSE b.TrangThaiThanhToan END [Thanh toán] FROM DatSan b JOIN KhachHang c ON c.KhachHangID=b.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID WHERE CAST(b.ThoiGianBatDau AS DATE)=@d AND (@st='' OR b.TrangThai=@st) AND (@s='' OR b.MaDatSan LIKE '%'+@s+'%' OR c.HoTen LIKE N'%'+@s+'%' OR f.TenSan LIKE N'%'+@s+'%') ORDER BY b.ThoiGianBatDau",new SqlParameter("@d",dtDate.Value.Date),new SqlParameter("@st",status),new SqlParameter("@s",txtSearch.Text.Trim()));
            if(grid.Columns.Contains("DatSanID"))grid.Columns["DatSanID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải lịch đặt");}
    }
    private void SetStatus(string status){
        var id=SelectedId();if(id==null){UiMsg.Warn("Chọn lịch cần cập nhật.");return;}
        if(status=="Cancelled"&&UiMsg.Ask("Hủy lịch đặt đã chọn? Đơn đã nhận tiền sẽ không được phép hủy trực tiếp.","Xác nhận")!=DialogResult.Yes)return;
        try{_booking.SetStatus(id.Value,status);LoadData();Toast.Success("Đã cập nhật trạng thái lịch.");}catch(Exception ex){UiMsg.Warn(ex.Message,"Cập nhật lịch");}
    }
    private void ShowHistory(){
        var id=SelectedId();if(id==null){UiMsg.Warn("Chọn lịch để xem lịch sử.");return;}
        try{
            var dt=Db.Query(@"SELECT ThoiDiem [Thời gian], ISNULL(TrangThaiCu,N'(mới)') [Từ trạng thái], TrangThaiMoi [Đến trạng thái], ISNULL(TrangThaiThanhToanCu,N'') [TT Thanh toán cũ], TrangThaiThanhToanMoi [TT Thanh toán mới] FROM LichSuDatSan WHERE DatSanID=@id ORDER BY ThoiDiem DESC", new SqlParameter("@id",id.Value));
            using var f=new Form{Text=$"Lịch sử đơn DS{id.Value:000000}",StartPosition=FormStartPosition.CenterParent,Size=new Size(720,420),BackColor=Color.White};
            var gridHist=new DataGridView{Dock=DockStyle.Fill, DataSource=dt, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, ReadOnly=true, AllowUserToAddRows=false};
            AppTheme.StyleGrid(gridHist);
            f.Controls.Add(gridHist);
            AppTheme.Upgrade(f);
            f.ShowDialogFx(this);
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lịch sử đặt sân");}
    }
}
