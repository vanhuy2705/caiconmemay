namespace QuanLyThueSanTheThao.Forms.Common;

public sealed class SimpleBarChart : Control
{
    public List<(string Label, decimal Value)> Items { get; set; } = new();

    public SimpleBarChart()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if (Items.Count == 0)
        {
            using var b = new SolidBrush(Color.Gray);
            g.DrawString("Chưa có dữ liệu", Font, b, 20, 20);
            return;
        }
        var rect = ClientRectangle;
        var left = 42f; var bottom = rect.Height - 34f; var top = 24f;
        var width = rect.Width - left - 16f; var height = bottom - top;
        var max = Items.Max(x => x.Value);
        if (max <= 0) max = 1;
        var gap = width / Items.Count;
        using var axisPen = new Pen(Color.FromArgb(220,230,235));
        g.DrawLine(axisPen, left, bottom, rect.Width - 12, bottom);
        for (int i = 0; i < Items.Count; i++)
        {
            var x = left + i * gap + gap * .2f;
            var barW = gap * .6f;
            var h = (float)(Items[i].Value / max) * (height - 20f);
            var y = bottom - h;
            using var br = new System.Drawing.Drawing2D.LinearGradientBrush(
                new RectangleF(x,y,barW,Math.Max(1,h)),
                Color.FromArgb(30,190,159), Color.FromArgb(59,130,246), 90f);
            g.FillRoundedRectangle(br, new RectangleF(x,y,barW,Math.Max(1,h)), 5);
            using var tb = new SolidBrush(Color.FromArgb(25,54,72));
            using var small = new Font("Segoe UI", 8F);
            g.DrawString(Items[i].Label, small, tb, x, bottom + 6);
            g.DrawString((Items[i].Value/1000m).ToString("0.#")+"k", small, tb, x, Math.Max(top, y-16));
        }
    }
}

internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, RectangleF bounds, float radius)
    {
        using var path = new System.Drawing.Drawing2D.GraphicsPath();
        var d = radius * 2;
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right-d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right-d, bounds.Bottom-d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom-d, d, d, 90, 90);
        path.CloseFigure();
        g.FillPath(brush, path);
    }
}
