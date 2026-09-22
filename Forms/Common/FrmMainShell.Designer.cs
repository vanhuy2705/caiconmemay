#nullable enable
namespace QuanLyThueSanTheThao.Forms.Common;

partial class FrmMainShell
{
    private System.ComponentModel.IContainer? components = null;
    private Panel root = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        root = new Panel();
        SuspendLayout();
        root.Dock = DockStyle.Fill;
        root.BackColor = Color.FromArgb(244, 250, 252);
        Controls.Add(root);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1540, 900);
        MinimumSize = new Size(1180, 700);
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Text = "SportField - Quản lý thuê sân thể thao";
        ResumeLayout(false);
    }
}
