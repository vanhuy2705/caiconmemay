using Microsoft.Data.SqlClient;using QuanLyThueSanTheThao.Data;using QuanLyThueSanTheThao.Helpers;
namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmVouchers:Form
{
    private int? _id;
    public FrmVouchers(){InitializeComponent();AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);try{var sp=this.Controls.OfType<System.Windows.Forms.SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}AppTheme.StyleGrid(grid);AppTheme.StyleSecondary(btnRefresh);AppTheme.StyleSecondary(btnNew);AppTheme.StylePrimary(btnSave);AppTheme.StyleDanger(btnDelete);AppTheme.StyleSecondary(btnAssign);cboType.Items.AddRange(new object[]{"Percent","Fixed"});cboType.DropDownStyle=ComboBoxStyle.DropDownList;cboType.SelectedIndex=0;grid.SelectionChanged+=(_,__)=>Bind();txtSearch.TextChanged+=(_,__)=>LoadData();btnRefresh.Click+=(_,__)=>LoadData();btnNew.Click+=(_,__)=>Clear();btnSave.Click+=(_,__)=>Save();btnDelete.Click+=(_,__)=>Delete();btnAssign.Click+=(_,__)=>Assign();Shown+=(_,__)=>LoadData();}
    private void LoadData(){try{grid.DataSource=Db.Query(@"SELECT PhieuGiamGiaID,MaPhieuGiamGia [Mã],TenPhieuGiamGia [Tên],LoaiGiamGia [Loại],GiaTriGiam [Giá trị],MucGiamToiDa [Tối đa],DonToiThieu [Đơn tối thiểu],NgayBatDau [Bắt đầu],NgayKetThuc [Kết thúc],SoLuong [SL],DangHoatDong [Hoạt động] FROM PhieuGiamGia WHERE @s='' OR MaPhieuGiamGia LIKE '%'+@s+'%' OR TenPhieuGiamGia LIKE N'%'+@s+'%' ORDER BY PhieuGiamGiaID DESC",new SqlParameter("@s",txtSearch.Text.Trim()));if(grid.Columns.Contains("PhieuGiamGiaID"))grid.Columns["PhieuGiamGiaID"].Visible=false;}catch(Exception ex){UiMsg.Error(ex.Message,"Tải voucher");}}
    private void Bind(){if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv)return;var r=rv.Row;_id=Convert.ToInt32(r["PhieuGiamGiaID"]);txtCode.Text=Convert.ToString(r["Mã"]);txtName.Text=Convert.ToString(r["Tên"]);cboType.SelectedItem=Convert.ToString(r["Loại"]);txtValue.Text=Convert.ToDecimal(r["Giá trị"]).ToString("0.##");txtMax.Text=r["Tối đa"]==DBNull.Value?"":Convert.ToDecimal(r["Tối đa"]).ToString("0");txtMin.Text=Convert.ToDecimal(r["Đơn tối thiểu"]).ToString("0");txtQty.Text=r["SL"]==DBNull.Value?"":Convert.ToString(r["SL"]);dtStart.Value=Convert.ToDateTime(r["Bắt đầu"]);dtEnd.Value=Convert.ToDateTime(r["Kết thúc"]);chkActive.Checked=Convert.ToBoolean(r["Hoạt động"]);}
    private void Clear(){_id=null;txtCode.Clear();txtName.Clear();txtValue.Text="10";txtMax.Clear();txtMin.Text="0";txtQty.Text="100";dtStart.Value=DateTime.Now;dtEnd.Value=DateTime.Now.AddMonths(1);chkActive.Checked=true;}
    private void Save(){
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)||!decimal.TryParse(txtValue.Text,out var val)||!decimal.TryParse(txtMin.Text,out var min)||val<=0||min<0||dtEnd.Value<=dtStart.Value||(cboType.Text=="Percent"&&val>100)){UiMsg.Warn("Voucher chưa hợp lệ: giá trị > 0, % không quá 100, đơn tối thiểu >= 0 và ngày kết thúc phải sau ngày bắt đầu.");return;}
        decimal? max=decimal.TryParse(txtMax.Text,out var mx)?mx:null;int? qty=int.TryParse(txtQty.Text,out var q)?q:null;
        if((max.HasValue&&max.Value<0)||(qty.HasValue&&qty.Value<0)){UiMsg.Warn("Mức giảm tối đa và số lượng không được âm.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM PhieuGiamGia WHERE MaPhieuGiamGia=@c AND (@id IS NULL OR PhieuGiamGiaID<>@id)) THEN N'Mã voucher đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()), new SqlParameter("@id",(object?)_id??DBNull.Value));
            var msg=Convert.ToString(dup.Rows[0][0])??"";
            if(msg.Length>0){UiMsg.Warn(msg);return;}

            var p=new[]{new SqlParameter("@c",txtCode.Text.Trim()),new SqlParameter("@n",txtName.Text.Trim()),new SqlParameter("@t",cboType.Text),new SqlParameter("@v",val),new SqlParameter("@m",(object?)max??DBNull.Value),new SqlParameter("@min",min),new SqlParameter("@s",dtStart.Value),new SqlParameter("@e",dtEnd.Value),new SqlParameter("@q",(object?)qty??DBNull.Value),new SqlParameter("@a",chkActive.Checked)};
            if(_id==null)Db.Execute("INSERT INTO PhieuGiamGia(MaPhieuGiamGia,TenPhieuGiamGia,LoaiGiamGia,GiaTriGiam,MucGiamToiDa,DonToiThieu,NgayBatDau,NgayKetThuc,SoLuong,DangHoatDong) VALUES(@c,@n,@t,@v,@m,@min,@s,@e,@q,@a)",p);
            else{var l=p.ToList();l.Add(new SqlParameter("@id",_id));Db.Execute("UPDATE PhieuGiamGia SET MaPhieuGiamGia=@c,TenPhieuGiamGia=@n,LoaiGiamGia=@t,GiaTriGiam=@v,MucGiamToiDa=@m,DonToiThieu=@min,NgayBatDau=@s,NgayKetThuc=@e,SoLuong=@q,DangHoatDong=@a WHERE PhieuGiamGiaID=@id",l.ToArray());}
            LoadData();Toast.Success("Đã lưu voucher.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu voucher");}
    }
    private void Delete(){if(_id==null||UiMsg.Ask("Xóa voucher?","Xác nhận",danger:true)!=DialogResult.Yes)return;try{Db.Execute("DELETE FROM PhieuGiamGia WHERE PhieuGiamGiaID=@id",new SqlParameter("@id",_id));Clear();LoadData();Toast.Success("Đã xóa voucher.");}catch(Exception ex){UiMsg.Error("Voucher đã được sử dụng/cấp cho khách nên không thể xóa. Có thể tắt hoạt động.\n"+ex.Message);}}
    private void Assign(){
        if(_id==null){UiMsg.Warn("Chọn voucher cần cấp.");return;}
        using var f=new Form{Text="Cấp voucher cho khách",StartPosition=FormStartPosition.CenterParent,Size=new Size(460,220),FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false,BackColor=Color.White};
        var combo=new ComboBox{Left=20,Top=30,Width=400,Height=36,DropDownStyle=ComboBoxStyle.DropDownList};
        var lbl=new Label{Text="Chọn khách hàng để cấp voucher",AutoSize=true,Location=new Point(20,10),Font=new Font("Segoe UI Semibold",9F)};
        var dt=Db.Query("SELECT KhachHangID,MaKhachHang+' - '+HoTen TenHienThi FROM KhachHang WHERE DangHoatDong=1 ORDER BY HoTen");
        combo.DataSource=dt;combo.ValueMember="KhachHangID";combo.DisplayMember="TenHienThi";
        var b=new Button{Text="🎁  Cấp voucher",Left=280,Top=90,Width=140,Height=40};
        AppTheme.StylePrimary(b);
        b.Click+=(_,__)=>{
            if(combo.SelectedValue==null)return;
            try{
                // Dùng transaction để check SoLuong và tránh cấp trùng
                using var cn=Db.OpenConnection();using var tx=cn.BeginTransaction(System.Data.IsolationLevel.Serializable);
                try{
                    using var check=new SqlCommand(@"SELECT SoLuong FROM PhieuGiamGia WITH (UPDLOCK,HOLDLOCK) WHERE PhieuGiamGiaID=@v",cn,tx);
                    check.Parameters.AddWithValue("@v",_id);
                    var soLuongObj=check.ExecuteScalar();
                    if(soLuongObj!=null && soLuongObj!=DBNull.Value && Convert.ToInt32(soLuongObj)<=0){
                        throw new InvalidOperationException("Voucher đã hết số lượng.");
                    }
                    using var exists=new SqlCommand(@"SELECT COUNT(*) FROM PhieuGiamGiaKhachHang WHERE KhachHangID=@c AND PhieuGiamGiaID=@v",cn,tx);
                    exists.Parameters.AddWithValue("@c",Convert.ToInt32(combo.SelectedValue));exists.Parameters.AddWithValue("@v",_id);
                    if(Convert.ToInt32(exists.ExecuteScalar())>0) throw new InvalidOperationException("Khách hàng này đã có voucher này.");
                    using var ins=new SqlCommand(@"INSERT INTO PhieuGiamGiaKhachHang(KhachHangID,PhieuGiamGiaID) VALUES(@c,@v)",cn,tx);
                    ins.Parameters.AddWithValue("@c",Convert.ToInt32(combo.SelectedValue));ins.Parameters.AddWithValue("@v",_id);
                    ins.ExecuteNonQuery();
                    tx.Commit();
                    Toast.Success("Đã cấp voucher.");f.Close();
                }catch(Exception exInner){try{tx.Rollback();}catch{} throw new InvalidOperationException(exInner.Message);}
            }catch(Exception ex){UiMsg.Error(ex.Message,"Cấp voucher");}
        };
        f.Controls.AddRange(new Control[]{lbl,combo,b});AppTheme.Upgrade(f);f.ShowDialogFx(this);
    }
}
