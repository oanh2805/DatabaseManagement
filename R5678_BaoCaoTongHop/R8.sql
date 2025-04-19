--BẢNG NHANVIEN
--SP Thêm Nhân Viên
CREATE or ALTER PROCEDURE AddNV
    @MaNV VARCHAR(20),
    @HoTen NVARCHAR(50),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @DiaChi NVARCHAR(70),
    @SDT CHAR(10),
    @MaCV VARCHAR(20)
AS
BEGIN
    INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, MaCV)
    VALUES (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @MaCV);
END;

--TEST
go
declare @MaNV VARCHAR(20) = '1001',
		@HoTen NVARCHAR(50) = N'Lê Văn Đạt',
		@NgaySinh DATE = '2003-12-22',
		@GioiTinh NVARCHAR(10)= N'Nam',
		@DiaChi NVARCHAR(70) = N'Bình Định',
		@SDT CHAR(10)= '0905120456',
		@MaCV VARCHAR(20) = 'N0539'

EXEC sp_ThemNhanVien 
@MaNV,
@HoTen,
@NgaySinh,
@GioiTinh,
@DiaChi,
@SDT,
@MaCV

select * from NhanVien 


--SP Lấy thông tin nhân viên
CREATE OR ALTER PROCEDURE GetNV
    @MaNV VARCHAR(20)
AS
BEGIN
	SELECT MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, MaCV
    FROM NHANVIEN
    WHERE MaNV = @MaNV
END
GO

--TEST
--Khi gọi Stored Procedure bằng tham số MaNV:
EXEC GetNV @MaNV = '0001'


--SP Xóa nhân viên 
CREATE OR ALTER PROC XoaNhanVien
	@MaNV VARCHAR(20)
AS
BEGIN
	DELETE FROM NhanVien
	WHERE MaNV = @MaNV
END

--test
GO
EXEC XoaNhanVien
	@MaNV = '1000'

------------------------------------------------------

--BẢNG CHAMCONG
--SP Cập nhật chấm công
CREATE OR ALTER PROC UPDATECHAMCONG
					@MaCC varchar(20),
					@MaNV varchar(20),
					@Ngay date,
					@Giovao time,
					@Giora time
AS
BEGIN
		UPDATE CHAMCONG
		Set MaNV=@MaNV,
			Ngay=@Ngay,
			GioVao=@Giovao,
			GioRa=@Giora
		where MaCC=@MaCC;
end
go

--TEST
select * from CHAMCONG
exec UPDATECHAMCONG
	@MaCC='CC1001',
	@MaNV='0003',
	@Ngay='2024-11-19',
	@Giovao='14:00:00',
	@Giora='18:30:00';

------------------------------------------

--BẢNG LUONG
--SP LẤY RA THÔNG TIN LƯƠNG
CREATE OR ALTER PROCEDURE GetLUONG
    @MaNV VARCHAR(20)
AS
BEGIN
	SELECT MaLuong, MaNV, LuongTheoGio, Thuong, Phat, Tongluong
	FROM LUONG 
	WHERE MaNV=@MaNV 
END

--TEST
EXEC GetLUONG @MaNV='0999'

