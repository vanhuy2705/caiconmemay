using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;
using QuanLyThueSanTheThao.Models;

namespace QuanLyThueSanTheThao.Services;

public sealed class AuthService
{
    public LoginResult Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return new LoginResult { Success = false, Message = "Vui lòng nhập tài khoản và mật khẩu." };

        Db.EnsureCompatibleSchema();

        const string sql = @"
SELECT TOP 1 u.TaiKhoanID,u.TenDangNhap,u.MatKhauBam,u.MuoiMatKhau,u.DangHoatDong,
       r.TenVaiTro,
       c.KhachHangID,
       e.NhanVienID,
       COALESCE(c.HoTen,e.HoTen,u.TenDangNhap) AS HoTen
FROM TaiKhoan u
INNER JOIN VaiTro r ON r.VaiTroID=u.VaiTroID
LEFT JOIN KhachHang c ON c.TaiKhoanID=u.TaiKhoanID AND c.DangHoatDong=1
LEFT JOIN NhanVien e ON e.TaiKhoanID=u.TaiKhoanID AND e.DangHoatDong=1
WHERE u.TenDangNhap=@TenDangNhap
   OR (c.Email IS NOT NULL AND c.Email=@TenDangNhap)
   OR (c.SoDienThoai IS NOT NULL AND c.SoDienThoai=@TenDangNhap)
ORDER BY CASE WHEN u.TenDangNhap=@TenDangNhap THEN 0 ELSE 1 END;";

        var dt = Db.Query(sql, new SqlParameter("@TenDangNhap", username.Trim()));
        if (dt.Rows.Count == 0)
            return new LoginResult { Success = false, Message = "Tài khoản không tồn tại." };

        var row = dt.Rows[0];
        if (!Convert.ToBoolean(row["DangHoatDong"]))
            return new LoginResult { Success = false, Message = "Tài khoản đang bị khóa." };

        var salt = Convert.ToString(row["MuoiMatKhau"]) ?? "";
        var expected = Convert.ToString(row["MatKhauBam"]) ?? "";

        // V8: PBKDF2 verify + legacy fallback
        if (!PasswordHelper.Verify(salt, password, expected))
            return new LoginResult { Success = false, Message = "Mật khẩu không đúng." };

        var role = Convert.ToString(row["TenVaiTro"]) ?? "";
        int? customerId = row["KhachHangID"] == DBNull.Value ? null : Convert.ToInt32(row["KhachHangID"]);
        int? employeeId = row["NhanVienID"] == DBNull.Value ? null : Convert.ToInt32(row["NhanVienID"]);

        if (role == "Customer" && customerId == null)
            return new LoginResult { Success = false, Message = "Tài khoản Customer chưa được gắn với hồ sơ khách hàng đang hoạt động." };
        if (role == "Employee" && employeeId == null)
            return new LoginResult { Success = false, Message = "Tài khoản Employee chưa được gắn với hồ sơ nhân viên đang hoạt động." };

        var userId = Convert.ToInt32(row["TaiKhoanID"]);

        // Tự động migrate hash legacy sang PBKDF2 nếu cần
        try
        {
            if (PasswordHelper.IsLegacyHash(salt, password, expected))
            {
                var newHash = PasswordHelper.Pbkdf2Hash(salt, password);
                Db.Execute("UPDATE TaiKhoan SET MatKhauBam=@h, DangNhapLanCuoi=SYSDATETIME() WHERE TaiKhoanID=@id",
                    new SqlParameter("@h", newHash),
                    new SqlParameter("@id", userId));
            }
            else
            {
                Db.Execute("UPDATE TaiKhoan SET DangNhapLanCuoi=SYSDATETIME() WHERE TaiKhoanID=@id",
                    new SqlParameter("@id", userId));
            }
        }
        catch { /* không chặn đăng nhập nếu update log fail */ }

        return new LoginResult
        {
            Success = true,
            UserId = userId,
            Username = Convert.ToString(row["TenDangNhap"]) ?? "",
            Role = role,
            FullName = Convert.ToString(row["HoTen"]) ?? username,
            CustomerId = customerId,
            EmployeeId = employeeId
        };
    }

    public int RegisterCustomer(string fullName, string phone, string email,
        string username, string password, string confirmation)
    {
        fullName = fullName.Trim(); phone = phone.Trim(); email = email.Trim(); username = username.Trim();
        if (fullName.Length < 2 || fullName.Length > 100)
            throw new InvalidOperationException("Họ tên phải có từ 2 đến 100 ký tự.");
        if (username.Length < 4 || username.Length > 50 || username.Any(char.IsWhiteSpace))
            throw new InvalidOperationException("Tên đăng nhập phải có 4-50 ký tự và không chứa khoảng trắng.");
        if (password.Length < 6 || password.Length > 100)
            throw new InvalidOperationException("Mật khẩu phải có từ 6 đến 100 ký tự.");
        if (!string.Equals(password, confirmation, StringComparison.Ordinal))
            throw new InvalidOperationException("Mật khẩu nhập lại chưa khớp.");
        if (phone.Length is < 9 or > 15 || phone.Any(c => !char.IsDigit(c)))
            throw new InvalidOperationException("Số điện thoại phải có 9-15 chữ số.");
        if (!string.IsNullOrWhiteSpace(email) &&
            (email.Length > 120 || !email.Contains('@') || !email[(email.IndexOf('@') + 1)..].Contains('.')))
            throw new InvalidOperationException("Email chưa đúng định dạng.");

        Db.EnsureCompatibleSchema();
        using var cn = Db.OpenConnection();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            using (var duplicate = new SqlCommand(@"
SELECT CASE
 WHEN EXISTS(SELECT 1 FROM TaiKhoan WITH (UPDLOCK,HOLDLOCK) WHERE TenDangNhap=@u) THEN N'Tên đăng nhập đã được sử dụng.'
 WHEN EXISTS(SELECT 1 FROM KhachHang WITH (UPDLOCK,HOLDLOCK) WHERE SoDienThoai=@p) THEN N'Số điện thoại đã được sử dụng.'
 WHEN @e<>N'' AND EXISTS(SELECT 1 FROM KhachHang WITH (UPDLOCK,HOLDLOCK) WHERE Email=@e) THEN N'Email đã được sử dụng.'
 ELSE N'' END;", cn, tx))
            {
                duplicate.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                duplicate.Parameters.Add("@p", SqlDbType.NVarChar, 20).Value = phone;
                duplicate.Parameters.Add("@e", SqlDbType.NVarChar, 120).Value = email;
                var message = Convert.ToString(duplicate.ExecuteScalar()) ?? "";
                if (message.Length > 0) throw new InvalidOperationException(message);
            }

            int roleId;
            using (var role = new SqlCommand("SELECT VaiTroID FROM VaiTro WHERE TenVaiTro=N'Customer'", cn, tx))
                roleId = Convert.ToInt32(role.ExecuteScalar() ?? throw new InvalidOperationException("CSDL thiếu vai trò Customer."));

            var salt = PasswordHelper.CreateSalt();
            int userId;
            using (var account = new SqlCommand(@"
INSERT INTO TaiKhoan(TenDangNhap,MatKhauBam,MuoiMatKhau,VaiTroID,DangHoatDong)
OUTPUT INSERTED.TaiKhoanID VALUES(@u,@h,@s,@r,1);", cn, tx))
            {
                account.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = username;
                account.Parameters.Add("@h", SqlDbType.VarChar, 64).Value = PasswordHelper.Hash(salt, password);
                account.Parameters.Add("@s", SqlDbType.VarChar, 32).Value = salt;
                account.Parameters.Add("@r", SqlDbType.Int).Value = roleId;
                userId = Convert.ToInt32(account.ExecuteScalar());
            }

            int customerId;
            using (var customer = new SqlCommand(@"
INSERT INTO KhachHang(MaKhachHang,TaiKhoanID,HoTen,SoDienThoai,Email,DangHoatDong)
OUTPUT INSERTED.KhachHangID
VALUES(N'TEMP-'+CONVERT(nvarchar(36),NEWID()),@uid,@name,@phone,NULLIF(@email,N''),1);", cn, tx))
            {
                customer.Parameters.Add("@uid", SqlDbType.Int).Value = userId;
                customer.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = fullName;
                customer.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = phone;
                customer.Parameters.Add("@email", SqlDbType.NVarChar, 120).Value = email;
                customerId = Convert.ToInt32(customer.ExecuteScalar());
            }

            using (var code = new SqlCommand("UPDATE KhachHang SET MaKhachHang=N'KH'+RIGHT(N'000000'+CONVERT(nvarchar(10),KhachHangID),6) WHERE KhachHangID=@id", cn, tx))
            {
                code.Parameters.Add("@id", SqlDbType.Int).Value = customerId;
                code.ExecuteNonQuery();
            }

            using (var welcome = new SqlCommand(@"
INSERT INTO PhieuGiamGiaKhachHang(PhieuGiamGiaID,KhachHangID)
SELECT TOP 1 v.PhieuGiamGiaID,@customer
FROM PhieuGiamGia v WITH (UPDLOCK,HOLDLOCK)
WHERE v.DangHoatDong=1 AND SYSDATETIME() BETWEEN v.NgayBatDau AND v.NgayKetThuc
  AND (v.SoLuong IS NULL OR v.SoLuong>0)
ORDER BY CASE WHEN v.MaPhieuGiamGia=N'WELCOME20' THEN 0 ELSE 1 END,v.PhieuGiamGiaID;", cn, tx))
            {
                welcome.Parameters.Add("@customer", SqlDbType.Int).Value = customerId;
                welcome.ExecuteNonQuery();
            }

            tx.Commit();
            return customerId;
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }
}
