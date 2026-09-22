using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Forms.Common;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerInvoices:Form
{
    public FrmCustomerInvoices(){InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnRefresh);AppTheme.StylePrimary(btnPay);btnRefresh.Click+=(_,__)=>LoadData();btnPay.Click+=(_,__)=>Pay();Shown+=(_,__)=>LoadData();}
    private void LoadData(){grid.DataSource=Db.Query(@"SELECT i.HoaDonID,b.DatSanID,i.MaHoaDon [Mã HĐ],b.MaDatSan [Mã đơn],f.TenSan [Sân],i.TienGoc [Tiền sân],i.TienGiam [Giảm],i.TongTien [Tổng tiền],i.SoTienDaTra [Đã trả],CASE i.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa thanh toán' WHEN 'PartiallyPaid' THEN N'Thanh toán một phần' WHEN 'Paid' THEN N'Đã thanh toán' ELSE i.TrangThaiThanhToan END [Trạng thái],i.NgayTao [Ngày tạo] FROM HoaDon i JOIN DatSan b ON b.DatSanID=i.DatSanID JOIN SanTheThao f ON f.SanID=b.SanID WHERE i.KhachHangID=@c ORDER BY i.NgayTao DESC",new SqlParameter("@c",SessionContext.CustomerId??0));foreach(var c in new[]{"HoaDonID","DatSanID"})if(grid.Columns.Contains(c))grid.Columns[c].Visible=false;}
    private void Pay(){if(grid.CurrentRow==null)return;var id=Convert.ToInt32(grid.CurrentRow.Cells["DatSanID"].Value);using var f=new FrmPayment(id);f.ShowDialogFx(this);LoadData();}
}
