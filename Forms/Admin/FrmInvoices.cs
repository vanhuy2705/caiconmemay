using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmInvoices:Form
{
    private readonly PaymentService _payment = new();
    public FrmInvoices(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StylePrimary(btnPay);
        AppTheme.StyleSecondary(btnConfirmTransfer);
        AppTheme.StyleSecondary(btnHistory);
        cboStatus.Items.AddRange(new object[]{"Tất cả","Chưa thanh toán","Thanh toán một phần","Đã thanh toán"});
        cboStatus.SelectedIndex=0;
        txtSearch.TextChanged+=(_,__)=>LoadData();
        cboStatus.SelectedIndexChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnPay.Click+=(_,__)=>Pay();
        btnConfirmTransfer.Click+=(_,__)=>ConfirmPending();
        btnHistory.Click+=(_,__)=>ShowPaymentHistory();
        grid.CellDoubleClick+=(_,__)=>ShowPaymentHistory();
        Shown+=(_,__)=>LoadData();
    }
    private string StatusCode()=> (Convert.ToString(cboStatus.SelectedItem) ?? "") switch{"Chưa thanh toán"=>"Unpaid","Thanh toán một phần"=>"PartiallyPaid","Đã thanh toán"=>"Paid",_=>""};
    private void LoadData(){
        try{
            var st=StatusCode();
            grid.DataSource=Db.Query(@"SELECT i.HoaDonID,b.DatSanID,i.MaHoaDon [Mã HĐ],b.MaDatSan [Mã đơn],c.HoTen [Khách hàng],f.TenSan [Sân],i.TongTien [Tổng tiền],i.SoTienDaTra [Đã trả],CASE i.TrangThaiThanhToan WHEN 'Unpaid' THEN N'Chưa thanh toán' WHEN 'PartiallyPaid' THEN N'Thanh toán một phần' WHEN 'Paid' THEN N'Đã thanh toán' ELSE i.TrangThaiThanhToan END [Trạng thái],i.NgayTao [Ngày tạo] FROM HoaDon i JOIN DatSan b ON b.DatSanID=i.DatSanID JOIN KhachHang c ON c.KhachHangID=i.KhachHangID JOIN SanTheThao f ON f.SanID=b.SanID WHERE (@st='' OR i.TrangThaiThanhToan=@st) AND (@s='' OR i.MaHoaDon LIKE '%'+@s+'%' OR b.MaDatSan LIKE '%'+@s+'%' OR c.HoTen LIKE N'%'+@s+'%') ORDER BY i.NgayTao DESC",new SqlParameter("@st",st),new SqlParameter("@s",txtSearch.Text.Trim()));
            foreach(var c in new[]{"HoaDonID","DatSanID"})if(grid.Columns.Contains(c))grid.Columns[c].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải hóa đơn");}
    }
    private void Pay(){
        if(grid.CurrentRow==null){UiMsg.Warn("Chọn hóa đơn cần thanh toán.");return;}
        var cell=grid.CurrentRow.Cells["DatSanID"].Value;
        if(cell==null || cell==DBNull.Value){UiMsg.Warn("Không xác định được đơn đặt.");return;}
        var bookingId=Convert.ToInt32(cell);
        using var f=new FrmPayment(bookingId);
        f.ShowDialogFx(this);
        LoadData();
    }
    private void ConfirmPending(){
        if(grid.CurrentRow==null){UiMsg.Warn("Chọn hóa đơn cần xác nhận.");return;}
        var cell=grid.CurrentRow.Cells["HoaDonID"].Value;
        if(cell==null || cell==DBNull.Value){UiMsg.Warn("Không xác định được hóa đơn.");return;}
        var invoiceId=Convert.ToInt32(cell);
        try{
            var dt=Db.Query("SELECT TOP 1 SoTien,PhuongThucThanhToan,MaGiaoDich FROM ThanhToan WHERE HoaDonID=@i AND TrangThai='Pending' ORDER BY NgayTao DESC",new SqlParameter("@i",invoiceId));
            if(dt.Rows.Count==0){UiMsg.Warn("Hóa đơn này không có giao dịch chuyển khoản/QR đang chờ xác nhận.");return;}
            var r=dt.Rows[0];
            var soTien= r["SoTien"]==DBNull.Value ? 0 : Convert.ToDecimal(r["SoTien"]);
            var pttt=Convert.ToString(r["PhuongThucThanhToan"]) ?? "";
            if(UiMsg.Ask($"Xác nhận đã nhận {soTien:N0} đ qua {pttt}?","Xác nhận tiền về")!=DialogResult.Yes)return;
            if(!_payment.ConfirmLatestPending(invoiceId)){UiMsg.Warn("Giao dịch không còn ở trạng thái chờ xác nhận.");return;}
            LoadData();Toast.Success("Đã xác nhận tiền về.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Xác nhận thanh toán");}
    }
    private void ShowPaymentHistory(){
        if(grid.CurrentRow==null){UiMsg.Warn("Chọn hóa đơn để xem lịch sử.");return;}
        var cell=grid.CurrentRow.Cells["HoaDonID"].Value;
        if(cell==null || cell==DBNull.Value){UiMsg.Warn("Không xác định được hóa đơn.");return;}
        var invoiceId=Convert.ToInt32(cell);
        try{
            var dt=Db.Query(@"SELECT NgayTao [Ngày tạo], SoTien [Số tiền], PhuongThucThanhToan [Phương thức], MaGiaoDich [Mã GD], TrangThai [Trạng thái], NgayThanhToan [Ngày TT] FROM ThanhToan WHERE HoaDonID=@id ORDER BY NgayTao DESC", new SqlParameter("@id",invoiceId));
            using var f=new Form{Text=$"Lịch sử thanh toán HD{invoiceId:000000}",StartPosition=FormStartPosition.CenterParent,Size=new Size(700,380),BackColor=Color.White};
            var g=new DataGridView{Dock=DockStyle.Fill, DataSource=dt, BackgroundColor=Color.White, BorderStyle=BorderStyle.None, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, ReadOnly=true, AllowUserToAddRows=false};
            AppTheme.StyleGrid(g);
            f.Controls.Add(g);
            AppTheme.Upgrade(f);
            f.ShowDialogFx(this);
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lịch sử thanh toán");}
    }
}
