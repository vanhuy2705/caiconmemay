using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;

namespace QuanLyThueSanTheThao.Services;

public sealed class BookingService
{
    public DataTable GetFields() => Db.Query(@"
SELECT f.SanID,f.MaSan,f.TenSan,ft.TenLoaiSan,f.GiaMoiGio,f.TrangThai
FROM SanTheThao f INNER JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID
WHERE f.DangHoatDong=1 ORDER BY ft.TenLoaiSan,f.TenSan;");

    public DataTable GetAvailableFields(DateTime start, DateTime end, int? fieldTypeId = null)
    {
        ValidateBookingTime(start, end);
        // V8 FIX: Dùng UPDLOCK,HOLDLOCK thay vì READCOMMITTEDLOCK để tránh race
        // khi 2 người cùng check trống cùng lúc -> cùng đặt trùng.
        var sql = @"
SELECT f.SanID,f.MaSan,f.TenSan,ft.TenLoaiSan,f.GiaMoiGio
FROM SanTheThao f
INNER JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID
WHERE f.DangHoatDong=1 AND ft.DangHoatDong=1 AND f.TrangThai=N'Available'
AND (@TypeID IS NULL OR f.LoaiSanID=@TypeID)
AND NOT EXISTS(
    SELECT 1 FROM DatSan b WITH (UPDLOCK,HOLDLOCK,ROWLOCK)
    WHERE b.SanID=f.SanID
      AND b.TrangThai NOT IN (N'Cancelled',N'Completed')
      AND @ThoiGianBatDau < b.ThoiGianKetThuc
      AND @ThoiGianKetThuc > b.ThoiGianBatDau
)
ORDER BY ft.TenLoaiSan,f.TenSan;";
        return Db.Query(sql,
            new SqlParameter("@TypeID", (object?)fieldTypeId ?? DBNull.Value),
            new SqlParameter("@ThoiGianBatDau", start),
            new SqlParameter("@ThoiGianKetThuc", end));
    }

    public bool IsAvailable(int fieldId, DateTime start, DateTime end, int? ignoreBookingId = null)
    {
        var count = Convert.ToInt32(Db.Scalar(@"
SELECT COUNT(*) FROM DatSan WITH (UPDLOCK,HOLDLOCK,ROWLOCK)
WHERE SanID=@SanID
  AND TrangThai NOT IN (N'Cancelled',N'Completed')
  AND (@IgnoreID IS NULL OR DatSanID<>@IgnoreID)
  AND @ThoiGianBatDau < ThoiGianKetThuc
  AND @ThoiGianKetThuc > ThoiGianBatDau;",
            new SqlParameter("@SanID", fieldId),
            new SqlParameter("@IgnoreID", (object?)ignoreBookingId ?? DBNull.Value),
            new SqlParameter("@ThoiGianBatDau", start),
            new SqlParameter("@ThoiGianKetThuc", end)) ?? 0);
        return count == 0;
    }

    public int CreateBooking(int customerId, int fieldId, DateTime start, DateTime end,
        int? voucherId, string note, int createdByUserId)
    {
        if (customerId <= 0) throw new InvalidOperationException("Khách hàng không hợp lệ.");
        if (fieldId <= 0) throw new InvalidOperationException("Sân không hợp lệ.");
        ValidateBookingTime(start, end);
        if (start < DateTime.Now.AddMinutes(-1)) throw new InvalidOperationException("Không thể đặt lịch ở thời điểm đã qua.");

        using var cn = Db.OpenConnection();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            using (var customer = new SqlCommand("SELECT COUNT(*) FROM KhachHang WITH (UPDLOCK,HOLDLOCK) WHERE KhachHangID=@id AND DangHoatDong=1", cn, tx))
            {
                customer.Parameters.AddWithValue("@id", customerId);
                if (Convert.ToInt32(customer.ExecuteScalar()) == 0)
                    throw new InvalidOperationException("Khách hàng không tồn tại hoặc đã ngừng hoạt động.");
            }

            using var check = new SqlCommand(@"
SELECT COUNT(*) FROM DatSan WITH (UPDLOCK,HOLDLOCK,ROWLOCK)
WHERE SanID=@SanID
  AND TrangThai NOT IN (N'Cancelled',N'Completed')
  AND @ThoiGianBatDau < ThoiGianKetThuc AND @ThoiGianKetThuc > ThoiGianBatDau;", cn, tx);
            check.Parameters.AddWithValue("@SanID", fieldId);
            check.Parameters.AddWithValue("@ThoiGianBatDau", start);
            check.Parameters.AddWithValue("@ThoiGianKetThuc", end);
            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                throw new InvalidOperationException("Khung giờ này vừa được người khác đặt. Vui lòng chọn thời gian khác.");

            decimal hourlyRate;
            using (var price = new SqlCommand(@"
SELECT f.GiaMoiGio
FROM SanTheThao f JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID
WHERE f.SanID=@id AND f.DangHoatDong=1 AND ft.DangHoatDong=1 AND f.TrangThai=N'Available';", cn, tx))
            {
                price.Parameters.AddWithValue("@id", fieldId);
                var raw = price.ExecuteScalar() ?? throw new InvalidOperationException("Sân không tồn tại, đang bận/bảo trì hoặc đã ngừng hoạt động.");
                hourlyRate = Convert.ToDecimal(raw);
            }

            var hours = (decimal)(end - start).TotalHours;
            var subtotal = Math.Round(hourlyRate * hours, 0, MidpointRounding.AwayFromZero);
            decimal discount = 0;
            decimal voucherDiscountApplied = 0;
            int? promotionId = null;

            using (var promo = new SqlCommand(@"
SELECT TOP 1 KhuyenMaiID,PhanTramGiam
FROM KhuyenMai WITH (UPDLOCK,HOLDLOCK)
WHERE DangHoatDong=1 AND @BookingTime BETWEEN NgayBatDau AND NgayKetThuc
ORDER BY PhanTramGiam DESC, KhuyenMaiID DESC;", cn, tx))
            {
                promo.Parameters.AddWithValue("@BookingTime", start);
                using var pr = promo.ExecuteReader();
                if (pr.Read())
                {
                    promotionId = Convert.ToInt32(pr["KhuyenMaiID"]);
                    discount += Math.Round(subtotal * Convert.ToDecimal(pr["PhanTramGiam"]) / 100m, 0, MidpointRounding.AwayFromZero);
                }
            }

            if (voucherId.HasValue)
            {
                // V8 FIX: Khóa voucher + check SoLuong tồn trong cùng transaction
                using var vc = new SqlCommand(@"
SELECT TOP 1 v.LoaiGiamGia,v.GiaTriGiam,v.MucGiamToiDa,v.DonToiThieu,v.SoLuong
FROM PhieuGiamGia v WITH (UPDLOCK,HOLDLOCK)
INNER JOIN PhieuGiamGiaKhachHang cv WITH (UPDLOCK,HOLDLOCK) ON cv.PhieuGiamGiaID=v.PhieuGiamGiaID
WHERE cv.KhachHangID=@KhachHangID AND v.PhieuGiamGiaID=@PhieuGiamGiaID
AND cv.DaSuDung=0 AND v.DangHoatDong=1 AND @BookingTime BETWEEN v.NgayBatDau AND v.NgayKetThuc
AND @TienGoc>=v.DonToiThieu;", cn, tx);
                vc.Parameters.AddWithValue("@KhachHangID", customerId);
                vc.Parameters.AddWithValue("@PhieuGiamGiaID", voucherId.Value);
                vc.Parameters.AddWithValue("@BookingTime", start);
                vc.Parameters.AddWithValue("@TienGoc", subtotal);
                using var r = vc.ExecuteReader();
                if (!r.Read())
                    throw new InvalidOperationException("Voucher không còn hợp lệ, đã dùng, hết hạn hoặc đơn chưa đạt giá trị tối thiểu.");

                // Kiểm tra SoLuong tổng nếu có giới hạn
                if (r["SoLuong"] != DBNull.Value && Convert.ToInt32(r["SoLuong"]) <= 0)
                    throw new InvalidOperationException("Voucher đã hết số lượng sử dụng.");

                var type = Convert.ToString(r["LoaiGiamGia"]) ?? "Fixed";
                var value = Convert.ToDecimal(r["GiaTriGiam"]);
                var max = r["MucGiamToiDa"] == DBNull.Value ? decimal.MaxValue : Convert.ToDecimal(r["MucGiamToiDa"]);
                var voucherBase = Math.Max(0, subtotal - discount);
                var voucherDiscount = type == "Percent" ? voucherBase * value / 100m : value;
                voucherDiscount = Math.Round(voucherDiscount, 0, MidpointRounding.AwayFromZero);
                voucherDiscountApplied = Math.Min(Math.Min(voucherDiscount, max), voucherBase);
                discount += voucherDiscountApplied;
            }

            var appliedVoucherId = voucherId.HasValue && voucherDiscountApplied > 0 ? voucherId : null;
            discount = Math.Min(discount, subtotal);
            var total = Math.Max(0, subtotal - discount);
            var paymentStatus = total == 0 ? "Paid" : "Unpaid";

            using var cmd = new SqlCommand(@"
INSERT INTO DatSan(MaDatSan,KhachHangID,SanID,ThoiGianBatDau,ThoiGianKetThuc,GiaMoiGio,TienGoc,TienGiam,TongTien,
 PhieuGiamGiaID,KhuyenMaiID,TrangThai,TrangThaiThanhToan,GhiChu,TaiKhoanTaoID,NgayTao)
OUTPUT INSERTED.DatSanID
VALUES(@TempCode,@KhachHangID,@SanID,@ThoiGianBatDau,@ThoiGianKetThuc,@GiaMoiGio,@TienGoc,@Discount,@Total,
 @PhieuGiamGiaID,@KhuyenMaiID,N'Confirmed',@PaymentStatus,@GhiChu,@CreatedBy,SYSDATETIME());", cn, tx);
            cmd.Parameters.AddWithValue("@TempCode", "TMP" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant());
            cmd.Parameters.AddWithValue("@KhachHangID", customerId);
            cmd.Parameters.AddWithValue("@SanID", fieldId);
            cmd.Parameters.AddWithValue("@ThoiGianBatDau", start);
            cmd.Parameters.AddWithValue("@ThoiGianKetThuc", end);
            cmd.Parameters.AddWithValue("@GiaMoiGio", hourlyRate);
            cmd.Parameters.AddWithValue("@TienGoc", subtotal);
            cmd.Parameters.AddWithValue("@Discount", discount);
            cmd.Parameters.AddWithValue("@Total", total);
            cmd.Parameters.AddWithValue("@PhieuGiamGiaID", (object?)appliedVoucherId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@KhuyenMaiID", (object?)promotionId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            cmd.Parameters.AddWithValue("@GhiChu", note ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", createdByUserId);
            var bookingId = Convert.ToInt32(cmd.ExecuteScalar());

            var bookingCode = $"DS{bookingId:000000}";
            using (var upd = new SqlCommand("UPDATE DatSan SET MaDatSan=@code WHERE DatSanID=@id", cn, tx))
            {
                upd.Parameters.AddWithValue("@code", bookingCode);
                upd.Parameters.AddWithValue("@id", bookingId);
                upd.ExecuteNonQuery();
            }

            using (var invoice = new SqlCommand(@"
INSERT INTO HoaDon(MaHoaDon,DatSanID,KhachHangID,TienGoc,TienGiam,TongTien,SoTienDaTra,TrangThaiThanhToan,NgayTao)
VALUES(@Code,@DatSanID,@KhachHangID,@TienGoc,@Discount,@Total,0,@PaymentStatus,SYSDATETIME());", cn, tx))
            {
                invoice.Parameters.AddWithValue("@Code", $"HD{bookingId:000000}");
                invoice.Parameters.AddWithValue("@DatSanID", bookingId);
                invoice.Parameters.AddWithValue("@KhachHangID", customerId);
                invoice.Parameters.AddWithValue("@TienGoc", subtotal);
                invoice.Parameters.AddWithValue("@Discount", discount);
                invoice.Parameters.AddWithValue("@Total", total);
                invoice.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                invoice.ExecuteNonQuery();
            }

            // V8 FIX: Đánh dấu voucher đã dùng + trừ SoLuong nếu có giới hạn
            if (appliedVoucherId.HasValue)
            {
                using var useVoucher = new SqlCommand(@"
UPDATE PhieuGiamGiaKhachHang SET DaSuDung=1, NgaySuDung=SYSDATETIME(), DatSanSuDungID=@BookingID
WHERE KhachHangID=@KhachHangID AND PhieuGiamGiaID=@VoucherID AND DaSuDung=0;", cn, tx);
                useVoucher.Parameters.AddWithValue("@BookingID", bookingId);
                useVoucher.Parameters.AddWithValue("@KhachHangID", customerId);
                useVoucher.Parameters.AddWithValue("@VoucherID", appliedVoucherId.Value);
                if (useVoucher.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("Voucher vừa được sử dụng ở nơi khác. Vui lòng chọn voucher khác.");

                // Trừ SoLuong tổng nếu có giới hạn (tránh vượt quá)
                using var decQty = new SqlCommand(@"
UPDATE PhieuGiamGia SET SoLuong = CASE WHEN SoLuong IS NULL THEN NULL ELSE SoLuong - 1 END
WHERE PhieuGiamGiaID=@id AND (SoLuong IS NULL OR SoLuong > 0);", cn, tx);
                decQty.Parameters.AddWithValue("@id", appliedVoucherId.Value);
                decQty.ExecuteNonQuery();
            }

            tx.Commit();
            return bookingId;
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }

    public void CancelBooking(int bookingId, int? customerId = null)
    {
        // V8: Hoàn voucher khi hủy nếu chưa thanh toán, trong transaction an toàn
        using var cn = Db.OpenConnection();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            // Kiểm tra tồn tại và chưa thu tiền (trigger cũng check)
            using var check = new SqlCommand(@"
SELECT b.DatSanID, b.KhachHangID, b.PhieuGiamGiaID, h.SoTienDaTra, b.TrangThai
FROM DatSan b JOIN HoaDon h ON h.DatSanID=b.DatSanID
WHERE b.DatSanID=@id AND (@cid IS NULL OR b.KhachHangID=@cid);", cn, tx);
            check.Parameters.AddWithValue("@id", bookingId);
            check.Parameters.AddWithValue("@cid", (object?)customerId ?? DBNull.Value);
            using var r = check.ExecuteReader();
            if (!r.Read()) throw new InvalidOperationException("Không tìm thấy đơn đặt sân.");
            var soTienDaTra = Convert.ToDecimal(r["SoTienDaTra"]);
            var trangThai = Convert.ToString(r["TrangThai"]);
            var khachHangId = Convert.ToInt32(r["KhachHangID"]);
            var phieuId = r["PhieuGiamGiaID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["PhieuGiamGiaID"]);
            r.Close();

            if (trangThai == "Cancelled") throw new InvalidOperationException("Đơn đã hủy trước đó.");
            if (soTienDaTra > 0) throw new InvalidOperationException("Đơn đã thu tiền, cần hoàn tiền trước khi hủy.");

            using var upd = new SqlCommand("UPDATE DatSan SET TrangThai=N'Cancelled', NgayCapNhat=SYSDATETIME() WHERE DatSanID=@id", cn, tx);
            upd.Parameters.AddWithValue("@id", bookingId);
            upd.ExecuteNonQuery();

            // Trigger TR_DatSan_XuLyKhiHuy sẽ tự hoàn voucher, nhưng ta đảm bảo SoLuong tổng được hoàn nếu cần
            if (phieuId.HasValue)
            {
                using var restoreQty = new SqlCommand(@"
UPDATE PhieuGiamGia SET SoLuong = CASE WHEN SoLuong IS NULL THEN NULL ELSE SoLuong + 1 END
WHERE PhieuGiamGiaID=@id;", cn, tx);
                restoreQty.Parameters.AddWithValue("@id", phieuId.Value);
                restoreQty.ExecuteNonQuery();
            }

            tx.Commit();
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }

    private static void ValidateBookingTime(DateTime start, DateTime end)
    {
        if (end <= start) throw new InvalidOperationException("Giờ kết thúc phải sau giờ bắt đầu.");
        if ((end - start).TotalMinutes < 30) throw new InvalidOperationException("Thời lượng đặt tối thiểu 30 phút.");
        if ((end - start).TotalHours > 12) throw new InvalidOperationException("Thời lượng đặt tối đa 12 giờ.");
    }
}
