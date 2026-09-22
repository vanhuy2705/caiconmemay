namespace QuanLyThueSanTheThao.Models;

public sealed class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int UserId { get; set; }
    public int? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
}
