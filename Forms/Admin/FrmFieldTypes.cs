using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmFieldTypes:Form
{
    private int? _id;private readonly bool _readOnly;
    public FrmFieldTypes(bool readOnly=false){_readOnly=readOnly;InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnRefresh);AppTheme.StyleSecondary(btnNew);AppTheme.StylePrimary(btnSave);AppTheme.StyleDanger(btnDelete);btnSave.Visible=btnDelete.Visible=btnNew.Visible=!readOnly;grid.SelectionChanged+=(_,__)=>Bind();btnRefresh.Click+=(_,__)=>LoadData();txtSearch.TextChanged+=(_,__)=>LoadData();btnNew.Click+=(_,__)=>ClearForm();btnSave.Click+=(_,__)=>Save();btnDelete.Click+=(_,__)=>Delete();Shown+=(_,__)=>LoadData();}
    private void LoadData(){try{var q="SELECT LoaiSanID,MaLoaiSan AS [Mã loại],TenLoaiSan AS [Tên loại sân],MoTa AS [Mô tả],DangHoatDong AS [Hoạt động] FROM LoaiSan WHERE @s='' OR MaLoaiSan LIKE '%'+@s+'%' OR TenLoaiSan LIKE N'%'+@s+'%' ORDER BY TenLoaiSan";grid.DataSource=Db.Query(q,new SqlParameter("@s",txtSearch.Text.Trim()));if(grid.Columns.Contains("LoaiSanID"))grid.Columns["LoaiSanID"].Visible=false;}catch(Exception ex){UiMsg.Error(ex.Message,"Tải loại sân");}}
    private void Bind(){if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv)return;_id=Convert.ToInt32(rv.Row["LoaiSanID"]);txtCode.Text=Convert.ToString(rv.Row["Mã loại"]);txtName.Text=Convert.ToString(rv.Row["Tên loại sân"]);txtDesc.Text=Convert.ToString(rv.Row["Mô tả"]);chkActive.Checked=Convert.ToBoolean(rv.Row["Hoạt động"]);}
    private void ClearForm(){_id=null;txtCode.Clear();txtName.Clear();txtDesc.Clear();chkActive.Checked=true;txtCode.Focus();}
    private void Save(){
        if(_readOnly)return;
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Nhập mã và tên loại sân.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM LoaiSan WHERE MaLoaiSan=@c AND (@id IS NULL OR LoaiSanID<>@id)) THEN N'Mã loại sân đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()), new SqlParameter("@id",(object?)_id??DBNull.Value));
            var msg=Convert.ToString(dup.Rows[0][0])??"";
            if(msg.Length>0){UiMsg.Warn(msg);return;}
            if(_id==null)Db.Execute("INSERT INTO LoaiSan(MaLoaiSan,TenLoaiSan,MoTa,DangHoatDong) VALUES(@c,@n,@d,@a)",new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)?DBNull.Value:txtDesc.Text.Trim()),new SqlParameter("@a",chkActive.Checked));
            else Db.Execute("UPDATE LoaiSan SET MaLoaiSan=@c,TenLoaiSan=@n,MoTa=@d,DangHoatDong=@a WHERE LoaiSanID=@id",new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)?DBNull.Value:txtDesc.Text.Trim()),new SqlParameter("@a",chkActive.Checked),new SqlParameter("@id",_id));
            LoadData();Toast.Success("Đã lưu loại sân.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu loại sân");}
    }
    private void Delete(){if(_id==null||UiMsg.Ask("Xóa loại sân này?","Xác nhận",danger:true)!=DialogResult.Yes)return;try{Db.Execute("DELETE FROM LoaiSan WHERE LoaiSanID=@id",new SqlParameter("@id",_id));ClearForm();LoadData();Toast.Success("Đã xóa loại sân.");}catch(Exception ex){UiMsg.Error("Không thể xóa loại sân đang được sử dụng.\n"+ex.Message);}}
}
