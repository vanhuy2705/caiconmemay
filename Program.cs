using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Auth;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetDefaultFont(new Font("Segoe UI", 10F));
        Application.ThreadException += (_, e) =>
            UiMsg.Error(e.Exception.Message, "Lỗi ứng dụng");

        // Preflight một lần trước khi mở giao diện: tránh trường hợp app chạy với DB cũ
        // rồi lỗi ngẫu nhiên khi người dùng mở từng chức năng.
        try
        {
            Db.EnsureCompatibleSchema();
        }
        catch (Exception ex)
        {
            UiMsg.Error(ex.Message, "Không thể khởi động");
            return;
        }

        Application.Run(new FrmLogin());
    }
}
