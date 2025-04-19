--Câu 1: Thủ tục in ra danh sách nhân viên chưa chấm công trong một ngày
CREATE PROCEDURE DanhSachNhanVienChuaChamCong
    @Ngay DATE
AS
BEGIN
    SELECT NV.HoTen, NV.MaNV
    FROM NHANVIEN NV
    LEFT JOIN CHAMCONG CC ON NV.MaNV = CC.MaNV AND CC.Ngay = @Ngay
    WHERE CC.MaCC IS NULL

    PRINT N'Danh sách nhân viên chưa chấm công cho ngày: ' + CAST(@Ngay AS NVARCHAR(10))
END

EXEC DanhSachNhanVienChuaChamCong '2024-10-07';

SELECT * FROM chamcong WHERE Ngay='2024-10-07'


--Câu 2: Thủ tục cập nhật chức vụ cho nhân viên
CREATE  or alter PROCEDURE CapNhatChucVuNhanVien
    @MaNV INT,
    @MaCV NVARCHAR(10)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM NHANVIEN WHERE MaNV = @MaNV)
    BEGIN
        PRINT N'Nhân viên không tồn tại.'
        RETURN
    END

    UPDATE NHANVIEN
    SET MaCV = @MaCV
    WHERE MaNV = @MaNV
    
    PRINT N'Cập nhật chức vụ thành công.'
END

EXEC CapNhatChucVuNhanVien 0001,'N0539'

SELECT * FROM nhanvien 
WHERE MaNV=0001


--Câu 3: Thay đổi mật khẩu cho nhân viên
CREATE or alter PROCEDURE ThayDoiMatKhau
    @TenDangNhap NVARCHAR(50),
    @MatKhauCu NVARCHAR(50),
    @MatKhauMoi NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TAIKHOAN 
				WHERE TenDangNhap = @TenDangNhap 
				AND MatKhau = @MatKhauCu)
    BEGIN
        UPDATE TAIKHOAN
        SET MatKhau = @MatKhauMoi
        WHERE TenDangNhap = @TenDangNhap
        PRINT N'Mật khẩu đã được thay đổi.'
    END
    ELSE
    BEGIN
        PRINT N'Mật khẩu cũ không chính xác.'
    END
END

EXEC ThayDoiMatKhau @TenDangNhap = '220000236',
					@MatKhauCu = 'password236', 
					@MatKhauMoi = 'newpassword123';

SELECT * FROM TAIKHOAN
WHERE MaNV=0001



--Câu 4: Trigger tính tổng lương cho nhân viên dựa trên tổng giờ làm.
ALTER TABLE LUONG ADD Tongluong float;

CREATE OR ALTER TRIGGER trg_UpdateLuong
ON CHAMCONG
AFTER INSERT
AS
BEGIN
    DECLARE @MaNV varchar(10),
            @Gio float, 
            @Phut float,
            @GioLam float,
            @Thuong numeric,
            @Phat numeric,
            @Luong money;

    -- Lấy MaNV từ bản ghi mới được thêm
    SELECT @MaNV = inserted.MaNV
    FROM inserted;
    
    -- Tính tổng số giờ làm
    SELECT @Gio = SUM(DATEDIFF(HOUR, GioVao, GioRa)) 
    FROM CHAMCONG 
    WHERE MaNV = @MaNV;

    SELECT @Phut = SUM(DATEDIFF(MINUTE, GioVao, GioRa)/60.0) 
    FROM CHAMCONG 
    WHERE MaNV = @MaNV;

    SET @GioLam = @Gio + @Phut;

    -- Lấy thông tin thưởng và phạt từ bảng LUONG
    SELECT @Thuong = Thuong, @Phat = Phat, @Luong = LuongTheoGio
    FROM LUONG 
    WHERE MaNV = @MaNV;

    SET @Thuong = @Thuong / 100;
    SET @Phat = @Phat / 100;

    -- Cập nhật lương trong bảng LUONG
    UPDATE LUONG
    SET Tongluong = @Luong * @GioLam * (1 + @Thuong - @Phat)
    WHERE MaNV = @MaNV;
END;


INSERT INTO CHAMCONG (MaCC, MaNV,Ngay, GioVao, GioRa)
VALUES ('CC001', '0001','2024-09-10', '08:00:00', '17:00:00')

select * from luong
where manv='0001'


--Câu 5: Kiểm tra nhân viên có phần trăm lỗi > 20% khi biết mã nhân viên. 
--Nếu có thì in ra tên nhân viên và đưa ra tb 'Cảnh báo đuổi việc'. 
--Ngược lại thì đưa ra thông báo 'Không cảnh báo đuổi việc'.
create or alter proc pLoik  ( @manv varchar(15), 
							@ten nvarchar (50) output,
							@nx nvarchar (50) output )
as 
begin
	declare @phat int
	select @ten = HoTen from NHANVIEN where MaNV = @manv
	if ((select Phat 
		from LUONG inner join NHANVIEN on LUONG.MaNV = NHANVIEN.MaNV 
		where NHANVIEN.MaNV = @manv) > 20)

		set @nx = N'Cảnh báo đuổi việc'
	else 
		set @nx = N'Không cảnh báo đuổi việc '
end

declare @a nvarchar(50), @b nvarchar(50)
exec pLoik '0001', @a out, @b out 
print @a
print @b

--Câu 6: Trả về các ca làm bất thường(ca làm bất thường: giờ vào - giờ ra <=3), 
--			nếu không có ca nào thì in ra thông báo 'không có ca làm bất thường'
create or alter proc spCalambatthuong 
as
begin
	declare @Manv varchar(10),
			@Ngay date,
			@Giovao time,
			@Giora time
	if (select count(*) from CHAMCONG where datediff(hour, @Giovao, @Giora) <=3) >1
	begin
		select @Manv= MaNV,
				@Ngay=Ngay,
				@Giovao=GioVao,
				@Giora=GioRa
		from CHAMCONG where datediff(hour, @Giovao, @Giora) <=3
	end
	else
	begin
		print N'Không có ca làm bất thường'
	end
end

exec spCalambatthuong

select * from CHAMCONG

--Câu 7: Khi xóa những tài khoản không đăng nhập liên tục trong 2 tuần thì 
--			thực hiện thao tác cập nhật tài khoản là QUIT thay vì xóa 
create or alter trigger tgQuit on TAIKHOAN
	instead of delete
	as
	begin
		declare @MaNV varchar (10),
				@dangnhap date
		select @MaNV = MaNV from deleted
		select @dangnhap=max(Ngay) from CHAMCONG where MaNV = @MaNV
		if datediff(day, @dangnhap, getdate()) >=14
		begin
			update TAIKHOAN
			set TenDangNhap='QUIT'
			where MaNV=@MaNV
		end
		else
		begin
			print N'Vẫn đăng nhập đều'
		end
	end

	delete TAIKHOAN where MaTK='TK0001'

--Câu 8: Cập nhật giờ ra cho nhân viên trong chấm công
CREATE PROCEDURE CapNhatGioRa
    @MaNV INT,
    @Ngay DATE,
    @GioRa TIME
AS
BEGIN
    -- Kiểm tra xem nhân viên đã có dữ liệu chấm công cho ngày đó hay chưa
    IF EXISTS (SELECT 1 FROM CHAMCONG WHERE MaNV = @MaNV AND Ngay = @Ngay)
    BEGIN
        -- Cập nhật giờ ra
        UPDATE CHAMCONG
        SET GioRa = @GioRa
        WHERE MaNV = @MaNV AND Ngay = @Ngay
        
        PRINT 'Cập nhật giờ ra thành công.'
    END
    ELSE
    BEGIN
        PRINT 'Không tìm thấy dữ liệu chấm công cho nhân viên trong ngày này.'
    END
END


SELECT * FROM CHAMCONG

EXEC CapNhatGioRa '0006', '2024-10-06', '22:30'

--Câu 9: Thêm nhân viên mới với kiểm tra trùng mã nhân viên
CREATE or ALTER PROCEDURE sp_ThemNhanVien
    @MaNV VARCHAR(15),
    @HoTen NVARCHAR(50),
    @NgaySinh DATE,
    @GioiTinh NVARCHAR(10),
    @DiaChi NVARCHAR(100),
    @SDT NVARCHAR(15),
    @MaCV VARCHAR(15)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM NHANVIEN WHERE MaNV = @MaNV)
    BEGIN
        SELECT N'Mã nhân viên đã tồn tại'
    END
    ELSE
    BEGIN
        INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, MaCV)
        VALUES (@MaNV, @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SDT, @MaCV)
        SELECT N'Nhân viên đã được thêm thành công'
    END
END
exec sp_ThemNhanVien '0001', 
					  N'Lê Hữu Đức Trọng', 
					  '2003-11-07', 
					  N'Nữ',
					  N'Pleiku', 
					  '0982231421', 
					  'N0621'
select *from nhanvien




--câu 10 thêm xử lí khi nhân viên đi làm muộn, về sớm ca chiều 
CREATE or alter PROCEDURE sp_XuLyChamCong
    @MaNV INT,
    @Ngay DATE,
    @GioVao TIME,
    @GioRa TIME
AS
BEGIN
    -- Quy định giờ vào và giờ ra cho hai ca làm việc 
	--(ca 1: ca chiều; ca 2: ca tối)
    DECLARE @GioVaoChuanCa1 TIME = '14:00:00', @GioRaChuanCa1 TIME = '17:30:00' 
    DECLARE @GioVaoChuanCa2 TIME = '18:30:00', @GioRaChuanCa2 TIME = '22:30:00'
    DECLARE @Phat DECIMAL(10, 2) = 0

    -- Kiểm tra giờ làm việc thuộc ca nào
    IF @GioVao BETWEEN @GioVaoChuanCa1 AND @GioRaChuanCa1
    BEGIN
        -- Ca 1: Kiểm tra nhân viên có đi làm muộn hay về sớm không
        IF @GioVao > @GioVaoChuanCa1
            SET @Phat = @Phat + 5  -- Phạt 5% cho mỗi lần đi muộn

        IF @GioRa < @GioRaChuanCa1
            SET @Phat = @Phat + 5  -- Phạt 5% cho mỗi lần về sớm
    END
    ELSE IF @GioVao BETWEEN @GioVaoChuanCa2 AND @GioRaChuanCa2
    BEGIN
        -- Ca 2: Kiểm tra nhân viên có đi làm muộn hay về sớm không
        IF @GioVao > @GioVaoChuanCa2
            SET @Phat = @Phat + 5  -- Phạt 5% cho mỗi lần đi muộn

        IF @GioRa < @GioRaChuanCa2
            SET @Phat = @Phat + 5  -- Phạt 5% cho mỗi lần về sớm
    END
    ELSE
    BEGIN
        -- Không thuộc ca nào
        SELECT N'Giờ làm việc không thuộc ca làm việc quy định.'
        RETURN
    END

    -- Cập nhật phạt trong bảng LUONG
    UPDATE LUONG
    SET Phat = Phat + @Phat
    WHERE MaNV = @MaNV
    SELECT N'Chấm công thành công với mức phạt là', @Phat AS MucPhat
END
exec sp_XuLyChamCong '0001','2024-09-19','14:00:00','18:00:00'
   

select * from luong where manv=0001