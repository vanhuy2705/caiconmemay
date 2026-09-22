using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmFields:Form
{
    private static string StatusToCode(string s)=>s switch{"Đang thuê"=>"Busy","Bảo trì"=>"Maintenance","Ngừng hoạt động"=>"Inactive",_=>"Available"};
    private static string CodeToStatus(string s)=>s switch{"Available"=>"Trống","Busy"=>"Đang thuê","Maintenance"=>"Bảo trì","Inactive"=>"Ngừng hoạt động",_=>s};
    private int? _id;private readonly bool _readOnly;
    public FrmFields(bool readOnly=false){_readOnly=readOnly;InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnRefresh);AppTheme.StyleSecondary(btnNew);AppTheme.StylePrimary(btnSave);AppTheme.StyleDanger(btnDelete);btnNew.Visible=btnSave.Visible=btnDelete.Visible=!readOnly;cboType.DropDownStyle=cboStatus.DropDownStyle=ComboBoxStyle.DropDownList;cboStatus.Items.AddRange(new object[]{"Trống","Đang thuê","Bảo trì","Ngừng hoạt động"});grid.SelectionChanged+=(_,__)=>Bind();txtSearch.TextChanged+=(_,__)=>LoadData();btnRefresh.Click+=(_,__)=>LoadData();btnNew.Click+=(_,__)=>ClearForm();btnSave.Click+=(_,__)=>Save();btnDelete.Click+=(_,__)=>Delete();Shown+=(_,__)=>{LoadTypes();LoadData();};}
    private void LoadTypes(){try{var dt=Db.Query("SELECT LoaiSanID,TenLoaiSan FROM LoaiSan WHERE DangHoatDong=1 ORDER BY TenLoaiSan");cboType.DataSource=dt;cboType.ValueMember="LoaiSanID";cboType.DisplayMember="TenLoaiSan";}catch(Exception ex){UiMsg.Error(ex.Message,"Loại sân");}}
    private void LoadData(){try{grid.DataSource=Db.Query(@"SELECT f.SanID,f.MaSan [Mã sân],f.TenSan [Tên sân],ft.TenLoaiSan [Loại sân],f.LoaiSanID,f.ViTri [Vị trí],f.GiaMoiGio [Giá/giờ],f.TrangThai [Trạng thái],f.MoTa [Mô tả],f.DangHoatDong [Hoạt động] FROM SanTheThao f JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID WHERE @s='' OR f.MaSan LIKE '%'+@s+'%' OR f.TenSan LIKE N'%'+@s+'%' OR ft.TenLoaiSan LIKE N'%'+@s+'%' ORDER BY ft.TenLoaiSan,f.TenSan",new SqlParameter("@s",txtSearch.Text.Trim()));foreach(var c in new[]{"SanID","LoaiSanID"})if(grid.Columns.Contains(c))grid.Columns[c].Visible=false;}catch(Exception ex){UiMsg.Error(ex.Message,"Tải sân");}}
    private void Bind(){if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv)return;var r=rv.Row;_id=Convert.ToInt32(r["SanID"]);txtCode.Text=Convert.ToString(r["Mã sân"]);txtName.Text=Convert.ToString(r["Tên sân"]);cboType.SelectedValue=r["LoaiSanID"];txtLocation.Text=Convert.ToString(r["Vị trí"]);txtPrice.Text=Convert.ToDecimal(r["Giá/giờ"]).ToString("0");cboStatus.SelectedItem=CodeToStatus(Convert.ToString(r["Trạng thái"])??"");txtDesc.Text=Convert.ToString(r["Mô tả"]);chkActive.Checked=Convert.ToBoolean(r["Hoạt động"]);}
    private void ClearForm(){_id=null;txtCode.Clear();txtName.Clear();txtLocation.Clear();txtPrice.Clear();txtDesc.Clear();if(cboType.Items.Count>0)cboType.SelectedIndex=0;cboStatus.SelectedIndex=0;chkActive.Checked=true;txtCode.Focus();}
    private void Save(){
        if(_readOnly)return;
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)||!decimal.TryParse(txtPrice.Text,out var price)||price<=0){UiMsg.Warn("Nhập đầy đủ mã sân, tên sân và giá lớn hơn 0.");return;}
        if(cboType.SelectedValue==null){UiMsg.Warn("Chọn loại sân.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM SanTheThao WHERE MaSan=@c AND (@id IS NULL OR SanID<>@id)) THEN N'Mã sân đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()), new SqlParameter("@id",(object?)_id??DBNull.Value));
            var msg=Convert.ToString(dup.Rows[0][0])??"";
            if(msg.Length>0){UiMsg.Warn(msg);return;}

            var pars=new[]{new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@t",Convert.ToInt32(cboType.SelectedValue)),new SqlParameter("@l",string.IsNullOrWhiteSpace(txtLocation.Text)?DBNull.Value:txtLocation.Text.Trim()),new SqlParameter("@p",price),new SqlParameter("@s",StatusToCode(cboStatus.Text)),new SqlParameter("@d",string.IsNullOrWhiteSpace(txtDesc.Text)?DBNull.Value:txtDesc.Text.Trim()),new SqlParameter("@a",chkActive.Checked)};
            if(_id==null)Db.Execute("INSERT INTO SanTheThao(MaSan,TenSan,LoaiSanID,ViTri,GiaMoiGio,TrangThai,MoTa,DangHoatDong) VALUES(@c,@n,@t,@l,@p,@s,@d,@a)",pars);
            else{var list=pars.ToList();list.Add(new SqlParameter("@id",_id));Db.Execute("UPDATE SanTheThao SET MaSan=@c,TenSan=@n,LoaiSanID=@t,ViTri=@l,GiaMoiGio=@p,TrangThai=@s,MoTa=@d,DangHoatDong=@a WHERE SanID=@id",list.ToArray());}
            LoadData();Toast.Success("Đã lưu thông tin sân.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu sân");}
    }
    private void Delete(){if(_readOnly||_id==null||UiMsg.Ask("Xóa sân này?","Xác nhận",danger:true)!=DialogResult.Yes)return;try{Db.Execute("DELETE FROM SanTheThao WHERE SanID=@id",new SqlParameter("@id",_id));ClearForm();LoadData();Toast.Success("Đã xóa sân.");}catch(Exception ex){UiMsg.Error("Không thể xóa sân đã phát sinh lịch đặt. Có thể chuyển sang Ngừng hoạt động.\n"+ex.Message);}}
}
