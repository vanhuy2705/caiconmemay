CSDL QUANLYSANTHETHAODB - V4 PRO

File cần chạy: QuanLySanTheThaoDB_V4_PRO.sql
QuanLySanTheThaoDB.sql có nội dung giống hệt để tiện tìm kiếm.

Yêu cầu: SQL Server 2019/2022 hoặc SQL Server Express, kết nối mặc định .\SQLEXPRESS.
Script là bản cài mới sạch: xóa database cũ rồi tạo lại toàn bộ bảng, khóa ngoại,
CHECK, index, trigger, view, procedure, tài khoản và dữ liệu mẫu. Script cũng tự
chạy kiểm thử chức năng trong transaction và rollback, nên không để lại dữ liệu rác.

Sau khi Execute:
1. Kết quả dbo.usp_KiemTraCSDL phải PASS; script sẽ tự dừng nếu có dòng FAIL.
2. DatabaseVersion phải là 4.0 PRO.
3. Có 3 vai trò và 3 tài khoản demo.
4. DBCC CHECKCONSTRAINTS không được trả về lỗi.
5. usp_KiemTraChucNang phải trả về PASS và xác nhận dữ liệu test đã rollback.

KIEM_TRA_HE_THONG_V4.sql dùng để kiểm tra lại bất kỳ lúc nào.
KIEM_TRA_KET_NOI.sql hiển thị server/database/tài khoản Windows đang dùng.
