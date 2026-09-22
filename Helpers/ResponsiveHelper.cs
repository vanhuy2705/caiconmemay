using System.Drawing.Drawing2D;

namespace QuanLyThueSanTheThao.Helpers;

/// <summary>
/// V8 PRO Responsive Engine - Dàn trang hoàn thiện cho tất cả form
/// Không chồng control, tự co giãn theo 900x600 -> 1920x1080
/// </summary>
public static class ResponsiveHelper
{
    public const int MinRightPanel = 320;
    public const int DefaultRightPanel = 360;
    public const int MinLeftPanel = 380;

    // ============ SPLIT CONTAINER CRUD ============
    public static void FixSplitContainer(SplitContainer split, int rightWidth = 360, int minRight = 320)
    {
        if (split == null) return;
        split.Dock = DockStyle.Fill;
        split.SplitterWidth = 6;
        split.Panel1MinSize = MinLeftPanel;
        split.Panel2MinSize = minRight;
        split.FixedPanel = FixedPanel.Panel2;

        void Resize(object? s, EventArgs e)
        {
            try
            {
                if (split.IsDisposed || split.Width < 600) return;
                int targetRight = Math.Min(rightWidth, split.Width - MinLeftPanel - split.SplitterWidth);
                targetRight = Math.Max(minRight, targetRight);
                // Nếu màn hình hẹp < 1000, right panel chiếm 38% thay vì cố định
                if (split.Width < 1000)
                    targetRight = Math.Max(minRight, (int)(split.Width * 0.42));
                int newDistance = split.Width - targetRight - split.SplitterWidth;
                if (newDistance > 0 && Math.Abs(split.SplitterDistance - newDistance) > 10)
                    split.SplitterDistance = newDistance;

                // Fix input widths trong Panel2
                FixPanelInputs(split.Panel2);
            }
            catch { }
        }

        split.Resize += Resize;
        // Gọi ngay lần đầu sau khi form show
        split.FindForm()?.BeginInvoke((Action)(() => Resize(null, EventArgs.Empty)));
    }

    public static void FixPanelInputs(Panel panel)
    {
        if (panel == null || panel.IsDisposed) return;
        try
        {
            int available = panel.ClientSize.Width - panel.Padding.Horizontal - 8;
            if (available < 200) return;

            foreach (Control c in panel.Controls)
            {
                if (c is Label && c.AutoSize) continue;
                if (c is Button) continue;
                if (c is Panel && c.Name == "pnlNut") continue;
                if (c is TextBox || c is ComboBox || c is DateTimePicker || c is CheckBox)
                {
                    // Giữ nguyên Top, chỉ chỉnh Width để responsive
                    if (c.Width < 150 || c.Width > available)
                    {
                        c.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                        c.Width = Math.Max(180, available);
                    }
                }
            }

            // Đảm bảo các control Label + Input xếp dọc không chồng
            int maxBottom = panel.Padding.Top;
            var ordered = panel.Controls.OfType<Control>()
                .Where(x => x.Visible && x is not Button && x.Dock == DockStyle.None)
                .OrderBy(x => x.Top).ToList();

            foreach (var ctrl in ordered)
            {
                if (ctrl is Panel p && p.Controls.OfType<Button>().Any()) continue; // panel chứa nút
                if (ctrl.Top < maxBottom && ctrl is not Label)
                {
                    // Bị chồng -> đẩy xuống
                    ctrl.Top = maxBottom + 6;
                }
                if (ctrl is Label lbl && lbl.AutoSize)
                {
                    maxBottom = Math.Max(maxBottom, lbl.Bottom + 2);
                }
                else if (ctrl is TextBox || ctrl is ComboBox || ctrl is DateTimePicker)
                {
                    maxBottom = Math.Max(maxBottom, ctrl.Bottom + 10);
                }
                else
                {
                    maxBottom = Math.Max(maxBottom, ctrl.Bottom + 6);
                }
            }

            // Đặt lại panel nút xuống cuối
            var btnPanel = panel.Controls.OfType<Panel>().FirstOrDefault(p => p.Name.Contains("Nut") || p.Controls.OfType<Button>().Count() >= 2);
            if (btnPanel != null)
            {
                btnPanel.Top = maxBottom + 12;
                btnPanel.Width = Math.Max(200, available);
                btnPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            else
            {
                // Nếu nút lẻ không trong panel, đặt chúng ở cuối
                var buttons = panel.Controls.OfType<Button>().OrderBy(b => b.Left).ToList();
                if (buttons.Count > 0)
                {
                    int y = maxBottom + 12;
                    int x = panel.Padding.Left;
                    foreach (var b in buttons)
                    {
                        if (b.Top < y) b.Top = y;
                        // Xếp hàng ngang nếu đủ chỗ, không thì xuống dòng
                        if (x + b.Width > panel.ClientSize.Width - panel.Padding.Right)
                        {
                            x = panel.Padding.Left;
                            y += b.Height + 8;
                            b.Top = y;
                        }
                        b.Left = x;
                        x += b.Width + 8;
                    }
                }
            }

            panel.AutoScroll = true;
        }
        catch { }
    }

    // ============ TOP FILTER PANEL ============
    public static void FixTopPanel(Panel top, TextBox? searchBox, params Button[] buttons)
    {
        if (top == null) return;
        top.Dock = DockStyle.Top;
        top.AutoScroll = false;

        void Resize(object? s, EventArgs e)
        {
            try
            {
                if (top.IsDisposed || top.Width < 400) return;
                int right = top.Width - 12;
                // Buttons từ phải sang trái
                foreach (var btn in buttons.Reverse())
                {
                    if (btn == null || btn.IsDisposed) continue;
                    btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    right -= btn.Width + 8;
                    btn.Left = right;
                    btn.Top = Math.Max(48, (top.Height - btn.Height) / 2 + 8);
                }
                if (searchBox != null && !searchBox.IsDisposed)
                {
                    searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    int left = searchBox.Left;
                    int width = Math.Max(160, right - left - 12);
                    searchBox.Width = width;
                }
            }
            catch { }
        }

        top.Resize += Resize;
        top.FindForm()?.BeginInvoke((Action)(() => Resize(null, EventArgs.Empty)));
    }

    // ============ BOOKING FORM - TABLE LAYOUT ============
    public static void FixBookingForm(TableLayoutPanel mainLayout, Panel left, Panel right, int breakpoint = 1000)
    {
        if (mainLayout == null) return;
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.ColumnCount = 2;
        mainLayout.RowCount = 1;
        mainLayout.ColumnStyles.Clear();
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));

        void Resize(object? s, EventArgs e)
        {
            try
            {
                if (mainLayout.IsDisposed) return;
                bool narrow = mainLayout.Width < breakpoint;
                mainLayout.SuspendLayout();
                if (narrow)
                {
                    mainLayout.ColumnCount = 1;
                    mainLayout.RowCount = 2;
                    mainLayout.ColumnStyles.Clear();
                    mainLayout.RowStyles.Clear();
                    mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
                    mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
                    mainLayout.SetCellPosition(left, new TableLayoutPanelCellPosition(0, 0));
                    mainLayout.SetCellPosition(right, new TableLayoutPanelCellPosition(0, 1));
                }
                else
                {
                    mainLayout.ColumnCount = 2;
                    mainLayout.RowCount = 1;
                    mainLayout.ColumnStyles.Clear();
                    mainLayout.RowStyles.Clear();
                    mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
                    mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
                    mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                    mainLayout.SetCellPosition(left, new TableLayoutPanelCellPosition(0, 0));
                    mainLayout.SetCellPosition(right, new TableLayoutPanelCellPosition(1, 0));
                }
                mainLayout.ResumeLayout(true);
            }
            catch { }
        }

        mainLayout.Resize += Resize;
        mainLayout.FindForm()?.BeginInvoke((Action)(() => Resize(null, EventArgs.Empty)));
    }

    // ============ GENERIC FORM APPLY ============
    public static void Apply(Form form)
    {
        if (form == null || form.IsDisposed) return;
        form.AutoScaleMode = AutoScaleMode.Dpi;
        form.BackColor = AppTheme.Background;

        // Tìm SplitContainer để fix
        foreach (var split in FindControls<SplitContainer>(form))
        {
            FixSplitContainer(split);
        }

        // Tìm top panel có search + buttons
        foreach (var panel in FindControls<Panel>(form))
        {
            if (panel.Dock == DockStyle.Top && panel.Height >= 60 && panel.Height <= 110)
            {
                var search = panel.Controls.OfType<TextBox>().FirstOrDefault();
                var buttons = panel.Controls.OfType<Button>().ToArray();
                if (search != null && buttons.Length >= 1)
                {
                    FixTopPanel(panel, search, buttons);
                }
            }
        }

        // Đảm bảo DataGridView Fill
        foreach (var grid in FindControls<DataGridView>(form))
        {
            if (grid.Dock != DockStyle.Fill)
            {
                // Nếu không Fill thì để nguyên, nhưng đảm bảo ScrollBars
                grid.ScrollBars = ScrollBars.Both;
            }
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }

    private static IEnumerable<T> FindControls<T>(Control root) where T : Control
    {
        foreach (Control c in root.Controls)
        {
            if (c is T t) yield return t;
            foreach (var child in FindControls<T>(c)) yield return child;
        }
    }

    // ============ ĐẸP HƠN: Bo tròn panel + shadow nhẹ ============
    public static void StyleCard(Panel panel)
    {
        if (panel == null) return;
        panel.BackColor = Color.White;
        panel.Padding = new Padding(16);
        // Bo tròn sẽ do RoundedPanel tự xử lý, ở đây chỉ đảm bảo border
    }
}
