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
                using var vc = new SqlCommand(@"
SELECT TOP 1 v.LoaiGiamGia,v.GiaTriGiam,v.GiaTriGiamToiDa,v.GiaTriDonHangToiThieu,v.SoLuong
FROM PhieuGiamGia v WITH (UPDLOCK,HOLDLOCK)
INNER JOIN PhieuGiamGiaKhachHang cv WITH (UPDLOCK,HOLDLOCK) ON cv.PhieuGiamGiaID=v.PhieuGiamGiaID
WHERE cv.KhachHangID=@KhachHangID AND v.PhieuGiamGiaID=@PhieuGiamGiaID
AND cv.DaSuDung=0 AND v.DangHoatDong=1 AND @BookingTime BETWEEN v.NgayBatDau AND v.NgayKetThuc
AND @TienGoc>=v.GiaTriDonHangToiThieu;", cn, tx);
                vc.Parameters.AddWithValue("@KhachHangID", customerId);
                vc.Parameters.AddWithValue("@PhieuGiamGiaID", voucherId.Value);
                vc.Parameters.AddWithValue("@BookingTime", start);
                vc.Parameters.AddWithValue("@TienGoc", subtotal);
                using var r = vc.ExecuteReader();
                if (!r.Read())
                    throw new InvalidOperationException("Voucher không còn hợp lệ, đã dùng, hết hạn hoặc đơn chưa đạt giá trị tối thiểu.");
                if (r["SoLuong"] != DBNull.Value && Convert.ToInt32(r["SoLuong"]) <= 0)
                    throw new InvalidOperationException("Voucher đã hết số lượng sử dụng.");

                var type = Convert.ToString(r["LoaiGiamGia"]) ?? "Fixed";
                var value = Convert.ToDecimal(r["GiaTriGiam"]);
                var max = r["GiaTriGiamToiDa"] == DBNull.Value ? decimal.MaxValue : Convert.ToDecimal(r["GiaTriGiamToiDa"]);
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

    public void SetStatus(int bookingId, string status, int? customerId = null)
    {
        if (status != "Confirmed" && status != "InUse" && status != "Completed" && status != "Cancelled")
            throw new InvalidOperationException("Trạng thái đặt sân không hợp lệ.");

        using var cn = Db.OpenConnection();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            using var q = new SqlCommand(@"
SELECT b.TrangThai,b.ThoiGianBatDau,ISNULL(h.SoTienDaTra,0) SoTienDaTra,b.KhachHangID,b.PhieuGiamGiaID
FROM DatSan b WITH (UPDLOCK,HOLDLOCK)
LEFT JOIN HoaDon h ON h.DatSanID=b.DatSanID
WHERE b.DatSanID=@id AND (@CustomerID IS NULL OR b.KhachHangID=@CustomerID);", cn, tx);
            q.Parameters.AddWithValue("@id", bookingId);
            q.Parameters.AddWithValue("@CustomerID", (object?)customerId ?? DBNull.Value);
            using var r = q.ExecuteReader();
            if (!r.Read()) throw new InvalidOperationException("Không tìm thấy lịch đặt sân.");
            var current = Convert.ToString(r["TrangThai"]) ?? "";
            var paid = Convert.ToDecimal(r["SoTienDaTra"]);
            var bookingStart = Convert.ToDateTime(r["ThoiGianBatDau"]);
            var khachHangId = Convert.ToInt32(r["KhachHangID"]);
            var phieuId = r["PhieuGiamGiaID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["PhieuGiamGiaID"]);
            r.Close();

            if (current == "Cancelled" || current == "Completed")
                throw new InvalidOperationException("Lịch đặt đã kết thúc nên không thể đổi trạng thái.");
            if (customerId.HasValue && status != "Cancelled")
                throw new InvalidOperationException("Khách hàng chỉ được tự hủy lịch của mình.");
            if (customerId.HasValue && status == "Cancelled")
            {
                int hours = SettingInt("CustomerCancellationHours", 2);
                if (bookingStart <= DateTime.Now.AddHours(hours))
                    throw new InvalidOperationException($"Chỉ có thể tự hủy trước giờ bắt đầu ít nhất {hours} giờ.");
            }
            if (status == "InUse" && current != "Confirmed" && current != "Pending")
                throw new InvalidOperationException("Chỉ lịch đã xác nhận mới có thể chuyển sang đang sử dụng.");
            if (status == "InUse" && bookingStart > DateTime.Now.AddMinutes(30))
                throw new InvalidOperationException("Chỉ có thể nhận sân sớm tối đa 30 phút trước giờ bắt đầu.");
            if (status == "Completed" && current != "InUse")
                throw new InvalidOperationException("Hãy chuyển lịch sang Đang sử dụng trước khi hoàn thành.");
            if (status == "Cancelled" && paid > 0)
                throw new InvalidOperationException("Đơn đã nhận tiền. Hãy xử lý hoàn tiền trước khi hủy.");

            using var u = new SqlCommand("UPDATE DatSan SET TrangThai=@s,NgayCapNhat=SYSDATETIME() WHERE DatSanID=@id", cn, tx);
            u.Parameters.AddWithValue("@s", status);
            u.Parameters.AddWithValue("@id", bookingId);
            u.ExecuteNonQuery();

            // Nếu hủy và có voucher, hoàn SoLuong tổng (trigger hoàn DaSuDung)
            if (status == "Cancelled" && phieuId.HasValue)
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

    public void CancelBooking(int bookingId, int? customerId = null)
    {
        SetStatus(bookingId, "Cancelled", customerId);
    }

    public DataTable GetBookingsForUser(string role, int? customerId)
    {
        var where = role == "Customer" ? "WHERE b.KhachHangID=@KhachHangID" : "";
        return Db.Query($@"
SELECT b.DatSanID,b.MaDatSan,c.HoTen AS Customer,f.TenSan,ft.TenLoaiSan,
       b.ThoiGianBatDau,b.ThoiGianKetThuc,b.TongTien,b.TrangThai,b.TrangThaiThanhToan,b.NgayTao
FROM DatSan b
INNER JOIN KhachHang c ON c.KhachHangID=b.KhachHangID
INNER JOIN SanTheThao f ON f.SanID=b.SanID
INNER JOIN LoaiSan ft ON ft.LoaiSanID=f.LoaiSanID
{where}
ORDER BY b.ThoiGianBatDau DESC;",
            new SqlParameter("@KhachHangID", (object?)customerId ?? DBNull.Value));
    }

    private static void ValidateBookingTime(DateTime start, DateTime end)
    {
        if (end <= start) throw new InvalidOperationException("Giờ kết thúc phải lớn hơn giờ bắt đầu.");
        if (start.Date != end.Date) throw new InvalidOperationException("Một lượt đặt sân phải bắt đầu và kết thúc trong cùng ngày.");
        int minimum = SettingInt("MinimumBookingMinutes", 30);
        if ((end - start).TotalMinutes < minimum)
            throw new InvalidOperationException($"Thời lượng đặt sân tối thiểu là {minimum} phút.");

        var settings = new SettingsService();
        if (!TimeSpan.TryParse(settings.Get("OpenTime", "05:00"), out var open)) open = TimeSpan.FromHours(5);
        if (!TimeSpan.TryParse(settings.Get("CloseTime", "23:00"), out var close)) close = TimeSpan.FromHours(23);
        if (start.TimeOfDay < open || end.TimeOfDay > close)
            throw new InvalidOperationException($"Thời gian hoạt động là {open:hh\\:mm} - {close:hh\\:mm}.");
        if ((end - start).TotalHours > 12) throw new InvalidOperationException("Thời lượng đặt tối đa 12 giờ.");
    }

    private static int SettingInt(string key, int fallback)
    {
        var value = new SettingsService().Get(key, fallback.ToString());
        return int.TryParse(value, out var result) && result > 0 ? result : fallback;
    }
}
