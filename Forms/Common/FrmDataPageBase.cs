using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Common;

public class FrmDataPageBase : Form
{
    protected readonly Panel Header = new();
    protected readonly Label TitleLabel = new();
    protected readonly Label SubTitleLabel = new();
    protected readonly TextBox SearchBox = new();
    protected readonly SearchField SearchHost;
    protected readonly Button RefreshButton = new();
    protected readonly DataGridView Grid = new();

    protected FrmDataPageBase(string title)
    {
        BackColor = AppTheme.Background;
        Padding = new Padding(0);

        Header.Dock = DockStyle.Top;
        Header.Height = 84;
        Header.BackColor = Color.White;
        Header.Padding = new Padding(16);

        TitleLabel.Text = title;
        TitleLabel.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        TitleLabel.ForeColor = AppTheme.Text;
        TitleLabel.AutoSize = true;
        TitleLabel.Location = new Point(16, 12);

        SubTitleLabel.Text = "Tìm kiếm và quản lý dữ liệu";
        SubTitleLabel.Font = new Font("Segoe UI", 8.3F);
        SubTitleLabel.ForeColor = AppTheme.Muted;
        SubTitleLabel.AutoSize = true;
        SubTitleLabel.Location = new Point(18, 42);

        SearchBox.PlaceholderText = "Tìm kiếm...";
        SearchHost = new SearchField(SearchBox)
        {
            Size = new Size(280, 40),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };

        RefreshButton.Text = "↻  Làm mới";
        RefreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        RefreshButton.Size = new Size(104, 40);
        AppTheme.StyleSecondary(RefreshButton);

        Header.Controls.AddRange(new Control[]{TitleLabel,SubTitleLabel,SearchHost,RefreshButton});
        Header.Resize += (_,__) =>
        {
            RefreshButton.Left = Header.Width - RefreshButton.Width - 16;
            SearchHost.Left = RefreshButton.Left - SearchHost.Width - 10;
            SearchHost.Top = RefreshButton.Top = 22;
        };

        Grid.Dock = DockStyle.Fill;
        AppTheme.EnhanceGrid(Grid);
        Controls.Add(Grid);
        Controls.Add(Header);
        RefreshButton.Click += (_,__) => LoadData();
        SearchBox.TextChanged += (_,__) => ApplyFilter();
        Load += (_,__) => LoadData();
    }

    protected virtual void LoadData() { }
    protected virtual void ApplyFilter() { }
}
