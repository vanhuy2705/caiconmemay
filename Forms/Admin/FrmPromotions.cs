using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmPromotions:Form
{
    private int? _id;
    public FrmPromotions(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnNew);
        AppTheme.StylePrimary(btnSave);
        AppTheme.StyleDanger(btnDelete);
        grid.SelectionChanged+=(_,__)=>Bind();
        btnNew.Click+=(_,__)=>Clear();
        btnSave.Click+=(_,__)=>Save();
        btnDelete.Click+=(_,__)=>Delete();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query("SELECT KhuyenMaiID,MaKhuyenMai [Mã KM],TenKhuyenMai [Tên KM],MoTa [Mô tả],PhanTramGiam [Phần trăm],NgayBatDau [Bắt đầu],NgayKetThuc [Kết thúc],DangHoatDong [Hoạt động] FROM KhuyenMai ORDER BY KhuyenMaiID DESC");
            if(grid.Columns.Contains("KhuyenMaiID")) grid.Columns["KhuyenMaiID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải khuyến mãi");}
    }
    private void Bind(){
        if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv) return;
        var r=rv.Row;
        _id= r["KhuyenMaiID"]==DBNull.Value ? null : Convert.ToInt32(r["KhuyenMaiID"]);
        txtCode.Text=Convert.ToString(r["Mã KM"]) ?? "";
        txtName.Text=Convert.ToString(r["Tên KM"]) ?? "";
        txtDesc.Text=Convert.ToString(r["Mô tả"]) ?? "";
        txtPercent.Text= r["Phần trăm"]==DBNull.Value ? "" : Convert.ToDecimal(r["Phần trăm"]).ToString("0.##");
        if(r["Bắt đầu"]!=DBNull.Value) dtStart.Value=Convert.ToDateTime(r["Bắt đầu"]);
        if(r["Kết thúc"]!=DBNull.Value) dtEnd.Value=Convert.ToDateTime(r["Kết thúc"]);
        chkActive.Checked= r["Hoạt động"]!=DBNull.Value && Convert.ToBoolean(r["Hoạt động"]);
    }
    private void Clear(){_id=null;txtCode.Text="KM"+DateTime.Now.ToString("HHmmss");txtName.Clear();txtDesc.Clear();txtPercent.Text="10";dtStart.Value=DateTime.Today;dtEnd.Value=DateTime.Today.AddMonths(1);chkActive.Checked=true;}
    private void Save(){
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Nhập mã và tên khuyến mãi.");return;}
        if(!decimal.TryParse(txtPercent.Text.Replace(",",".").Trim(), out var pct) || pct<=0 || pct>100){UiMsg.Warn("Phần trăm phải (0..100].");return;}
        if(dtEnd.Value<=dtStart.Value){UiMsg.Warn("Ngày kết thúc phải sau bắt đầu.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM KhuyenMai WHERE MaKhuyenMai=@c AND (@id IS NULL OR KhuyenMaiID<>@id)) THEN N'Mã khuyến mãi đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()),
                new SqlParameter("@id",(object?)_id ?? DBNull.Value));
            var msg=dup.Rows.Count>0 ? (Convert.ToString(dup.Rows[0][0]) ?? "") : "";
            if(msg.Length>0){UiMsg.Warn(msg);return;}
            var pars=new[]{new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)? (object)DBNull.Value : txtDesc.Text.Trim()),new SqlParameter("@p",pct),new SqlParameter("@s",dtStart.Value.Date),new SqlParameter("@e",dtEnd.Value.Date.AddDays(1).AddSeconds(-1)),new SqlParameter("@a",chkActive.Checked)};
            if(_id==null) Db.Execute("INSERT INTO KhuyenMai(MaKhuyenMai,TenKhuyenMai,MoTa,PhanTramGiam,NgayBatDau,NgayKetThuc,DangHoatDong) VALUES(@c,@n,@d,@p,@s,@e,@a)",pars);
            else{var l=pars.ToList();l.Add(new SqlParameter("@id",_id.Value));Db.Execute("UPDATE KhuyenMai SET MaKhuyenMai=@c,TenKhuyenMai=@n,MoTa=@d,PhanTramGiam=@p,NgayBatDau=@s,NgayKetThuc=@e,DangHoatDong=@a WHERE KhuyenMaiID=@id",l.ToArray());}
            LoadData();Toast.Success("Đã lưu khuyến mãi.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu khuyến mãi");}
    }
    private void Delete(){
        if(_id==null){UiMsg.Warn("Chọn khuyến mãi cần xóa.");return;}
        if(UiMsg.Ask("Xóa khuyến mãi?","Xác nhận",danger:true)!=DialogResult.Yes)return;
        try{Db.Execute("DELETE FROM KhuyenMai WHERE KhuyenMaiID=@id",new SqlParameter("@id",_id.Value));Clear();LoadData();Toast.Success("Đã xóa khuyến mãi.");}
        catch(Exception ex){UiMsg.Error(ex.Message);}
    }
}
