#nullable enable
namespace QuanLyThueSanTheThao.Forms.Employee;

partial class FrmEmployeeAccount
{
    private System.ComponentModel.IContainer? components = null;
    private QuanLyThueSanTheThao.Forms.Common.RoundedPanel card = null!;
    private TextBox txtCode = null!, txtName = null!, txtPhone = null!, txtEmail = null!, txtPosition = null!;
    private QuanLyThueSanTheThao.Forms.Common.RoundedButton btnSave = null!, btnPassword = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        card = new QuanLyThueSanTheThao.Forms.Common.RoundedPanel(); txtCode = new TextBox(); txtName = new TextBox();
        txtPhone = new TextBox(); txtEmail = new TextBox(); txtPosition = new TextBox();
        btnSave = new QuanLyThueSanTheThao.Forms.Common.RoundedButton(); btnPassword = new QuanLyThueSanTheThao.Forms.Common.RoundedButton();
        SuspendLayout(); BackColor = QuanLyThueSanTheThao.Helpers.AppTheme.Background; AutoScaleMode = AutoScaleMode.Dpi; AutoScroll = true; Padding=new Padding(12);
        card.Dock = DockStyle.Top; card.Height = 420; card.Radius = 16; card.BorderColor = QuanLyThueSanTheThao.Helpers.AppTheme.Border; card.Padding = new Padding(24); card.Margin = new Padding(0,8,0,12);
        var badge = new QuanLyThueSanTheThao.Forms.Common.IconBadge { IconImage = QuanLyThueSanTheThao.Helpers.SportIcons.Get(QuanLyThueSanTheThao.Helpers.SportIcon.Employee,20,Color.White), AccentColor = QuanLyThueSanTheThao.Helpers.AppTheme.Accent2, Size = new Size(42,42), Location = new Point(24,20) };
        var title = new Label { Text="Thông tin cá nhân",AutoSize=true,Font=new Font("Segoe UI Semibold",13.5F,FontStyle.Bold),ForeColor=QuanLyThueSanTheThao.Helpers.AppTheme.Text,Location=new Point(76,22) };
        var sub = new Label { Text="Cập nhật thông tin cá nhân và bảo mật tài khoản nhân viên.",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=QuanLyThueSanTheThao.Helpers.AppTheme.Muted,Location=new Point(77,48) };
        var sep = new Panel { BackColor=Color.FromArgb(238,244,248),Location=new Point(24,80),Height=1,Width=700,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right };
        card.Controls.AddRange(new Control[] { badge,title,sub,sep });

        var table=new TableLayoutPanel{Location=new Point(24,100),Size=new Size(700,260),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right,ColumnCount=2,RowCount=3,BackColor=Color.Transparent};
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        for(int i=0;i<3;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,86F));

        Control MakeField(string l,Control c){
            var p=new Panel{Dock=DockStyle.Fill,Padding=new Padding(0,0,8,0),BackColor=Color.Transparent};
            var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(2,2)};
            c.Location=new Point(2,22);c.Height=36;c.Dock=DockStyle.Bottom;
            p.Controls.AddRange(new Control[]{lb,c});
            return p;
        }
        table.Controls.Add(MakeField("Mã nhân viên",txtCode),0,0);txtCode.ReadOnly=true;
        table.Controls.Add(MakeField("Email",txtEmail),1,0);
        table.Controls.Add(MakeField("Họ tên *",txtName),0,1);
        table.Controls.Add(MakeField("Chức vụ",txtPosition),1,1);txtPosition.ReadOnly=true;
        table.Controls.Add(MakeField("Số điện thoại",txtPhone),0,2);

        var pnlBtn=new Panel{Location=new Point(24,360),Size=new Size(700,48),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right,BackColor=Color.Transparent};
        btnSave.Text="✓  Lưu thay đổi";btnSave.Size=new Size(140,40);btnSave.Location=new Point(0,4);
        btnPassword.Text="🔒  Đổi mật khẩu";btnPassword.Size=new Size(140,40);btnPassword.Location=new Point(150,4);
        pnlBtn.Controls.AddRange(new Control[]{btnSave,btnPassword});

        card.Controls.AddRange(new Control[]{table,pnlBtn});
        card.Resize+=(s,e)=>{
            sep.Width=card.Width-48;
            table.Width=card.Width-48;
            pnlBtn.Width=card.Width-48;
            bool narrow=card.Width<600;
            if(narrow && table.ColumnCount==2){
                table.ColumnCount=1;table.RowCount=5;
                table.ColumnStyles.Clear();table.RowStyles.Clear();
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F));
                for(int i=0;i<5;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,86F));
                var cs=table.Controls.Cast<Control>().ToList();table.Controls.Clear();
                int r=0;foreach(var c in cs){table.Controls.Add(c,0,r++);}
                card.Height=520;
            }else if(!narrow && table.ColumnCount==1){
                table.ColumnCount=2;table.RowCount=3;
                table.ColumnStyles.Clear();table.RowStyles.Clear();
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
                for(int i=0;i<3;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,86F));
                table.Controls.Clear();
                table.Controls.Add(MakeField("Mã nhân viên",txtCode),0,0);
                table.Controls.Add(MakeField("Email",txtEmail),1,0);
                table.Controls.Add(MakeField("Họ tên *",txtName),0,1);
                table.Controls.Add(MakeField("Chức vụ",txtPosition),1,1);
                table.Controls.Add(MakeField("Số điện thoại",txtPhone),0,2);
                card.Height=420;
            }
        };

        Controls.Add(card);ResumeLayout(false);
    }
}
