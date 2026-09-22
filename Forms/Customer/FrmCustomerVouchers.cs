using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Forms.Common;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Customer;

public partial class FrmCustomerVouchers : Form
{
    public FrmCustomerVouchers()
    {
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        Shown += (_, __) => LoadData();
    }

    private void LoadData()
    {
        flow.Controls.Clear();
        var dt = Db.Query(@"SELECT v.MaPhieuGiamGia,v.TenPhieuGiamGia,v.LoaiGiamGia,v.GiaTriGiam,v.MucGiamToiDa,v.DonToiThieu,v.NgayKetThuc,cv.DaSuDung
                            FROM PhieuGiamGiaKhachHang cv JOIN PhieuGiamGia v ON v.PhieuGiamGiaID=cv.PhieuGiamGiaID
                            WHERE cv.KhachHangID=@c ORDER BY cv.DaSuDung,v.NgayKetThuc",
            new SqlParameter("@c", SessionContext.CustomerId ?? 0));

        foreach (System.Data.DataRow r in dt.Rows)
        {
            bool used = Convert.ToBoolean(r["DaSuDung"]);
            bool expired = Convert.ToDateTime(r["NgayKetThuc"]) < DateTime.Now;
            bool percent = Convert.ToString(r["LoaiGiamGia"]) == "Percent";
            decimal val = Convert.ToDecimal(r["GiaTriGiam"]);
            string bigValue = percent ? $"{val:0.#}%" : (val >= 1000 ? $"{val / 1000:0.#}K" : $"{val:0.#}");

            var card = new RoundedPanel
            {
                Width = 402,
                Height = 148,
                BackColor = Color.White,
                Margin = new Padding(6),
                Radius = 14,
                BorderColor = (used || expired) ? Color.FromArgb(224, 230, 235) : Color.FromArgb(172, 224, 200)
            };

            // ---- dải giá trị bên trái ----
            var band = new Panel { Location = new Point(1, 1), Size = new Size(116, 146) };
            Color c1 = (used || expired) ? Color.FromArgb(168, 177, 187) : Color.FromArgb(16, 185, 110);
            Color c2 = (used || expired) ? Color.FromArgb(140, 150, 161) : AppTheme.Accent2;
            band.Paint += (_, e) =>
            {
                if (band.Width <= 1 || band.Height <= 1) return;
                using var br = new System.Drawing.Drawing2D.LinearGradientBrush(band.ClientRectangle, c1, c2, 90f);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(br, band.ClientRectangle);
                // lỗ đục kiểu vé
                using var hole = new SolidBrush(Color.FromArgb(243, 249, 248));
                e.Graphics.FillEllipse(hole, band.Width - 8, -8, 16, 16);
                e.Graphics.FillEllipse(hole, band.Width - 8, band.Height - 8, 16, 16);
                // đường xé chấm tròn
                using var dot = new SolidBrush(Color.FromArgb(90, Color.White));
                for (int yy = 16; yy < band.Height - 14; yy += 10)
                    e.Graphics.FillEllipse(dot, band.Width - 3, yy, 3.4f, 3.4f);
            };
            var capReduce = new Label { Text = "GIẢM", AutoSize = false, Size = new Size(116, 20), Location = new Point(0, 40), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold), ForeColor = Color.FromArgb(215, 245, 230) };
            var lblValue = new Label { Text = bigValue, AutoSize = false, Size = new Size(116, 40), Location = new Point(0, 58), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI Semibold", 17.5F, FontStyle.Bold), ForeColor = Color.White };
            band.Controls.AddRange(new Control[] { capReduce, lblValue });

            // ---- nội dung bên phải ----
            var name = new Label
            {
                Text = Convert.ToString(r["TenPhieuGiamGia"]),
                AutoSize = false,
                Size = new Size(252, 26),
                Location = new Point(134, 14),
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                ForeColor = (used || expired) ? Color.FromArgb(150, 160, 170) : AppTheme.Text
            };
            var codeChip = new Label
            {
                Text = Convert.ToString(r["MaPhieuGiamGia"]),
                Image = SportIcons.Get(SportIcon.Voucher, 14, (used || expired) ? Color.FromArgb(160, 170, 178) : AppTheme.AccentDark),
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(19, 0, 0, 0),
                AutoSize = true,
                Location = new Point(134, 44),
                Font = new Font("Segoe UI Semibold", 8.3F),
                ForeColor = (used || expired) ? Color.FromArgb(160, 170, 178) : AppTheme.AccentDark
            };
            string maxTxt = r["MucGiamToiDa"] != DBNull.Value ? $"\nTối đa {Convert.ToDecimal(r["MucGiamToiDa"]):N0}đ" : "";
            var info = new Label
            {
                Text = $"Đơn tối thiểu {Convert.ToDecimal(r["DonToiThieu"]):N0}đ{maxTxt}\nHSD: {Convert.ToDateTime(r["NgayKetThuc"]):dd/MM/yyyy}",
                AutoSize = true,
                Location = new Point(134, 68),
                Font = new Font("Segoe UI", 8.2F),
                ForeColor = (used || expired) ? Color.FromArgb(165, 174, 182) : AppTheme.Muted
            };

            var status = new Label
            {
                AutoSize = true,
                Location = new Point(134, 112),
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                Text = used ? "●  ĐÃ SỬ DỤNG" : (expired ? "●  HẾT HẠN" : "●  CÒN HIỆU LỰC"),
                ForeColor = (used || expired) ? Color.FromArgb(165, 174, 182) : AppTheme.AccentDark
            };

            card.Controls.AddRange(new Control[] { band, name, codeChip, info, status });
            flow.Controls.Add(card);
        }

        if (flow.Controls.Count == 0)
        {
            var empty = new Label
            {
                Text = "Bạn chưa có voucher nào.\nVoucher được tặng khi tích lũy điểm hoặc theo chương trình khuyến mãi.",
                Image = SportIcons.Get(SportIcon.Voucher, 22, AppTheme.Muted),
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(34, 0, 0, 0),
                AutoSize = false,
                Size = new Size(460, 70),
                Font = new Font("Segoe UI", 9F),
                ForeColor = AppTheme.Muted,
                Location = new Point(12, 20)
            };
            flow.Controls.Add(empty);
        }
    }
}
