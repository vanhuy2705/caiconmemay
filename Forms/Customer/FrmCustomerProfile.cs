using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Customer;
public partial class FrmCustomerProfile:Form
{
    public FrmCustomerProfile(){InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StylePrimary(btnSave);AppTheme.StyleSecondary(btnChangePassword);btnSave.Click+=(_,__)=>Save();btnChangePassword.Click+=(_,__)=>ChangePassword();Shown+=(_,__)=>LoadData();}
    private void LoadData(){
        try{
            var dt=Db.Query("SELECT MaKhachHang,HoTen,SoDienThoai,Email,DiaChi,NgaySinh,DiemTichLuy FROM KhachHang WHERE KhachHangID=@id",new SqlParameter("@id",SessionContext.CustomerId??0));
            if(dt.Rows.Count==0)return;
            var r=dt.Rows[0];
            txtCode.Text=Convert.ToString(r["MaKhachHang"]);
            txtName.Text=Convert.ToString(r["HoTen"]);
            txtPhone.Text=Convert.ToString(r["SoDienThoai"]);
            txtEmail.Text=Convert.ToString(r["Email"]);
            txtAddress.Text=Convert.ToString(r["DiaChi"]);
            if(r["NgaySinh"]!=DBNull.Value)dtBirth.Value=Convert.ToDateTime(r["NgaySinh"]);
            lblPoints.Text="Điểm tích lũy: "+Convert.ToString(r["DiemTichLuy"]);
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải hồ sơ");}
    }
    private void Save(){
        if(string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Họ tên không được để trống.");return;}
        var phone=txtPhone.Text.Trim();
        var email=txtEmail.Text.Trim();
        if(phone.Length>0 && (phone.Length<9 || phone.Any(c=>!char.IsDigit(c)))){UiMsg.Warn("SĐT phải có ít nhất 9 chữ số.");return;}
        if(email.Length>0 && (!email.Contains('@') || !email.Contains('.'))){UiMsg.Warn("Email chưa đúng định dạng.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN @p<>'' AND EXISTS(SELECT 1 FROM KhachHang WHERE SoDienThoai=@p AND KhachHangID<>@id) THEN N'Số điện thoại đã tồn tại'
                WHEN @e<>'' AND EXISTS(SELECT 1 FROM KhachHang WHERE Email=@e AND KhachHangID<>@id) THEN N'Email đã tồn tại' ELSE '' END",
                new SqlParameter("@p",phone), new SqlParameter("@e",email), new SqlParameter("@id",SessionContext.CustomerId??0));
            var msg=Convert.ToString(dup.Rows[0][0])??"";
            if(msg.Length>0){UiMsg.Warn(msg);return;}

            Db.Execute("UPDATE KhachHang SET HoTen=@n,SoDienThoai=NULLIF(@p,N''),Email=NULLIF(@e,N''),DiaChi=NULLIF(@a,N''),NgaySinh=@d WHERE KhachHangID=@id",
                new SqlParameter("@n",txtName.Text.Trim()),
                new SqlParameter("@p",phone),
                new SqlParameter("@e",email),
                new SqlParameter("@a",txtAddress.Text.Trim()),
                new SqlParameter("@d",dtBirth.Value.Date),
                new SqlParameter("@id",SessionContext.CustomerId??0));
            SessionContext.FullName=txtName.Text.Trim();
            Toast.Success("Đã cập nhật thông tin.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu hồ sơ");}
    }
    private void ChangePassword(){
        using var f=new Form{Text="Đổi mật khẩu",StartPosition=FormStartPosition.CenterParent,Size=new Size(430,280),FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false,BackColor=Color.White,Padding=new Padding(20)};
        var oldP=new TextBox{Left=20,Top=20,Width=360,PlaceholderText="Mật khẩu hiện tại",UseSystemPasswordChar=true};
        var newP=new TextBox{Left=20,Top=70,Width=360,PlaceholderText="Mật khẩu mới (tối thiểu 6 ký tự)",UseSystemPasswordChar=true};
        var confirm=new TextBox{Left=20,Top=120,Width=360,PlaceholderText="Nhập lại mật khẩu mới",UseSystemPasswordChar=true};
        var b=new Button{Left=230,Top=170,Width=150,Height=40,Text="Đổi mật khẩu"};
        AppTheme.StylePrimary(b);
        b.Click+=(_,__)=>{
            try{
                var dt=Db.Query("SELECT MatKhauBam,MuoiMatKhau FROM TaiKhoan WHERE TaiKhoanID=@id",new SqlParameter("@id",SessionContext.UserId));
                if(dt.Rows.Count==0)return;
                var salt=Convert.ToString(dt.Rows[0]["MuoiMatKhau"])??"";
                if(!PasswordHelper.Verify(salt,oldP.Text,Convert.ToString(dt.Rows[0]["MatKhauBam"])??"")){UiMsg.Warn("Mật khẩu hiện tại không đúng.");return;}
                if(newP.Text.Length<6||newP.Text!=confirm.Text){UiMsg.Warn("Mật khẩu mới tối thiểu 6 ký tự và phải nhập lại chính xác.");return;}
                var ns=PasswordHelper.CreateSalt();
                Db.Execute("UPDATE TaiKhoan SET MuoiMatKhau=@s,MatKhauBam=@h WHERE TaiKhoanID=@id",new SqlParameter("@s",ns),new SqlParameter("@h",PasswordHelper.Hash(ns,newP.Text)),new SqlParameter("@id",SessionContext.UserId));
                Toast.Success("Đã đổi mật khẩu.");f.Close();
            }catch(Exception ex){UiMsg.Error(ex.Message,"Đổi mật khẩu");}
        };
        f.Controls.AddRange(new Control[]{oldP,newP,confirm,b});AppTheme.Upgrade(f);f.ShowDialogFx(this);
    }
}
