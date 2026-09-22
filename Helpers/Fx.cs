namespace QuanLyThueSanTheThao.Helpers;

/// <summary>Hiệu ứng chuyển động mượt cho giao diện: fade, trượt, chuyển màu nội suy.</summary>
public static class Fx
{
    // ---------- Fade in (form phải được đặt Opacity = 0 trước khi hiển thị) ----------
    public static void FadeIn(Form form, int ms = 180)
    {
        var t = new System.Windows.Forms.Timer { Interval = 16 };
        double step = 16.0 / Math.Max(60, ms);
        t.Tick += (_, _) =>
        {
            if (form.IsDisposed || form.Disposing) { t.Stop(); t.Dispose(); return; }
            form.Opacity = Math.Min(1, form.Opacity + step);
            if (form.Opacity >= 1) { form.Opacity = 1; t.Stop(); t.Dispose(); }
        };
        t.Start();
    }

    // ---------- Trượt nhẹ "đặt xuống" khi trang con xuất hiện ----------
    public static void Settle(Control c, int distance = 14, int ms = 170)
    {
        int target = c.Padding.Top;
        var pad = c.Padding; pad.Top = target + distance; c.Padding = pad;
        var t = new System.Windows.Forms.Timer { Interval = 16 };
        int frames = Math.Max(5, ms / 16);
        int i = 0;
        t.Tick += (_, _) =>
        {
            if (c.IsDisposed || c.Disposing) { t.Stop(); t.Dispose(); return; }
            i++;
            float k = Math.Min(1f, i / (float)frames);
            float e = 1f - (1f - k) * (1f - k);          // easeOutQuad
            var p = c.Padding; p.Top = target + (int)Math.Round(distance * (1 - e)); c.Padding = p;
            if (k >= 1) { p.Top = target; c.Padding = p; t.Stop(); t.Dispose(); }
        };
        t.Start();
    }

    // ---------- Chuyển màu mượt cho control (hover tween) ----------
    private sealed class ColorAnim { public Control C = null!; public Color From, To; public int Frame, Frames; }
    private static readonly List<ColorAnim> Active = new();
    private static readonly System.Windows.Forms.Timer Anim = new() { Interval = 16 };

    static Fx()
    {
        Anim.Tick += (_, _) => Step();
    }

    public static void ToColor(Control c, Color to, int ms = 130)
    {
        lock (Active)
        {
            Active.RemoveAll(a => a.C == c || a.C.IsDisposed);
            var from = c.BackColor;
            if (from.ToArgb() == to.ToArgb()) return;
            Active.Add(new ColorAnim { C = c, From = from, To = to, Frames = Math.Max(3, ms / 16) });
            if (!Anim.Enabled) Anim.Start();
        }
    }

    public static void StopColor(Control c)
    {
        lock (Active) Active.RemoveAll(a => a.C == c);
    }

    private static void Step()
    {
        lock (Active)
        {
            for (int i = Active.Count - 1; i >= 0; i--)
            {
                var a = Active[i];
                if (a.C.IsDisposed) { Active.RemoveAt(i); continue; }
                a.Frame++;
                float k = Math.Min(1f, a.Frame / (float)a.Frames);
                float e = 1f - (1f - k) * (1f - k);
                a.C.BackColor = Lerp(a.From, a.To, e);
                if (k >= 1) Active.RemoveAt(i);
            }
            if (Active.Count == 0) Anim.Stop();
        }
    }

    public static Color Lerp(Color a, Color b, float t) => Color.FromArgb(
        Mix(a.A, b.A, t), Mix(a.R, b.R, t), Mix(a.G, b.G, t), Mix(a.B, b.B, t));

    private static int Mix(int x, int y, float t) => x + (int)Math.Round((y - x) * Math.Clamp(t, 0f, 1f));
}
