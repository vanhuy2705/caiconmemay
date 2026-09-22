#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmBookingSchedule
{
    private System.ComponentModel.IContainer? components=null;private Panel top=null!;private DateTimePicker dtDate=null!;private ComboBox cboStatus=null!;private TextBox txtSearch=null!;private Button btnRefresh=null!,btnStart=null!,btnCancel=null!,btnComplete=null!;private DataGridView grid=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){top=new Panel();dtDate=new DateTimePicker();cboStatus=new ComboBox();txtSearch=new TextBox();btnRefresh=new Button();btnStart=new Button();btnCancel=new Button();btnComplete=new Button();grid=new DataGridView();SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=92;top.BackColor=Color.White;
        var pageTitle=new Label{Text="Lịch đặt sân",AutoSize=true,Font=new Font("Segoe UI Semibold",15.5F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(16,10)};
        var pageSub=new Label{Text="Tra cứu và cập nhật trạng thái các lượt đặt trong ngày",AutoSize=true,Font=new Font("Segoe UI",8.3F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(18,38)};
        dtDate.Format=DateTimePickerFormat.Short;dtDate.Location=new Point(16,48);dtDate.Width=150;
        cboStatus.Location=new Point(174,48);cboStatus.Width=160;cboStatus.DropDownStyle=ComboBoxStyle.DropDownList;
        txtSearch.PlaceholderText="Tìm mã đơn / khách / sân...";txtSearch.Location=new Point(342,48);txtSearch.Width=240;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        btnRefresh.Text="Làm mới";btnRefresh.Size=new Size(92,38);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnStart.Text="Bắt đầu";btnStart.Size=new Size(104,38);btnStart.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnComplete.Text="Hoàn tất";btnComplete.Size=new Size(108,38);btnComplete.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnCancel.Text="Hủy đơn";btnCancel.Size=new Size(104,38);btnCancel.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,dtDate,cboStatus,txtSearch,btnRefresh,btnStart,btnComplete,btnCancel});
        top.Resize+=(_,__)=>{
            btnCancel.Left=top.Width-btnCancel.Width-16;
            btnComplete.Left=btnCancel.Left-btnComplete.Width-8;
            btnStart.Left=btnComplete.Left-btnStart.Width-8;
            btnRefresh.Left=btnStart.Left-btnRefresh.Width-8;
            txtSearch.Width=Math.Max(160,btnRefresh.Left-txtSearch.Left-14);
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);}
}
