#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmBookingSchedule
{
    private System.ComponentModel.IContainer? components=null;private Panel top=null!;private DateTimePicker dtDate=null!;private ComboBox cboStatus=null!;private TextBox txtSearch=null!;private Button btnRefresh=null!,btnStart=null!,btnCancel=null!,btnComplete=null!;private DataGridView grid=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        top=new Panel();dtDate=new DateTimePicker();cboStatus=new ComboBox();txtSearch=new TextBox();btnRefresh=new Button();btnStart=new Button();btnCancel=new Button();btnComplete=new Button();grid=new DataGridView();
        SuspendLayout();
        top.Dock=DockStyle.Top;top.Height=96;top.BackColor=Color.White;top.Padding=new Padding(12,8,12,8);
        var pageTitle=new Label{Text="Lịch đặt sân",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(12,8)};
        var pageSub=new Label{Text="Tra cứu và cập nhật trạng thái các lượt đặt",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(14,34)};
        dtDate.Format=DateTimePickerFormat.Short;dtDate.Location=new Point(12,58);dtDate.Width=140;dtDate.Height=32;
        cboStatus.Location=new Point(160,58);cboStatus.Width=150;cboStatus.Height=32;cboStatus.DropDownStyle=ComboBoxStyle.DropDownList;
        txtSearch.PlaceholderText="Tìm mã đơn / khách / sân...";txtSearch.Location=new Point(320,58);txtSearch.Width=220;txtSearch.Height=32;txtSearch.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;
        btnRefresh.Text="Làm mới";btnRefresh.Size=new Size(84,36);btnRefresh.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnStart.Text="Bắt đầu";btnStart.Size=new Size(92,36);btnStart.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnComplete.Text="Hoàn tất";btnComplete.Size=new Size(96,36);btnComplete.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        btnCancel.Text="Hủy đơn";btnCancel.Size=new Size(88,36);btnCancel.Anchor=AnchorStyles.Top|AnchorStyles.Right;
        top.Controls.AddRange(new Control[]{pageTitle,pageSub,dtDate,cboStatus,txtSearch,btnRefresh,btnStart,btnComplete,btnCancel});
        top.Resize+=(s,e)=>{
            btnCancel.Left=top.Width-btnCancel.Width-12;
            btnComplete.Left=btnCancel.Left-btnComplete.Width-8;
            btnStart.Left=btnComplete.Left-btnStart.Width-8;
            btnRefresh.Left=btnStart.Left-btnRefresh.Width-8;
            txtSearch.Width=Math.Max(140,btnRefresh.Left-txtSearch.Left-16);
        };
        grid.Dock=DockStyle.Fill;Controls.Add(grid);Controls.Add(top);BackColor=Color.FromArgb(243,249,248);ResumeLayout(false);
    }
}
