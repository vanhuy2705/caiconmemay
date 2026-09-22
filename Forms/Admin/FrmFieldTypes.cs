using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmFieldTypes:Form
{
    private int? _id;
    private readonly bool _readOnly;
    public FrmFieldTypes(bool readOnly=false){
        _readOnly=readOnly;
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StyleSecondary(btnNew);
        AppTheme.StylePrimary(btnSave);
        AppTheme.StyleDanger(btnDelete);
        if(_readOnly){btnNew.Enabled=false;btnSave.Enabled=false;btnDelete.Enabled=false;txtCode.ReadOnly=true;txtName.ReadOnly=true;txtDesc.ReadOnly=true;chkActive.Enabled=false;}
        grid.SelectionChanged+=(_,__)=>Bind();
        txtSearch.TextChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnNew.Click+=(_,__)=>Clear();
        btnSave.Click+=(_,__)=>Save();
        btnDelete.Click+=(_,__)=>Delete();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query("SELECT LoaiSanID,MaLoaiSan [Mã loại],TenLoaiSan [Tên loại],MoTa [Mô tả],DangHoatDong [Hoạt động] FROM LoaiSan WHERE @s='' OR MaLoaiSan LIKE '%'+@s+'%' OR TenLoaiSan LIKE N'%'+@s+'%' ORDER BY LoaiSanID", new SqlParameter("@s",txtSearch.Text.Trim()));
            if(grid.Columns.Contains("LoaiSanID")) grid.Columns["LoaiSanID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải loại sân");}
    }
    private void Bind(){
        if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv) return;
        var r=rv.Row;
        _id= r["LoaiSanID"]==DBNull.Value ? null : Convert.ToInt32(r["LoaiSanID"]);
        txtCode.Text=Convert.ToString(r["Mã loại"]) ?? "";
        txtName.Text=Convert.ToString(r["Tên loại"]) ?? "";
        txtDesc.Text=Convert.ToString(r["Mô tả"]) ?? "";
        chkActive.Checked= r["Hoạt động"]!=DBNull.Value && Convert.ToBoolean(r["Hoạt động"]);
    }
    private void Clear(){_id=null;txtCode.Text="LT"+DateTime.Now.ToString("HHmmss");txtName.Clear();txtDesc.Clear();chkActive.Checked=true;txtName.Focus();}
    private void Save(){
        if(_readOnly){UiMsg.Warn("Chế độ chỉ xem.");return;}
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Nhập mã loại và tên loại.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM LoaiSan WHERE MaLoaiSan=@c AND (@id IS NULL OR LoaiSanID<>@id)) THEN N'Mã loại sân đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()),
                new SqlParameter("@id",(object?)_id ?? DBNull.Value));
            var msg=dup.Rows.Count>0 ? (Convert.ToString(dup.Rows[0][0]) ?? "") : "";
            if(msg.Length>0){UiMsg.Warn(msg);return;}
            var pars=new[]{new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)? (object)DBNull.Value : txtDesc.Text.Trim()),new SqlParameter("@a",chkActive.Checked)};
            if(_id==null) Db.Execute("INSERT INTO LoaiSan(MaLoaiSan,TenLoaiSan,MoTa,DangHoatDong) VALUES(@c,@n,@d,@a)",pars);
            else{var l=pars.ToList();l.Add(new SqlParameter("@id",_id.Value));Db.Execute("UPDATE LoaiSan SET MaLoaiSan=@c,TenLoaiSan=@n,MoTa=@d,DangHoatDong=@a WHERE LoaiSanID=@id",l.ToArray());}
            LoadData();Toast.Success("Đã lưu loại sân.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu loại sân");}
    }
    private void Delete(){
        if(_readOnly){UiMsg.Warn("Chế độ chỉ xem.");return;}
        if(_id==null){UiMsg.Warn("Chọn loại sân cần xóa.");return;}
        if(UiMsg.Ask("Xóa loại sân?","Xác nhận",danger:true)!=DialogResult.Yes)return;
        try{Db.Execute("DELETE FROM LoaiSan WHERE LoaiSanID=@id",new SqlParameter("@id",_id.Value));Clear();LoadData();Toast.Success("Đã xóa loại sân.");}
        catch(Exception ex){UiMsg.Error("Không thể xóa loại đã có sân. Bỏ hoạt động thay vì xóa.\n"+ex.Message);}
    }
}
