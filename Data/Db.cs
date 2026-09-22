using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyThueSanTheThao.Data;

public static class Db
{
    private const int CommandTimeoutSeconds = 30;

    public static SqlConnection OpenConnection()
    {
        var cn = new SqlConnection(DatabaseConfig.ConnectionString);
        try
        {
            cn.Open();
            return cn;
        }
        catch
        {
            cn.Dispose();
            throw;
        }
    }

    private static SqlCommand Command(string sql, SqlConnection cn, params SqlParameter[] parameters)
    {
        var cmd = new SqlCommand(sql, cn) { CommandTimeout = CommandTimeoutSeconds };
        if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
        return cmd;
    }

    public static DataTable Query(string sql, params SqlParameter[] parameters)
    {
        using var cn = OpenConnection();
        using var cmd = Command(sql, cn, parameters);
        using var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    public static object? Scalar(string sql, params SqlParameter[] parameters)
    {
        using var cn = OpenConnection();
        using var cmd = Command(sql, cn, parameters);
        return cmd.ExecuteScalar();
    }

    public static int Execute(string sql, params SqlParameter[] parameters)
    {
        using var cn = OpenConnection();
        using var cmd = Command(sql, cn, parameters);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Bắt buộc app và SQL phải đúng cùng phiên bản. Nếu người dùng mở nhầm DB cũ,
    /// lỗi sẽ được báo ngay tại đăng nhập thay vì văng ở một form ngẫu nhiên.
    /// </summary>
    public static void EnsureCompatibleSchema()
    {
        try
        {
            var version = Scalar(@"
IF OBJECT_ID(N'dbo.CauHinhHeThong', N'U') IS NULL
    SELECT CAST(NULL AS nvarchar(300));
ELSE
    SELECT GiaTriCauHinh FROM dbo.CauHinhHeThong WHERE KhoaCauHinh=N'DatabaseVersion';");

            var actual = version == null || version == DBNull.Value ? "" : Convert.ToString(version) ?? "";
            if (!string.Equals(actual, DatabaseConfig.ExpectedDatabaseVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"CSDL không khớp phiên bản ứng dụng. App cần DatabaseVersion={DatabaseConfig.ExpectedDatabaseVersion}, " +
                    $"nhưng CSDL hiện tại là '{(string.IsNullOrWhiteSpace(actual) ? "không xác định" : actual)}'. " +
                    "Hãy chạy Database\\QuanLySanTheThaoDB_V4_PRO.sql trong SSMS.");
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException(
                $"Không kết nối được SQL Server tại {DatabaseConfig.DisplayTarget}. " +
                "Kiểm tra dịch vụ SQL Server (SQLEXPRESS) và chạy file SQL V4 PRO.\n" + ex.Message, ex);
        }
    }
}
