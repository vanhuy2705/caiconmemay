using System.Drawing.Drawing2D;
using QuanLyThueSanTheThao.Forms.Common;

namespace QuanLyThueSanTheThao.Helpers;

public enum UiIcon { Success, Info, Warning, Error, Question }

/// <summary>Thông báo dạng toast trượt từ góc phải trên, tự tắt — thay cho MessageBox thông báo thành công.</summary>
public static class Toast
{
    private static readonly List<ToastForm> Active = new();

    public static void Success(string message) => Show(message, UiIcon.Success);
    public static void Info(string message) => Show(message, UiIcon.Info);
    public static void Warning(string message) => Show(message, UiIcon.Warning);
    public static void Error(string message) => Show(message, UiIcon.Error);

    public static void Show(string message, UiIcon kind)
    {
        if (ProgramInvokeRequired()) { return; }
        var anchorCtl = (Form?)Form.ActiveForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
        var screen = anchorCtl != null ? Screen.FromControl(anchorCtl).WorkingArea : Screen.PrimaryScreen!.WorkingArea;
        var t = new ToastForm(message, kind);
        int index = Active.Count(x => x.Visible);
        t.PositionAt(screen.Right - t.Width - 24, screen.Top + 24 + index * (t.Height + 12));
        Active.Add(t);
        t.Closed += (_, _) => Active.Remove(t);
        t.Show(Form.ActiveForm);
    }

    private static bool ProgramInvokeRequired()
    {
        foreach (Form? form in Application.OpenForms)
            if (form?.InvokeRequired == true) return true;
        return false;
    }
}

internal sealed class ToastForm : Form
{
    private const int LifeMs = 3000;
    private readonly string _message;
    private readonly UiIcon _kind;
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = LifeMs };
    private readonly System.Windows.Forms.Timer _fade = new() { Interval = 40 };
    private bool _closing;

    public ToastForm(string message, UiIcon kind)
    {
        _message = message;
        _kind = kind;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = Color.White;
        DoubleBuffered = true;

        using var probe = CreateGraphics();
        var size = TextRenderer.MeasureText(message, F("Segoe UI", 9F), new Size(330, 9999), TextFormatFlags.WordBreak);
        int width = Math.Max(300, size.Width + 74);
        int height = Math.Max(56, size.Height + 30);
        ClientSize = new Size(width, height);
        _timer.Tick += (_, _) => BeginFade();
        _fade.Tick += (_, _) =>
        {
            Opacity -= 0.14;
            if (Opacity <= 0.05) Close();
        };
        Click += (_, _) => Close();
    }

    private static Font F(string family, float size) => new(family, size);

    public void PositionAt(int x, int y)
    {
        Location = new Point(Math.Max(8, x), Math.Max(8, y));
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Opacity = 0;
        var slide = new System.Windows.Forms.Timer { Interval = 12 };
        slide.Tick += (_, _) =>
        {
            Opacity = Math.Min(1, Opacity + 0.16);
            Top -= 4;
            if (Opacity >= 1) { slide.Stop(); slide.Dispose(); _timer.Start(); }
        };
        slide.Start();
    }

    private void BeginFade()
    {
        if (_closing) return;
        _closing = true;
        _timer.Stop();
        _fade.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedPanel.CreateRoundPath(rect, 12);
        Region?.Dispose();
        Region = new Region(path);
        g.Clear(Color.Transparent);
        using (var b = new SolidBrush(Color.White)) g.FillPath(b, path);
        using (var p = new Pen(Color.FromArgb(232, 239, 244))) g.DrawPath(p, path);

        var (accent, glyph) = _kind switch
        {
            UiIcon.Success => (AppTheme.Success, "✓"),
            UiIcon.Warning => (AppTheme.Warning, "!"),
            UiIcon.Error => (AppTheme.Danger, "✕"),
            _ => (AppTheme.Info, "i")
        };
        using (var bg = new SolidBrush(Color.FromArgb(26, accent)))
            g.FillEllipse(bg, 14, Height / 2 - 16, 32, 32);
        using (var f = new Font("Segoe UI", 10.5F, FontStyle.Bold))
        using (var tb = new SolidBrush(accent))
        {
            var sz = g.MeasureString(glyph, f);
            g.DrawString(glyph, f, tb, 14 + (32 - sz.Width) / 2, Height / 2 - 16 + (32 - sz.Height) / 2);
        }
        TextRenderer.DrawText(g, _message, F("Segoe UI", 9F),
            new Rectangle(56, 0, Width - 66, Height), AppTheme.Text,
            TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _timer.Stop();          // dừng tự tắt khi người dùng đang đưa chuột vào
        if (!_closing) { Opacity = 1; }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (!_closing) BeginFade();
    }
}

/// <summary>Hộp thoại xác nhận/thông báo bo tròn, icon màu — thay cho MessageBox mặc định.</summary>
public static class UiMsg
{
    public static DialogResult Info(string text, string caption = "Thông báo")
        => Show(text, caption, UiIcon.Info, MessageBoxButtons.OK, danger: false);

    public static DialogResult Success(string text, string caption = "Thành công")
        => Show(text, caption, UiIcon.Success, MessageBoxButtons.OK, danger: false);

    public static DialogResult Warn(string text, string caption = "Cảnh báo")
        => Show(text, caption, UiIcon.Warning, MessageBoxButtons.OK, danger: false);

    public static DialogResult Error(string text, string caption = "Lỗi")
        => Show(text, caption, UiIcon.Error, MessageBoxButtons.OK, danger: false);

    public static DialogResult Ask(string text, string caption = "Xác nhận", bool danger = false)
        => Show(text, caption, UiIcon.Question, MessageBoxButtons.YesNo, danger);

    private static DialogResult Show(string text, string caption, UiIcon icon, MessageBoxButtons buttons, bool danger)
    {
        var owner = Form.ActiveForm;
        using var dlg = new UiDialog(text, caption, icon, buttons, danger);
        return owner != null ? dlg.ShowDialog(owner) : dlg.ShowDialog();
    }
}

internal sealed class UiDialog : Form
{
    private readonly string _text;
    private readonly string _caption;
    private readonly UiIcon _icon;
    private readonly MessageBoxButtons _buttons;
    private readonly bool _danger;
    private bool _drag;
    private Point _dragStart;

    public UiDialog(string text, string caption, UiIcon icon, MessageBoxButtons buttons, bool danger)
    {
        _text = text; _caption = caption; _icon = icon; _buttons = buttons; _danger = danger;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        BackColor = Color.White;
        DoubleBuffered = true;
        Opacity = 0;
        KeyPreview = true;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.No; Close(); }
            if (e.KeyCode == Keys.Enter) { DialogResult = DialogResult.Yes; Close(); }
        };

        var size = TextRenderer.MeasureText(text, new Font("Segoe UI", 9.8F), new Size(360, 9999), TextFormatFlags.WordBreak);
        int width = 430;
        int bodyHeight = Math.Max(44, size.Height);
        int height = 118 + bodyHeight + 56;
        ClientSize = new Size(width, height);
    }

    protected override void OnMouseDown(MouseEventArgs e) { _drag = true; _dragStart = e.Location; }
    protected override void OnMouseUp(MouseEventArgs e) => _drag = false;
    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_drag) { Location = new Point(Left + e.X - _dragStart.X, Top + e.Y - _dragStart.Y); }
    }

    private (Color color, string glyph) Visuals => _icon switch
    {
        UiIcon.Success => (AppTheme.Success, "✓"),
        UiIcon.Warning => (AppTheme.Warning, "!"),
        UiIcon.Error => (AppTheme.Danger, "✕"),
        UiIcon.Question => (AppTheme.Info, "?"),
        _ => (AppTheme.Info, "i")
    };

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = RoundedPanel.CreateRoundPath(rect, 14);
        Region?.Dispose();
        Region = new Region(path);
        using (var b = new SolidBrush(Color.White)) g.FillPath(b, path);
        using (var p = new Pen(Color.FromArgb(228, 236, 242))) g.DrawPath(p, path);

        // dải màu trên cùng
        var (accent, glyph) = Visuals;
        var strip = new Rectangle(0, 0, Width, 6);
        var stripClip = new Rectangle(0, 0, Width - 1, 14);
        using (var clip = RoundedPanel.CreateRoundPath(stripClip, 14))
        {
            g.SetClip(clip);
            using var sb = new SolidBrush(accent);
            g.FillRectangle(sb, strip);
            g.ResetClip();
        }

        // icon badge
        using (var bg = new SolidBrush(Color.FromArgb(26, accent)))
            g.FillEllipse(bg, 26, 34, 44, 44);
        using (var f = new Font("Segoe UI", 14F, FontStyle.Bold))
        using (var tb = new SolidBrush(accent))
        {
            var sz = g.MeasureString(glyph, f);
            g.DrawString(glyph, f, tb, 26 + (44 - sz.Width) / 2, 34 + (44 - sz.Height) / 2);
        }

        TextRenderer.DrawText(g, _caption, new Font("Segoe UI Semibold", 12.5F, FontStyle.Bold),
            new Point(84, 40), AppTheme.Text);
        TextRenderer.DrawText(g, _text, new Font("Segoe UI", 9.8F),
            new Rectangle(84, 72, Width - 110, Height - 72 - 74), AppTheme.Muted,
            TextFormatFlags.WordBreak);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Fx.FadeIn(this, 150);
        int y = Height - 52;
        int x = Width - 24;
        if (_buttons == MessageBoxButtons.YesNo)
        {
            var no = new RoundedButton { Text = "Không", Size = new Size(96, 38), Radius = 10, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            no.Location = new Point(x - 96, y);
            no.BackColor = Color.FromArgb(242, 246, 249);
            no.ForeColor = AppTheme.Text;
            no.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            no.HoverColor = Color.FromArgb(229, 236, 241);
            no.Click += (_, _) => { DialogResult = DialogResult.No; Close(); };

            var yes = new RoundedButton { Text = _danger ? "Xóa" : "Có", Size = new Size(96, 38), Radius = 10, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            yes.Location = new Point(x - 96 - 96 - 10, y);
            yes.BackColor = _danger ? AppTheme.Danger : AppTheme.Accent;
            yes.ForeColor = Color.White;
            yes.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            yes.HoverColor = _danger ? Color.FromArgb(217, 62, 62) : AppTheme.AccentDark;
            yes.Click += (_, _) => { DialogResult = DialogResult.Yes; Close(); };

            Controls.Add(no);
            Controls.Add(yes);
            ActiveControl = yes;
        }
        else
        {
            var ok = new RoundedButton { Text = "OK", Size = new Size(110, 38), Radius = 10, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            ok.Location = new Point(x - 110, y);
            ok.BackColor = AppTheme.Accent;
            ok.ForeColor = Color.White;
            ok.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            ok.HoverColor = AppTheme.AccentDark;
            ok.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
            Controls.Add(ok);
            ActiveControl = ok;
        }
    }
}
