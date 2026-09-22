using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmAccounts:Form
{
    public FrmAccounts(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StyleSecondary(btnToggle);
        AppTheme.StylePrimary(btnReset);
        txtSearch.TextChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnToggle.Click+=(_,__)=>Toggle();
        btnReset.Click+=(_,__)=>ResetPassword();
        Shown+=(_,__)=>LoadData();
    }
    private int? Id(){
        if(grid.CurrentRow==null) return null;
        var v=grid.CurrentRow.Cells["TaiKhoanID"].Value;
        if(v==null || v==DBNull.Value) return null;
        return Convert.ToInt32(v);
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query(@"SELECT u.TaiKhoanID,u.TenDangNhap [Tài khoản],r.TenHienThi [Vai trò],COALESCE(c.HoTen,e.HoTen,u.TenDangNhap) [Họ tên],u.DangHoatDong [Hoạt động],u.DangNhapLanCuoi [Đăng nhập cuối],u.NgayTao [Ngày tạo] FROM TaiKhoan u JOIN VaiTro r ON r.VaiTroID=u.VaiTroID LEFT JOIN KhachHang c ON c.TaiKhoanID=u.TaiKhoanID LEFT JOIN NhanVien e ON e.TaiKhoanID=u.TaiKhoanID WHERE @s='' OR u.TenDangNhap LIKE '%'+@s+'%' OR COALESCE(c.HoTen,e.HoTen,u.TenDangNhap) LIKE N'%'+@s+'%' ORDER BY u.TaiKhoanID",new SqlParameter("@s",txtSearch.Text.Trim()));
            if(grid.Columns.Contains("TaiKhoanID"))grid.Columns["TaiKhoanID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải tài khoản");}
    }
    private void Toggle(){
        var id=Id();
        if(id==null){UiMsg.Warn("Chọn tài khoản cần đổi trạng thái.");return;}
        if(id==SessionContext.UserId){UiMsg.Warn("Không thể khóa tài khoản đang đăng nhập.");return;}
        try{
            Db.Execute("UPDATE TaiKhoan SET DangHoatDong=CASE WHEN DangHoatDong=1 THEN 0 ELSE 1 END WHERE TaiKhoanID=@id",new SqlParameter("@id",id.Value));
            LoadData();Toast.Success("Đã đổi trạng thái tài khoản.");
        }catch(Exception ex){UiMsg.Error(ex.Message);}
    }
    private void ResetPassword(){
        var id=Id();if(id==null){UiMsg.Warn("Chọn tài khoản cần đặt lại mật khẩu.");return;}
        using var f=new Form{Text="Đặt lại mật khẩu",StartPosition=FormStartPosition.CenterParent,Size=new Size(420,190),FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false};
        var tb=new TextBox{Left=20,Top=35,Width=360,UseSystemPasswordChar=true,PlaceholderText="Mật khẩu mới"};
        var b=new Button{Left=230,Top=85,Width=150,Height=38,Text="Cập nhật"};
        AppTheme.StylePrimary(b);
        b.Click+=(_,__)=>{
            if(tb.Text.Length<6){UiMsg.Warn("Mật khẩu tối thiểu 6 ký tự.");return;}
            try{
                var salt=PasswordHelper.CreateSalt();
                var hash=PasswordHelper.Hash(salt,tb.Text);
                Db.Execute("UPDATE TaiKhoan SET MuoiMatKhau=@s,MatKhauBam=@h WHERE TaiKhoanID=@id",new SqlParameter("@s",salt),new SqlParameter("@h",hash),new SqlParameter("@id",id.Value));
                Toast.Success("Đã đặt lại mật khẩu.");f.Close();
            }catch(Exception ex){UiMsg.Error(ex.Message);}
        };
        f.Controls.AddRange(new Control[]{tb,b});AppTheme.Upgrade(f);f.ShowDialogFx(this);
    }
}
