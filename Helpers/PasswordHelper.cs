using System.Security.Cryptography;
using System.Text;

namespace QuanLyThueSanTheThao.Helpers;

/// <summary>
/// V8 PRO: PBKDF2 HMACSHA256 100k iterations, 32 bytes, hex 64 chars
/// Tương thích ngược: Verify thử PBKDF2 trước, nếu fail thử SHA256 legacy
/// để tự động migrate khi đăng nhập thành công.
/// </summary>
public static class PasswordHelper
{
    private const int Iterations = 100_000;
    private const int KeyLength = 32; // 256-bit

    public static string CreateSalt()
    {
        // 16 chars uppercase hex-like, tương thích cột VARCHAR(32)
        return Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
    }

    public static string Hash(string salt, string password)
    {
        // PBKDF2 mới
        return Pbkdf2Hash(salt, password);
    }

    public static string Pbkdf2Hash(string salt, string password)
    {
        salt ??= "";
        password ??= "";
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeyLength);
        return Convert.ToHexString(key).ToLowerInvariant(); // 64 chars
    }

    public static string LegacySha256(string salt, string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool Verify(string salt, string password, string expectedHash)
    {
        if (string.IsNullOrEmpty(expectedHash)) return false;
        expectedHash = expectedHash.Trim().ToLowerInvariant();

        // Thử PBKDF2 trước (định dạng mới)
        var pbkdf2 = Pbkdf2Hash(salt, password);
        if (CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(pbkdf2),
            Encoding.UTF8.GetBytes(expectedHash)))
            return true;

        // Fallback SHA256 legacy để migrate
        var legacy = LegacySha256(salt, password);
        if (CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(legacy),
            Encoding.UTF8.GetBytes(expectedHash)))
            return true;

        return false;
    }

    public static bool IsLegacyHash(string salt, string password, string expectedHash)
    {
        var legacy = LegacySha256(salt, password);
        return string.Equals(legacy, expectedHash, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(Pbkdf2Hash(salt, password), expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
