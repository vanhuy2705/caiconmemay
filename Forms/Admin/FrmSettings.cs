using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Services;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmSettings:Form
{
    private readonly SettingsService _svc=new();
    public FrmSettings(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StylePrimary(btnSave);
        btnSave.Click+=(_,__)=>Save();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            txtCompany.Text=_svc.Get("CompanyName");
            txtBankName.Text=_svc.Get("BankName");
            txtBankBin.Text=_svc.Get("BankBin");
            txtAccount.Text=_svc.Get("BankAccountNumber");
            txtAccountName.Text=_svc.Get("BankAccountName");
            txtPrefix.Text=_svc.Get("QrTransferPrefix","THANHTOAN");
            txtOpen.Text=_svc.Get("OpenTime","05:00");
            txtClose.Text=_svc.Get("CloseTime","23:00");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải cấu hình");}
    }
    private void Save(){
        if(string.IsNullOrWhiteSpace(txtCompany.Text)){UiMsg.Warn("Tên hệ thống không được để trống.");return;}
        if(string.IsNullOrWhiteSpace(txtBankBin.Text)||txtBankBin.Text.Any(c=>!char.IsDigit(c))||string.IsNullOrWhiteSpace(txtAccount.Text)||txtAccount.Text.Any(c=>!char.IsDigit(c))){UiMsg.Warn("Bank BIN và số tài khoản chỉ được chứa chữ số.");return;}
        if(!TimeSpan.TryParse(txtOpen.Text.Trim(),out var open)||!TimeSpan.TryParse(txtClose.Text.Trim(),out var close)||close<=open){UiMsg.Warn("Giờ hoạt động chưa hợp lệ. Hãy nhập dạng HH:mm và giờ đóng phải sau giờ mở.");return;}
        try{
            _svc.Set("CompanyName",txtCompany.Text.Trim());
            _svc.Set("BankName",txtBankName.Text.Trim());
            _svc.Set("BankBin",txtBankBin.Text.Trim());
            _svc.Set("BankAccountNumber",txtAccount.Text.Trim());
            _svc.Set("BankAccountName",txtAccountName.Text.Trim().ToUpperInvariant());
            _svc.Set("QrTransferPrefix",string.IsNullOrWhiteSpace(txtPrefix.Text)?"THANHTOAN":txtPrefix.Text.Trim().ToUpperInvariant());
            _svc.Set("OpenTime",open.ToString(@"hh\:mm"));
            _svc.Set("CloseTime",close.ToString(@"hh\:mm"));
            Toast.Success("Đã lưu cấu hình.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu cấu hình");}
    }
}
