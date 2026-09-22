/* ============================================================================
   QuanLySanTheThaoDB_V4_PRO.sql
   Database V4 PRO cho project WinForms QuanLyThueSanTheThao (.NET 8 + SQL Server)

   MỤC TIÊU V4
   - Khớp đúng 13 bảng mà source code hiện tại đang truy cập.
   - Tăng ràng buộc dữ liệu, index và chống dữ liệu sai/trùng lịch.
   - Bảo vệ thanh toán khỏi vượt số tiền hóa đơn.
   - Đồng bộ trạng thái thanh toán HoaDon -> DatSan.
   - Voucher có kiểm soát số lượng và được hoàn lại trạng thái sử dụng khi hủy
     đơn chưa phát sinh tiền đã xác nhận.
   - Có lịch sử thay đổi trạng thái đặt sân.
   - Có VIEW, stored procedure kiểm tra sức khỏe CSDL và dữ liệu mẫu xuyên suốt.
   - Hỗ trợ đăng ký khách hàng, đăng nhập bằng tài khoản/email/SĐT.
   - Chuẩn hóa email/SĐT duy nhất để tránh hồ sơ và đăng nhập bị nhập nhằng.
   - Không hard-code đường dẫn MDF/LDF: SQL Server tự dùng DATA directory của
     instance đang chạy. Trên SQL Server 2022 Express thường là:
     C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\

   CẢNH BÁO QUAN TRỌNG
   Script này là CÀI MỚI SẠCH: nếu QuanLySanTheThaoDB đã tồn tại, database cũ
   sẽ bị xóa rồi tạo lại. Hãy backup dữ liệu cần giữ trước khi Execute.

   Tài khoản demo:
     admin    / admin123
     staff    / staff123
     customer / customer123

   Connection string tương thích project:
     Server=.\SQLEXPRESS;Database=QuanLySanTheThaoDB;
     Trusted_Connection=True;TrustServerCertificate=True;
     MultipleActiveResultSets=True;Connect Timeout=15;
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* ========================= 1. TẠO DATABASE SẠCH ========================= */
USE [master];
GO

IF DB_ID(N'QuanLySanTheThaoDB') IS NOT NULL
BEGIN
    PRINT N'[V4] Đang xóa database QuanLySanTheThaoDB cũ...';
    ALTER DATABASE [QuanLySanTheThaoDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [QuanLySanTheThaoDB];
END
GO

CREATE DATABASE [QuanLySanTheThaoDB];
GO

ALTER DATABASE [QuanLySanTheThaoDB] SET AUTO_CLOSE OFF;
ALTER DATABASE [QuanLySanTheThaoDB] SET AUTO_SHRINK OFF;
ALTER DATABASE [QuanLySanTheThaoDB] SET PAGE_VERIFY CHECKSUM;
ALTER DATABASE [QuanLySanTheThaoDB] SET RECOVERY SIMPLE;
GO

USE [QuanLySanTheThaoDB];
GO

/* ========================= 2. PHÂN QUYỀN / TÀI KHOẢN ========================= */
CREATE TABLE dbo.VaiTro
(
    VaiTroID     INT IDENTITY(1,1) NOT NULL,
    TenVaiTro    NVARCHAR(30) NOT NULL,
    TenHienThi   NVARCHAR(100) NOT NULL,

    CONSTRAINT PK_VaiTro PRIMARY KEY (VaiTroID),
    CONSTRAINT UQ_VaiTro_TenVaiTro UNIQUE (TenVaiTro),
    CONSTRAINT CK_VaiTro_TenVaiTro CHECK (TenVaiTro IN (N'Admin', N'Employee', N'Customer'))
);
GO

CREATE TABLE dbo.TaiKhoan
(
    TaiKhoanID       INT IDENTITY(1,1) NOT NULL,
    TenDangNhap      NVARCHAR(50) NOT NULL,
    MatKhauBam       VARCHAR(64) NOT NULL,
    MuoiMatKhau      VARCHAR(32) NOT NULL,
    VaiTroID         INT NOT NULL,
    DangHoatDong     BIT NOT NULL CONSTRAINT DF_TaiKhoan_DangHoatDong DEFAULT (1),
    DangNhapLanCuoi  DATETIME2(0) NULL,
    NgayTao          DATETIME2(0) NOT NULL CONSTRAINT DF_TaiKhoan_NgayTao DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_TaiKhoan PRIMARY KEY (TaiKhoanID),
    CONSTRAINT UQ_TaiKhoan_TenDangNhap UNIQUE (TenDangNhap),
    CONSTRAINT FK_TaiKhoan_VaiTro FOREIGN KEY (VaiTroID) REFERENCES dbo.VaiTro(VaiTroID),
    CONSTRAINT CK_TaiKhoan_TenDangNhap CHECK
        (LEN(TenDangNhap) BETWEEN 4 AND 50 AND TenDangNhap NOT LIKE N'% %'),
    CONSTRAINT CK_TaiKhoan_MatKhauBam CHECK (LEN(MatKhauBam) = 64),
    CONSTRAINT CK_TaiKhoan_Muoi CHECK (LEN(MuoiMatKhau) >= 8)
);
GO

CREATE INDEX IX_TaiKhoan_VaiTro_HoatDong
ON dbo.TaiKhoan(VaiTroID, DangHoatDong)
INCLUDE (TenDangNhap, DangNhapLanCuoi, NgayTao);
GO

/* ========================= 3. KHÁCH HÀNG / NHÂN VIÊN ========================= */
CREATE TABLE dbo.KhachHang
(
    KhachHangID    INT IDENTITY(1,1) NOT NULL,
    MaKhachHang    NVARCHAR(20) NOT NULL,
    TaiKhoanID     INT NULL,
    HoTen          NVARCHAR(100) NOT NULL,
    SoDienThoai    NVARCHAR(20) NULL,
    Email          NVARCHAR(120) NULL,
    DiaChi         NVARCHAR(250) NULL,
    NgaySinh       DATE NULL,
    DiemTichLuy    INT NOT NULL CONSTRAINT DF_KhachHang_DiemTichLuy DEFAULT (0),
    DangHoatDong   BIT NOT NULL CONSTRAINT DF_KhachHang_DangHoatDong DEFAULT (1),
    NgayTao        DATETIME2(0) NOT NULL CONSTRAINT DF_KhachHang_NgayTao DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_KhachHang PRIMARY KEY (KhachHangID),
    CONSTRAINT UQ_KhachHang_MaKhachHang UNIQUE (MaKhachHang),
    CONSTRAINT FK_KhachHang_TaiKhoan FOREIGN KEY (TaiKhoanID) REFERENCES dbo.TaiKhoan(TaiKhoanID),
    CONSTRAINT CK_KhachHang_DiemTichLuy CHECK (DiemTichLuy >= 0),
    CONSTRAINT CK_KhachHang_SoDienThoai CHECK
        (SoDienThoai IS NULL OR SoDienThoai=N'' OR (LEN(SoDienThoai) BETWEEN 9 AND 15 AND SoDienThoai NOT LIKE N'%[^0-9]%')),
    CONSTRAINT CK_KhachHang_Email CHECK
        (Email IS NULL OR Email=N'' OR (Email LIKE N'%_@_%._%' AND Email NOT LIKE N'% %'))
);
GO

-- SQL Server UNIQUE thông thường chỉ cho 1 NULL. Filtered unique index cho phép
-- nhiều khách hàng chưa có tài khoản nhưng mỗi tài khoản chỉ gắn với 1 khách.
CREATE UNIQUE INDEX UX_KhachHang_TaiKhoanID_NotNull
ON dbo.KhachHang(TaiKhoanID)
WHERE TaiKhoanID IS NOT NULL;
GO

CREATE INDEX IX_KhachHang_TimKiem
ON dbo.KhachHang(MaKhachHang, HoTen, SoDienThoai)
INCLUDE (Email, DangHoatDong, DiemTichLuy, NgayTao);
GO

CREATE UNIQUE INDEX UX_KhachHang_SoDienThoai_NotBlank
ON dbo.KhachHang(SoDienThoai)
WHERE SoDienThoai IS NOT NULL AND SoDienThoai <> N'';
GO

CREATE UNIQUE INDEX UX_KhachHang_Email_NotBlank
ON dbo.KhachHang(Email)
WHERE Email IS NOT NULL AND Email <> N'';
GO

CREATE TABLE dbo.NhanVien
(
    NhanVienID     INT IDENTITY(1,1) NOT NULL,
    MaNhanVien     NVARCHAR(20) NOT NULL,
    TaiKhoanID     INT NULL,
    HoTen          NVARCHAR(100) NOT NULL,
    SoDienThoai    NVARCHAR(20) NULL,
    Email          NVARCHAR(120) NULL,
    ChucVu         NVARCHAR(60) NULL,
    DangHoatDong   BIT NOT NULL CONSTRAINT DF_NhanVien_DangHoatDong DEFAULT (1),
    NgayTao        DATETIME2(0) NOT NULL CONSTRAINT DF_NhanVien_NgayTao DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_NhanVien PRIMARY KEY (NhanVienID),
    CONSTRAINT UQ_NhanVien_MaNhanVien UNIQUE (MaNhanVien),
    CONSTRAINT FK_NhanVien_TaiKhoan FOREIGN KEY (TaiKhoanID) REFERENCES dbo.TaiKhoan(TaiKhoanID),
    CONSTRAINT CK_NhanVien_SoDienThoai CHECK
        (SoDienThoai IS NULL OR SoDienThoai=N'' OR (LEN(SoDienThoai) BETWEEN 9 AND 15 AND SoDienThoai NOT LIKE N'%[^0-9]%')),
    CONSTRAINT CK_NhanVien_Email CHECK
        (Email IS NULL OR Email=N'' OR (Email LIKE N'%_@_%._%' AND Email NOT LIKE N'% %'))
);
GO

CREATE UNIQUE INDEX UX_NhanVien_TaiKhoanID_NotNull
ON dbo.NhanVien(TaiKhoanID)
WHERE TaiKhoanID IS NOT NULL;
GO

CREATE INDEX IX_NhanVien_TimKiem
ON dbo.NhanVien(MaNhanVien, HoTen, SoDienThoai)
INCLUDE (Email, ChucVu, DangHoatDong, TaiKhoanID);
GO

CREATE UNIQUE INDEX UX_NhanVien_SoDienThoai_NotBlank
ON dbo.NhanVien(SoDienThoai)
WHERE SoDienThoai IS NOT NULL AND SoDienThoai <> N'';
GO

CREATE UNIQUE INDEX UX_NhanVien_Email_NotBlank
ON dbo.NhanVien(Email)
WHERE Email IS NOT NULL AND Email <> N'';
GO

/* ========================= 4. LOẠI SÂN / SÂN ========================= */
CREATE TABLE dbo.LoaiSan
(
    LoaiSanID      INT IDENTITY(1,1) NOT NULL,
    MaLoaiSan      NVARCHAR(20) NOT NULL,
    TenLoaiSan     NVARCHAR(100) NOT NULL,
    MoTa           NVARCHAR(300) NULL,
    DangHoatDong   BIT NOT NULL CONSTRAINT DF_LoaiSan_DangHoatDong DEFAULT (1),

    CONSTRAINT PK_LoaiSan PRIMARY KEY (LoaiSanID),
    CONSTRAINT UQ_LoaiSan_MaLoaiSan UNIQUE (MaLoaiSan)
);
GO

CREATE TABLE dbo.SanTheThao
(
    SanID          INT IDENTITY(1,1) NOT NULL,
    MaSan          NVARCHAR(20) NOT NULL,
    TenSan         NVARCHAR(100) NOT NULL,
    LoaiSanID      INT NOT NULL,
    ViTri          NVARCHAR(120) NULL,
    GiaMoiGio      DECIMAL(14,2) NOT NULL,
    TrangThai      NVARCHAR(30) NOT NULL CONSTRAINT DF_SanTheThao_TrangThai DEFAULT (N'Available'),
    MoTa           NVARCHAR(300) NULL,
    DangHoatDong   BIT NOT NULL CONSTRAINT DF_SanTheThao_DangHoatDong DEFAULT (1),

    CONSTRAINT PK_SanTheThao PRIMARY KEY (SanID),
    CONSTRAINT UQ_SanTheThao_MaSan UNIQUE (MaSan),
    CONSTRAINT FK_SanTheThao_LoaiSan FOREIGN KEY (LoaiSanID) REFERENCES dbo.LoaiSan(LoaiSanID),
    CONSTRAINT CK_SanTheThao_GiaMoiGio CHECK (GiaMoiGio > 0),
    CONSTRAINT CK_SanTheThao_TrangThai CHECK
        (TrangThai IN (N'Available', N'Busy', N'Maintenance', N'Inactive'))
);
GO

CREATE INDEX IX_SanTheThao_Loai_TrangThai
ON dbo.SanTheThao(LoaiSanID, DangHoatDong, TrangThai)
INCLUDE (MaSan, TenSan, GiaMoiGio, ViTri);
GO

/* ========================= 5. KHUYẾN MÃI / VOUCHER ========================= */
CREATE TABLE dbo.KhuyenMai
(
    KhuyenMaiID    INT IDENTITY(1,1) NOT NULL,
    MaKhuyenMai    NVARCHAR(20) NOT NULL,
    TenKhuyenMai   NVARCHAR(150) NOT NULL,
    MoTa           NVARCHAR(300) NULL,
    PhanTramGiam   DECIMAL(5,2) NOT NULL,
    NgayBatDau     DATETIME2(0) NOT NULL,
    NgayKetThuc    DATETIME2(0) NOT NULL,
    DangHoatDong   BIT NOT NULL CONSTRAINT DF_KhuyenMai_DangHoatDong DEFAULT (1),

    CONSTRAINT PK_KhuyenMai PRIMARY KEY (KhuyenMaiID),
    CONSTRAINT UQ_KhuyenMai_MaKhuyenMai UNIQUE (MaKhuyenMai),
    CONSTRAINT CK_KhuyenMai_PhanTramGiam CHECK (PhanTramGiam > 0 AND PhanTramGiam <= 100),
    CONSTRAINT CK_KhuyenMai_ThoiGian CHECK (NgayKetThuc > NgayBatDau)
);
GO

CREATE INDEX IX_KhuyenMai_HieuLuc
ON dbo.KhuyenMai(DangHoatDong, NgayBatDau, NgayKetThuc, PhanTramGiam DESC)
INCLUDE (MaKhuyenMai, TenKhuyenMai);
GO

CREATE TABLE dbo.PhieuGiamGia
(
    PhieuGiamGiaID    INT IDENTITY(1,1) NOT NULL,
    MaPhieuGiamGia    NVARCHAR(20) NOT NULL,
    TenPhieuGiamGia   NVARCHAR(150) NOT NULL,
    LoaiGiamGia       NVARCHAR(10) NOT NULL,
    GiaTriGiam        DECIMAL(14,2) NOT NULL,
    MucGiamToiDa      DECIMAL(14,2) NULL,
    DonToiThieu       DECIMAL(14,2) NOT NULL CONSTRAINT DF_PhieuGiamGia_DonToiThieu DEFAULT (0),
    NgayBatDau        DATETIME2(0) NOT NULL,
    NgayKetThuc       DATETIME2(0) NOT NULL,
    SoLuong           INT NULL,
    DangHoatDong      BIT NOT NULL CONSTRAINT DF_PhieuGiamGia_DangHoatDong DEFAULT (1),

    CONSTRAINT PK_PhieuGiamGia PRIMARY KEY (PhieuGiamGiaID),
    CONSTRAINT UQ_PhieuGiamGia_MaPhieuGiamGia UNIQUE (MaPhieuGiamGia),
    CONSTRAINT CK_PhieuGiamGia_Loai CHECK (LoaiGiamGia IN (N'Percent', N'Fixed')),
    CONSTRAINT CK_PhieuGiamGia_GiaTri CHECK
    (
        GiaTriGiam > 0
        AND (LoaiGiamGia <> N'Percent' OR GiaTriGiam <= 100)
    ),
    CONSTRAINT CK_PhieuGiamGia_MucGiamToiDa CHECK (MucGiamToiDa IS NULL OR MucGiamToiDa >= 0),
    CONSTRAINT CK_PhieuGiamGia_DonToiThieu CHECK (DonToiThieu >= 0),
    CONSTRAINT CK_PhieuGiamGia_SoLuong CHECK (SoLuong IS NULL OR SoLuong >= 0),
    CONSTRAINT CK_PhieuGiamGia_ThoiGian CHECK (NgayKetThuc > NgayBatDau)
);
GO

CREATE INDEX IX_PhieuGiamGia_HieuLuc
ON dbo.PhieuGiamGia(DangHoatDong, NgayBatDau, NgayKetThuc)
INCLUDE (MaPhieuGiamGia, TenPhieuGiamGia, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, DonToiThieu, SoLuong);
GO

/* ========================= 6. ĐẶT SÂN ========================= */
CREATE TABLE dbo.DatSan
(
    DatSanID             INT IDENTITY(1,1) NOT NULL,
    MaDatSan             NVARCHAR(20) NOT NULL,
    KhachHangID          INT NOT NULL,
    SanID                INT NOT NULL,
    ThoiGianBatDau       DATETIME2(0) NOT NULL,
    ThoiGianKetThuc      DATETIME2(0) NOT NULL,
    GiaMoiGio            DECIMAL(14,2) NOT NULL CONSTRAINT DF_DatSan_GiaMoiGio DEFAULT (0),
    TienGoc              DECIMAL(14,2) NOT NULL CONSTRAINT DF_DatSan_TienGoc DEFAULT (0),
    TienGiam             DECIMAL(14,2) NOT NULL CONSTRAINT DF_DatSan_TienGiam DEFAULT (0),
    TongTien             DECIMAL(14,2) NOT NULL CONSTRAINT DF_DatSan_TongTien DEFAULT (0),
    PhieuGiamGiaID       INT NULL,
    KhuyenMaiID          INT NULL,
    TrangThai            NVARCHAR(20) NOT NULL CONSTRAINT DF_DatSan_TrangThai DEFAULT (N'Pending'),
    TrangThaiThanhToan   NVARCHAR(20) NOT NULL CONSTRAINT DF_DatSan_TrangThaiThanhToan DEFAULT (N'Unpaid'),
    GhiChu               NVARCHAR(300) NULL,
    TaiKhoanTaoID        INT NULL,
    NgayTao              DATETIME2(0) NOT NULL CONSTRAINT DF_DatSan_NgayTao DEFAULT (SYSDATETIME()),
    NgayCapNhat          DATETIME2(0) NULL,

    CONSTRAINT PK_DatSan PRIMARY KEY (DatSanID),
    CONSTRAINT UQ_DatSan_MaDatSan UNIQUE (MaDatSan),
    CONSTRAINT FK_DatSan_KhachHang FOREIGN KEY (KhachHangID) REFERENCES dbo.KhachHang(KhachHangID),
    CONSTRAINT FK_DatSan_SanTheThao FOREIGN KEY (SanID) REFERENCES dbo.SanTheThao(SanID),
    CONSTRAINT FK_DatSan_PhieuGiamGia FOREIGN KEY (PhieuGiamGiaID) REFERENCES dbo.PhieuGiamGia(PhieuGiamGiaID),
    CONSTRAINT FK_DatSan_KhuyenMai FOREIGN KEY (KhuyenMaiID) REFERENCES dbo.KhuyenMai(KhuyenMaiID) ON DELETE SET NULL,
    CONSTRAINT FK_DatSan_TaiKhoanTao FOREIGN KEY (TaiKhoanTaoID) REFERENCES dbo.TaiKhoan(TaiKhoanID),
    CONSTRAINT CK_DatSan_ThoiGian CHECK
    (
        ThoiGianKetThuc > ThoiGianBatDau
        AND CONVERT(DATE,ThoiGianBatDau) = CONVERT(DATE,ThoiGianKetThuc)
        AND DATEDIFF(MINUTE,ThoiGianBatDau,ThoiGianKetThuc) >= 30
    ),
    CONSTRAINT CK_DatSan_Tien CHECK
    (
        GiaMoiGio >= 0
        AND TienGoc >= 0
        AND TienGiam >= 0
        AND TongTien >= 0
        AND TienGiam <= TienGoc
        AND TongTien = TienGoc - TienGiam
    ),
    CONSTRAINT CK_DatSan_TrangThai CHECK
        (TrangThai IN (N'Pending', N'Confirmed', N'InUse', N'Completed', N'Cancelled')),
    CONSTRAINT CK_DatSan_TrangThaiThanhToan CHECK
        (TrangThaiThanhToan IN (N'Unpaid', N'PartiallyPaid', N'Paid'))
);
GO

-- Index phục vụ đúng câu query GetAvailableFields/CreateBooking trong BookingService.
CREATE INDEX IX_DatSan_San_ThoiGian
ON dbo.DatSan(SanID, ThoiGianBatDau, ThoiGianKetThuc, TrangThai)
INCLUDE (KhachHangID, MaDatSan, TongTien, TrangThaiThanhToan, NgayTao);
GO

CREATE INDEX IX_DatSan_KhachHang_ThoiGian
ON dbo.DatSan(KhachHangID, ThoiGianBatDau DESC)
INCLUDE (SanID, MaDatSan, ThoiGianKetThuc, TongTien, TrangThai, TrangThaiThanhToan, NgayTao);
GO

CREATE INDEX IX_DatSan_TheoNgay_TrangThai
ON dbo.DatSan(ThoiGianBatDau, TrangThai)
INCLUDE (SanID, KhachHangID, ThoiGianKetThuc, TongTien, TrangThaiThanhToan);
GO

/* ========================= 7. VOUCHER CỦA KHÁCH ========================= */
CREATE TABLE dbo.PhieuGiamGiaKhachHang
(
    PhieuGiamGiaKhachHangID   INT IDENTITY(1,1) NOT NULL,
    PhieuGiamGiaID            INT NOT NULL,
    KhachHangID               INT NOT NULL,
    NgayNhan                  DATETIME2(0) NOT NULL CONSTRAINT DF_PGGKH_NgayNhan DEFAULT (SYSDATETIME()),
    DaSuDung                  BIT NOT NULL CONSTRAINT DF_PGGKH_DaSuDung DEFAULT (0),
    NgaySuDung                DATETIME2(0) NULL,
    DatSanSuDungID            INT NULL,

    CONSTRAINT PK_PhieuGiamGiaKhachHang PRIMARY KEY (PhieuGiamGiaKhachHangID),
    CONSTRAINT UQ_PhieuGiamGiaKhachHang UNIQUE (PhieuGiamGiaID, KhachHangID),
    CONSTRAINT FK_PGGKH_PhieuGiamGia FOREIGN KEY (PhieuGiamGiaID) REFERENCES dbo.PhieuGiamGia(PhieuGiamGiaID),
    CONSTRAINT FK_PGGKH_KhachHang FOREIGN KEY (KhachHangID) REFERENCES dbo.KhachHang(KhachHangID),
    CONSTRAINT FK_PGGKH_DatSan FOREIGN KEY (DatSanSuDungID) REFERENCES dbo.DatSan(DatSanID),
    CONSTRAINT CK_PGGKH_SuDung CHECK
    (
        (DaSuDung = 0 AND NgaySuDung IS NULL AND DatSanSuDungID IS NULL)
        OR
        (DaSuDung = 1 AND NgaySuDung IS NOT NULL AND DatSanSuDungID IS NOT NULL)
    )
);
GO

CREATE INDEX IX_PGGKH_KhachHang_DaSuDung
ON dbo.PhieuGiamGiaKhachHang(KhachHangID, DaSuDung)
INCLUDE (PhieuGiamGiaID, NgayNhan, NgaySuDung, DatSanSuDungID);
GO

/* ========================= 8. HÓA ĐƠN / THANH TOÁN ========================= */
CREATE TABLE dbo.HoaDon
(
    HoaDonID              INT IDENTITY(1,1) NOT NULL,
    MaHoaDon              NVARCHAR(20) NOT NULL,
    DatSanID              INT NOT NULL,
    KhachHangID           INT NOT NULL,
    TienGoc               DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_TienGoc DEFAULT (0),
    TienGiam              DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_TienGiam DEFAULT (0),
    TongTien              DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_TongTien DEFAULT (0),
    SoTienDaTra           DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_SoTienDaTra DEFAULT (0),
    TrangThaiThanhToan    NVARCHAR(20) NOT NULL CONSTRAINT DF_HoaDon_TrangThaiThanhToan DEFAULT (N'Unpaid'),
    NgayTao               DATETIME2(0) NOT NULL CONSTRAINT DF_HoaDon_NgayTao DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_HoaDon PRIMARY KEY (HoaDonID),
    CONSTRAINT UQ_HoaDon_MaHoaDon UNIQUE (MaHoaDon),
    CONSTRAINT UQ_HoaDon_DatSanID UNIQUE (DatSanID),
    CONSTRAINT FK_HoaDon_DatSan FOREIGN KEY (DatSanID) REFERENCES dbo.DatSan(DatSanID),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (KhachHangID) REFERENCES dbo.KhachHang(KhachHangID),
    CONSTRAINT CK_HoaDon_Tien CHECK
    (
        TienGoc >= 0
        AND TienGiam >= 0
        AND TongTien >= 0
        AND SoTienDaTra >= 0
        AND TienGiam <= TienGoc
        AND TongTien = TienGoc - TienGiam
        AND SoTienDaTra <= TongTien
    ),
    CONSTRAINT CK_HoaDon_TrangThaiThanhToan CHECK
        (TrangThaiThanhToan IN (N'Unpaid', N'PartiallyPaid', N'Paid')),
    CONSTRAINT CK_HoaDon_TrangThai_KhopSoTien CHECK
    (
        (SoTienDaTra = 0 AND TrangThaiThanhToan = N'Unpaid')
        OR
        (SoTienDaTra > 0 AND SoTienDaTra < TongTien AND TrangThaiThanhToan = N'PartiallyPaid')
        OR
        (SoTienDaTra = TongTien AND TrangThaiThanhToan = N'Paid')
    )
);
GO

CREATE INDEX IX_HoaDon_KhachHang_TrangThai
ON dbo.HoaDon(KhachHangID, TrangThaiThanhToan, NgayTao DESC)
INCLUDE (DatSanID, MaHoaDon, TongTien, SoTienDaTra);
GO

CREATE INDEX IX_HoaDon_TrangThai_NgayTao
ON dbo.HoaDon(TrangThaiThanhToan, NgayTao DESC)
INCLUDE (KhachHangID, DatSanID, MaHoaDon, TongTien, SoTienDaTra);
GO

CREATE TABLE dbo.ThanhToan
(
    ThanhToanID            INT IDENTITY(1,1) NOT NULL,
    HoaDonID               INT NOT NULL,
    SoTien                 DECIMAL(14,2) NOT NULL,
    PhuongThucThanhToan    NVARCHAR(20) NOT NULL,
    MaGiaoDich             NVARCHAR(100) NULL,
    TrangThai              NVARCHAR(20) NOT NULL CONSTRAINT DF_ThanhToan_TrangThai DEFAULT (N'Pending'),
    NgayThanhToan          DATETIME2(0) NULL,
    NgayTao                DATETIME2(0) NOT NULL CONSTRAINT DF_ThanhToan_NgayTao DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_ThanhToan PRIMARY KEY (ThanhToanID),
    CONSTRAINT FK_ThanhToan_HoaDon FOREIGN KEY (HoaDonID) REFERENCES dbo.HoaDon(HoaDonID),
    CONSTRAINT CK_ThanhToan_SoTien CHECK (SoTien > 0),
    CONSTRAINT CK_ThanhToan_PhuongThuc CHECK
        (PhuongThucThanhToan IN (N'Cash', N'QR', N'BankTransfer')),
    CONSTRAINT CK_ThanhToan_TrangThai CHECK
        (TrangThai IN (N'Pending', N'Confirmed', N'Cancelled')),
    CONSTRAINT CK_ThanhToan_NgayThanhToan CHECK
    (
        (TrangThai = N'Confirmed' AND NgayThanhToan IS NOT NULL)
        OR
        (TrangThai IN (N'Pending', N'Cancelled') AND NgayThanhToan IS NULL)
    )
);
GO

CREATE INDEX IX_ThanhToan_HoaDon_TrangThai
ON dbo.ThanhToan(HoaDonID, TrangThai, NgayTao DESC)
INCLUDE (SoTien, PhuongThucThanhToan, MaGiaoDich, NgayThanhToan);
GO

CREATE INDEX IX_ThanhToan_ThongKeNgay
ON dbo.ThanhToan(TrangThai, NgayThanhToan)
INCLUDE (SoTien, HoaDonID, PhuongThucThanhToan);
GO

-- Một hóa đơn chỉ có tối đa 1 giao dịch Pending tại một thời điểm.
CREATE UNIQUE INDEX UX_ThanhToan_MotGiaoDichCho
ON dbo.ThanhToan(HoaDonID)
WHERE TrangThai = N'Pending';
GO

/* ========================= 9. CẤU HÌNH HỆ THỐNG ========================= */
CREATE TABLE dbo.CauHinhHeThong
(
    KhoaCauHinh       NVARCHAR(50) NOT NULL,
    GiaTriCauHinh     NVARCHAR(300) NOT NULL,
    NgayCapNhat       DATETIME2(0) NOT NULL CONSTRAINT DF_CauHinhHeThong_NgayCapNhat DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_CauHinhHeThong PRIMARY KEY (KhoaCauHinh)
);
GO

/* ========================= 10. LỊCH SỬ TRẠNG THÁI ========================= */
-- Bảng lịch sử không làm thay đổi query CRUD hiện có của WinForms.
CREATE TABLE dbo.LichSuDatSan
(
    LichSuDatSanID              BIGINT IDENTITY(1,1) NOT NULL,
    DatSanID                    INT NOT NULL,
    TrangThaiCu                 NVARCHAR(20) NULL,
    TrangThaiMoi                NVARCHAR(20) NOT NULL,
    TrangThaiThanhToanCu        NVARCHAR(20) NULL,
    TrangThaiThanhToanMoi       NVARCHAR(20) NOT NULL,
    ThoiDiem                    DATETIME2(0) NOT NULL CONSTRAINT DF_LichSuDatSan_ThoiDiem DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_LichSuDatSan PRIMARY KEY (LichSuDatSanID),
    CONSTRAINT FK_LichSuDatSan_DatSan FOREIGN KEY (DatSanID) REFERENCES dbo.DatSan(DatSanID)
);
GO

CREATE INDEX IX_LichSuDatSan_DatSan_ThoiDiem
ON dbo.LichSuDatSan(DatSanID, ThoiDiem DESC);
GO

/* ========================= 11. TRIGGER BẢO VỆ DỮ LIỆU ========================= */

-- 11.1: Tài khoản gắn với KhachHang phải có role Customer.
CREATE OR ALTER TRIGGER dbo.TR_KhachHang_DungVaiTro
ON dbo.KhachHang
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.TaiKhoan u ON u.TaiKhoanID = i.TaiKhoanID
        JOIN dbo.VaiTro r ON r.VaiTroID = u.VaiTroID
        WHERE i.TaiKhoanID IS NOT NULL
          AND r.TenVaiTro <> N'Customer'
    )
        THROW 52001, N'Tài khoản gắn với khách hàng phải có vai trò Customer.', 1;
END;
GO

-- 11.2: Tài khoản gắn với NhanVien phải có role Employee.
CREATE OR ALTER TRIGGER dbo.TR_NhanVien_DungVaiTro
ON dbo.NhanVien
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.TaiKhoan u ON u.TaiKhoanID = i.TaiKhoanID
        JOIN dbo.VaiTro r ON r.VaiTroID = u.VaiTroID
        WHERE i.TaiKhoanID IS NOT NULL
          AND r.TenVaiTro <> N'Employee'
    )
        THROW 52002, N'Tài khoản gắn với nhân viên phải có vai trò Employee.', 1;
END;
GO

-- 11.3: Không cho đổi vai trò tài khoản làm sai liên kết hồ sơ hiện có.
CREATE OR ALTER TRIGGER dbo.TR_TaiKhoan_BaoVeVaiTroLienKet
ON dbo.TaiKhoan
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(VaiTroID) RETURN;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.VaiTro r ON r.VaiTroID=i.VaiTroID
        LEFT JOIN dbo.KhachHang c ON c.TaiKhoanID=i.TaiKhoanID
        LEFT JOIN dbo.NhanVien e ON e.TaiKhoanID=i.TaiKhoanID
        WHERE (c.KhachHangID IS NOT NULL AND r.TenVaiTro<>N'Customer')
           OR (e.NhanVienID IS NOT NULL AND r.TenVaiTro<>N'Employee')
    )
        THROW 52009, N'Không thể đổi vai trò vì tài khoản đã liên kết với hồ sơ khách hàng/nhân viên.', 1;
END;
GO

-- 11.4: Bảo vệ các cấu hình mà nghiệp vụ đặt sân và VietQR phụ thuộc.
CREATE OR ALTER TRIGGER dbo.TR_CauHinhHeThong_KiemTraGiaTri
ON dbo.CauHinhHeThong
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE KhoaCauHinh=N'DatabaseVersion' AND GiaTriCauHinh<>N'4.0 PRO')
        THROW 52010, N'DatabaseVersion phải giữ nguyên 4.0 PRO để đồng bộ với ứng dụng.', 1;

    IF EXISTS
    (
        SELECT 1 FROM inserted
        WHERE KhoaCauHinh IN (N'OpenTime',N'CloseTime')
          AND TRY_CONVERT(TIME(0),GiaTriCauHinh) IS NULL
    )
        THROW 52011, N'OpenTime/CloseTime phải có định dạng HH:mm hợp lệ.', 1;

    IF EXISTS
    (
        SELECT 1 FROM inserted
        WHERE (KhoaCauHinh=N'MinimumBookingMinutes'
               AND (TRY_CONVERT(INT,GiaTriCauHinh) IS NULL OR TRY_CONVERT(INT,GiaTriCauHinh)<30))
           OR (KhoaCauHinh=N'CustomerCancellationHours'
               AND (TRY_CONVERT(INT,GiaTriCauHinh) IS NULL OR TRY_CONVERT(INT,GiaTriCauHinh)<=0))
    )
        THROW 52012, N'MinimumBookingMinutes phải >= 30; CustomerCancellationHours phải là số nguyên dương.', 1;

    IF EXISTS
    (
        SELECT 1 FROM inserted
        WHERE KhoaCauHinh IN (N'BankBin',N'BankAccountNumber')
          AND (GiaTriCauHinh=N'' OR GiaTriCauHinh LIKE N'%[^0-9]%')
    )
        THROW 52013, N'BankBin và BankAccountNumber chỉ được chứa chữ số.', 1;

    DECLARE @Open TIME(0)=TRY_CONVERT(TIME(0),(SELECT GiaTriCauHinh FROM dbo.CauHinhHeThong WHERE KhoaCauHinh=N'OpenTime'));
    DECLARE @Close TIME(0)=TRY_CONVERT(TIME(0),(SELECT GiaTriCauHinh FROM dbo.CauHinhHeThong WHERE KhoaCauHinh=N'CloseTime'));
    IF @Open IS NOT NULL AND @Close IS NOT NULL AND @Close<=@Open
        THROW 52014, N'CloseTime phải lớn hơn OpenTime.', 1;
END;
GO

-- 11.5: Bảo vệ ở tầng DB khỏi 2 lịch hoạt động chồng nhau trên cùng sân.
-- BookingService đã dùng Serializable + UPDLOCK/HOLDLOCK; trigger này là lớp
-- bảo vệ thứ hai nếu sau này có câu INSERT/UPDATE từ nơi khác.
CREATE OR ALTER TRIGGER dbo.TR_DatSan_KhongTrungLich
ON dbo.DatSan
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.DatSan d WITH (UPDLOCK, HOLDLOCK)
          ON d.SanID = i.SanID
         AND d.DatSanID <> i.DatSanID
         AND i.ThoiGianBatDau < d.ThoiGianKetThuc
         AND i.ThoiGianKetThuc > d.ThoiGianBatDau
        WHERE i.TrangThai NOT IN (N'Cancelled', N'Completed')
          AND d.TrangThai NOT IN (N'Cancelled', N'Completed')
    )
        THROW 52003, N'Sân đã có lịch đặt trùng trong khung giờ này.', 1;
END;
GO

-- 11.6: Quản lý tồn voucher khi cấp/xóa quyền sở hữu voucher.
CREATE OR ALTER TRIGGER dbo.TR_PGGKH_TruSoLuongVoucher
ON dbo.PhieuGiamGiaKhachHang
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PhieuGiamGia v WITH (UPDLOCK, HOLDLOCK)
        JOIN
        (
            SELECT PhieuGiamGiaID, COUNT(*) AS SoLuongCap
            FROM inserted
            GROUP BY PhieuGiamGiaID
        ) x ON x.PhieuGiamGiaID = v.PhieuGiamGiaID
        WHERE v.SoLuong IS NOT NULL
          AND v.SoLuong < x.SoLuongCap
    )
        THROW 52004, N'Voucher đã hết số lượng để cấp.', 1;

    UPDATE v
       SET v.SoLuong = v.SoLuong - x.SoLuongCap
    FROM dbo.PhieuGiamGia v
    JOIN
    (
        SELECT PhieuGiamGiaID, COUNT(*) AS SoLuongCap
        FROM inserted
        GROUP BY PhieuGiamGiaID
    ) x ON x.PhieuGiamGiaID = v.PhieuGiamGiaID
    WHERE v.SoLuong IS NOT NULL;
END;
GO

CREATE OR ALTER TRIGGER dbo.TR_PGGKH_HoanSoLuongVoucher
ON dbo.PhieuGiamGiaKhachHang
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE v
       SET v.SoLuong = v.SoLuong + x.SoLuongTra
    FROM dbo.PhieuGiamGia v
    JOIN
    (
        SELECT PhieuGiamGiaID, COUNT(*) AS SoLuongTra
        FROM deleted
        WHERE DaSuDung = 0
        GROUP BY PhieuGiamGiaID
    ) x ON x.PhieuGiamGiaID = v.PhieuGiamGiaID
    WHERE v.SoLuong IS NOT NULL;
END;
GO

-- 11.8: Chặn giao dịch vượt số tiền còn lại / thanh toán đơn đã hủy.
CREATE OR ALTER TRIGGER dbo.TR_ThanhToan_KiemTraSoTien
ON dbo.ThanhToan
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.HoaDon h ON h.HoaDonID = i.HoaDonID
        JOIN dbo.DatSan b ON b.DatSanID = h.DatSanID
        WHERE i.TrangThai IN (N'Pending', N'Confirmed')
          AND b.TrangThai = N'Cancelled'
    )
        THROW 52005, N'Không thể thanh toán cho lịch đặt đã hủy.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.HoaDon h ON h.HoaDonID = i.HoaDonID
        WHERE i.TrangThai IN (N'Pending', N'Confirmed')
          AND i.SoTien > (h.TongTien - h.SoTienDaTra)
    )
        THROW 52006, N'Số tiền giao dịch vượt số tiền còn phải thanh toán.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN dbo.HoaDon h ON h.HoaDonID = i.HoaDonID
        CROSS APPLY
        (
            SELECT ISNULL(SUM(p.SoTien), 0) AS TongDaXacNhan
            FROM dbo.ThanhToan p WITH (UPDLOCK, HOLDLOCK)
            WHERE p.HoaDonID = i.HoaDonID
              AND p.TrangThai = N'Confirmed'
        ) x
        WHERE x.TongDaXacNhan > h.TongTien
    )
        THROW 52007, N'Tổng giao dịch đã xác nhận vượt tổng tiền hóa đơn.', 1;
END;
GO

-- 11.9: Đồng bộ trạng thái thanh toán HoaDon -> DatSan và hủy các giao dịch
-- Pending cũ khi hóa đơn đã được trả đủ.
CREATE OR ALTER TRIGGER dbo.TR_HoaDon_DongBoTrangThaiThanhToan
ON dbo.HoaDon
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(TrangThaiThanhToan) OR UPDATE(SoTienDaTra)
    BEGIN
        UPDATE b
           SET b.TrangThaiThanhToan = i.TrangThaiThanhToan,
               b.NgayCapNhat = SYSDATETIME()
        FROM dbo.DatSan b
        JOIN inserted i ON i.DatSanID = b.DatSanID;

        UPDATE p
           SET p.TrangThai = N'Cancelled',
               p.NgayThanhToan = NULL
        FROM dbo.ThanhToan p
        JOIN inserted i ON i.HoaDonID = p.HoaDonID
        WHERE i.TrangThaiThanhToan = N'Paid'
          AND p.TrangThai = N'Pending';
    END;
END;
GO

-- 11.10: Khi hóa đơn chuyển sang Paid lần đầu, cộng điểm cho khách (1 điểm / 10.000đ).
CREATE OR ALTER TRIGGER dbo.TR_HoaDon_CongDiemKhiThanhToanDu
ON dbo.HoaDon
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
       SET c.DiemTichLuy = c.DiemTichLuy + CONVERT(INT, FLOOR(i.TongTien / 10000.0))
    FROM dbo.KhachHang c
    JOIN inserted i ON i.KhachHangID=c.KhachHangID
    JOIN deleted d ON d.HoaDonID=i.HoaDonID
    WHERE i.TrangThaiThanhToan=N'Paid' AND d.TrangThaiThanhToan<>N'Paid';
END;
GO

-- 11.11: Khi hủy đơn chưa nhận tiền: hủy yêu cầu thanh toán Pending và trả lại
-- quyền dùng voucher cho chính khách hàng đó.
CREATE OR ALTER TRIGGER dbo.TR_DatSan_XuLyKhiHuy
ON dbo.DatSan
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Huy AS
    (
        SELECT i.DatSanID, i.KhachHangID, i.PhieuGiamGiaID
        FROM inserted i
        JOIN deleted d ON d.DatSanID = i.DatSanID
        WHERE i.TrangThai = N'Cancelled'
          AND d.TrangThai <> N'Cancelled'
    )
    UPDATE p
       SET p.TrangThai = N'Cancelled',
           p.NgayThanhToan = NULL
    FROM dbo.ThanhToan p
    JOIN dbo.HoaDon h ON h.HoaDonID = p.HoaDonID
    JOIN Huy x ON x.DatSanID = h.DatSanID
    WHERE p.TrangThai = N'Pending';

    ;WITH Huy AS
    (
        SELECT i.DatSanID, i.KhachHangID, i.PhieuGiamGiaID
        FROM inserted i
        JOIN deleted d ON d.DatSanID = i.DatSanID
        WHERE i.TrangThai = N'Cancelled'
          AND d.TrangThai <> N'Cancelled'
    )
    UPDATE cv
       SET cv.DaSuDung = 0,
           cv.NgaySuDung = NULL,
           cv.DatSanSuDungID = NULL
    FROM dbo.PhieuGiamGiaKhachHang cv
    JOIN Huy x
      ON x.PhieuGiamGiaID = cv.PhieuGiamGiaID
     AND x.KhachHangID = cv.KhachHangID
     AND x.DatSanID = cv.DatSanSuDungID
    JOIN dbo.HoaDon h ON h.DatSanID = x.DatSanID
    WHERE h.SoTienDaTra = 0;
END;
GO

-- 11.12: Không cho hủy trực tiếp đơn đã thu tiền. Ứng dụng hiện chưa có nghiệp vụ hoàn tiền,
-- nên chặn hủy để không làm lệch doanh thu / trạng thái hóa đơn.
CREATE OR ALTER TRIGGER dbo.TR_DatSan_KhongHuyDonDaThuTien
ON dbo.DatSan
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON d.DatSanID=i.DatSanID
        JOIN dbo.HoaDon h ON h.DatSanID=i.DatSanID
        WHERE i.TrangThai=N'Cancelled' AND d.TrangThai<>N'Cancelled' AND h.SoTienDaTra>0
    )
        THROW 52008, N'Đơn đã nhận tiền. Cần xử lý hoàn tiền trước khi hủy.', 1;
END;
GO

-- 11.13: Ghi lịch sử khi trạng thái đơn hoặc trạng thái thanh toán thay đổi.
CREATE OR ALTER TRIGGER dbo.TR_DatSan_GhiLichSu
ON dbo.DatSan
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.LichSuDatSan
    (
        DatSanID,
        TrangThaiCu,
        TrangThaiMoi,
        TrangThaiThanhToanCu,
        TrangThaiThanhToanMoi
    )
    SELECT
        i.DatSanID,
        d.TrangThai,
        i.TrangThai,
        d.TrangThaiThanhToan,
        i.TrangThaiThanhToan
    FROM inserted i
    LEFT JOIN deleted d ON d.DatSanID = i.DatSanID
    WHERE d.DatSanID IS NULL
       OR ISNULL(d.TrangThai, N'') <> ISNULL(i.TrangThai, N'')
       OR ISNULL(d.TrangThaiThanhToan, N'') <> ISNULL(i.TrangThaiThanhToan, N'');
END;
GO

-- 11.14: Đồng bộ trạng thái sân theo các lượt đang sử dụng; vẫn giữ nguyên sân
-- bảo trì/ngừng hoạt động do quản trị viên thiết lập.
CREATE OR ALTER TRIGGER dbo.TR_DatSan_DongBoTrangThaiSan
ON dbo.DatSan
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH SanBiAnhHuong AS
    (
        SELECT SanID FROM inserted
        UNION
        SELECT SanID FROM deleted
    )
    UPDATE f
       SET TrangThai = CASE
            WHEN f.TrangThai IN (N'Maintenance',N'Inactive') THEN f.TrangThai
            WHEN EXISTS(SELECT 1 FROM dbo.DatSan b WHERE b.SanID=f.SanID AND b.TrangThai=N'InUse') THEN N'Busy'
            ELSE N'Available' END
    FROM dbo.SanTheThao f
    JOIN SanBiAnhHuong a ON a.SanID=f.SanID;
END;
GO

/* ========================= 12. VIEW HỖ TRỢ KIỂM TRA / BÁO CÁO ========================= */
CREATE OR ALTER VIEW dbo.vw_LichDatSanChiTiet
AS
SELECT
    b.DatSanID,
    b.MaDatSan,
    c.MaKhachHang,
    c.HoTen AS KhachHang,
    f.MaSan,
    f.TenSan,
    ft.TenLoaiSan,
    b.ThoiGianBatDau,
    b.ThoiGianKetThuc,
    b.TienGoc,
    b.TienGiam,
    b.TongTien,
    b.TrangThai,
    b.TrangThaiThanhToan,
    b.NgayTao,
    b.NgayCapNhat
FROM dbo.DatSan b
JOIN dbo.KhachHang c ON c.KhachHangID = b.KhachHangID
JOIN dbo.SanTheThao f ON f.SanID = b.SanID
JOIN dbo.LoaiSan ft ON ft.LoaiSanID = f.LoaiSanID;
GO

CREATE OR ALTER VIEW dbo.vw_HoaDonChiTiet
AS
SELECT
    i.HoaDonID,
    i.MaHoaDon,
    b.DatSanID,
    b.MaDatSan,
    c.MaKhachHang,
    c.HoTen AS KhachHang,
    f.TenSan,
    i.TienGoc,
    i.TienGiam,
    i.TongTien,
    i.SoTienDaTra,
    i.TongTien - i.SoTienDaTra AS SoTienConLai,
    i.TrangThaiThanhToan,
    i.NgayTao
FROM dbo.HoaDon i
JOIN dbo.DatSan b ON b.DatSanID = i.DatSanID
JOIN dbo.KhachHang c ON c.KhachHangID = i.KhachHangID
JOIN dbo.SanTheThao f ON f.SanID = b.SanID;
GO

/* ========================= 13. STORED PROCEDURE TIỆN ÍCH ========================= */
CREATE OR ALTER PROCEDURE dbo.usp_KiemTraSanTrong
    @SanID INT,
    @ThoiGianBatDau DATETIME2(0),
    @ThoiGianKetThuc DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ThoiGianKetThuc <= @ThoiGianBatDau
        THROW 52101, N'Giờ kết thúc phải sau giờ bắt đầu.', 1;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.DatSan b
        WHERE b.SanID = @SanID
          AND b.TrangThai NOT IN (N'Cancelled', N'Completed')
          AND @ThoiGianBatDau < b.ThoiGianKetThuc
          AND @ThoiGianKetThuc > b.ThoiGianBatDau
    ) THEN 0 ELSE 1 END AS BIT) AS SanTrong;
END;
GO

-- Procedure dùng để kiểm tra nhanh database sau cài đặt.
CREATE OR ALTER PROCEDURE dbo.usp_KiemTraCSDL
    @DungNeuCoLoi BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @KQ TABLE
    (
        HangMuc NVARCHAR(200) NOT NULL,
        KetQua NVARCHAR(20) NOT NULL,
        ChiTiet NVARCHAR(500) NULL
    );

    INSERT INTO @KQ
    SELECT N'13 bảng WinForms bắt buộc',
           CASE WHEN COUNT(*) = 13 THEN N'PASS' ELSE N'FAIL' END,
           N'Tìm thấy ' + CAST(COUNT(*) AS NVARCHAR(20)) + N'/13 bảng'
    FROM sys.tables
    WHERE name IN
    (
        N'VaiTro', N'TaiKhoan', N'KhachHang', N'NhanVien', N'LoaiSan', N'SanTheThao',
        N'KhuyenMai', N'PhieuGiamGia', N'DatSan', N'PhieuGiamGiaKhachHang',
        N'HoaDon', N'ThanhToan', N'CauHinhHeThong'
    )
      AND schema_id=SCHEMA_ID(N'dbo');

    INSERT INTO @KQ
    SELECT N'3 vai trò bắt buộc',
           CASE WHEN COUNT(*) = 3 THEN N'PASS' ELSE N'FAIL' END,
           N'Admin / Employee / Customer'
    FROM dbo.VaiTro
    WHERE TenVaiTro IN (N'Admin', N'Employee', N'Customer');

    INSERT INTO @KQ
    SELECT N'Tài khoản demo',
           CASE WHEN COUNT(*) = 3 THEN N'PASS' ELSE N'FAIL' END,
           N'admin / staff / customer'
    FROM dbo.TaiKhoan
    WHERE TenDangNhap IN (N'admin', N'staff', N'customer');

    INSERT INTO @KQ
    SELECT N'Phiên bản CSDL',
           CASE WHEN EXISTS(SELECT 1 FROM dbo.CauHinhHeThong WHERE KhoaCauHinh=N'DatabaseVersion' AND GiaTriCauHinh=N'4.0 PRO') THEN N'PASS' ELSE N'FAIL' END,
           ISNULL((SELECT GiaTriCauHinh FROM dbo.CauHinhHeThong WHERE KhoaCauHinh=N'DatabaseVersion'),N'Không có DatabaseVersion');

    INSERT INTO @KQ
    SELECT N'Khóa ngoại',
           CASE WHEN COUNT(*) >= 15 THEN N'PASS' ELSE N'FAIL' END,
           N'Số FK: ' + CAST(COUNT(*) AS NVARCHAR(20))
    FROM sys.foreign_keys
    WHERE parent_object_id IN (SELECT object_id FROM sys.tables WHERE schema_id = SCHEMA_ID(N'dbo'));

    INSERT INTO @KQ
    SELECT N'Trigger bảo vệ V4',
           CASE WHEN COUNT(*) >= 14 THEN N'PASS' ELSE N'FAIL' END,
           N'Số trigger: ' + CAST(COUNT(*) AS NVARCHAR(20))
    FROM sys.triggers
    WHERE parent_id <> 0
      AND is_disabled = 0;

    DECLARE @TriggerBatBuoc TABLE(Ten SYSNAME NOT NULL PRIMARY KEY);
    INSERT INTO @TriggerBatBuoc(Ten) VALUES
    (N'TR_KhachHang_DungVaiTro'),(N'TR_NhanVien_DungVaiTro'),(N'TR_TaiKhoan_BaoVeVaiTroLienKet'),
    (N'TR_CauHinhHeThong_KiemTraGiaTri'),(N'TR_DatSan_KhongTrungLich'),
    (N'TR_PGGKH_TruSoLuongVoucher'),(N'TR_PGGKH_HoanSoLuongVoucher'),
    (N'TR_ThanhToan_KiemTraSoTien'),(N'TR_HoaDon_DongBoTrangThaiThanhToan'),
    (N'TR_HoaDon_CongDiemKhiThanhToanDu'),(N'TR_DatSan_XuLyKhiHuy'),
    (N'TR_DatSan_KhongHuyDonDaThuTien'),(N'TR_DatSan_GhiLichSu'),
    (N'TR_DatSan_DongBoTrangThaiSan');

    INSERT INTO @KQ
    SELECT N'Đủ trigger nghiệp vụ bắt buộc',
           CASE WHEN COUNT(*)=0 THEN N'PASS' ELSE N'FAIL' END,
           CASE WHEN COUNT(*)=0 THEN N'Đủ 14 trigger nghiệp vụ'
                ELSE N'Thiếu ' + CAST(COUNT(*) AS NVARCHAR(20)) + N' trigger' END
    FROM @TriggerBatBuoc x
    WHERE NOT EXISTS
    (
        SELECT 1 FROM sys.triggers t
        WHERE t.name=x.Ten AND t.parent_id<>0 AND t.is_disabled=0
    );

    INSERT INTO @KQ
    SELECT N'Kiểm tra constraint dữ liệu hiện tại',
           CASE WHEN EXISTS
           (
               SELECT 1
               FROM sys.check_constraints cc
               WHERE cc.is_disabled = 1 OR cc.is_not_trusted = 1
           ) THEN N'WARN' ELSE N'PASS' END,
           N'CHECK constraints đang bật và trusted';

    DECLARE @CotBatBuoc TABLE(Bang SYSNAME NOT NULL, Cot SYSNAME NOT NULL);
    INSERT INTO @CotBatBuoc(Bang,Cot) VALUES
    (N'TaiKhoan',N'TenDangNhap'),(N'TaiKhoan',N'MatKhauBam'),(N'TaiKhoan',N'MuoiMatKhau'),(N'TaiKhoan',N'VaiTroID'),
    (N'KhachHang',N'TaiKhoanID'),(N'KhachHang',N'DiemTichLuy'),(N'NhanVien',N'TaiKhoanID'),
    (N'SanTheThao',N'GiaMoiGio'),(N'SanTheThao',N'TrangThai'),
    (N'DatSan',N'ThoiGianBatDau'),(N'DatSan',N'ThoiGianKetThuc'),(N'DatSan',N'TongTien'),(N'DatSan',N'TrangThaiThanhToan'),
    (N'PhieuGiamGiaKhachHang',N'DatSanSuDungID'),
    (N'HoaDon',N'SoTienDaTra'),(N'HoaDon',N'TrangThaiThanhToan'),
    (N'ThanhToan',N'PhuongThucThanhToan'),(N'ThanhToan',N'TrangThai'),
    (N'CauHinhHeThong',N'GiaTriCauHinh');

    INSERT INTO @KQ
    SELECT N'Cột dữ liệu ứng dụng bắt buộc',
           CASE WHEN COUNT(*)=0 THEN N'PASS' ELSE N'FAIL' END,
           CASE WHEN COUNT(*)=0 THEN N'Đủ tất cả cột app đang sử dụng'
                ELSE N'Thiếu ' + CAST(COUNT(*) AS NVARCHAR(20)) + N' cột' END
    FROM @CotBatBuoc x
    WHERE NOT EXISTS
    (
        SELECT 1 FROM sys.tables t JOIN sys.columns c ON c.object_id=t.object_id
        WHERE t.schema_id=SCHEMA_ID(N'dbo') AND t.name=x.Bang AND c.name=x.Cot
    );

    INSERT INTO @KQ
    SELECT N'Cấu hình bắt buộc',
           CASE WHEN COUNT(*)=11 THEN N'PASS' ELSE N'FAIL' END,
           N'Tìm thấy ' + CAST(COUNT(*) AS NVARCHAR(20)) + N'/11 cấu hình'
    FROM dbo.CauHinhHeThong
    WHERE KhoaCauHinh IN
    (N'DatabaseVersion',N'CompanyName',N'BankName',N'BankBin',N'BankAccountNumber',N'BankAccountName',
     N'QrTransferPrefix',N'OpenTime',N'CloseTime',N'MinimumBookingMinutes',N'CustomerCancellationHours');

    INSERT INTO @KQ
    SELECT N'Liên kết tài khoản đúng vai trò',
           CASE WHEN EXISTS
           (
               SELECT 1 FROM dbo.TaiKhoan u JOIN dbo.VaiTro r ON r.VaiTroID=u.VaiTroID
               LEFT JOIN dbo.KhachHang c ON c.TaiKhoanID=u.TaiKhoanID
               LEFT JOIN dbo.NhanVien e ON e.TaiKhoanID=u.TaiKhoanID
               WHERE (c.KhachHangID IS NOT NULL AND r.TenVaiTro<>N'Customer')
                  OR (e.NhanVienID IS NOT NULL AND r.TenVaiTro<>N'Employee')
           ) THEN N'FAIL' ELSE N'PASS' END,
           N'Customer ↔ KhachHang; Employee ↔ NhanVien';

    INSERT INTO @KQ
    SELECT N'Không có lịch sân đang hoạt động bị trùng',
           CASE WHEN EXISTS
           (
               SELECT 1 FROM dbo.DatSan a JOIN dbo.DatSan b
                 ON a.SanID=b.SanID AND a.DatSanID<b.DatSanID
                AND a.ThoiGianBatDau<b.ThoiGianKetThuc AND a.ThoiGianKetThuc>b.ThoiGianBatDau
               WHERE a.TrangThai NOT IN (N'Cancelled',N'Completed')
                 AND b.TrangThai NOT IN (N'Cancelled',N'Completed')
           ) THEN N'FAIL' ELSE N'PASS' END,
           N'Kiểm tra toàn bộ lịch chưa kết thúc';

    INSERT INTO @KQ
    SELECT N'Số tiền hóa đơn khớp giao dịch xác nhận',
           CASE WHEN EXISTS
           (
               SELECT 1 FROM dbo.HoaDon h
               OUTER APPLY
               (
                   SELECT ISNULL(SUM(p.SoTien),0) TongXacNhan
                   FROM dbo.ThanhToan p WHERE p.HoaDonID=h.HoaDonID AND p.TrangThai=N'Confirmed'
               ) x
               WHERE h.SoTienDaTra<>x.TongXacNhan
           ) THEN N'FAIL' ELSE N'PASS' END,
           N'SoTienDaTra = tổng ThanhToan Confirmed';

    INSERT INTO @KQ
    SELECT N'Hóa đơn khớp đơn đặt sân',
           CASE WHEN EXISTS
           (
               SELECT 1
               FROM dbo.HoaDon h
               JOIN dbo.DatSan b ON b.DatSanID=h.DatSanID
               WHERE h.KhachHangID<>b.KhachHangID
                  OR h.TienGoc<>b.TienGoc
                  OR h.TienGiam<>b.TienGiam
                  OR h.TongTien<>b.TongTien
                  OR h.TrangThaiThanhToan<>b.TrangThaiThanhToan
           ) THEN N'FAIL' ELSE N'PASS' END,
           N'Khách hàng, số tiền và trạng thái thanh toán đồng nhất';

    INSERT INTO @KQ
    SELECT N'Voucher đã dùng có đơn tương ứng',
           CASE WHEN EXISTS
           (
               SELECT 1
               FROM dbo.PhieuGiamGiaKhachHang cv
               LEFT JOIN dbo.DatSan b
                 ON b.DatSanID=cv.DatSanSuDungID
                AND b.KhachHangID=cv.KhachHangID
                AND b.PhieuGiamGiaID=cv.PhieuGiamGiaID
               WHERE cv.DaSuDung=1 AND b.DatSanID IS NULL
           ) THEN N'FAIL' ELSE N'PASS' END,
           N'Voucher sử dụng phải thuộc đúng khách và đúng đơn';

    INSERT INTO @KQ
    SELECT N'View và procedure hỗ trợ',
           CASE WHEN
               OBJECT_ID(N'dbo.vw_LichDatSanChiTiet',N'V') IS NOT NULL AND
               OBJECT_ID(N'dbo.vw_HoaDonChiTiet',N'V') IS NOT NULL AND
               OBJECT_ID(N'dbo.usp_KiemTraSanTrong',N'P') IS NOT NULL
           THEN N'PASS' ELSE N'FAIL' END,
           N'2 view chi tiết + procedure kiểm tra sân';

    DECLARE @SoLoi INT=(SELECT COUNT(*) FROM @KQ WHERE KetQua=N'FAIL');
    SELECT * FROM @KQ ORDER BY CASE KetQua WHEN N'FAIL' THEN 1 WHEN N'WARN' THEN 2 ELSE 3 END, HangMuc;

    IF @DungNeuCoLoi=1 AND @SoLoi>0
        THROW 52102, N'Kiểm tra CSDL thất bại. Xem các dòng FAIL trong bảng kết quả phía trên.', 1;
END;
GO

-- Kiểm thử CRUD/đặt sân/voucher/thanh toán trong transaction và ROLLBACK toàn bộ.
-- Có thể chạy lại bất cứ lúc nào, không để lại dữ liệu test.
CREATE OR ALTER PROCEDURE dbo.usp_KiemTraChucNang
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @@TRANCOUNT<>0 THROW 52200, N'Không chạy kiểm thử chức năng bên trong transaction khác.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Suffix VARCHAR(8)=RIGHT(REPLACE(CONVERT(VARCHAR(36),NEWID()),'-',''),8);
        DECLARE @Digits VARCHAR(8)=RIGHT('00000000'+CONVERT(VARCHAR(8),ABS(CONVERT(BIGINT,CHECKSUM(NEWID())))%100000000),8);
        DECLARE @CustomerRole INT=(SELECT VaiTroID FROM dbo.VaiTro WHERE TenVaiTro=N'Customer');
        DECLARE @EmployeeRole INT=(SELECT VaiTroID FROM dbo.VaiTro WHERE TenVaiTro=N'Employee');
        DECLARE @Hash VARCHAR(64)=REPLICATE('0',64);

        INSERT dbo.TaiKhoan(TenDangNhap,MatKhauBam,MuoiMatKhau,VaiTroID)
        VALUES(N'smoke-c-'+@Suffix,@Hash,'SMOKE123',@CustomerRole);
        DECLARE @CustomerAccount INT=CONVERT(INT,SCOPE_IDENTITY());

        INSERT dbo.KhachHang(MaKhachHang,TaiKhoanID,HoTen,SoDienThoai,Email)
        VALUES(N'SMKC'+@Suffix,@CustomerAccount,N'Khách kiểm thử',N'07'+@Digits,N'smoke.c.'+@Suffix+N'@test.local');
        DECLARE @CustomerID INT=CONVERT(INT,SCOPE_IDENTITY());

        INSERT dbo.TaiKhoan(TenDangNhap,MatKhauBam,MuoiMatKhau,VaiTroID)
        VALUES(N'smoke-e-'+@Suffix,@Hash,'SMOKE123',@EmployeeRole);
        DECLARE @EmployeeAccount INT=CONVERT(INT,SCOPE_IDENTITY());

        INSERT dbo.NhanVien(MaNhanVien,TaiKhoanID,HoTen,SoDienThoai,Email,ChucVu)
        VALUES(N'SMKE'+@Suffix,@EmployeeAccount,N'Nhân viên kiểm thử',N'08'+@Digits,N'smoke.e.'+@Suffix+N'@test.local',N'Kiểm thử');

        INSERT dbo.LoaiSan(MaLoaiSan,TenLoaiSan,MoTa) VALUES(N'SMKL'+@Suffix,N'Loại sân kiểm thử',N'Tự động rollback');
        DECLARE @LoaiSanID INT=CONVERT(INT,SCOPE_IDENTITY());
        INSERT dbo.SanTheThao(MaSan,TenSan,LoaiSanID,ViTri,GiaMoiGio)
        VALUES(N'SMKS'+@Suffix,N'Sân kiểm thử',@LoaiSanID,N'Khu test',100000);
        DECLARE @SanID INT=CONVERT(INT,SCOPE_IDENTITY());

        INSERT dbo.KhuyenMai(MaKhuyenMai,TenKhuyenMai,PhanTramGiam,NgayBatDau,NgayKetThuc)
        VALUES(N'SMKP'+@Suffix,N'Khuyến mãi kiểm thử',10,DATEADD(DAY,-1,SYSDATETIME()),DATEADD(DAY,10,SYSDATETIME()));
        DECLARE @KhuyenMaiID INT=CONVERT(INT,SCOPE_IDENTITY());

        INSERT dbo.PhieuGiamGia(MaPhieuGiamGia,TenPhieuGiamGia,LoaiGiamGia,GiaTriGiam,DonToiThieu,NgayBatDau,NgayKetThuc,SoLuong)
        VALUES(N'SMKV'+@Suffix,N'Voucher kiểm thử',N'Fixed',10000,0,DATEADD(DAY,-1,SYSDATETIME()),DATEADD(DAY,10,SYSDATETIME()),1);
        DECLARE @VoucherID INT=CONVERT(INT,SCOPE_IDENTITY());
        INSERT dbo.PhieuGiamGiaKhachHang(PhieuGiamGiaID,KhachHangID) VALUES(@VoucherID,@CustomerID);

        DECLARE @BatDau DATETIME2(0)=DATEADD(HOUR,10,DATEADD(DAY,DATEDIFF(DAY,0,SYSDATETIME())+1,0));
        DECLARE @KetThuc DATETIME2(0)=DATEADD(HOUR,11,DATEADD(DAY,DATEDIFF(DAY,0,SYSDATETIME())+1,0));
        INSERT dbo.DatSan(MaDatSan,KhachHangID,SanID,ThoiGianBatDau,ThoiGianKetThuc,GiaMoiGio,TienGoc,TienGiam,TongTien,
                          PhieuGiamGiaID,KhuyenMaiID,TrangThai,TrangThaiThanhToan,TaiKhoanTaoID)
        VALUES(N'SMKB'+@Suffix,@CustomerID,@SanID,@BatDau,@KetThuc,100000,100000,10000,90000,
               @VoucherID,@KhuyenMaiID,N'Confirmed',N'Unpaid',@EmployeeAccount);
        DECLARE @DatSanID INT=CONVERT(INT,SCOPE_IDENTITY());

        UPDATE dbo.PhieuGiamGiaKhachHang
        SET DaSuDung=1,NgaySuDung=SYSDATETIME(),DatSanSuDungID=@DatSanID
        WHERE PhieuGiamGiaID=@VoucherID AND KhachHangID=@CustomerID;

        INSERT dbo.HoaDon(MaHoaDon,DatSanID,KhachHangID,TienGoc,TienGiam,TongTien,SoTienDaTra,TrangThaiThanhToan)
        VALUES(N'SMKH'+@Suffix,@DatSanID,@CustomerID,100000,10000,90000,0,N'Unpaid');
        DECLARE @HoaDonID INT=CONVERT(INT,SCOPE_IDENTITY());

        INSERT dbo.ThanhToan(HoaDonID,SoTien,PhuongThucThanhToan,MaGiaoDich,TrangThai)
        VALUES(@HoaDonID,90000,N'QR',N'SMOKE-'+@Suffix,N'Pending');
        DECLARE @ThanhToanID INT=CONVERT(INT,SCOPE_IDENTITY());
        UPDATE dbo.ThanhToan SET TrangThai=N'Confirmed',NgayThanhToan=SYSDATETIME() WHERE ThanhToanID=@ThanhToanID;
        UPDATE dbo.HoaDon SET SoTienDaTra=90000,TrangThaiThanhToan=N'Paid' WHERE HoaDonID=@HoaDonID;
        UPDATE dbo.DatSan SET TrangThai=N'InUse' WHERE DatSanID=@DatSanID;
        UPDATE dbo.DatSan SET TrangThai=N'Completed' WHERE DatSanID=@DatSanID;
        INSERT dbo.CauHinhHeThong(KhoaCauHinh,GiaTriCauHinh) VALUES(N'SmokeTest',N'PASS');

        IF NOT EXISTS(SELECT 1 FROM dbo.HoaDon WHERE HoaDonID=@HoaDonID AND SoTienDaTra=90000 AND TrangThaiThanhToan=N'Paid')
            THROW 52201, N'Kiểm thử đồng bộ hóa đơn/thanh toán thất bại.', 1;
        IF NOT EXISTS(SELECT 1 FROM dbo.DatSan WHERE DatSanID=@DatSanID AND TrangThai=N'Completed' AND TrangThaiThanhToan=N'Paid')
            THROW 52202, N'Kiểm thử vòng đời đặt sân thất bại.', 1;
        IF NOT EXISTS(SELECT 1 FROM dbo.PhieuGiamGiaKhachHang WHERE PhieuGiamGiaID=@VoucherID AND KhachHangID=@CustomerID AND DaSuDung=1)
            THROW 52203, N'Kiểm thử voucher thất bại.', 1;
        IF (SELECT COUNT(*) FROM dbo.LichSuDatSan WHERE DatSanID=@DatSanID)<3
            THROW 52204, N'Kiểm thử lịch sử trạng thái thất bại.', 1;

        ROLLBACK TRANSACTION;
        SELECT N'PASS' AS KetQua,
               N'CRUD tài khoản/khách/nhân viên/sân/voucher/đặt sân/hóa đơn/thanh toán hoạt động và dữ liệu test đã rollback.' AS ChiTiet;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

/* ========================= 14. DỮ LIỆU KHỞI TẠO ========================= */
INSERT INTO dbo.VaiTro(TenVaiTro, TenHienThi) VALUES
(N'Admin',    N'Quản trị viên'),
(N'Employee', N'Nhân viên'),
(N'Customer', N'Khách hàng');
GO

DECLARE @AdminRoleID INT = (SELECT VaiTroID FROM dbo.VaiTro WHERE TenVaiTro=N'Admin');
DECLARE @EmployeeRoleID INT = (SELECT VaiTroID FROM dbo.VaiTro WHERE TenVaiTro=N'Employee');
DECLARE @CustomerRoleID INT = (SELECT VaiTroID FROM dbo.VaiTro WHERE TenVaiTro=N'Customer');

-- Hash = SHA256(salt + password), khớp Helpers/PasswordHelper.cs.
INSERT INTO dbo.TaiKhoan(TenDangNhap, MatKhauBam, MuoiMatKhau, VaiTroID, DangHoatDong) VALUES
(N'admin',
 '41fc5551ad85815175b1680b4d5f75ce0680dd001af44c201ec537a8cb4324f6',
 '9F3C81EA', @AdminRoleID, 1),
(N'staff',
 '9a44e76a10bb39c0c012a029b06103768e44461149b3c67af9762b126f008ce1',
 '5D7E24B6', @EmployeeRoleID, 1),
(N'customer',
 '2fac7e3589e41edcecba65961cf4a798920181f319fee2eba0e5ee6d11792fce',
 'C8E41A72', @CustomerRoleID, 1);
GO

DECLARE @StaffAccountID INT = (SELECT TaiKhoanID FROM dbo.TaiKhoan WHERE TenDangNhap=N'staff');
DECLARE @CustomerAccountID INT = (SELECT TaiKhoanID FROM dbo.TaiKhoan WHERE TenDangNhap=N'customer');

INSERT INTO dbo.NhanVien(MaNhanVien, TaiKhoanID, HoTen, SoDienThoai, Email, ChucVu, DangHoatDong)
VALUES
(N'NV001', @StaffAccountID, N'Trần Minh Staff', N'0987654321', N'staff@sportfield.vn', N'Nhân viên lễ tân', 1);

INSERT INTO dbo.KhachHang
(MaKhachHang, TaiKhoanID, HoTen, SoDienThoai, Email, DiaChi, NgaySinh, DiemTichLuy, DangHoatDong)
VALUES
(N'KH001', @CustomerAccountID, N'Nguyễn Văn An', N'0901234567', N'an.nguyen@gmail.com',
 N'12 Nguyễn Trãi, Thanh Xuân, Hà Nội', '1990-03-12', 120, 1),
(N'KH002', NULL, N'Trần Thị Bích', N'0912345678', N'bich.tran@outlook.com',
 N'45 Giải Phóng, Hai Bà Trưng, Hà Nội', '1995-06-01', 60, 1),
(N'KH003', NULL, N'Lê Minh Tuấn', N'0934567890', N'tuan.le@yahoo.com',
 N'78 Cầu Giấy, Hà Nội', '1988-11-22', 35, 1),
(N'KH004', NULL, N'Phạm Thu Hà', N'0968123456', N'ha.pham@gmail.com',
 N'Sóc Sơn, Hà Nội', '1998-04-18', 0, 1);
GO

INSERT INTO dbo.LoaiSan(MaLoaiSan, TenLoaiSan, MoTa, DangHoatDong) VALUES
(N'LS01', N'Sân bóng đá 5 người',  N'Sân cỏ nhân tạo dành cho bóng đá 5 người', 1),
(N'LS02', N'Sân bóng đá 7 người',  N'Sân cỏ nhân tạo có hệ thống đèn chiếu sáng', 1),
(N'LS03', N'Sân bóng đá 11 người', N'Sân kích thước lớn phục vụ thi đấu', 1),
(N'LS04', N'Sân cầu lông',         N'Sân trong nhà tiêu chuẩn', 1),
(N'LS05', N'Sân bóng chuyền',      N'Sân trong nhà thoáng mát', 1),
(N'LS06', N'Sân bóng rổ',          N'Sân bóng rổ có bảng rổ tiêu chuẩn', 1),
(N'LS07', N'Sân tennis',           N'Sân tennis mặt cứng', 1);
GO

DECLARE @LS01 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS01');
DECLARE @LS02 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS02');
DECLARE @LS03 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS03');
DECLARE @LS04 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS04');
DECLARE @LS05 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS05');
DECLARE @LS06 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS06');
DECLARE @LS07 INT=(SELECT LoaiSanID FROM dbo.LoaiSan WHERE MaLoaiSan=N'LS07');

INSERT INTO dbo.SanTheThao(MaSan, TenSan, LoaiSanID, ViTri, GiaMoiGio, TrangThai, MoTa, DangHoatDong) VALUES
(N'SA1',  N'Sân A1 (5 người)',   @LS01, N'Khu A',        120000, N'Available', N'Cỏ nhân tạo mới', 1),
(N'SA2',  N'Sân A2 (5 người)',   @LS01, N'Khu A',        120000, N'Available', N'Có đèn chiếu sáng', 1),
(N'SB1',  N'Sân B1 (7 người)',   @LS02, N'Khu B',        200000, N'Available', N'Đèn LED hai bên sân', 1),
(N'SB2',  N'Sân B2 (7 người)',   @LS02, N'Khu B',        200000, N'Available', N'Cỏ nhân tạo tiêu chuẩn', 1),
(N'SC1',  N'Sân C1 (11 người)',  @LS03, N'Sân chính',    350000, N'Available', N'Phù hợp thi đấu', 1),
(N'SCL1', N'Sân cầu lông 01',    @LS04, N'Nhà thi đấu',   80000, N'Available', N'Sân số 01', 1),
(N'SCL2', N'Sân cầu lông 02',    @LS04, N'Nhà thi đấu',   80000, N'Available', N'Sân số 02', 1),
(N'SCL3', N'Sân cầu lông 03',    @LS04, N'Nhà thi đấu',   80000, N'Available', N'Sân số 03', 1),
(N'SBC1', N'Sân bóng chuyền 01', @LS05, N'Nhà thi đấu',  100000, N'Available', N'Sân trong nhà', 1),
(N'SBR1', N'Sân bóng rổ 01',     @LS06, N'Khu D',        120000, N'Available', N'Mặt sân tiêu chuẩn', 1),
(N'STN1', N'Sân tennis 01',      @LS07, N'Khu E',        180000, N'Available', N'Mặt sân cứng', 1);
GO

-- Khuyến mãi mẫu có hiệu lực đủ dài để thử chức năng.
INSERT INTO dbo.KhuyenMai
(MaKhuyenMai, TenKhuyenMai, MoTa, PhanTramGiam, NgayBatDau, NgayKetThuc, DangHoatDong)
VALUES
(N'WELCOME10', N'Ưu đãi khai trương', N'Giảm 10% cho mọi lượt đặt sân trong thời gian chương trình',
 10, DATEADD(DAY,-30,SYSDATETIME()), DATEADD(DAY,365,SYSDATETIME()), 1);
GO

INSERT INTO dbo.PhieuGiamGia
(MaPhieuGiamGia, TenPhieuGiamGia, LoaiGiamGia, GiaTriGiam, MucGiamToiDa, DonToiThieu,
 NgayBatDau, NgayKetThuc, SoLuong, DangHoatDong)
VALUES
(N'VOUCHER20', N'Voucher thành viên - giảm 20%', N'Percent', 20, 100000, 100000,
 DATEADD(DAY,-30,SYSDATETIME()), DATEADD(DAY,730,SYSDATETIME()), 100, 1),
(N'SAVE50K', N'Giảm 50.000đ cho đơn từ 300.000đ', N'Fixed', 50000, NULL, 300000,
 DATEADD(DAY,-30,SYSDATETIME()), DATEADD(DAY,730,SYSDATETIME()), 100, 1),
(N'SAVE500', N'Giảm 500.000đ cho đơn từ 2.000.000đ', N'Fixed', 500000, NULL, 2000000,
 DATEADD(DAY,-30,SYSDATETIME()), DATEADD(DAY,730,SYSDATETIME()), 50, 1),
(N'WELCOME20', N'Chào mừng thành viên mới - giảm 20%', N'Percent', 20, 80000, 100000,
 DATEADD(DAY,-30,SYSDATETIME()), DATEADD(DAY,1095,SYSDATETIME()), 1000, 1);
GO

DECLARE @Customer1 INT=(SELECT KhachHangID FROM dbo.KhachHang WHERE MaKhachHang=N'KH001');
DECLARE @Voucher20 INT=(SELECT PhieuGiamGiaID FROM dbo.PhieuGiamGia WHERE MaPhieuGiamGia=N'VOUCHER20');
DECLARE @Save50K INT=(SELECT PhieuGiamGiaID FROM dbo.PhieuGiamGia WHERE MaPhieuGiamGia=N'SAVE50K');

-- Trigger sẽ tự trừ SoLuong còn lại của voucher khi cấp.
INSERT INTO dbo.PhieuGiamGiaKhachHang(PhieuGiamGiaID, KhachHangID)
VALUES
(@Voucher20, @Customer1),
(@Save50K, @Customer1);
GO

INSERT INTO dbo.CauHinhHeThong(KhoaCauHinh, GiaTriCauHinh) VALUES
(N'DatabaseVersion',   N'4.0 PRO'),
(N'CompanyName',       N'SportField - Quản lý thuê sân thể thao'),
(N'BankName',          N'TECHCOMBANK'),
(N'BankBin',           N'970407'),
(N'BankAccountNumber', N'190366528888'),
(N'BankAccountName',   N'NGUYEN VAN AN'),
(N'QrTransferPrefix',  N'THANHTOAN'),
(N'OpenTime',          N'05:00'),
(N'CloseTime',         N'23:00'),
(N'MinimumBookingMinutes', N'30'),
(N'CustomerCancellationHours', N'2');
GO

/* Dữ liệu giao dịch mẫu để dashboard, lịch, hóa đơn và thống kê có thể kiểm thử ngay. */
DECLARE @DemoCustomer INT=(SELECT KhachHangID FROM dbo.KhachHang WHERE MaKhachHang=N'KH001');
DECLARE @DemoAdmin INT=(SELECT TaiKhoanID FROM dbo.TaiKhoan WHERE TenDangNhap=N'admin');
DECLARE @DemoFieldA INT=(SELECT SanID FROM dbo.SanTheThao WHERE MaSan=N'SA1');
DECLARE @DemoFieldB INT=(SELECT SanID FROM dbo.SanTheThao WHERE MaSan=N'SB1');
DECLARE @DemoPromo INT=(SELECT KhuyenMaiID FROM dbo.KhuyenMai WHERE MaKhuyenMai=N'WELCOME10');

DECLARE @PastStart DATETIME2(0)=DATEADD(HOUR,18,DATEADD(DAY,DATEDIFF(DAY,0,SYSDATETIME())-2,0));
DECLARE @PastEnd DATETIME2(0)=DATEADD(HOUR,20,DATEADD(DAY,DATEDIFF(DAY,0,SYSDATETIME())-2,0));
DECLARE @FutureStart DATETIME2(0)=DATEADD(HOUR,18,DATEADD(DAY,DATEDIFF(DAY,0,SYSDATETIME())+1,0));
DECLARE @FutureEnd DATETIME2(0)=DATEADD(HOUR,20,DATEADD(DAY,DATEDIFF(DAY,0,SYSDATETIME())+1,0));

INSERT INTO dbo.DatSan(MaDatSan,KhachHangID,SanID,ThoiGianBatDau,ThoiGianKetThuc,GiaMoiGio,TienGoc,TienGiam,TongTien,KhuyenMaiID,TrangThai,TrangThaiThanhToan,GhiChu,TaiKhoanTaoID)
VALUES
(N'DS000001',@DemoCustomer,@DemoFieldA,@PastStart,@PastEnd,120000,240000,24000,216000,@DemoPromo,N'Completed',N'Unpaid',N'Giao dịch mẫu đã hoàn thành',@DemoAdmin),
(N'DS000002',@DemoCustomer,@DemoFieldB,@FutureStart,@FutureEnd,200000,400000,40000,360000,@DemoPromo,N'Confirmed',N'Unpaid',N'Lịch mẫu sắp tới',@DemoAdmin);

DECLARE @PastBooking INT=(SELECT DatSanID FROM dbo.DatSan WHERE MaDatSan=N'DS000001');
DECLARE @FutureBooking INT=(SELECT DatSanID FROM dbo.DatSan WHERE MaDatSan=N'DS000002');
INSERT INTO dbo.HoaDon(MaHoaDon,DatSanID,KhachHangID,TienGoc,TienGiam,TongTien,SoTienDaTra,TrangThaiThanhToan)
VALUES
(N'HD000001',@PastBooking,@DemoCustomer,240000,24000,216000,0,N'Unpaid'),
(N'HD000002',@FutureBooking,@DemoCustomer,400000,40000,360000,0,N'Unpaid');

DECLARE @PastInvoice INT=(SELECT HoaDonID FROM dbo.HoaDon WHERE MaHoaDon=N'HD000001');
INSERT INTO dbo.ThanhToan(HoaDonID,SoTien,PhuongThucThanhToan,MaGiaoDich,TrangThai,NgayThanhToan)
VALUES(@PastInvoice,216000,N'Cash',N'DEMO-PAID-001',N'Confirmed',DATEADD(DAY,-2,SYSDATETIME()));
UPDATE dbo.HoaDon SET SoTienDaTra=216000,TrangThaiThanhToan=N'Paid' WHERE HoaDonID=@PastInvoice;
GO

/* ========================= 15. KIỂM TRA SAU CÀI ĐẶT ========================= */
DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;
GO

EXEC dbo.usp_KiemTraCSDL @DungNeuCoLoi=1;
GO

EXEC dbo.usp_KiemTraChucNang;
GO

-- Hiện đúng vị trí MDF/LDF thực tế trên máy đang chạy SQL Server.
SELECT
    DB_NAME(database_id) AS DatabaseName,
    name AS LogicalFileName,
    type_desc AS FileType,
    physical_name AS PhysicalPath
FROM sys.master_files
WHERE database_id = DB_ID(N'QuanLySanTheThaoDB')
ORDER BY type_desc;
GO

-- Kiểm tra dữ liệu nền quan trọng.
SELECT N'VaiTro' AS Bang, COUNT(*) AS SoDong FROM dbo.VaiTro
UNION ALL SELECT N'TaiKhoan', COUNT(*) FROM dbo.TaiKhoan
UNION ALL SELECT N'KhachHang', COUNT(*) FROM dbo.KhachHang
UNION ALL SELECT N'NhanVien', COUNT(*) FROM dbo.NhanVien
UNION ALL SELECT N'LoaiSan', COUNT(*) FROM dbo.LoaiSan
UNION ALL SELECT N'SanTheThao', COUNT(*) FROM dbo.SanTheThao
UNION ALL SELECT N'KhuyenMai', COUNT(*) FROM dbo.KhuyenMai
UNION ALL SELECT N'PhieuGiamGia', COUNT(*) FROM dbo.PhieuGiamGia
UNION ALL SELECT N'PhieuGiamGiaKhachHang', COUNT(*) FROM dbo.PhieuGiamGiaKhachHang;
GO

PRINT N'============================================================';
PRINT N'QuanLySanTheThaoDB V4 PRO đã tạo xong.';
PRINT N'Login: admin/admin123 | staff/staff123 | customer/customer123';
PRINT N'Hãy kiểm tra kết quả usp_KiemTraCSDL và usp_KiemTraChucNang: tất cả phải PASS.';
PRINT N'============================================================';
GO
