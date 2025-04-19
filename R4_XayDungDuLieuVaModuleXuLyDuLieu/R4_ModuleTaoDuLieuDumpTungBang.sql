--Tạo dữ liệu dump cho bảng CHUCVU
CREATE OR ALTER PROCEDURE ThemChucVu --(đã chèn)
AS
BEGIN
    DECLARE @i INT = 1;
    DECLARE @TenCV NVARCHAR(50), @MaCV NVARCHAR(50);
    
    -- Bảng tạm để lưu các chức vụ mẫu
    DECLARE @DanhSachChucVu TABLE (ID INT IDENTITY(1,1), Ten NVARCHAR(50));

    -- Các giá trị mẫu cho tên chức vụ
    INSERT INTO @DanhSachChucVu (Ten) 
    VALUES 
        (N'Quản lý'), 
        (N'Ca trưởng'), 
        (N'Nhân viên full-time'), 
        (N'Nhân viên part-time'), 
        (N'Phụ bếp')
    
    WHILE @i <= 1000
    BEGIN
        -- Chọn ngẫu nhiên tên chức vụ từ danh sách
        DECLARE @RandomChucVu INT = ABS(CHECKSUM(NEWID()) % 7) + 1;
        SELECT @TenCV = Ten FROM @DanhSachChucVu WHERE ID = @RandomChucVu;

        -- Lấy 1 ký tự đầu của tên chức vụ để làm mã chức vụ
        SET @MaCV = LEFT(@TenCV, 1) + RIGHT('000' + CAST(@i AS VARCHAR), 4);

        -- Thêm vào bảng CHUCVU
        INSERT INTO CHUCVU (MaCV, TenCV)
        VALUES (
            @MaCV,  -- Mã chức vụ là 1 ký tự đầu + số thứ tự (vd: G0001 cho Giám đốc)
            @TenCV) -- Tên chức vụ 
        
        SET @i = @i + 1;  -- Tăng biến đếm
    END;
END

EXEC ThemChucVu

SELECT *  from CHUCVU



--Tạo dữ liệu dump cho bảng NHANVIEN
CREATE OR ALTER PROCEDURE ThemNhanVien
AS
BEGIN
    DECLARE @i INT = 1;
    DECLARE @Ho NVARCHAR(50), @HoLot NVARCHAR(50), @Ten NVARCHAR(50), @DiaChi NVARCHAR(100), 
			@SDT NVARCHAR(20), @GioiTinh NVARCHAR(10), @MaCV NVARCHAR(50);
    
    -- Bảng tạm lưu họ, họ lót và tên
    DECLARE @DanhSachHo TABLE (ID INT IDENTITY(1,1), Ho NVARCHAR(50));
    DECLARE @DanhSachHoLot TABLE (ID INT IDENTITY(1,1), HoLot NVARCHAR(50));
    DECLARE @DanhSachTen TABLE (ID INT IDENTITY(1,1), Ten NVARCHAR(50));
    DECLARE @DanhSachDiaChi TABLE (ID INT IDENTITY(1,1), DiaChi NVARCHAR(100));
    DECLARE @DanhSachGioiTinh TABLE (ID INT IDENTITY(1,1), GioiTinh NVARCHAR(10));

    -- Dữ liệu ví dụ
    INSERT INTO @DanhSachHo (Ho) VALUES (N'Nguyễn'), (N'Trần'), (N'Phạm'), (N'Lê'), (N'Hoàng');
    INSERT INTO @DanhSachHoLot (HoLot) VALUES (N'Văn'), (N'Thị'), (N'Trường'), (N'Hữu'), (N'Công');
    INSERT INTO @DanhSachTen (Ten) VALUES (N'Tuấn'), (N'Anh'), (N'Mỹ'), (N'Phương'), (N'Quang');
    INSERT INTO @DanhSachDiaChi (DiaChi) VALUES (N'Hà Nội'), (N'Hồ Chí Minh'), (N'Đà Nẵng'), (N'Hải Phòng'), (N'Cần Thơ');
    INSERT INTO @DanhSachGioiTinh (GioiTinh) VALUES (N'Nam'), (N'Nữ');

    WHILE @i <= 1000
    BEGIN
        -- Chọn ngẫu nhiên các giá trị
        DECLARE @RandomHo INT = ABS(CHECKSUM(NEWID()) % 5) + 1;    
        DECLARE @RandomHoLot INT = ABS(CHECKSUM(NEWID()) % 5) + 1;
        DECLARE @RandomTen INT = ABS(CHECKSUM(NEWID()) % 5) + 1;
        DECLARE @RandomDiaChi INT = ABS(CHECKSUM(NEWID()) % 5) + 1;
        DECLARE @RandomGioiTinh INT = ABS(CHECKSUM(NEWID()) % 2) + 1;

        -- Lấy giá trị ngẫu nhiên từ bảng tạm
        SELECT @Ho = Ho FROM @DanhSachHo WHERE ID = @RandomHo;
        SELECT @HoLot = HoLot FROM @DanhSachHoLot WHERE ID = @RandomHoLot;
        SELECT @Ten = Ten FROM @DanhSachTen WHERE ID = @RandomTen;
        SELECT @DiaChi = DiaChi FROM @DanhSachDiaChi WHERE ID = @RandomDiaChi;
        SELECT @GioiTinh = GioiTinh FROM @DanhSachGioiTinh WHERE ID = @RandomGioiTinh;

		SELECT TOP 1 @MaCV = MaCV FROM CHUCVU ORDER BY NEWID();

        -- Thêm vào bảng NHANVIEN
        INSERT INTO NHANVIEN (MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SDT, MaCV)
        VALUES (
            RIGHT('0000' + CAST(@i AS VARCHAR), 4),
            @Ho + ' ' + @HoLot + ' ' + @Ten,
            DATEADD(YEAR, -22 - ABS(CHECKSUM(NEWID()) % 20), GETDATE()),  -- Ngày sinh từ 22 đến 42 tuổi
            @GioiTinh,
            @DiaChi,
            '0' + CAST(ABS(CHECKSUM(NEWID()) % 900000000 + 100000000) AS VARCHAR),-- SDT ngẫu nhiên
            @MaCV)-- -- Lấy ngẫu nhiên MaCV từ bảng CHUCVU
        SET @i = @i + 1;  -- Tăng biến đếm
    END;
END;

-- Thực thi thủ tục
EXEC ThemNhanVien;

select * from Nhanvien

--Tạo dữ liệu dump cho bảng LUONG
CREATE OR ALTER PROCEDURE ThemLuong
AS
BEGIN
    DECLARE @i INT = 1;
    DECLARE @MaCV NVARCHAR(10), @LuongTheoGio FLOAT;

    WHILE @i <= 1000
    BEGIN
        -- Lấy MaCV từ bảng NHANVIEN dựa trên MaNV
        SELECT @MaCV = MaCV FROM NHANVIEN WHERE MaNV = RIGHT('0000' + CAST(@i AS VARCHAR), 4);

        -- Xác định mức lương theo giờ dựa trên chức vụ
        IF @MaCV like 'Q%'  -- Quản lý
            SET @LuongTheoGio = 35000;
        ELSE IF @MaCV like 'C%'  -- Ca trưởng
            SET @LuongTheoGio = 30000;
        ELSE IF @MaCV like 'P%'  -- Phụ bếp
            SET @LuongTheoGio = 28000;
        ELSE IF @MaCV like 'N%'  -- Nhân viên full-time/ part-time
            SET @LuongTheoGio = 20000;
        ELSE 
            SET @LuongTheoGio = ABS(CHECKSUM(NEWID()) % 100) + 50;  
			-- Lương theo giờ ngẫu nhiên nếu không có chức vụ

        -- Thêm vào bảng LUONG
        INSERT INTO LUONG (MaLuong, MaNV, LuongTheoGio, Thuong, Phat)
        VALUES (
            'L' + RIGHT('0000' + CAST(@i AS VARCHAR), 4),   -- Mã lương
            RIGHT('0000' + CAST(@i AS VARCHAR), 4),         -- Mã nhân viên
            @LuongTheoGio,                                  -- Lương theo giờ
            ABS(CHECKSUM(NEWID()) % 31),                   --  Phần trăm Thưởng ngẫu nhiên (0% - 30%)
            ABS(CHECKSUM(NEWID()) % 31)                    -- Phần trăm Phạt ngẫu nhiên (0% - 30%)
        );

        SET @i = @i + 1;
    END;
END;

EXEC ThemLuong

select * from Luong



--Tạo dữ liệu dump cho bảng CHAMCONG
CREATE OR ALTER PROCEDURE ThemChamCong
AS
BEGIN
    DECLARE @Ngay DATE, @GioVao DATETIME, @GioRa DATETIME;

	DECLARE @i INT = 1;

    WHILE @i <= 1000
    
    BEGIN
        -- Tạo dữ liệu ngẫu nhiên cho Ngay (ngày trong khoảng 30 ngày gần nhất)
        SET @Ngay = DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE());

        -- Tạo ngẫu nhiên GioVao từ 13h30-14h hoặc 18h-18h30
        IF ABS(CHECKSUM(NEWID()) % 2) = 0 
			BEGIN
				-- Khung 13h30-14h
				SET @GioVao = CAST(CONVERT(VARCHAR(10), @Ngay, 120) + ' ' + 
                               CAST(13 + ABS(CHECKSUM(NEWID()) % 1) AS VARCHAR) + ':' + 
                               CAST(30 + ABS(CHECKSUM(NEWID()) % 31) AS VARCHAR) + ':00' AS DATETIME);
			END

        ELSE
			BEGIN
            -- Khung 18h-18h30
            SET @GioVao = CAST(CONVERT(VARCHAR(10), @Ngay, 120) + ' ' + 
                               CAST(18 + ABS(CHECKSUM(NEWID()) % 1) AS VARCHAR) + ':' + 
                               CAST(ABS(CHECKSUM(NEWID()) % 31) AS VARCHAR) + ':00' AS DATETIME);
			END

        -- Tạo ngẫu nhiên GioRa từ 16h-22h30
        SET @GioRa = CAST(CONVERT(VARCHAR(10), @Ngay, 120) + ' ' + 
                          CAST(16 + ABS(CHECKSUM(NEWID()) % 7) AS VARCHAR) + ':' + 
                          CAST(ABS(CHECKSUM(NEWID()) % 61) AS VARCHAR) + ':00' AS DATETIME);

        -- Thêm dữ liệu vào bảng CHAMCONG
        INSERT INTO CHAMCONG (MaCC,MaNV, Ngay, GioVao, GioRa)
        VALUES ('CC' + RIGHT('0000' + CAST(@i AS VARCHAR), 4),
				RIGHT('0000' + CAST(@i AS VARCHAR), 4),
				@Ngay, @GioVao, @GioRa);
		SET @i = @i + 1;
    END;
END;

-- Thực thi thủ tục
EXEC ThemChamCong;

select * from CHAMCONG



--Tạo dữ liệu dump cho bảng TAIKHOAN
CREATE OR ALTER PROCEDURE ThemTaiKhoan
AS
BEGIN
	DECLARE @MaNV VARCHAR(20);
    DECLARE @i INT = 1;

    WHILE @i <= 1000
    BEGIN
		-- Lấy ngẫu nhiên một nhân viên
        SELECT TOP 1 @MaNV = MaNV FROM NHANVIEN ORDER BY NEWID();

        -- Thêm vào bảng TAIKHOAN
        INSERT INTO TAIKHOAN (MaTK, TenDangNhap, MatKhau, MaNV)
        VALUES (
             'TK' + RIGHT('0000' + CAST(@i AS VARCHAR), 4),
            '220000' + CAST(@i AS VARCHAR), -- Tên đăng nhập 220001, 220002, ...
            'password' + CAST(@i AS VARCHAR),  -- Mật khẩu dạng password1, password2, ...
            @MaNV);
        SET @i = @i + 1;
    END;
END;

EXEC ThemTaiKhoan

select * from taikhoan