/* KIỂM TRA KẾT NỐI CSDL CHO PROJECT WINFORMS */
USE [QuanLySanTheThaoDB];
GO

SELECT
    @@SERVERNAME AS [SQLServer],
    DB_NAME() AS [DatabaseDangDung],
    SUSER_SNAME() AS [TaiKhoanWindows],
    GETDATE() AS [ThoiGianKiemTra];
GO

SELECT COUNT(*) AS [SoBangNguoiDung]
FROM sys.tables;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.CauHinhHeThong
    WHERE KhoaCauHinh=N'DatabaseVersion' AND GiaTriCauHinh=N'4.0 PRO'
)
    THROW 52300, N'Kết nối được nhưng CSDL không đúng phiên bản 4.0 PRO.', 1;
GO

SELECT KhoaCauHinh, GiaTriCauHinh
FROM dbo.CauHinhHeThong
WHERE KhoaCauHinh=N'DatabaseVersion';
GO

PRINT N'Kết nối QuanLySanTheThaoDB thành công.';
GO
