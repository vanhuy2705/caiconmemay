using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Common;
public partial class FrmPayment:Form
{
    private readonly int _bookingId;
    private readonly PaymentService _payment=new();
    private readonly SettingsService _settings=new();
    private int _invoiceId;
    private decimal _amount;
    private string _bookingCode="";
    public FrmPayment(int bookingId){
        _bookingId=bookingId;
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        Opacity=0;Shown+=(_,__)=>Fx.FadeIn(this,160);
        AppTheme.StylePrimary(btnConfirm);
        AppTheme.StyleSecondary(btnClose);
        if(SessionContext.Role=="Customer") cboMethod.Items.AddRange(new object[]{"QR chuyển khoản","Chuyển khoản","Chưa thanh toán"});
        else cboMethod.Items.AddRange(new object[]{"Tiền mặt","QR chuyển khoản","Chuyển khoản","Chưa thanh toán"});
        cboMethod.SelectedIndex=0;
        cboMethod.SelectedIndexChanged+=(_,__)=>RenderMethod();
        btnConfirm.Click+=(_,__)=>Confirm();
        btnClose.Click+=(_,__)=>Close();
        Shown+=(_,__)=>LoadInfo();
    }
    private void LoadInfo(){
        try{
            var r=_payment.GetPaymentInfo(_bookingId);
            _invoiceId= r["HoaDonID"]==DBNull.Value ? 0 : Convert.ToInt32(r["HoaDonID"]);
            var tong= r["TongTien"]==DBNull.Value ? 0 : Convert.ToDecimal(r["TongTien"]);
            var daTra= r["SoTienDaTra"]==DBNull.Value ? 0 : Convert.ToDecimal(r["SoTienDaTra"]);
            _amount=tong-daTra;
            _bookingCode=Convert.ToString(r["MaDatSan"]) ?? "";
            var trangThai=Convert.ToString(r["TrangThai"]) ?? "";
            if(trangThai=="Cancelled"){UiMsg.Warn("Đơn đặt sân đã hủy nên không thể thanh toán.");Close();return;}
            var tenSan=Convert.ToString(r["TenSan"]) ?? "";
            var batDau= r["ThoiGianBatDau"]==DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["ThoiGianBatDau"]);
            var ketThuc= r["ThoiGianKetThuc"]==DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["ThoiGianKetThuc"]);
            lblBooking.Text=$"{_bookingCode} • {tenSan} • {batDau:dd/MM HH:mm} - {ketThuc:HH:mm}";
            lblAmount.Text=_amount.ToString("N0")+" đ";
            RenderMethod();
        }catch(Exception ex){UiMsg.Error(ex.Message);Close();}
    }
    private void RenderMethod(){
        var txt=Convert.ToString(cboMethod.SelectedItem) ?? "";
        var show=txt=="QR chuyển khoản"||txt=="Chuyển khoản";
        pnlQr.Visible=show;
        if(!show) return;
        try{
            var bank=_settings.Get("BankName","TECHCOMBANK");
            var bin=_settings.Get("BankBin","970407");
            var acc=_settings.Get("BankAccountNumber","");
            var name=_settings.Get("BankAccountName","");
            var prefix=_settings.Get("QrTransferPrefix","THANHTOAN");
            var content=$"{prefix} {_bookingCode}";
            lblBank.Text="Ngân hàng: "+bank;
            lblAccount.Text="Số tài khoản: "+acc;
            lblAccountName.Text="Chủ tài khoản: "+name;
            lblContent.Text="Nội dung: "+content;
            picQr.Image?.Dispose();
            picQr.Image=QrImageHelper.Create(VietQrPayloadBuilder.Build(bin,acc,_amount,content),6);
        }catch(Exception ex){picQr.Image=null;lblContent.Text="Chưa thể tạo QR: "+ex.Message;}
    }
    private void Confirm(){
        if(_amount<=0){UiMsg.Warn("Hóa đơn đã thanh toán đủ.");return;}
        var selected=Convert.ToString(cboMethod.SelectedItem) ?? "";
        string method=selected switch{"Tiền mặt"=>"Cash","QR chuyển khoản"=>"QR","Chuyển khoản"=>"BankTransfer",_=>"Unpaid"};
        if(method=="Unpaid"){Close();return;}
        if((method=="QR"||method=="BankTransfer")&&string.IsNullOrWhiteSpace(_settings.Get("BankAccountNumber"))){UiMsg.Warn("Chưa cấu hình số tài khoản nhận tiền. Quản trị viên cần cập nhật trong Cấu hình hệ thống.");return;}
        var isStaff=SessionContext.Role=="Admin"||SessionContext.Role=="Employee";
        var confirmPaid=method=="Cash"&&isStaff;
        if(method=="QR"||method=="BankTransfer") confirmPaid=false;
        try{
            _payment.RecordPayment(_invoiceId,_amount,method,txtReference.Text.Trim(),confirmPaid);
            if(confirmPaid) Toast.Success("Đã ghi nhận thanh toán thành công.");
            else Toast.Info("Đã ghi nhận yêu cầu thanh toán. Chuyển khoản/QR sẽ ở trạng thái chờ xác nhận cho đến khi nhân viên xác nhận tiền về.");
            DialogResult=DialogResult.OK;
            Close();
        }catch(Exception ex){UiMsg.Error(ex.Message, "Thanh toán");}
    }
}
