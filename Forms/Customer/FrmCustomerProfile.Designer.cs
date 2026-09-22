#nullable enable
namespace QuanLyThueSanTheThao.Forms.Customer;
partial class FrmCustomerProfile
{
    private System.ComponentModel.IContainer? components=null;private TextBox txtCode=null!,txtName=null!,txtPhone=null!,txtEmail=null!,txtAddress=null!;private DateTimePicker dtBirth=null!;private Label lblPoints=null!;private Button btnSave=null!,btnChangePassword=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        txtCode=new TextBox();txtName=new TextBox();txtPhone=new TextBox();txtEmail=new TextBox();txtAddress=new TextBox();dtBirth=new DateTimePicker();lblPoints=new Label();btnSave=new Button();btnChangePassword=new Button();
        SuspendLayout();
        BackColor=Color.FromArgb(239,248,253);AutoScroll=true;Padding=new Padding(12);AutoScaleMode=AutoScaleMode.Dpi;
        var header=new Panel{Dock=DockStyle.Top,Height=60,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Hồ sơ của tôi",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Cập nhật thông tin cá nhân và bảo mật tài khoản",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        header.Controls.AddRange(new Control[]{pageTitle,pageSub});
        var banner=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{Dock=DockStyle.Top,Height=96,BackColor=Color.White,Radius=14,BorderColor=Color.FromArgb(218,232,240),Padding=new Padding(16),Margin=new Padding(0,4,0,8)};
        var avatar=new Label{Text="♙",AutoSize=false,Size=new Size(52,52),Font=new Font("Segoe UI Symbol",18F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(16,20)};
        avatar.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,51,51),14));
        var hello=new Label{Text="Tài khoản khách hàng",AutoSize=true,Font=new Font("Segoe UI Semibold",12F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(84,22)};
        var helloSub=new Label{Text="Giữ thông tin chính xác để nhận ưu đãi nhanh nhất",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(85,46)};
        lblPoints.AutoSize=true;lblPoints.Font=new Font("Segoe UI Semibold",11F,FontStyle.Bold);lblPoints.ForeColor=Color.FromArgb(9,140,100);lblPoints.Anchor=AnchorStyles.Top|AnchorStyles.Right;lblPoints.Location=new Point(600,36);
        banner.Controls.AddRange(new Control[]{avatar,hello,helloSub,lblPoints});
        banner.Resize+=(s,e)=>{lblPoints.Left=banner.Width-lblPoints.Width-20;};
        var card=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{Dock=DockStyle.Top,Height=380,BackColor=Color.White,Radius=14,BorderColor=Color.FromArgb(218,232,240),Padding=new Padding(20),Margin=new Padding(0,0,0,12)};
        var table=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=4,BackColor=Color.Transparent};
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        for(int i=0;i<4;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,78F));
        Control MakeField(string l,Control c){
            var p=new Panel{Dock=DockStyle.Fill,Padding=new Padding(0,0,8,0),BackColor=Color.Transparent};
            var lb=new Label{Text=l,AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(2,2)};
            c.Location=new Point(2,22);c.Height=36;c.Dock=DockStyle.Bottom;
            p.Controls.AddRange(new Control[]{lb,c});
            return p;
        }
        dtBirth.Format=DateTimePickerFormat.Short;
        table.Controls.Add(MakeField("Mã khách hàng",txtCode),0,0);txtCode.ReadOnly=true;
        table.Controls.Add(MakeField("Email",txtEmail),1,0);
        table.Controls.Add(MakeField("Họ tên *",txtName),0,1);
        table.Controls.Add(MakeField("Ngày sinh",dtBirth),1,1);
        table.Controls.Add(MakeField("Số điện thoại",txtPhone),0,2);
        table.Controls.Add(MakeField("Địa chỉ",txtAddress),1,2);
        var pnlBtn=new Panel{Dock=DockStyle.Bottom,Height=56,BackColor=Color.Transparent,Padding=new Padding(0,8,0,0)};
        btnSave.Text="✓  Lưu thay đổi";btnSave.Size=new Size(140,40);btnSave.Location=new Point(0,8);
        btnChangePassword.Text="🔒  Đổi mật khẩu";btnChangePassword.Size=new Size(140,40);btnChangePassword.Location=new Point(150,8);
        pnlBtn.Controls.AddRange(new Control[]{btnSave,btnChangePassword});
        card.Controls.Add(table);card.Controls.Add(pnlBtn);
        card.Resize+=(s,e)=>{
            bool narrow=card.Width<600;
            if(narrow && table.ColumnCount==2){
                table.ColumnCount=1;table.RowCount=6;
                table.ColumnStyles.Clear();table.RowStyles.Clear();
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F));
                for(int i=0;i<6;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,78F));
                var cs=table.Controls.Cast<Control>().ToList();table.Controls.Clear();
                int r=0;foreach(var c in cs){table.Controls.Add(c,0,r++);}
            }else if(!narrow && table.ColumnCount==1){
                table.ColumnCount=2;table.RowCount=3;
                table.ColumnStyles.Clear();table.RowStyles.Clear();
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
                for(int i=0;i<4;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,78F));
                // rebuild order
                table.Controls.Clear();
                table.Controls.Add(MakeField("Mã khách hàng",txtCode),0,0);
                table.Controls.Add(MakeField("Email",txtEmail),1,0);
                table.Controls.Add(MakeField("Họ tên *",txtName),0,1);
                table.Controls.Add(MakeField("Ngày sinh",dtBirth),1,1);
                table.Controls.Add(MakeField("Số điện thoại",txtPhone),0,2);
                table.Controls.Add(MakeField("Địa chỉ",txtAddress),1,2);
            }
        };
        Controls.Add(card);Controls.Add(banner);Controls.Add(header);ResumeLayout(false);
    }
}
