using System.Security.Cryptography;
using System.Text;

namespace QuanLyThueSanTheThao.Helpers;

public static class PasswordHelper
{
    public static string Hash(string salt, string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
