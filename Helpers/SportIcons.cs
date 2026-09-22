using System.Drawing.Drawing2D;

namespace QuanLyThueSanTheThao.Helpers;

/// <summary>
/// Bộ icon vector riêng của SportField. Icon được vẽ bằng GDI+ nên luôn sắc nét,
/// không phụ thuộc font biểu tượng hoặc file ảnh bên ngoài khi chép app sang máy khác.
/// </summary>
public enum SportIcon
{
    Home, Category, Field, Customer, Booking, Calendar, Invoice, Voucher,
    Promotion, Account, Employee, Statistics, Settings, Logout, Add, Save,
    Edit, Delete, Refresh, Search, Filter, Pay, Cash, QrCode, Bank, Start,
    Complete, Cancel, Back, Close, Lock, Unlock, Password, User, Eye, EyeOff,
    Shield, Clock, Location, Check, Gift, Report, Print, Download, Phone,
    Email, Info, Database, Bell, Football, Badminton, Volleyball, Star, Sun
}

public static class SportIcons
{
    private static readonly Dictionary<(SportIcon Kind, int Size, int Color), Image> Cache = new();

    public static Image Get(SportIcon kind, int size = 20, Color? color = null)
    {
        var ink = color ?? AppTheme.Text;
        var key = (kind, Math.Max(12, size), ink.ToArgb());
        if (Cache.TryGetValue(key, out var cached)) return cached;

        var image = Draw(kind, key.Item2, ink);
        Cache[key] = image;
        return image;
    }

    private static Bitmap Draw(SportIcon kind, int size, Color ink)
    {
        var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.ScaleTransform(size / 24f, size / 24f);
        using var p = new Pen(ink, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        using var b = new SolidBrush(ink);

        switch (kind)
        {
            case SportIcon.Home:
                Lines(g, p, (3,11),(12,3),(21,11));
                Lines(g, p, (5,10),(5,21),(19,21),(19,10));
                g.DrawRectangle(p, 10, 15, 4, 6);
                break;
            case SportIcon.Category:
                g.DrawRectangle(p, 3, 4, 7, 7); g.DrawRectangle(p, 14, 4, 7, 7);
                g.DrawRectangle(p, 3, 15, 7, 6); g.DrawRectangle(p, 14, 15, 7, 6);
                break;
            case SportIcon.Field:
                RoundRect(g, p, 2.5f, 4, 19, 16, 2.5f); g.DrawLine(p, 12, 4, 12, 20);
                g.DrawEllipse(p, 9, 9, 6, 6); g.DrawArc(p, 1, 8, 6, 8, -90, 180); g.DrawArc(p, 17, 8, 6, 8, 90, 180);
                break;
            case SportIcon.Customer:
            case SportIcon.User:
                g.DrawEllipse(p, 8, 3, 8, 8); g.DrawArc(p, 4, 12, 16, 10, 180, 180);
                break;
            case SportIcon.Employee:
                g.DrawEllipse(p, 8, 3, 8, 8); g.DrawArc(p, 4, 12, 16, 10, 180, 180);
                g.DrawLine(p, 8, 14, 12, 18); g.DrawLine(p, 16, 14, 12, 18); g.DrawLine(p, 12, 18, 12, 22);
                break;
            case SportIcon.Booking:
                RoundRect(g, p, 4, 3, 16, 18, 2.5f); g.DrawLine(p, 8, 2, 8, 6); g.DrawLine(p, 16, 2, 16, 6);
                g.DrawLine(p, 4, 8, 20, 8); Lines(g, p, (8,14),(11,17),(17,11));
                break;
            case SportIcon.Calendar:
                RoundRect(g, p, 3, 4, 18, 17, 2.5f); g.DrawLine(p, 3, 9, 21, 9); g.DrawLine(p, 8, 2, 8, 6); g.DrawLine(p, 16, 2, 16, 6);
                foreach (var x in new[] { 7f, 12f, 17f }) { g.FillEllipse(b, x - 1, 12, 2, 2); g.FillEllipse(b, x - 1, 16, 2, 2); }
                break;
            case SportIcon.Invoice:
                Lines(g, p, (5,3),(17,3),(20,6),(20,21),(5,21),(5,3)); Lines(g, p, (17,3),(17,7),(20,7));
                g.DrawLine(p, 8, 11, 17, 11); g.DrawLine(p, 8, 15, 17, 15); g.DrawLine(p, 8, 18, 14, 18);
                break;
            case SportIcon.Voucher:
                using (var path = new GraphicsPath())
                {
                    path.AddLines(new[] { new PointF(3,8),new PointF(8,3),new PointF(21,3),new PointF(21,16),new PointF(16,21),new PointF(3,21),new PointF(3,8) });
                    g.DrawPath(p, path);
                }
                g.DrawEllipse(p, 8, 8, 2.5f, 2.5f); g.DrawEllipse(p, 14, 14, 2.5f, 2.5f); g.DrawLine(p, 9, 17, 17, 9);
                break;
            case SportIcon.Promotion:
                Lines(g, p, (3,13),(15,6),(15,18),(3,13)); g.DrawLine(p, 7, 15, 9, 21); g.DrawLine(p, 18, 8, 21, 5); g.DrawLine(p, 19, 12, 23, 12); g.DrawLine(p, 18, 16, 21, 19);
                break;
            case SportIcon.Account:
                RoundRect(g, p, 3, 4, 18, 16, 2.5f); g.DrawEllipse(p, 6, 8, 5, 5); g.DrawArc(p, 5, 13, 7, 5, 180, 180); g.DrawLine(p, 14, 9, 18, 9); g.DrawLine(p, 14, 13, 18, 13);
                break;
            case SportIcon.Statistics:
                g.DrawLine(p, 3, 21, 21, 21); g.DrawLine(p, 3, 21, 3, 4);
                RoundRect(g, p, 6, 13, 3, 6, 1); RoundRect(g, p, 11, 9, 3, 10, 1); RoundRect(g, p, 16, 5, 3, 14, 1);
                break;
            case SportIcon.Settings:
                g.DrawEllipse(p, 8, 8, 8, 8); g.DrawEllipse(p, 4, 4, 16, 16);
                for (int i = 0; i < 8; i++) { double a=i*Math.PI/4; g.DrawLine(p, 12+(float)Math.Cos(a)*8,12+(float)Math.Sin(a)*8,12+(float)Math.Cos(a)*10,12+(float)Math.Sin(a)*10); }
                break;
            case SportIcon.Logout:
                g.DrawArc(p, 3, 3, 13, 18, 80, 200); Lines(g, p, (12,12),(22,12),(18,8)); g.DrawLine(p, 22, 12, 18, 16);
                break;
            case SportIcon.Add:
                g.DrawEllipse(p, 3, 3, 18, 18); g.DrawLine(p, 12, 7, 12, 17); g.DrawLine(p, 7, 12, 17, 12);
                break;
            case SportIcon.Save:
                RoundRect(g, p, 3, 3, 18, 18, 2); g.DrawRectangle(p, 7, 3, 9, 6); g.DrawRectangle(p, 7, 14, 10, 7); g.FillEllipse(b, 14, 5, 1.5f, 1.5f);
                break;
            case SportIcon.Edit:
                Lines(g, p, (4,20),(8,19),(20,7),(17,4),(5,16),(4,20));
                g.DrawLine(p, 14.5f, 6.5f, 17.5f, 9.5f); g.DrawLine(p, 5, 16, 8, 19);
                break;
            case SportIcon.Delete:
                g.DrawLine(p, 5, 7, 19, 7); g.DrawLine(p, 9, 3, 15, 3); RoundRect(g, p, 7, 7, 10, 14, 1.5f); g.DrawLine(p, 10, 11, 10, 17); g.DrawLine(p, 14, 11, 14, 17);
                break;
            case SportIcon.Refresh:
                g.DrawArc(p, 4, 4, 16, 16, -60, 265); Lines(g, p, (18,4),(20,10),(14,8));
                break;
            case SportIcon.Search:
                g.DrawEllipse(p, 3, 3, 13, 13); g.DrawLine(p, 14, 14, 21, 21);
                break;
            case SportIcon.Filter:
                Lines(g, p, (3,4),(21,4),(14,12),(14,19),(10,21),(10,12),(3,4));
                break;
            case SportIcon.Pay:
                RoundRect(g, p, 2.5f, 5, 19, 14, 2.5f); g.DrawLine(p, 3, 10, 21, 10); g.DrawLine(p, 6, 15, 11, 15);
                break;
            case SportIcon.Cash:
                RoundRect(g, p, 2.5f, 5, 19, 14, 2.5f); g.DrawEllipse(p, 9, 8, 6, 8);
                g.DrawArc(p, 3, 6, 5, 5, 0, 90); g.DrawArc(p, 16, 13, 5, 5, 180, 90);
                break;
            case SportIcon.QrCode:
                g.DrawRectangle(p, 3, 3, 7, 7); g.DrawRectangle(p, 14, 3, 7, 7); g.DrawRectangle(p, 3, 14, 7, 7);
                g.FillRectangle(b, 5, 5, 3, 3); g.FillRectangle(b, 16, 5, 3, 3); g.FillRectangle(b, 5, 16, 3, 3);
                g.FillRectangle(b, 14, 14, 3, 3); g.FillRectangle(b, 19, 14, 2, 4); g.FillRectangle(b, 14, 19, 4, 2); g.FillRectangle(b, 20, 20, 2, 2);
                break;
            case SportIcon.Bank:
                Lines(g, p, (3,9),(12,3),(21,9),(3,9)); g.DrawLine(p, 4, 20, 20, 20); g.DrawLine(p, 3, 22, 21, 22);
                g.DrawLine(p, 6, 10, 6, 19); g.DrawLine(p, 10, 10, 10, 19); g.DrawLine(p, 14, 10, 14, 19); g.DrawLine(p, 18, 10, 18, 19);
                break;
            case SportIcon.Start:
                g.DrawEllipse(p, 3, 3, 18, 18); g.FillPolygon(b, new[] { new PointF(10,8),new PointF(17,12),new PointF(10,16) });
                break;
            case SportIcon.Complete:
            case SportIcon.Check:
                g.DrawEllipse(p, 3, 3, 18, 18); Lines(g, p, (7,12),(10.5f,15.5f),(17.5f,8));
                break;
            case SportIcon.Cancel:
                g.DrawEllipse(p, 3, 3, 18, 18); g.DrawLine(p, 8, 8, 16, 16); g.DrawLine(p, 16, 8, 8, 16);
                break;
            case SportIcon.Back:
                Lines(g, p, (10,5),(3,12),(10,19)); g.DrawLine(p, 4, 12, 21, 12);
                break;
            case SportIcon.Close:
                g.DrawLine(p, 5, 5, 19, 19); g.DrawLine(p, 19, 5, 5, 19);
                break;
            case SportIcon.Lock:
                RoundRect(g, p, 5, 10, 14, 11, 2); g.DrawArc(p, 8, 3, 8, 12, 180, 180); g.FillEllipse(b, 11, 14, 2, 3);
                break;
            case SportIcon.Unlock:
                RoundRect(g, p, 5, 10, 14, 11, 2); g.DrawArc(p, 9, 3, 8, 12, 190, 145); g.DrawLine(p, 17, 7, 20, 7); g.FillEllipse(b, 11, 14, 2, 3);
                break;
            case SportIcon.Password:
                g.DrawEllipse(p, 3, 7, 8, 8); g.DrawLine(p, 10, 11, 21, 11); g.DrawLine(p, 17, 11, 17, 15); g.DrawLine(p, 20, 11, 20, 14);
                break;
            case SportIcon.Eye:
                using (var path = new GraphicsPath()) { path.AddBezier(2,12,7,5,17,5,22,12); path.AddBezier(22,12,17,19,7,19,2,12); g.DrawPath(p,path); }
                g.DrawEllipse(p, 9, 9, 6, 6); g.FillEllipse(b, 11, 11, 2, 2);
                break;
            case SportIcon.EyeOff:
                using (var path = new GraphicsPath()) { path.AddBezier(2,12,7,5,17,5,22,12); path.AddBezier(22,12,17,19,7,19,2,12); g.DrawPath(p,path); }
                g.DrawLine(p, 4, 3, 20, 21);
                break;
            case SportIcon.Shield:
                using (var path = new GraphicsPath()) { path.AddLines(new[] { new PointF(12,2),new PointF(20,5),new PointF(19,14),new PointF(12,22),new PointF(5,14),new PointF(4,5),new PointF(12,2) }); g.DrawPath(p,path); }
                Lines(g, p, (8,12),(11,15),(16,9));
                break;
            case SportIcon.Clock:
                g.DrawEllipse(p, 3, 3, 18, 18); g.DrawLine(p, 12, 7, 12, 13); g.DrawLine(p, 12, 13, 16, 15);
                break;
            case SportIcon.Location:
                using (var path = new GraphicsPath())
                {
                    // AddBezier nhận 4 điểm (8 tọa độ). Bốn đoạn cong tạo thành ghim vị trí khép kín.
                    path.AddBezier(12, 22, 7, 17, 4, 12, 5, 8);
                    path.AddBezier(5, 8, 6, 3, 9, 2, 12, 2);
                    path.AddBezier(12, 2, 18, 2, 20, 7, 19, 11);
                    path.AddBezier(19, 11, 18, 15, 15, 19, 12, 22);
                    path.CloseFigure();
                    g.DrawPath(p, path);
                }
                g.DrawEllipse(p, 9, 7, 6, 6);
                break;
            case SportIcon.Gift:
                RoundRect(g, p, 3, 9, 18, 12, 2); g.DrawRectangle(p, 2, 7, 20, 5); g.DrawLine(p, 12, 7, 12, 21);
                g.DrawArc(p, 6, 2, 6, 6, 180, 180); g.DrawArc(p, 12, 2, 6, 6, 180, 180);
                break;
            case SportIcon.Report:
                RoundRect(g, p, 4, 2.5f, 16, 19, 2); g.DrawLine(p, 8, 17, 8, 13); g.DrawLine(p, 12, 17, 12, 9); g.DrawLine(p, 16, 17, 16, 6);
                g.DrawLine(p, 7, 5, 13, 5);
                break;
            case SportIcon.Print:
                g.DrawRectangle(p, 6, 3, 12, 6); RoundRect(g, p, 3, 8, 18, 9, 2); g.DrawRectangle(p, 6, 14, 12, 7); g.FillEllipse(b, 17, 11, 1.7f, 1.7f);
                break;
            case SportIcon.Download:
                g.DrawLine(p, 12, 3, 12, 15); Lines(g, p, (7,11),(12,16),(17,11)); Lines(g, p, (4,18),(4,21),(20,21),(20,18));
                break;
            case SportIcon.Phone:
                Lines(g, p, (6,3),(3,6),(5,12),(12,19),(18,21),(21,18)); g.DrawLine(p, 6, 3, 10, 8); g.DrawLine(p, 16, 14, 21, 18);
                g.DrawLine(p, 10, 8, 7, 11); g.DrawLine(p, 16, 14, 13, 17);
                break;
            case SportIcon.Email:
                RoundRect(g, p, 2.5f, 5, 19, 14, 2); Lines(g, p, (3,7),(12,14),(21,7)); Lines(g, p, (3,18),(9,12)); Lines(g, p, (21,18),(15,12));
                break;
            case SportIcon.Info:
                g.DrawEllipse(p, 3, 3, 18, 18); g.FillEllipse(b, 11, 7, 2, 2); g.DrawLine(p, 12, 11, 12, 17);
                break;
            case SportIcon.Database:
                g.DrawEllipse(p, 4, 3, 16, 6); g.DrawArc(p, 4, 7, 16, 6, 0, 180); g.DrawArc(p, 4, 12, 16, 6, 0, 180); g.DrawArc(p, 4, 16, 16, 6, 0, 180);
                g.DrawLine(p, 4, 6, 4, 19); g.DrawLine(p, 20, 6, 20, 19);
                break;
            case SportIcon.Bell:
                g.DrawArc(p, 6, 4, 12, 14, 180, 180); Lines(g, p, (6,11),(5,18),(19,18),(18,11)); g.DrawArc(p, 10, 17, 4, 5, 0, 180); g.DrawLine(p, 12, 2, 12, 4);
                break;
            case SportIcon.Football:
                g.DrawEllipse(p, 2.5f, 2.5f, 19, 19);
                g.FillPolygon(b, new[] { new PointF(12,7),new PointF(16,10),new PointF(14.5f,15),new PointF(9.5f,15),new PointF(8,10) });
                g.DrawLine(p, 12, 7, 12, 3); g.DrawLine(p, 16, 10, 20, 8); g.DrawLine(p, 14.5f, 15, 17, 19);
                g.DrawLine(p, 9.5f, 15, 7, 19); g.DrawLine(p, 8, 10, 4, 8);
                break;
            case SportIcon.Badminton:
                g.DrawEllipse(p, 5, 3, 9, 7); Lines(g, p, (6,9),(14,18),(18,14),(10,5));
                g.DrawLine(p, 9, 9, 16, 16); g.DrawLine(p, 12, 7, 18, 14); g.FillEllipse(b, 16, 16, 5, 5);
                break;
            case SportIcon.Volleyball:
                g.DrawEllipse(p, 2.5f, 2.5f, 19, 19); g.DrawArc(p, 4, 2, 14, 13, 25, 105);
                g.DrawArc(p, 7, 8, 15, 13, 120, 105); g.DrawArc(p, 2, 8, 14, 14, 250, 100); g.DrawLine(p, 12, 3, 12, 10);
                break;
            case SportIcon.Star:
                g.FillPolygon(b, new[] { new PointF(12,2),new PointF(14.8f,8.5f),new PointF(22,9),new PointF(16.5f,13.5f),new PointF(18.2f,21),new PointF(12,17),new PointF(5.8f,21),new PointF(7.5f,13.5f),new PointF(2,9),new PointF(9.2f,8.5f) });
                break;
            case SportIcon.Sun:
                g.DrawEllipse(p, 7, 7, 10, 10);
                for (int i = 0; i < 8; i++) { double a=i*Math.PI/4; g.DrawLine(p, 12+(float)Math.Cos(a)*7,12+(float)Math.Sin(a)*7,12+(float)Math.Cos(a)*10,12+(float)Math.Sin(a)*10); }
                break;
        }
        return bmp;
    }

    private static void Lines(Graphics g, Pen p, params (float X, float Y)[] points)
        => g.DrawLines(p, points.Select(x => new PointF(x.X, x.Y)).ToArray());

    private static void RoundRect(Graphics g, Pen p, float x, float y, float width, float height, float radius)
    {
        using var path = new GraphicsPath();
        float d = radius * 2;
        path.AddArc(x, y, d, d, 180, 90); path.AddArc(x + width - d, y, d, d, 270, 90);
        path.AddArc(x + width - d, y + height - d, d, d, 0, 90); path.AddArc(x, y + height - d, d, d, 90, 90);
        path.CloseFigure(); g.DrawPath(p, path);
    }
}
