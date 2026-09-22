using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;
using QuanLyThueSanTheThao.Helpers;

namespace QuanLyThueSanTheThao.Forms.Admin;
public partial class FrmVouchers:Form
{
    private int? _id;
    public FrmVouchers(){
        InitializeComponent();
        AppTheme.Upgrade(this);ResponsiveHelper.Apply(this);
        try{var sp=this.Controls.OfType<SplitContainer>().FirstOrDefault(); if(sp!=null) ResponsiveHelper.FixSplitContainer(sp);}catch{}
        AppTheme.StyleGrid(grid);
        AppTheme.StyleSecondary(btnRefresh);
        AppTheme.StyleSecondary(btnNew);
        AppTheme.StylePrimary(btnSave);
        AppTheme.StyleDanger(btnDelete);
        AppTheme.StyleSecondary(btnAssign);
        cboType.Items.AddRange(new object[]{"Phần trăm","Số tiền"});
        cboType.SelectedIndex=0;
        grid.SelectionChanged+=(_,__)=>Bind();
        txtSearch.TextChanged+=(_,__)=>LoadData();
        btnRefresh.Click+=(_,__)=>LoadData();
        btnNew.Click+=(_,__)=>Clear();
        btnSave.Click+=(_,__)=>Save();
        btnDelete.Click+=(_,__)=>Delete();
        btnAssign.Click+=(_,__)=>Assign();
        Shown+=(_,__)=>LoadData();
    }
    private void LoadData(){
        try{
            grid.DataSource=Db.Query(@"SELECT PhieuGiamGiaID,MaPhieuGiamGia [Mã voucher],TenPhieuGiamGia [Tên voucher],LoaiGiamGia [Loại],GiaTriGiam [Giá trị],GiaTriDonHangToiThieu [ĐH tối thiểu],GiaTriGiamToiDa [Giảm tối đa],SoLuong [Số lượng],NgayBatDau [Bắt đầu],NgayKetThuc [Kết thúc],DangHoatDong [Hoạt động] FROM PhieuGiamGia WHERE @s='' OR MaPhieuGiamGia LIKE '%'+@s+'%' OR TenPhieuGiamGia LIKE N'%'+@s+'%' ORDER BY PhieuGiamGiaID DESC",new SqlParameter("@s",txtSearch.Text.Trim()));
            if(grid.Columns.Contains("PhieuGiamGiaID")) grid.Columns["PhieuGiamGiaID"].Visible=false;
        }catch(Exception ex){UiMsg.Error(ex.Message,"Tải voucher");}
    }
    private void Bind(){
        if(grid.CurrentRow?.DataBoundItem is not System.Data.DataRowView rv) return;
        var r=rv.Row;
        _id= r["PhieuGiamGiaID"]==DBNull.Value ? null : Convert.ToInt32(r["PhieuGiamGiaID"]);
        txtCode.Text=Convert.ToString(r["Mã voucher"]) ?? "";
        txtName.Text=Convert.ToString(r["Tên voucher"]) ?? "";
        cboType.SelectedItem= (Convert.ToString(r["Loại"]) ?? "")=="Percent" ? "Phần trăm" : "Số tiền";
        if(cboType.SelectedIndex<0) cboType.SelectedIndex=0;
        txtValue.Text= r["Giá trị"]==DBNull.Value ? "" : Convert.ToDecimal(r["Giá trị"]).ToString("0.##");
        txtMin.Text= r["ĐH tối thiểu"]==DBNull.Value ? "" : Convert.ToDecimal(r["ĐH tối thiểu"]).ToString("0.##");
        txtMax.Text= r["Giảm tối đa"]==DBNull.Value ? "" : Convert.ToDecimal(r["Giảm tối đa"]).ToString("0.##");
        txtQty.Text= r["Số lượng"]==DBNull.Value ? "" : Convert.ToInt32(r["Số lượng"]).ToString();
        if(r["Bắt đầu"]!=DBNull.Value) dtStart.Value=Convert.ToDateTime(r["Bắt đầu"]);
        if(r["Kết thúc"]!=DBNull.Value) dtEnd.Value=Convert.ToDateTime(r["Kết thúc"]);
        chkActive.Checked= r["Hoạt động"]!=DBNull.Value && Convert.ToBoolean(r["Hoạt động"]);
    }
    private void Clear(){
        _id=null;
        txtCode.Text="VC"+DateTime.Now.ToString("HHmmss");
        txtName.Clear();cboType.SelectedIndex=0;txtValue.Text="10";txtMin.Text="0";txtMax.Text="";txtQty.Text="100";
        dtStart.Value=DateTime.Today;dtEnd.Value=DateTime.Today.AddMonths(1);chkActive.Checked=true;
    }
    private bool TryDec(string s, out decimal v){
        return decimal.TryParse((s??"").Replace(",","").Trim(), out v);
    }
    private void Save(){
        if(string.IsNullOrWhiteSpace(txtCode.Text)||string.IsNullOrWhiteSpace(txtName.Text)){UiMsg.Warn("Nhập mã và tên voucher.");return;}
        if(!TryDec(txtValue.Text, out var val) || val<=0){UiMsg.Warn("Giá trị giảm phải >0.");return;}
        var typeStr=Convert.ToString(cboType.SelectedItem) ?? "";
        if(typeStr.Contains("Phần trăm") && val>100){UiMsg.Warn("Giảm % không quá 100%.");return;}
        if(dtEnd.Value<=dtStart.Value){UiMsg.Warn("Ngày kết thúc phải sau ngày bắt đầu.");return;}
        TryDec(txtMin.Text, out var minVal);
        decimal? maxVal=null;
        if(TryDec(txtMax.Text, out var mx) && mx>0) maxVal=mx;
        int qty=100;
        if(!string.IsNullOrWhiteSpace(txtQty.Text) && !int.TryParse(txtQty.Text.Trim(), out qty)) qty=0;
        if(qty<0){UiMsg.Warn("Số lượng không âm.");return;}
        try{
            var dup=Db.Query(@"SELECT CASE WHEN EXISTS(SELECT 1 FROM PhieuGiamGia WHERE MaPhieuGiamGia=@c AND (@id IS NULL OR PhieuGiamGiaID<>@id)) THEN N'Mã voucher đã tồn tại' ELSE '' END",
                new SqlParameter("@c",txtCode.Text.Trim()),
                new SqlParameter("@id",(object?)_id ?? DBNull.Value));
            var msg=dup.Rows.Count>0 ? (Convert.ToString(dup.Rows[0][0]) ?? "") : "";
            if(msg.Length>0){UiMsg.Warn(msg);return;}
            var loai= typeStr.Contains("Phần trăm") ? "Percent" : "FixedAmount";
            var pars=new List<SqlParameter>{
                new("@c",txtCode.Text.Trim()),
                new("@n",txtName.Text.Trim()),
                new("@l",loai),
                new("@v",val),
                new("@min",minVal),
                new("@max",(object?)maxVal ?? DBNull.Value),
                new("@q",qty),
                new("@s",dtStart.Value),
                new("@e",dtEnd.Value),
                new("@a",chkActive.Checked)
            };
            if(_id==null) Db.Execute("INSERT INTO PhieuGiamGia(MaPhieuGiamGia,TenPhieuGiamGia,LoaiGiamGia,GiaTriGiam,GiaTriDonHangToiThieu,GiaTriGiamToiDa,SoLuong,NgayBatDau,NgayKetThuc,DangHoatDong) VALUES(@c,@n,@l,@v,@min,@max,@q,@s,@e,@a)",pars.ToArray());
            else{pars.Add(new SqlParameter("@id",_id.Value));Db.Execute("UPDATE PhieuGiamGia SET MaPhieuGiamGia=@c,TenPhieuGiamGia=@n,LoaiGiamGia=@l,GiaTriGiam=@v,GiaTriDonHangToiThieu=@min,GiaTriGiamToiDa=@max,SoLuong=@q,NgayBatDau=@s,NgayKetThuc=@e,DangHoatDong=@a WHERE PhieuGiamGiaID=@id",pars.ToArray());}
            LoadData();Toast.Success("Đã lưu voucher.");
        }catch(Exception ex){UiMsg.Error(ex.Message,"Lưu voucher");}
    }
    private void Delete(){
        if(_id==null){UiMsg.Warn("Chọn voucher cần xóa.");return;}
        if(UiMsg.Ask("Xóa voucher?","Xác nhận",danger:true)!=DialogResult.Yes)return;
        try{Db.Execute("DELETE FROM PhieuGiamGia WHERE PhieuGiamGiaID=@id",new SqlParameter("@id",_id.Value));Clear();LoadData();Toast.Success("Đã xóa voucher.");}
        catch(Exception ex){UiMsg.Error("Không thể xóa voucher đã được sử dụng.\n"+ex.Message);}
    }
    private void Assign(){
        if(_id==null){UiMsg.Warn("Chọn voucher cần gán.");return;}
        var phone=Microsoft.VisualBasic.Interaction.InputBox("Nhập SĐT khách hàng để gán voucher:", "Gán voucher", "");
        if(string.IsNullOrWhiteSpace(phone)) return;
        phone=phone.Trim();
        try{
            using var cn=Db.OpenConnection();using var tx=cn.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try{
                var checkQty=new SqlCommand("SELECT SoLuong FROM PhieuGiamGia WITH (UPDLOCK,HOLDLOCK) WHERE PhieuGiamGiaID=@id",cn,tx);
                checkQty.Parameters.AddWithValue("@id",_id.Value);
                var qtyObj=checkQty.ExecuteScalar();
                if(qtyObj==null||qtyObj==DBNull.Value) throw new InvalidOperationException("Voucher không tồn tại");
                var qty=Convert.ToInt32(qtyObj);
                if(qty<=0) throw new InvalidOperationException("Voucher đã hết số lượng");

                var custCmd=new SqlCommand("SELECT KhachHangID FROM KhachHang WHERE SoDienThoai=@p",cn,tx);
                custCmd.Parameters.AddWithValue("@p",phone);
                var custObj=custCmd.ExecuteScalar();
                if(custObj==null) throw new InvalidOperationException("Không tìm thấy khách với SĐT này");
                var custId=Convert.ToInt32(custObj);

                var dupCmd=new SqlCommand("SELECT COUNT(*) FROM PhieuGiamGiaKhachHang WHERE PhieuGiamGiaID=@v AND KhachHangID=@c",cn,tx);
                dupCmd.Parameters.AddWithValue("@v",_id.Value);
                dupCmd.Parameters.AddWithValue("@c",custId);
                var exists=Convert.ToInt32(dupCmd.ExecuteScalar());
                if(exists>0) throw new InvalidOperationException("Khách đã có voucher này");

                var ins=new SqlCommand("INSERT INTO PhieuGiamGiaKhachHang(PhieuGiamGiaID,KhachHangID,DaSuDung) VALUES(@v,@c,0)",cn,tx);
                ins.Parameters.AddWithValue("@v",_id.Value);
                ins.Parameters.AddWithValue("@c",custId);
                ins.ExecuteNonQuery();
                tx.Commit();
                Toast.Success("Đã gán voucher cho khách.");
                LoadData();
            }catch(Exception ex){try{tx.Rollback();}catch{} throw new InvalidOperationException(ex.Message,ex);}
        }catch(Exception ex){UiMsg.Error(ex.Message,"Gán voucher");}
    }
}
