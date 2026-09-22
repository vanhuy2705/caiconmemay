namespace QuanLyThueSanTheThao.Helpers;

public static class SessionContext
{
    public static int UserId { get; set; }
    public static int? CustomerId { get; set; }
    public static int? EmployeeId { get; set; }
    public static string Username { get; set; } = "";
    public static string FullName { get; set; } = "";
    public static string Role { get; set; } = "";

    public static void Clear()
    {
        UserId = 0;
        CustomerId = null;
        EmployeeId = null;
        Username = FullName = Role = "";
    }
}
