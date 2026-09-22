using System.Text.Json;

namespace QuanLyThueSanTheThao.Helpers;

public static class RememberMeStore
{
    private sealed class RememberData
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public bool Enabled { get; set; }
    }

    private static string FilePath
    {
        get
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuanLyThueSanTheThao");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "remember.json");
        }
    }

    public static (string username, string password, bool enabled) Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return ("", "", false);
            var data = JsonSerializer.Deserialize<RememberData>(File.ReadAllText(FilePath));
            return data is null ? ("", "", false) : (data.Username, data.Password, data.Enabled);
        }
        catch { return ("", "", false); }
    }

    public static void Save(string username, string password, bool enabled)
    {
        if (!enabled)
        {
            try { if (File.Exists(FilePath)) File.Delete(FilePath); } catch { }
            return;
        }
        var data = new RememberData { Username = username, Password = password, Enabled = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(data));
    }
}
