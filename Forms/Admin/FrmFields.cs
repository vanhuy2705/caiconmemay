using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmFields:Form
{
    private int? _id;
    private readonly bool _readOnly;
    public FrmFields(bool readOnly=false){
        _readOnly=readOnly;
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StyleSecondary(btnNew);
        AppTheme.StylePrimary(btnSave);
        AppTheme.StyleDanger(btnDelete);
        if(_readOnly){
            btnNew.Enabled=false;btnSave.Enabled=false;btnDelete.Enabled=false;
            txtCode.ReadOnly=true;txtName.ReadOnly=true;txtLocation.ReadOnly=true;txtPrice.ReadOnly=true;txtDesc.ReadOnly=true;
            cboType.Enabled=false;cboStatus.Enabled=false;chkActive.Enabled=false;
        }
        LoadTypes();
        grid.SelectionChanged+=(_,__)=>Bind();
        txtSearch.TextChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>{LoadTypes();LoadData();};
        btnNew.Click+=(_,__)=>Clear();
        btnSave.Click+=(_,__)=>Save();
        btnDelete.Click+=(_,__)=>Delete();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadTypes(){
        try{
            var dt=Db.Query("SELECT LoaiSanID,TenLoaiSan FROM LoaiSan WHERE DangHoatDong=1 ORDER BY TenLoaiSan");
            cboType.DataSource=dt;
            cboType.DisplayMember="TenLoaiSan";
            cboType.ValueMember="LoaiSanID";
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải loại sân");}
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query(@"SELECT s.SanID,s.MaSan [Mã sân],s.TenSan [Tên sân],lt.TenLoaiSan [Loại sân],s.ViTri [Vị trí],s.GiaMoiGio [Giá/giờ],s.TrangThai [Trạng thái],s.DangHoatDong [Hoạt động] FROM SanTheThao s JOIN LoaiSan lt ON lt.LoaiSanID=s.LoaiSanID WHERE @s='' OR s.MaSan LIKE '%'+@s+'%' OR s.TenSan LIKE N'%'+@s+'%' OR lt.TenLoaiSan LIKE N'%'+@s+'%' ORDER BY s.SanID",new SqlParameter("@s",txtSearch.Text.Trim()));
            if(grid.Columns.Contains("SanID")) grid.Columns["SanID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải sân");}
    }
    private void Bind(){
        if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv) return;
        var r=rv.Row;
        _id= r["SanID"]==DBNull.Value ? null : Convert.ToInt32(r["SanID"]);
        txtCode.Text=Convert.ToString(r["Mã sân"]) ?? "";
        txtName.Text=Convert.ToString(r["Tên sân"]) ?? "";
        txtLocation.Text=Convert.ToString(r["Vị trí"]) ?? "";
        txtPrice.Text= r["Giá/giờ"]==DBNull.Value ? "" : Convert.ToDecimal(r["Giá/giờ"]).ToString("N0");
        var loaiTen=Convert.ToString(r["Loại sân"]) ?? "";
        if(cboType.DataSource is System.Data.DataTable dt){
            foreach(System.Data.DataRow dr in dt.Rows){ if((Convert.ToString(dr["TenLoaiSan"]) ?? "")==loaiTen){cboType.SelectedValue=dr["LoaiSanID"];break;}}
        }
        var tt=Convert.ToString(r["Trạng thái"]) ?? "Available";
        cboStatus.SelectedItem= tt=="Maintenance" ? "Bảo trì" : "Sẵn sàng";
        if(cboStatus.SelectedIndex<0) cboStatus.SelectedIndex=0;
        chkActive.Checked= r["Hoạt động"]!=DBNull.Value && Convert.ToBoolean(r["Hoạt động"]);
        txtDesc.Text="";
    }
    private void Clear(){
        _id=null;
        txtCode.Text="S"+DateTime.Now.ToString("HHmmss");
        txtName.Clear();txtLocation.Clear();txtPrice.Text="100000";txtDesc.Clear();
        if(cboType.Items.Count>0) cboType.SelectedIndex=0;
        if(cboStatus.Items.Count>0) cboStatus.SelectedIndex=0;
        chkActive.Checked=true;
    }
    private bool TryParsePrice(out decimal price){
        var txt=(txtPrice.Text ?? "").Replace(",","").Replace(".","").Trim();
        // Allow N0 format
        if(decimal.TryParse(txtPrice.Text.Replace(",",""), out price) && price>0) return true;
        if(decimal.TryParse(txt, out price) && price>0) return true;
        price=0;return false;
    }
    private void Save(){
        if(_readOnly){UiMsg.Warn("Chế độ chỉ xem.");return;}
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Nhập mã sân và tên sân.");return;}
        if(cboType.SelectedValue==null || cboType.SelectedValue==DBNull.Value){UiMsg.Warn("Chọn loại sân.");return;}
        if(!TryParsePrice(out var priceVal)){UiMsg.Warn("Giá phải >0 (nhập số).");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM SanTheThao WHERE MaSan=@c AND (@id IS NULL OR SanID<>@id)) THEN N'Mã sân đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()),
                new SqlParameter("@id",(object?)_id ?? DBNull.Value));
            var msg=dup.Rows.Count>0 ? (Convert.ToString(dup.Rows[0][0]) ?? "") : "";
            if(msg.Length>0){UiMsg.Warn(msg);return;}

            var loaiId=Convert.ToInt32(cboType.SelectedValue);
            var trangThai=(Convert.ToString(cboStatus.SelectedItem) ?? "").Contains("Bảo trì") ? "Maintenance" : "Available";
            var pars=new List<SqlParameter>{
                new("@c",txtCode.Text.Trim()),
                new("@n",txtName.Text.Trim()),
                new("@l",loaiId),
                new("@vt",string.IsNullOrWhiteSpace(txtLocation.Text)? (object)DBNull.Value : txtLocation.Text.Trim()),
                new("@g",priceVal),
                new("@tt",trangThai),
                new("@a",chkActive.Checked),
                new("@d",string.IsNullOrWhiteSpace(txtDesc.Text)? (object)DBNull.Value : txtDesc.Text.Trim())
            };
            if(_id==null) Db.Execute("INSERT INTO SanTheThao(MaSan,TenSan,LoaiSanID,ViTri,GiaMoiGio,TrangThai,DangHoatDong,MoTa) VALUES(@c,@n,@l,@vt,@g,@tt,@a,@d)",pars.ToArray());
            else{pars.Add(new SqlParameter("@id",_id.Value));Db.Execute("UPDATE SanTheThao SET MaSan=@c,TenSan=@n,LoaiSanID=@l,ViTri=@vt,GiaMoiGio=@g,TrangThai=@tt,DangHoatDong=@a,MoTa=@d WHERE SanID=@id",pars.ToArray());}
            LoadData();Toast.Success("Đã lưu sân.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu sân");}
    }
    private void Delete(){
        if(_readOnly){UiMsg.Warn("Chế độ chỉ xem.");return;}
        if(_id==null){UiMsg.Warn("Chọn sân cần xóa.");return;}
        if(UiMsg.Ask("Xóa sân này?","Xác nhận",danger:true)!=DialogResult.Yes)return;
        try{Db.Execute("DELETE FROM SanTheThao WHERE SanID=@id",new SqlParameter("@id",_id.Value));Clear();LoadData();Toast.Success("Đã xóa sân.");}
        catch(Exception ex){UiMsg.Error("Không thể xóa sân đã có lịch đặt. Có thể chuyển Bảo trì và bỏ hoạt động.\n"+ex.Message);}
    }
}
