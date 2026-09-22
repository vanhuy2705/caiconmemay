using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmInvoices:Form
{
    private readonly PaymentService _payment = new();
    public FrmInvoices(){InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnRefresh);AppTheme.StylePrimary(btnPay);AppTheme.StyleSecondary(btnConfirmTransfer);cboStatus.Items.AddRange(new object[]{"Tất cả","Chưa thanh toán","Thanh toán một phần","Đã thanh toán"});cboStatus.SelectedIndex=0;txtSearch.TextChanged+=(_,__)=>LoadData();cboStatus.SelectedIndexChanged+=(_,__)=>LoadData();btnRefresh.Click+=(_,__)=>LoadData();btnPay.Click+=(_,__)=>Pay();btnConfirmTransfer.Click+=(_,__)=>ConfirmPending();Shown+=(_,__)=>LoadData();}
    private string StatusCode()=>cboStatus.Text switch{"Chưa thanh toán"=>"Unpaid","Thanh toán một phần"=>"PartiallyPaid","Đã thanh toán"=>"Paid",_=>""};
    private void LoadData(){var st=StatusCode();grid.DataSource=Db.Query(@"SELECT i.HoaDonID,b.DatSanID,i.MaHoaDon [Mã HĐ],b.MaDatSan [Mã đơn],c.HoTen [Khách hàng],f.TenSan [Sân],i.TongTien [Tổng tiền],i.SoTienDaTra [Đã trả],CASE i.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa thanh toán' WHEN 'PartiallyPaid' THEN N'Thanh toán một phần' WHEN 'Paid' THEN N'Đã thanh toán' ELSE i.TrangThaiThanhToan END [Trạng thái],i.NgayTao [Ngày tạo] FROM HoaDon i JOIN DatSan b ON b.DatSanID=i.DatSanID JOIN KhachHang c ON c.KhachHangID=i.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID WHERE (@st='' OR i.TrangThaiThanhToan=@st) AND (@s='' OR i.MaHoaDon LIKE '%'+@s+'%' OR b.MaDatSan LIKE '%'+@s+'%' OR c.HoTen LIKE N'%'+@s+'%') ORDER BY i.NgayTao DESC",new SqlParameter("@st",st),new SqlParameter("@s",txtSearch.Text.Trim()));foreach(var c in new[]{"HoaDonID","DatSanID"})if(grid.Columns.Contains(c))grid.Columns[c].Visible=false;}
    private void Pay(){if(grid.CurrentRow==null)return;var bookingId=Convert.ToInt32(grid.CurrentRow.Cells["DatSanID"].Value);using var f=new FrmPayment(bookingId);f.ShowDialogFx(this);LoadData();}
    private void ConfirmPending(){if(grid.CurrentRow==null)return;var invoiceId=Convert.ToInt32(grid.CurrentRow.Cells["HoaDonID"].Value);var dt=Db.Query("SELECT TOP 1 SoTien,PhuongThucThanhToan,MaGiaoDich FROM ThanhToan WHERE HoaDonID=@i AND TrangThai='Pending' ORDER BY NgayTao DESC",new SqlParameter("@i",invoiceId));if(dt.Rows.Count==0){UiMsg.Warn("Hóa đơn này không có giao dịch chuyển khoản/QR đang chờ xác nhận.");return;}var r=dt.Rows[0];if(UiMsg.Ask($"Xác nhận đã nhận {Convert.ToDecimal(r["SoTien"]):N0} đ qua {r["PhuongThucThanhToan"]}?", "Xác nhận tiền về")!=DialogResult.Yes)return;try{if(!_payment.ConfirmLatestPending(invoiceId)){UiMsg.Warn("Giao dịch không còn ở trạng thái chờ xác nhận.");return;}LoadData();Toast.Success("Đã xác nhận tiền về.");}catch(Exception ex){UiMsg.Error(ex.Message,"Xác nhận thanh toán");}}
}
