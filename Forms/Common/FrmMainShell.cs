using QuanLyThueSanTheThao.Forms.Admin;
using QuanLyThueSanTheThao.Forms.Customer;
using QuanLyThueSanTheThao.Forms.Employee;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Common;

public partial class FrmMainShell : Form
{
    private Form? _current;
    private Button? _activeButton;
    private FlowLayoutPanel _menu = null!;
    private Panel _content = null!;
    private Label _pageTitle = null!;
    private Label _clock = null!;
    private readonly System.Windows.Forms.Timer _clockTimer = new() { Interval = 1000 };
    private readonly List<(RoundedButton Button, MenuDef Definition)> _menuButtons = new();

    public FrmMainShell()
    {
        InitializeComponent();AppTheme.Upgrade(this);
        BuildShell();
        _clockTimer.Tick += (_,__) => UpdateClock();
        _clockTimer.Start();
        UpdateClock();
        Shown += (_,__) => OpenDefault();
        FormClosed += (_,__) => _clockTimer.Stop();
    }

    private record MenuDef(string Text, SportIcon Icon, Func<Form> Create);

    private List<MenuDef> GetMenuItems() => SessionContext.Role switch
    {
        "Admin" => new List<MenuDef>
        {
            new("Tổng quan",SportIcon.Home,()=>new FrmAdminDashboard()),
            new("Loại sân",SportIcon.Category,()=>new FrmFieldTypes()),
            new("Quản lý sân",SportIcon.Field,()=>new FrmFields()),
            new("Khách hàng",SportIcon.Customer,()=>new FrmCustomers()),
            new("Đặt sân",SportIcon.Booking,()=>new FrmBooking()),
            new("Lịch đặt sân",SportIcon.Calendar,()=>new FrmBookingSchedule()),
            new("Hóa đơn",SportIcon.Invoice,()=>new FrmInvoices()),
            new("Voucher",SportIcon.Voucher,()=>new FrmVouchers()),
            new("Khuyến mãi",SportIcon.Promotion,()=>new FrmPromotions()),
            new("Tài khoản",SportIcon.Account,()=>new FrmAccounts()),
            new("Nhân viên",SportIcon.Employee,()=>new FrmEmployees()),
            new("Thống kê",SportIcon.Statistics,()=>new FrmStatistics()),
            new("Cấu hình",SportIcon.Settings,()=>new FrmSettings())
        },
        "Employee" => new List<MenuDef>
        {
            new("Trang chủ",SportIcon.Home,()=>new FrmEmployeeDashboard()),
            new("Đặt sân",SportIcon.Booking,()=>new FrmBooking()),
            new("Lịch đặt sân",SportIcon.Calendar,()=>new FrmBookingSchedule()),
            new("Hóa đơn",SportIcon.Invoice,()=>new FrmInvoices()),
            new("Khách hàng",SportIcon.Customer,()=>new FrmCustomers()),
            new("Thông tin sân",SportIcon.Field,()=>new FrmFields(true)),
            new("Loại sân",SportIcon.Category,()=>new FrmFieldTypes(true)),
            new("Thống kê",SportIcon.Statistics,()=>new FrmStatistics()),
            new("Tài khoản",SportIcon.Account,()=>new FrmEmployeeAccount())
        },
        _ => new List<MenuDef>
        {
            new("Trang chủ",SportIcon.Home,()=>new FrmCustomerHome()),
            new("Đặt sân",SportIcon.Booking,()=>new FrmBooking(customerMode:true)),
            new("Lịch sử đặt sân",SportIcon.Calendar,()=>new FrmCustomerBookingHistory()),
            new("Hóa đơn của tôi",SportIcon.Invoice,()=>new FrmCustomerInvoices()),
            new("Voucher của tôi",SportIcon.Voucher,()=>new FrmCustomerVouchers()),
            new("Thông tin cá nhân",SportIcon.Account,()=>new FrmCustomerProfile())
        }
    };

    private void BuildShell()
    {
        root.Controls.Clear();
        _menuButtons.Clear();
        // Một sidebar thống nhất giúp menu Admin không còn bị ép/ẩn trên màn hình nhỏ.
        BuildSideShell(SessionContext.Role == "Customer");
        foreach (var def in GetMenuItems()) AddMenuButton(def);
        AppTheme.Smooth(this);
    }

    private void BuildAdminShell()
    {
        root.BackColor = AppTheme.Background;

        var top = new Panel
        {
            Dock = DockStyle.Top,
            Height = 78,
            BackColor = Color.White,
            Padding = new Padding(18, 6, 18, 6)
        };
        top.Paint += (_, e) =>
        {
            using var p = new Pen(Color.FromArgb(229, 237, 242));
            e.Graphics.DrawLine(p, 0, top.Height - 1, top.Width, top.Height - 1);
        };

        var brand = new Panel { Dock = DockStyle.Left, Width = 205, BackColor = Color.White };
        var logo = new IconBadge { IconImage = SportIcons.Get(SportIcon.Field,20,Color.White), AccentColor = AppTheme.Accent2, Size = new Size(44,44), Location = new Point(2,10) };
        var brandTitle = new Label { Text = "THUÊ SÂN", AutoSize = true, Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold), ForeColor = Color.FromArgb(18,53,78), Location = new Point(52,8) };
        var brandSub = new Label { Text = "Đặt sân dễ dàng · Chơi hết mình", AutoSize = true, Font = new Font("Segoe UI", 7.2F), ForeColor = AppTheme.Muted, Location = new Point(53,37) };
        brand.Controls.AddRange(new Control[] { logo, brandTitle, brandSub });

        _menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White,
            Padding = new Padding(2, 0, 2, 0)
        };

        var user = new Panel { Dock = DockStyle.Right, Width = 220, BackColor = Color.White };
        var avatar = new IconBadge { IconImage = SportIcons.Get(SportIcon.User,18,AppTheme.Text), AccentColor = Color.FromArgb(215,228,237), Filled = true, Size = new Size(38,38), Location = new Point(8,11) };
        var name = new Label { Text = SessionContext.FullName, AutoSize = false, Width = 145, Height = 22, Font = new Font("Segoe UI Semibold", 8.8F, FontStyle.Bold), ForeColor = AppTheme.Text, Location = new Point(54,10), TextAlign = ContentAlignment.MiddleLeft };
        var role = new Label { Text = "Quản trị viên ⌄", AutoSize = false, Width = 145, Height = 20, Font = new Font("Segoe UI", 7.8F), ForeColor = AppTheme.Muted, Location = new Point(54,31), TextAlign = ContentAlignment.MiddleLeft };
        var logout = new RoundedButton { Text = "Đăng xuất", Width = 78, Height = 25, Radius = 8, BackColor = Color.FromArgb(241,248,250), ForeColor = AppTheme.Text, Font = new Font("Segoe UI",7.5F), Location = new Point(128,51) };
        logout.Click += (_,__) => Close();
        user.Controls.AddRange(new Control[] { avatar, name, role, logout });

        top.Controls.Add(_menu);
        top.Controls.Add(user);
        top.Controls.Add(brand);

        _content = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background, Padding = new Padding(16, 12, 16, 16) };
        _pageTitle = new Label { Visible = false };
        _clock = new Label { Visible = false };

        var accentStrip = new Panel { Dock = DockStyle.Top, Height = 3, BackColor = AppTheme.Accent };
        accentStrip.Paint += (_, e) =>
        {
            if (accentStrip.Width <= 1 || accentStrip.Height <= 1) return;
            using var br = new System.Drawing.Drawing2D.LinearGradientBrush(accentStrip.ClientRectangle, AppTheme.Accent, AppTheme.Accent2, 0f);
            e.Graphics.FillRectangle(br, accentStrip.ClientRectangle);
        };

        root.Controls.Add(_content);
        root.Controls.Add(top);
        root.Controls.Add(accentStrip);
    }

    private void BuildSideShell(bool customer)
    {
        root.BackColor = customer ? AppTheme.BackgroundBlue : Color.FromArgb(238, 248, 252);
        var sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 232,
            BackColor = customer ? Color.White : AppTheme.Sidebar,
            Padding = new Padding(12, 10, 12, 12)
        };
        if (customer)
        {
            sidebar.Paint += (_,e) => { using var p = new Pen(Color.FromArgb(224,234,239)); e.Graphics.DrawLine(p, sidebar.Width-1,0,sidebar.Width-1,sidebar.Height); };
        }

        var brand = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = sidebar.BackColor };
        var brandIcon = new IconBadge { IconImage = SportIcons.Get(SportIcon.Field,20,Color.White), AccentColor = AppTheme.Accent, Size = new Size(40,40), Location = new Point(2,5) };
        var brandText = new Label { Text = "SportField", AutoSize = true, Font = new Font("Segoe UI Semibold",15F,FontStyle.Bold), ForeColor = customer ? AppTheme.Text : Color.White, Location = new Point(48,6) };
        var fieldStart = brandText.Text.IndexOf("Field", StringComparison.Ordinal);
        var brandSub = new Label { Text = "Đặt sân dễ dàng · Chơi hết mình", AutoSize = true, Font = new Font("Segoe UI",6.8F), ForeColor = customer ? AppTheme.Muted : Color.FromArgb(178,209,215), Location = new Point(49,34) };
        brand.Controls.AddRange(new Control[] { brandIcon, brandText, brandSub });

        var profile = new Panel { Dock = DockStyle.Top, Height = 66, BackColor = sidebar.BackColor, Padding = new Padding(0,4,0,4) };
        var av = new IconBadge { IconImage = SportIcons.Get(SportIcon.User,19,customer ? AppTheme.Text : AppTheme.Sidebar), AccentColor = customer ? Color.FromArgb(205,226,235) : Color.FromArgb(221,230,234), Size = new Size(40,40), Location = new Point(4,8) };
        var n = new Label { Text = SessionContext.FullName, AutoSize = false, Width = 155, Height = 22, Font = new Font("Segoe UI Semibold",9F,FontStyle.Bold), ForeColor = customer ? AppTheme.Text : Color.White, Location = new Point(52,7), TextAlign = ContentAlignment.MiddleLeft };
        var roleName = SessionContext.Role == "Admin" ? "Quản trị viên" : customer ? "Khách hàng" : "Nhân viên";
        var r = new Label { Text = roleName, AutoSize = false, Width = 155, Height = 20, Font = new Font("Segoe UI",8F), ForeColor = customer ? AppTheme.Muted : Color.FromArgb(166,201,208), Location = new Point(52,28), TextAlign = ContentAlignment.MiddleLeft };
        profile.Controls.AddRange(new Control[] { av,n,r });

        _menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = sidebar.BackColor,
            Padding = new Padding(0, 4, 0, 4)
        };

        var logout = new RoundedButton
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            Text = "Đăng xuất",
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10,0,0,0),
            BackColor = sidebar.BackColor,
            ForeColor = customer ? AppTheme.Text : Color.White,
            HoverColor = customer ? Color.FromArgb(238,247,248) : Color.FromArgb(16,51,61),
            Font = new Font("Segoe UI",9F)
        };
        logout.Image = SportIcons.Get(SportIcon.Logout,18,customer ? AppTheme.Text : Color.White);
        logout.TextImageRelation = TextImageRelation.ImageBeforeText;
        logout.Click += (_,__) => Close();

        sidebar.Controls.Add(_menu);
        sidebar.Controls.Add(logout);
        sidebar.Controls.Add(profile);
        sidebar.Controls.Add(brand);

        var main = new Panel { Dock = DockStyle.Fill, BackColor = root.BackColor };
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = customer ? 60 : 50,
            BackColor = customer ? Color.FromArgb(239,249,253) : Color.FromArgb(12,60,80),
            Padding = new Padding(14, 8, 16, 8)
        };
        if (!customer)
        {
            header.Paint += (_,e) =>
            {
                if (header.Width <= 1 || header.Height <= 1) return;
                using var br = new System.Drawing.Drawing2D.LinearGradientBrush(header.ClientRectangle, Color.FromArgb(12,63,82), Color.FromArgb(31,107,145), 0f);
                e.Graphics.FillRectangle(br, header.ClientRectangle);
                using var turf = new SolidBrush(Color.FromArgb(30, AppTheme.Accent));
                e.Graphics.FillEllipse(turf, header.Width-360, -45, 340, 130);
            };
        }

        _pageTitle = new Label
        {
            Text = customer ? "" : "SportField",
            AutoSize = true,
            Font = new Font("Segoe UI Semibold",10F,FontStyle.Bold),
            ForeColor = customer ? AppTheme.Text : Color.White,
            Location = new Point(16,17)
        };
        _clock = new Label
        {
            AutoSize = false,
            Width = 235,
            Height = 36,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = customer ? AppTheme.Text : Color.White,
            Font = new Font("Segoe UI Semibold",8.2F),
            Location = new Point(900,6)
        };
        var notify = new IconBadge { IconImage = SportIcons.Get(SportIcon.Clock,15,Color.White), AccentColor = customer ? AppTheme.Info : Color.FromArgb(44,84,103), Size = new Size(32,32), Anchor = AnchorStyles.Top|AnchorStyles.Right, Location = new Point(1160,8) };
        var headerAvatar = new IconBadge { IconImage = SportIcons.Get(SportIcon.User,15,AppTheme.Text), AccentColor = customer ? Color.FromArgb(221,236,244) : Color.FromArgb(234,240,243), Size = new Size(32,32), Anchor = AnchorStyles.Top|AnchorStyles.Right, Location = new Point(1200,8) };
        header.Controls.AddRange(new Control[] { _pageTitle,_clock,notify,headerAvatar });
        header.Resize += (_,__) =>
        {
            headerAvatar.Left = header.Width - 48;
            notify.Left = headerAvatar.Left - 42;
            _clock.Left = notify.Left - _clock.Width - 12;
        };

        _content = new Panel { Dock = DockStyle.Fill, BackColor = root.BackColor, Padding = customer ? new Padding(14,8,14,14) : new Padding(12) };
        main.Controls.Add(_content);
        main.Controls.Add(header);

        root.Controls.Add(main);
        root.Controls.Add(sidebar);
    }

    private void AddMenuButton(MenuDef def)
    {
        // Tất cả vai trò dùng menu dọc để không chồng chữ khi DPI/màn hình thay đổi.
        bool admin = false;
        bool customer = SessionContext.Role == "Customer";
        var btn = new RoundedButton
        {
            Text = def.Text,
            TextAlign = admin ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI Semibold", admin ? 7.8F : 9.2F, FontStyle.Regular),
            ForeColor = admin ? Color.FromArgb(27,79,136) : (customer ? AppTheme.Text : Color.White),
            BackColor = admin ? Color.White : (customer ? Color.White : AppTheme.Sidebar),
            HoverColor = admin ? Color.FromArgb(234,247,240) : (customer ? Color.FromArgb(237,249,245) : Color.FromArgb(14,58,65)),
            Radius = admin ? 8 : 10,
            Cursor = Cursors.Hand,
            Margin = admin ? new Padding(2, 0, 2, 0) : new Padding(0, 2, 0, 2),
            Width = admin ? Math.Max(68, Math.Min(90, TextRenderer.MeasureText(def.Text, new Font("Segoe UI",7.5F)).Width + 18)) : 204,
            Height = admin ? 62 : 38,
            Padding = admin ? new Padding(0) : new Padding(10,0,0,0),
            Image = SportIcons.Get(def.Icon, admin ? 19 : 18, admin || customer ? AppTheme.Text : Color.White),
            ImageAlign = admin ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft,
            TextImageRelation = admin ? TextImageRelation.ImageAboveText : TextImageRelation.ImageBeforeText
        };
        btn.Click += (_,__) => { Activate(btn); OpenChild(def.Text, def.Create()); };
        _menu.Controls.Add(btn);
        _menuButtons.Add((btn, def));
    }

    private void Activate(Button button)
    {
        if (_activeButton is RoundedButton prev)
        {
            bool customer = SessionContext.Role == "Customer";
            prev.BackColor = customer ? Color.White : AppTheme.Sidebar;
            prev.ForeColor = customer ? AppTheme.Text : Color.White;
            prev.Font = new Font("Segoe UI Semibold", 9.2F, FontStyle.Regular);
            prev.IndicatorBar = null;
            var old = _menuButtons.FirstOrDefault(x => ReferenceEquals(x.Button, prev));
            if (old.Button != null)
                prev.Image = SportIcons.Get(old.Definition.Icon, 18, customer ? AppTheme.Text : Color.White);
            prev.Invalidate();
        }
        _activeButton = button;
        if (button is RoundedButton rb)
        {
            Fx.StopColor(rb);
            rb.BackColor = SessionContext.Role == "Customer" ? Color.FromArgb(16, 172, 99) : Color.FromArgb(19, 156, 101);
            rb.ForeColor = Color.White;
            rb.IndicatorBar = Color.FromArgb(120, 255, 255, 255);
            rb.IndicatorBottom = false;
            rb.SyncBaseColor();
            var current = _menuButtons.FirstOrDefault(x => ReferenceEquals(x.Button, rb));
            if (current.Button != null)
                rb.Image = SportIcons.Get(current.Definition.Icon, 18, Color.White);
            rb.Invalidate();
        }
        button.Font = new Font("Segoe UI Semibold", 9.2F, FontStyle.Bold);
    }

    private void OpenDefault()
    {
        var first = _menuButtons.FirstOrDefault();
        if (first.Button == null) return;
        Activate(first.Button);
        OpenChild(first.Definition.Text, first.Definition.Create());
    }

    private void OpenChild(string title, Form form)
    {
        _current?.Close();
        _current = form;
        _pageTitle.Text = title;
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        AppTheme.ApplyToForm(form, SessionContext.Role);
        _content.SuspendLayout();
        _content.Controls.Clear();
        _content.Controls.Add(form);
        _content.ResumeLayout();
        form.Show();
        Fx.Settle(form);
    }

    private void UpdateClock()
    {
        if (_clock == null) return;
        _clock.Text = SessionContext.Role == "Customer"
            ? DateTime.Now.ToString("dd/MM/yyyy   HH:mm")
            : DateTime.Now.ToString("dddd, dd/MM/yyyy\nHH:mm tt");
    }
}
