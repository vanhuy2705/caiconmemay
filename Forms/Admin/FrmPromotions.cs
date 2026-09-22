using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmPromotions:Form
{
    private int? _id;
    public FrmPromotions(){InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnNew);AppTheme.StylePrimary(btnSave);AppTheme.StyleDanger(btnDelete);grid.SelectionChanged+=(_,__)=>Bind();btnNew.Click+=(_,__)=>Clear();btnSave.Click+=(_,__)=>Save();btnDelete.Click+=(_,__)=>Delete();Shown+=(_,__)=>LoadData();}
    private void LoadData(){try{grid.DataSource=Db.Query("SELECT KhuyenMaiID,MaKhuyenMai [Mã],TenKhuyenMai [Tên],MoTa [Mô tả],PhanTramGiam [Giảm %],NgayBatDau [Bắt đầu],NgayKetThuc [Kết thúc],DangHoatDong [Hoạt động] FROM KhuyenMai ORDER BY KhuyenMaiID DESC");if(grid.Columns.Contains("KhuyenMaiID"))grid.Columns["KhuyenMaiID"].Visible=false;}catch(Exception ex){UiMsg.Error(ex.Message,"Tải khuyến mãi");}}
    private void Bind(){if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView v)return;var r=v.Row;_id=Convert.ToInt32(r["KhuyenMaiID"]);txtCode.Text=Convert.ToString(r["Mã"]);txtName.Text=Convert.ToString(r["Tên"]);txtDesc.Text=Convert.ToString(r["Mô tả"]);txtPercent.Text=Convert.ToDecimal(r["Giảm %"]).ToString("0.##");dtStart.Value=Convert.ToDateTime(r["Bắt đầu"]);dtEnd.Value=Convert.ToDateTime(r["Kết thúc"]);chkActive.Checked=Convert.ToBoolean(r["Hoạt động"]);}
    private void Clear(){_id=null;txtCode.Clear();txtName.Clear();txtDesc.Clear();txtPercent.Text="10";dtStart.Value=DateTime.Now;dtEnd.Value=DateTime.Now.AddMonths(1);chkActive.Checked=true;}
    private void Save(){
        if(!decimal.TryParse(txtPercent.Text,out var p)||p<=0||p>100||dtEnd.Value<=dtStart.Value||string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Mã/tên bắt buộc, % giảm phải trong (0..100] và ngày kết thúc phải sau ngày bắt đầu.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM KhuyenMai WHERE MaKhuyenMai=@c AND (@id IS NULL OR KhuyenMaiID<>@id)) THEN N'Mã khuyến mãi đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()), new SqlParameter("@id",(object?)_id??DBNull.Value));
            var msg=Convert.ToString(dup.Rows[0][0])??"";
            if(msg.Length>0){UiMsg.Warn(msg);return;}
            if(_id==null)Db.Execute("INSERT INTO KhuyenMai(MaKhuyenMai,TenKhuyenMai,MoTa,PhanTramGiam,NgayBatDau,NgayKetThuc,DangHoatDong) VALUES(@c,@n,@d,@p,@s,@e,@a)",new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)?DBNull.Value:txtDesc.Text.Trim()),new SqlParameter("@p",p),new SqlParameter("@s",dtStart.Value),new SqlParameter("@e",dtEnd.Value),new SqlParameter("@a",chkActive.Checked));
            else Db.Execute("UPDATE KhuyenMai SET MaKhuyenMai=@c,TenKhuyenMai=@n,MoTa=@d,PhanTramGiam=@p,NgayBatDau=@s,NgayKetThuc=@e,DangHoatDong=@a WHERE KhuyenMaiID=@id",new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)?DBNull.Value:txtDesc.Text.Trim()),new SqlParameter("@p",p),new SqlParameter("@s",dtStart.Value),new SqlParameter("@e",dtEnd.Value),new SqlParameter("@a",chkActive.Checked),new SqlParameter("@id",_id));
            LoadData();Toast.Success("Đã lưu khuyến mãi.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu khuyến mãi");}
    }
    private void Delete(){if(_id==null||UiMsg.Ask("Xóa khuyến mãi?","Xác nhận",danger:true)!=DialogResult.Yes)return;try{Db.Execute("DELETE FROM KhuyenMai WHERE KhuyenMaiID=@id",new SqlParameter("@id",_id));Clear();LoadData();Toast.Success("Đã xóa khuyến mãi.");}catch(Exception ex){UiMsg.Error("Khuyến mãi đã được áp dụng cho lịch đặt nên không thể xóa. Hãy tắt trạng thái hoạt động.\n"+ex.Message);}}
}
