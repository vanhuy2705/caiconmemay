using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Common;

public sealed class DashboardCard : RoundedPanel
{
    private readonly Label _title = new();
    private readonly Label _value = new();
    private readonly Label _sub = new();
    private readonly IconBadge _icon = new();
    private readonly Panel _accentLine = new();

    public DashboardCard(string title, string value, string sub)
        : this(title, value, sub, AppTheme.Success, "▣") { }

    public DashboardCard(string title, string value, string sub, Color accent, string glyph)
    {
        BackColor = Color.White;
        BorderColor = Color.FromArgb(220, 233, 239);
        Radius = 14;
        Padding = new Padding(16);
        Margin = new Padding(6, 5, 6, 5);
        Width = 245;
        Height = 100;

        _accentLine.BackColor = accent;
        _accentLine.Width = 4;
        _accentLine.Height = 58;
        _accentLine.Location = new Point(0, 21);

        _icon.IconImage = SportIcons.Get(IconFor(title, glyph), 20, Color.White);
        _icon.AccentColor = accent;
        _icon.Size = new Size(44, 44);
        _icon.Location = new Point(14, 18);

        _title.Text = title;
        _title.AutoSize = true;
        _title.ForeColor = AppTheme.Muted;
        _title.Font = new Font("Segoe UI", 8.7F);
        _title.Location = new Point(72, 13);

        _value.Text = value;
        _value.AutoSize = true;
        _value.Font = new Font("Segoe UI Semibold", 15.8F, FontStyle.Bold);
        _value.ForeColor = AppTheme.Text;
        _value.Location = new Point(72, 34);

        _sub.Text = sub;
        _sub.AutoSize = true;
        _sub.Font = new Font("Segoe UI", 8.2F);
        _sub.ForeColor = accent;
        _sub.Location = new Point(72, 70);

        Controls.AddRange(new Control[] { _accentLine, _icon, _title, _value, _sub });
        MouseEnter += (_, _) => { BorderColor = Color.FromArgb(168, 210, 199); Invalidate(); };
        MouseLeave += (_, _) => { BorderColor = Color.FromArgb(220, 233, 239); Invalidate(); };
    }

    public void SetValue(string value, string sub = "")
    {
        _value.Text = value;
        if (!string.IsNullOrWhiteSpace(sub)) _sub.Text = sub;
    }

    public void SetAccent(Color color, string glyph)
    {
        _accentLine.BackColor = color;
        _icon.AccentColor = color;
        _icon.IconImage = SportIcons.Get(IconFor(_title.Text, glyph), 20, Color.White);
        _sub.ForeColor = color;
        _icon.Invalidate();
    }

    private static SportIcon IconFor(string title, string glyph)
    {
        var text = title.ToLowerInvariant();
        if (text.Contains("doanh thu") || glyph == "$") return SportIcon.Pay;
        if (text.Contains("đặt") || text.Contains("booking")) return SportIcon.Booking;
        if (text.Contains("hóa đơn")) return SportIcon.Invoice;
        if (text.Contains("khách")) return SportIcon.Customer;
        if (text.Contains("sân")) return SportIcon.Field;
        return SportIcon.Statistics;
    }
}
