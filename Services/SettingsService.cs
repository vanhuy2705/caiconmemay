using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;

namespace QuanLyThueSanTheThao.Services;

public sealed class SettingsService
{
    public string Get(string key, string fallback = "")
    {
        var value = Db.Scalar("SELECT GiaTriCauHinh FROM CauHinhHeThong WHERE KhoaCauHinh=@k", new SqlParameter("@k", key));
        return value == null || value == DBNull.Value ? fallback : Convert.ToString(value) ?? fallback;
    }

    public void Set(string key, string value)
    {
        const string sql = @"
IF EXISTS(SELECT 1 FROM CauHinhHeThong WHERE KhoaCauHinh=@k)
 UPDATE CauHinhHeThong SET GiaTriCauHinh=@v,NgayCapNhat=SYSDATETIME() WHERE KhoaCauHinh=@k;
ELSE
 INSERT INTO CauHinhHeThong(KhoaCauHinh,GiaTriCauHinh,NgayCapNhat) VALUES(@k,@v,SYSDATETIME());";
        Db.Execute(sql, new SqlParameter("@k", key), new SqlParameter("@v", value));
    }
}
