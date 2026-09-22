using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerInvoices:Form
{
    public FrmCustomerInvoices(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StylePrimary(btnPay);
        txtSearch.TextChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnPay.Click+=(_,__)=>Pay();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query(@"SELECT i.HoaDonID,b.DatSanID,i.MaHoaDon [Mã HĐ],b.MaDatSan [Mã đơn],f.TenSan [Sân],i.TongTien [Tổng tiền],i.SoTienDaTra [Đã trả],CASE i.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa thanh toán' WHEN 'PartiallyPaid' THEN N'Thanh toán một phần' WHEN 'Paid' THEN N'Đã thanh toán' ELSE i.TrangThaiThanhToan END [Trạng thái],i.NgayTao [Ngày tạo] FROM HoaDon i JOIN DatSan b ON b.DatSanID=i.DatSanID JOIN SanTheThao f ON f.SanID=b.SanID WHERE i.KhachHangID=@c AND (@s='' OR i.MaHoaDon LIKE '%'+@s+'%' OR b.MaDatSan LIKE '%'+@s+'%') ORDER BY i.NgayTao DESC",
                new SqlParameter("@c",SessionContext.CustomerId??0), new SqlParameter("@s",txtSearch.Text.Trim()));
            foreach(var c in new[]{"HoaDonID","DatSanID"}) if(grid.Columns.Contains(c)) grid.Columns[c].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải hóa đơn");}
    }
    private void Pay(){
        if(grid.CurrentRow==null){UiMsg.Warn("Chọn hóa đơn cần thanh toán.");return;}
        var v=grid.CurrentRow.Cells["DatSanID"].Value;
        if(v==null || v==DBNull.Value){UiMsg.Warn("Không xác định được đơn đặt.");return;}
        var bookingId=Convert.ToInt32(v);
        using var f=new FrmPayment(bookingId);f.ShowDialogFx(this);LoadData();
    }
}
