USE [QuanLySanTheThaoDB];
GO
SET NOCOUNT ON;

PRINT N'=== SPORTFIELD V4 PRO - KIỂM TRA TƯƠNG THÍCH APP / CSDL ===';
EXEC dbo.usp_KiemTraCSDL @DungNeuCoLoi=1;

PRINT N'=== KIỂM THỬ CHỨC NĂNG TRONG TRANSACTION (TỰ ROLLBACK) ===';
EXEC dbo.usp_KiemTraChucNang;

SELECT N'DatabaseVersion' AS HangMuc, GiaTriCauHinh AS GiaTri
FROM dbo.CauHinhHeThong WHERE KhoaCauHinh=N'DatabaseVersion';

SELECT r.TenVaiTro, u.TenDangNhap, u.DangHoatDong,
       c.KhachHangID, e.NhanVienID
FROM dbo.TaiKhoan u
JOIN dbo.VaiTro r ON r.VaiTroID=u.VaiTroID
LEFT JOIN dbo.KhachHang c ON c.TaiKhoanID=u.TaiKhoanID
LEFT JOIN dbo.NhanVien e ON e.TaiKhoanID=u.TaiKhoanID
WHERE u.TenDangNhap IN (N'admin',N'staff',N'customer')
ORDER BY u.TaiKhoanID;

SELECT COUNT(*) AS SoSanHoatDong
FROM dbo.SanTheThao
WHERE DangHoatDong=1 AND TrangThai=N'Available';

SELECT N'Lịch đặt' AS HangMuc, COUNT(*) AS SoDong FROM dbo.DatSan
UNION ALL SELECT N'Hóa đơn',COUNT(*) FROM dbo.HoaDon
UNION ALL SELECT N'Thanh toán',COUNT(*) FROM dbo.ThanhToan
UNION ALL SELECT N'Lịch sử',COUNT(*) FROM dbo.LichSuDatSan;

SELECT TOP 20 * FROM dbo.vw_LichDatSanChiTiet ORDER BY ThoiGianBatDau DESC;

DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;

SELECT h.MaHoaDon,h.SoTienDaTra,ISNULL(SUM(CASE WHEN p.TrangThai=N'Confirmed' THEN p.SoTien ELSE 0 END),0) AS TongGiaoDichXacNhan
FROM dbo.HoaDon h
LEFT JOIN dbo.ThanhToan p ON p.HoaDonID=h.HoaDonID
GROUP BY h.MaHoaDon,h.SoTienDaTra
HAVING h.SoTienDaTra<>ISNULL(SUM(CASE WHEN p.TrangThai=N'Confirmed' THEN p.SoTien ELSE 0 END),0);
-- Kết quả cuối cùng phải không có dòng nào.
GO
