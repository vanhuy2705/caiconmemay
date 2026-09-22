using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Employee;

public partial class FrmEmployeeAccount : Form
{
    public FrmEmployeeAccount()
    {
        InitializeComponent();
        AppTheme.Upgrade(this);
        AppTheme.StylePrimary(btnSave);AppTheme.StyleSecondary(btnPassword);
        btnSave.Click+=(_,__)=>Save();btnPassword.Click+=(_,__)=>ChangePassword();Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){var dt=Db.Query("SELECT MaNhanVien,HoTen,SoDienThoai,Email,ChucVu FROM NhanVien WHERE NhanVienID=@id",new SqlParameter("@id",SessionContext.EmployeeId??0));if(dt.Rows.Count==0)return;var r=dt.Rows[0];txtCode.Text=Convert.ToString(r["MaNhanVien"]);txtName.Text=Convert.ToString(r["HoTen"]);txtPhone.Text=Convert.ToString(r["SoDienThoai"]);txtEmail.Text=Convert.ToString(r["Email"]);txtPosition.Text=Convert.ToString(r["ChucVu"]);}
    private void Save(){if(string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Họ tên không được để trống.");return;}try{Db.Execute("UPDATE NhanVien SET HoTen=@n,SoDienThoai=NULLIF(@p,N''),Email=NULLIF(@e,N'') WHERE NhanVienID=@id",new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@p",txtPhone.Text.Trim()),new SqlParameter("@e",txtEmail.Text.Trim()),new SqlParameter("@id",SessionContext.EmployeeId??0));SessionContext.FullName=txtName.Text.Trim();Toast.Success("Đã cập nhật thông tin.");}catch(Exception ex){UiMsg.Error(ex.Message,"Cập nhật tài khoản");}}
    private void ChangePassword(){using var f=new Form{Text="Đổi mật khẩu",StartPosition=FormStartPosition.CenterParent,Size=new Size(450,290),FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false,BackColor=AppTheme.Background};var card=new RoundedPanel{Dock=DockStyle.Fill,Padding=new Padding(24),Radius=14,BorderColor=AppTheme.Border};var oldP=new TextBox{Left=26,Top=40,Width=360,PlaceholderText="Mật khẩu hiện tại",UseSystemPasswordChar=true};var newP=new TextBox{Left=26,Top=88,Width=360,PlaceholderText="Mật khẩu mới",UseSystemPasswordChar=true};var confirm=new TextBox{Left=26,Top=136,Width=360,PlaceholderText="Nhập lại mật khẩu mới",UseSystemPasswordChar=true};var b=new RoundedButton{Left=236,Top=188,Width=150,Height=40,Text="Đổi mật khẩu"};AppTheme.StylePrimary(b);foreach(var c in new Control[]{oldP,newP,confirm})AppTheme.StyleInput(c);b.Click+=(_,__)=>{var dt=Db.Query("SELECT MatKhauBam,MuoiMatKhau FROM TaiKhoan WHERE TaiKhoanID=@id",new SqlParameter("@id",SessionContext.UserId));if(dt.Rows.Count==0)return;var salt=Convert.ToString(dt.Rows[0]["MuoiMatKhau"])??"";if(!string.Equals(Convert.ToString(dt.Rows[0]["MatKhauBam"]),PasswordHelper.Hash(salt,oldP.Text),StringComparison.OrdinalIgnoreCase)){UiMsg.Warn("Mật khẩu hiện tại không đúng.");return;}if(newP.Text.Length<6||newP.Text!=confirm.Text){UiMsg.Warn("Mật khẩu mới tối thiểu 6 ký tự và phải nhập lại chính xác.");return;}var ns=Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();Db.Execute("UPDATE TaiKhoan SET MuoiMatKhau=@s,MatKhauBam=@h WHERE TaiKhoanID=@id",new SqlParameter("@s",ns),new SqlParameter("@h",PasswordHelper.Hash(ns,newP.Text)),new SqlParameter("@id",SessionContext.UserId));Toast.Success("Đã đổi mật khẩu.");f.Close();};card.Controls.AddRange(new Control[]{oldP,newP,confirm,b});f.Controls.Add(card);AppTheme.Upgrade(f);f.ShowDialogFx(this);}
}
