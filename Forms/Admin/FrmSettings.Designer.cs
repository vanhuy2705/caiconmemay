#nullable enable
namespace QuanLyThueSanTheThao.Forms.Admin;
partial class FrmSettings
{
    private System.ComponentModel.IContainer? components=null;private TextBox txtCompany=null!,txtBankName=null!,txtBankBin=null!,txtAccount=null!,txtAccountName=null!,txtPrefix=null!,txtOpen=null!,txtClose=null!;private Button btnSave=null!;
    protected override void Dispose(bool disposing){if(disposing&&components!=null)components.Dispose();base.Dispose(disposing);}
    private void InitializeComponent(){
        txtCompany=new TextBox();txtBankName=new TextBox();txtBankBin=new TextBox();txtAccount=new TextBox();txtAccountName=new TextBox();txtPrefix=new TextBox();txtOpen=new TextBox();txtClose=new TextBox();btnSave=new Button();
        SuspendLayout();
        BackColor=Color.FromArgb(243,249,248);AutoScroll=true;Padding=new Padding(12);AutoScaleMode=AutoScaleMode.Dpi;
        var header=new Panel{Dock=DockStyle.Top,Height=64,BackColor=Color.Transparent};
        var pageTitle=new Label{Text="Cấu hình hệ thống",AutoSize=true,Font=new Font("Segoe UI Semibold",15F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(6,6)};
        var pageSub=new Label{Text="Thông tin đơn vị, tài khoản nhận tiền VietQR và giờ hoạt động",AutoSize=true,Font=new Font("Segoe UI",8.2F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(8,34)};
        header.Controls.AddRange(new Control[]{pageTitle,pageSub});

        var card=new QuanLyThueSanTheThao.Forms.Common.RoundedPanel{Dock=DockStyle.Top,Height=720,BackColor=Color.White,Radius=16,BorderColor=Color.FromArgb(218,232,240),Padding=new Padding(24),Margin=new Padding(0,8,0,12)};
        var badge=new Label{Text="⚙",AutoSize=false,Size=new Size(40,40),Font=new Font("Segoe UI Symbol",13F),ForeColor=Color.White,BackColor=Color.FromArgb(19,170,157),TextAlign=ContentAlignment.MiddleCenter,Location=new Point(24,20)};
        badge.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,39,39),12));
        var title=new Label{Text="Thiết lập chung & thanh toán",AutoSize=true,Font=new Font("Segoe UI Semibold",13F,FontStyle.Bold),ForeColor=Color.FromArgb(18,53,78),Location=new Point(76,22)};
        var sub=new Label{Text="Các thiết lập áp dụng cho toàn hệ thống và mã QR",AutoSize=true,Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(103,126,145),Location=new Point(77,48)};
        var sep=new Panel{BackColor=Color.FromArgb(238,244,248),Location=new Point(24,80),Height=1,Width=700,Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        card.Controls.AddRange(new Control[]{badge,title,sub,sep});

        // Dùng TableLayout 2 cột cho responsive
        var table=new TableLayoutPanel{Location=new Point(24,100),Size=new Size(700,540),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right,ColumnCount=2,RowCount=6,BackColor=Color.Transparent};
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
        for(int i=0;i<6;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,88F));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute,80F));

        Control MakeField(string lbl, TextBox tb){
            var p=new Panel{Dock=DockStyle.Fill,Padding=new Padding(0,0,8,0),BackColor=Color.Transparent};
            var l=new Label{Text=lbl,AutoSize=true,Font=new Font("Segoe UI Semibold",8.5F),ForeColor=Color.FromArgb(52,84,105),Location=new Point(2,2)};
            tb.Location=new Point(2,22);tb.Height=36;tb.Dock=DockStyle.Bottom;tb.Anchor=AnchorStyles.Left|AnchorStyles.Right|AnchorStyles.Bottom;
            p.Controls.AddRange(new Control[]{l,tb});
            return p;
        }

        table.Controls.Add(MakeField("Tên hệ thống / công ty",txtCompany),0,0);
        table.Controls.Add(MakeField("Tên ngân hàng",txtBankName),0,1);
        table.Controls.Add(MakeField("Bank BIN (VietQR)",txtBankBin),0,2);
        table.Controls.Add(MakeField("Số tài khoản",txtAccount),0,3);
        table.Controls.Add(MakeField("Chủ tài khoản",txtAccountName),1,0);
        table.Controls.Add(MakeField("Tiền tố nội dung CK",txtPrefix),1,1);
        table.Controls.Add(MakeField("Giờ mở cửa (HH:mm)",txtOpen),0,4);
        table.Controls.Add(MakeField("Giờ đóng cửa (HH:mm)",txtClose),1,4);

        var banner=new Panel{Location=new Point(24,600),Size=new Size(700,56),BackColor=Color.FromArgb(236,249,243),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right};
        banner.Region=new Region(QuanLyThueSanTheThao.Forms.Common.RoundedPanel.CreateRoundPath(new Rectangle(0,0,699,55),10));
        var note=new Label{Text="ⓘ  Mã QR tạo động theo số tiền còn phải trả. Giao dịch CK ở trạng thái chờ cho đến khi nhân viên xác nhận.",AutoSize=false,Size=new Size(660,44),Font=new Font("Segoe UI",8F),ForeColor=Color.FromArgb(16,120,84),Location=new Point(12,6)};
        banner.Controls.Add(note);

        btnSave.Text="✓  Lưu cấu hình";btnSave.Location=new Point(24,670);btnSave.Size=new Size(160,44);

        card.Controls.AddRange(new Control[]{table,banner,btnSave});
        card.Resize+=(s,e)=>{
            sep.Width=card.Width-48;
            table.Width=card.Width-48;
            banner.Width=card.Width-48;
            note.Width=banner.Width-24;
            bool narrow=card.Width<700;
            if(narrow && table.ColumnCount==2){
                table.ColumnCount=1;table.RowCount=9;
                table.ColumnStyles.Clear();table.RowStyles.Clear();
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F));
                for(int i=0;i<9;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,88F));
                // Reorder controls to single column
                var controls=table.Controls.Cast<Control>().ToList();
                table.Controls.Clear();
                int r=0;
                foreach(var c in controls){table.Controls.Add(c,0,r++);}
            }else if(!narrow && table.ColumnCount==1){
                table.ColumnCount=2;table.RowCount=5;
                table.ColumnStyles.Clear();table.RowStyles.Clear();
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50F));
                for(int i=0;i<6;i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute,88F));
                // Rebuild 2 columns order original
                table.Controls.Clear();
                table.Controls.Add(MakeField("Tên hệ thống / công ty",txtCompany),0,0);
                table.Controls.Add(MakeField("Tên ngân hàng",txtBankName),0,1);
                table.Controls.Add(MakeField("Bank BIN (VietQR)",txtBankBin),0,2);
                table.Controls.Add(MakeField("Số tài khoản",txtAccount),0,3);
                table.Controls.Add(MakeField("Chủ tài khoản",txtAccountName),1,0);
                table.Controls.Add(MakeField("Tiền tố nội dung CK",txtPrefix),1,1);
                table.Controls.Add(MakeField("Giờ mở cửa (HH:mm)",txtOpen),0,4);
                table.Controls.Add(MakeField("Giờ đóng cửa (HH:mm)",txtClose),1,4);
            }
        };

        Controls.Add(card);Controls.Add(header);ResumeLayout(false);
    }
}
