using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyThueSanTheThao.Data;

namespace QuanLyThueSanTheThao.Services;

public sealed class PaymentService
{
    public DataRow GetPaymentInfo(int bookingId)
    {
        var dt = Db.Query(@"
SELECT b.DatSanID,b.MaDatSan,b.TongTien,b.TrangThai,b.TrangThaiThanhToan,
       i.HoaDonID,i.MaHoaDon,i.SoTienDaTra,
       c.HoTen AS CustomerName,f.TenSan,b.ThoiGianBatDau,b.ThoiGianKetThuc
FROM DatSan b
INNER JOIN HoaDon i ON i.DatSanID=b.DatSanID
INNER JOIN KhachHang c ON c.KhachHangID=b.KhachHangID
INNER JOIN SanTheThao f ON f.SanID=b.SanID
WHERE b.DatSanID=@id;", new SqlParameter("@id", bookingId));
        if (dt.Rows.Count == 0) throw new InvalidOperationException("Không tìm thấy đơn đặt sân.");
        return dt.Rows[0];
    }

    public void RecordPayment(int invoiceId, decimal amount, string method, string transactionRef, bool confirmPaid)
    {
        if (amount <= 0) throw new InvalidOperationException("Số tiền thanh toán phải lớn hơn 0.");
        if (method != "Cash" && method != "QR" && method != "BankTransfer")
            throw new InvalidOperationException("Phương thức thanh toán không hợp lệ.");

        using var cn = Db.OpenConnection();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            decimal total, paid;
            string bookingStatus;
            using (var q = new SqlCommand(@"
SELECT h.TongTien,h.SoTienDaTra,b.TrangThai
FROM HoaDon h WITH (UPDLOCK,HOLDLOCK)
JOIN DatSan b ON b.DatSanID=h.DatSanID
WHERE h.HoaDonID=@id;", cn, tx))
            {
                q.Parameters.AddWithValue("@id", invoiceId);
                using var r = q.ExecuteReader();
                if (!r.Read()) throw new InvalidOperationException("Không tìm thấy hóa đơn.");
                total = Convert.ToDecimal(r["TongTien"]);
                paid = Convert.ToDecimal(r["SoTienDaTra"]);
                bookingStatus = Convert.ToString(r["TrangThai"]) ?? "";
            }

            if (bookingStatus == "Cancelled") throw new InvalidOperationException("Không thể thanh toán đơn đã hủy.");
            var remaining = total - paid;
            if (remaining <= 0) throw new InvalidOperationException("Hóa đơn đã thanh toán đủ.");
            if (amount > remaining) throw new InvalidOperationException($"Số tiền vượt quá số còn phải thanh toán ({remaining:N0} đ).");

            if (confirmPaid)
            {
                using (var cancelPending = new SqlCommand(@"
UPDATE ThanhToan SET TrangThai=N'Cancelled',NgayThanhToan=NULL
WHERE HoaDonID=@HoaDonID AND TrangThai=N'Pending';", cn, tx))
                {
                    cancelPending.Parameters.AddWithValue("@HoaDonID", invoiceId);
                    cancelPending.ExecuteNonQuery();
                }

                using var cmd = new SqlCommand(@"
INSERT INTO ThanhToan(HoaDonID,SoTien,PhuongThucThanhToan,MaGiaoDich,TrangThai,NgayThanhToan,NgayTao)
VALUES(@HoaDonID,@SoTien,@Method,@Ref,N'Confirmed',SYSDATETIME(),SYSDATETIME());", cn, tx);
                cmd.Parameters.AddWithValue("@HoaDonID", invoiceId);
                cmd.Parameters.AddWithValue("@SoTien", amount);
                cmd.Parameters.AddWithValue("@Method", method);
                cmd.Parameters.AddWithValue("@Ref", transactionRef ?? "");
                cmd.ExecuteNonQuery();

                using var upd = new SqlCommand(@"
UPDATE HoaDon SET SoTienDaTra=SoTienDaTra+@SoTien,
TrangThaiThanhToan=CASE WHEN SoTienDaTra+@SoTien>=TongTien THEN N'Paid' ELSE N'PartiallyPaid' END
WHERE HoaDonID=@HoaDonID;", cn, tx);
                upd.Parameters.AddWithValue("@SoTien", amount);
                upd.Parameters.AddWithValue("@HoaDonID", invoiceId);
                upd.ExecuteNonQuery();
            }
            else
            {
                // Mỗi hóa đơn chỉ giữ một yêu cầu QR/CK chờ xác nhận; bấm lại sẽ cập nhật
                // giao dịch đang chờ thay vì lỗi unique index.
                using var pending = new SqlCommand(@"
IF EXISTS(SELECT 1 FROM ThanhToan WITH (UPDLOCK,HOLDLOCK) WHERE HoaDonID=@HoaDonID AND TrangThai=N'Pending')
BEGIN
    UPDATE ThanhToan SET SoTien=@SoTien,PhuongThucThanhToan=@Method,MaGiaoDich=@Ref,NgayTao=SYSDATETIME()
    WHERE HoaDonID=@HoaDonID AND TrangThai=N'Pending';
END
ELSE
BEGIN
    INSERT INTO ThanhToan(HoaDonID,SoTien,PhuongThucThanhToan,MaGiaoDich,TrangThai,NgayThanhToan,NgayTao)
    VALUES(@HoaDonID,@SoTien,@Method,@Ref,N'Pending',NULL,SYSDATETIME());
END", cn, tx);
                pending.Parameters.AddWithValue("@HoaDonID", invoiceId);
                pending.Parameters.AddWithValue("@SoTien", amount);
                pending.Parameters.AddWithValue("@Method", method);
                pending.Parameters.AddWithValue("@Ref", transactionRef ?? "");
                pending.ExecuteNonQuery();
            }

            tx.Commit();
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }

    public bool ConfirmLatestPending(int invoiceId)
    {
        using var cn = Db.OpenConnection();
        using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            int paymentId;
            decimal amount;
            using (var q = new SqlCommand(@"
SELECT TOP 1 p.ThanhToanID,p.SoTien
FROM ThanhToan p WITH (UPDLOCK,HOLDLOCK)
JOIN HoaDon h WITH (UPDLOCK,HOLDLOCK) ON h.HoaDonID=p.HoaDonID
JOIN DatSan b ON b.DatSanID=h.DatSanID
WHERE p.HoaDonID=@i AND p.TrangThai=N'Pending' AND b.TrangThai<>N'Cancelled'
ORDER BY p.NgayTao DESC;", cn, tx))
            {
                q.Parameters.AddWithValue("@i", invoiceId);
                using var r = q.ExecuteReader();
                if (!r.Read()) { tx.Rollback(); return false; }
                paymentId = Convert.ToInt32(r["ThanhToanID"]);
                amount = Convert.ToDecimal(r["SoTien"]);
            }

            using (var p = new SqlCommand("UPDATE ThanhToan SET TrangThai=N'Confirmed',NgayThanhToan=SYSDATETIME() WHERE ThanhToanID=@p AND TrangThai=N'Pending'", cn, tx))
            {
                p.Parameters.AddWithValue("@p", paymentId);
                if (p.ExecuteNonQuery() != 1) throw new InvalidOperationException("Giao dịch đã được xử lý ở cửa sổ khác.");
            }

            using (var u = new SqlCommand(@"
UPDATE HoaDon SET SoTienDaTra=SoTienDaTra+@a,
TrangThaiThanhToan=CASE WHEN SoTienDaTra+@a>=TongTien THEN N'Paid' ELSE N'PartiallyPaid' END
WHERE HoaDonID=@i;", cn, tx))
            {
                u.Parameters.AddWithValue("@a", amount);
                u.Parameters.AddWithValue("@i", invoiceId);
                u.ExecuteNonQuery();
            }

            tx.Commit();
            return true;
        }
        catch
        {
            try { tx.Rollback(); } catch { }
            throw;
        }
    }
}
