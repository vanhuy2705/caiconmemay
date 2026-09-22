namespace QuanLyThueSanTheThao.Data;

public static class DatabaseConfig
{
    public const string DatabaseName = "QuanLySanTheThaoDB";
    public const string ExpectedDatabaseVersion = "4.0 PRO";

    // Dùng .\\SQLEXPRESS thay cho tên máy cố định. Trên máy trong ảnh,
    // .\\SQLEXPRESS chính là DESKTOP-B9A50IM\\SQLEXPRESS; khi chép sang máy khác
    // vẫn dùng đúng instance SQLEXPRESS cục bộ.
    public static string SqlServerInstance { get; set; } =
        Environment.GetEnvironmentVariable("SPORTFIELD_SQLSERVER")?.Trim() is { Length: > 0 } custom
            ? custom
            : @".\SQLEXPRESS";

    public static string ConnectionString { get; set; } = BuildConnectionString(SqlServerInstance);

    public static string BuildConnectionString(string server) =>
        $@"Server={server};Database={DatabaseName};Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Connect Timeout=15;Application Name=SportField.WinForms;";

    public static string DisplayTarget => $"{SqlServerInstance} / {DatabaseName}";
}
