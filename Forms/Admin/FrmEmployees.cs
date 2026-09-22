using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmEmployees:Form
{
    private int? _employeeId;private int? _userId;
    public FrmEmployees(){InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnNew);AppTheme.StylePrimary(btnSave);AppTheme.StyleDanger(btnDelete);grid.SelectionChanged+=(_,__)=>Bind();btnNew.Click+=(_,__)=>Clear();btnSave.Click+=(_,__)=>Save();btnDelete.Click+=(_,__)=>Delete();Shown+=(_,__)=>LoadData();}
    private void LoadData(){try{grid.DataSource=Db.Query(@"SELECT e.NhanVienID,e.TaiKhoanID,e.MaNhanVien [Mã NV],e.HoTen [Họ tên],e.SoDienThoai [SĐT],e.Email,e.ChucVu [Chức vụ],u.TenDangNhap [Tài khoản],e.DangHoatDong [Hoạt động] FROM NhanVien e JOIN TaiKhoan u ON u.TaiKhoanID=e.TaiKhoanID ORDER BY e.NhanVienID");foreach(var c in new[]{"NhanVienID","TaiKhoanID"})if(grid.Columns.Contains(c))grid.Columns[c].Visible=false;}catch(Exception ex){UiMsg.Error(ex.Message,"Tải nhân viên");}}
    private void Bind(){if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView v)return;var r=v.Row;_employeeId=Convert.ToInt32(r["NhanVienID"]);_userId=Convert.ToInt32(r["TaiKhoanID"]);txtCode.Text=Convert.ToString(r["Mã NV"]);txtName.Text=Convert.ToString(r["Họ tên"]);txtPhone.Text=Convert.ToString(r["SĐT"]);txtEmail.Text=Convert.ToString(r["Email"]);txtPosition.Text=Convert.ToString(r["Chức vụ"]);txtUsername.Text=Convert.ToString(r["Tài khoản"]);txtUsername.Enabled=false;txtPassword.Clear();chkActive.Checked=Convert.ToBoolean(r["Hoạt động"]);}
    private void Clear(){_employeeId=_userId=null;txtCode.Text="NV"+DateTime.Now.ToString("HHmmss");txtName.Clear();txtPhone.Clear();txtEmail.Clear();txtPosition.Text="Nhân viên";txtUsername.Clear();txtUsername.Enabled=true;txtPassword.Clear();chkActive.Checked=true;}
    private void Save(){
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)||string.IsNullOrWhiteSpace(txtUsername.Text)){UiMsg.Warn("Nhập mã nhân viên, họ tên và tài khoản.");return;}
        var phone=txtPhone.Text.Trim();
        if(phone.Length>0 && (phone.Length<9 || phone.Any(c=>!char.IsDigit(c)))){UiMsg.Warn("SĐT phải có ít nhất 9 chữ số.");return;}
        try{
            // Kiểm tra trùng mã, username, SĐT, email
            if(_employeeId==null){
                var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM NhanVien WHERE MaNhanVien=@c) THEN N'Mã nhân viên đã tồn tại'
                    WHEN EXISTS(SELECT 1 FROM TaiKhoan WHERE TenDangNhap=@u) THEN N'Tên đăng nhập đã tồn tại'
                    WHEN @p<>'' AND EXISTS(SELECT 1 FROM NhanVien WHERE SoDienThoai=@p) THEN N'SĐT đã tồn tại'
                    WHEN @e<>'' AND EXISTS(SELECT 1 FROM NhanVien WHERE Email=@e) THEN N'Email đã tồn tại'
                    ELSE '' END", new SqlParameter("@c",txtCode.Text.Trim()), new SqlParameter("@u",txtUsername.Text.Trim()), new SqlParameter("@p",phone), new SqlParameter("@e",txtEmail.Text.Trim()));
                var msg=Convert.ToString(dup.Rows[0][0])??"";
                if(msg.Length>0){UiMsg.Warn(msg);return;}
                if(txtPassword.Text.Length<6){UiMsg.Warn("Mật khẩu tối thiểu 6 ký tự.");return;}
            }else{
                var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM NhanVien WHERE MaNhanVien=@c AND NhanVienID<>@id) THEN N'Mã nhân viên đã tồn tại'
                    WHEN @p<>'' AND EXISTS(SELECT 1 FROM NhanVien WHERE SoDienThoai=@p AND NhanVienID<>@id) THEN N'SĐT đã tồn tại'
                    WHEN @e<>'' AND EXISTS(SELECT 1 FROM NhanVien WHERE Email=@e AND NhanVienID<>@id) THEN N'Email đã tồn tại'
                    ELSE '' END", new SqlParameter("@c",txtCode.Text.Trim()), new SqlParameter("@p",phone), new SqlParameter("@e",txtEmail.Text.Trim()), new SqlParameter("@id",_employeeId));
                var msg=Convert.ToString(dup.Rows[0][0])??"";
                if(msg.Length>0){UiMsg.Warn(msg);return;}
            }

            using var cn=Db.OpenConnection();using var tx=cn.BeginTransaction();try{
                if(_employeeId==null){
                    var roleCmd=new SqlCommand("SELECT VaiTroID FROM VaiTro WHERE TenVaiTro='Employee'",cn,tx);var roleId=Convert.ToInt32(roleCmd.ExecuteScalar());
                    var salt=PasswordHelper.CreateSalt();var hash=PasswordHelper.Hash(salt,txtPassword.Text);
                    var u=new SqlCommand("INSERT INTO TaiKhoan(TenDangNhap,MatKhauBam,MuoiMatKhau,VaiTroID,DangHoatDong) OUTPUT INSERTED.TaiKhoanID VALUES(@u,@h,@s,@r,@a)",cn,tx);
                    u.Parameters.AddWithValue("@u",txtUsername.Text.Trim());u.Parameters.AddWithValue("@h",hash);u.Parameters.AddWithValue("@s",salt);u.Parameters.AddWithValue("@r",roleId);u.Parameters.AddWithValue("@a",chkActive.Checked);
                    var uid=Convert.ToInt32(u.ExecuteScalar());
                    var e=new SqlCommand("INSERT INTO NhanVien(MaNhanVien,TaiKhoanID,HoTen,SoDienThoai,Email,ChucVu,DangHoatDong) VALUES(@c,@u,@n,@p,@e,@pos,@a)",cn,tx);
                    e.Parameters.AddWithValue("@c",txtCode.Text.Trim());e.Parameters.AddWithValue("@u",uid);e.Parameters.AddWithValue("@n",txtName.Text.Trim());e.Parameters.AddWithValue("@p",string.IsNullOrWhiteSpace(phone)?DBNull.Value:phone);e.Parameters.AddWithValue("@e",string.IsNullOrWhiteSpace(txtEmail.Text)?DBNull.Value:txtEmail.Text.Trim());e.Parameters.AddWithValue("@pos",txtPosition.Text.Trim());e.Parameters.AddWithValue("@a",chkActive.Checked);e.ExecuteNonQuery();
                }else{
                    var e=new SqlCommand("UPDATE NhanVien SET MaNhanVien=@c,HoTen=@n,SoDienThoai=@p,Email=@e,ChucVu=@pos,DangHoatDong=@a WHERE NhanVienID=@id;UPDATE TaiKhoan SET DangHoatDong=@a WHERE TaiKhoanID=@uid;",cn,tx);
                    e.Parameters.AddWithValue("@c",txtCode.Text.Trim());e.Parameters.AddWithValue("@n",txtName.Text.Trim());e.Parameters.AddWithValue("@p",string.IsNullOrWhiteSpace(phone)?DBNull.Value:phone);e.Parameters.AddWithValue("@e",string.IsNullOrWhiteSpace(txtEmail.Text)?DBNull.Value:txtEmail.Text.Trim());e.Parameters.AddWithValue("@pos",txtPosition.Text.Trim());e.Parameters.AddWithValue("@a",chkActive.Checked);e.Parameters.AddWithValue("@id",_employeeId);e.Parameters.AddWithValue("@uid",_userId);e.ExecuteNonQuery();
                }
                tx.Commit();LoadData();Toast.Success("Đã lưu nhân viên.");
            }catch(Exception ex){tx.Rollback();throw new InvalidOperationException(ex.Message);}
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu nhân viên");}
    }
    private void Delete(){if(_employeeId==null||_userId==SessionContext.UserId){UiMsg.Warn("Không thể xóa tài khoản đang đăng nhập.");return;}if(UiMsg.Ask("Xóa nhân viên?","Xác nhận",danger:true)!=DialogResult.Yes)return;try{Db.Execute("UPDATE NhanVien SET DangHoatDong=0 WHERE NhanVienID=@id;UPDATE TaiKhoan SET DangHoatDong=0 WHERE TaiKhoanID=@uid",new SqlParameter("@id",_employeeId),new SqlParameter("@uid",_userId));LoadData();Toast.Success("Đã khóa nhân viên.");}catch(Exception ex){UiMsg.Error(ex.Message);}}
}
