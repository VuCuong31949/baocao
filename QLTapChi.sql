CREATE DATABASE QLTapChi;
USE QLTapChi;

-- Bảng Lĩnh Vực
CREATE TABLE LinhVuc (
    IDLinhVuc INT IDENTITY(1,1) PRIMARY KEY,
    TenLinhVuc NVARCHAR(100) NOT NULL
);
GO

-- Bảng Vai Trò
/*CREATE TABLE VaiTro (
    IDVaiTro INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(100) NOT NULL
);
GO*/

-- Bảng Người Dùng
CREATE TABLE NguoiDung (
    IDNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(300) NOT NULL,
    Email VARCHAR(200) NOT NULL,
    MatKhau VARCHAR(70) NOT NULL,
    SDT VARCHAR(15) NOT NULL,
    DiaChi NVARCHAR(300) NOT NULL,
    QuocGia NVARCHAR(50) NOT NULL,
	ChucDanh nvarchar(100),
	GioiTinh int,
	ToChuc nvarchar(100),
	PhanBien bit,
    IDLinhVuc INT NULL,
    FOREIGN KEY (IDLinhVuc) REFERENCES LinhVuc(IDLinhVuc)
);
GO

-- Bảng Biên Tập Viên
CREATE TABLE BienTapVien (
    IDBienTapVien INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(300) NOT NULL,
    Email VARCHAR(200) NOT NULL,
    MatKhau VARCHAR(70) NOT NULL,
    SDT VARCHAR(15) NOT NULL,
    DiaChi NVARCHAR(300) NOT NULL,
    QuocGia NVARCHAR(50) NOT NULL,
    ChuyenNganh NVARCHAR(100) NOT NULL,
	LoaiBienTapVien NVARCHAR(50);  -- 'TongBienTap' hoặc 'BienTapVien'
);
GO

-- Bảng Người Dùng - Vai Trò
/*CREATE TABLE NguoiDung_VaiTro (
    IDNguoiDung INT NOT NULL,
    IDVaiTro INT NOT NULL,
    PRIMARY KEY (IDNguoiDung, IDVaiTro),
    FOREIGN KEY (IDNguoiDung) REFERENCES NguoiDung(IDNguoiDung),
    FOREIGN KEY (IDVaiTro) REFERENCES VaiTro(IDVaiTro)
);
GO*/

-- Bảng Tạp Chí - Bài Viết
CREATE TABLE TapChiBaiViet (
    IDTapChiBaiViet INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(200) NOT NULL,
    TacGia NVARCHAR(300) NOT NULL,
    NoiDung NVARCHAR(MAX)NOT NULL,
    IDLinhVuc INT NOT NULL,
    TrangThai INT DEFAULT 0,  -- 0: Chờ duyệt, 1: Đã duyệt, 2: đã phan công phản biện, 3:Xuất bản
    NgayGui DATE NOT NULL DEFAULT GETDATE(),
	TuKhoa nvarchar(300),	
	IDNguoiGui INT,
	DongTacGia NVARCHAR(500),
	TrangThaiPhanBien INT DEFAULT 0,
	-- 0: Chờ phản biện 1: Đang phản biện 2: Đạt (chờ xuất bản)
	-- 3: Không đạt (chờ chỉnh sửa) 4: Từ chối
	TomTat nvarchar(max),
    FOREIGN KEY (IDLinhVuc) REFERENCES LinhVuc(IDLinhVuc),
	CONSTRAINT FK_TapChiBaiViet_NguoiDung FOREIGN KEY (IDNguoiGui) REFERENCES NguoiDung(IDNguoiDung)
);
GO

CREATE TABLE PhanBien (
    IDPhanBien INT IDENTITY(1,1) PRIMARY KEY,
    NhanXet NVARCHAR(500) NULL,
    NgayPhanBien DATE NOT NULL DEFAULT GETDATE(),
    IDTapChiBaiViet INT NOT NULL,
    IDNguoiPhanBien INT NOT NULL,
	filePB varchar(200),
    FOREIGN KEY (IDTapChiBaiViet) REFERENCES TapChiBaiViet(IDTapChiBaiViet),
    FOREIGN KEY (IDNguoiPhanBien) REFERENCES NguoiDung(IDNguoiDung)
);
GO

-- Bảng Phân Công (Phân công phản biện tạp chí)
CREATE TABLE PhanCong (
    IDPhanCong INT IDENTITY(1,1) PRIMARY KEY,
    NgayPhanCong DATE NOT NULL DEFAULT GETDATE(),
    NgayKetThuc DATE NULL,
    IDTapChiBaiViet INT NOT NULL,
    IDNguoiPhanBien INT NOT NULL,
	VongPhanBien INT DEFAULT 1,
    TrangThaiPhanBien INT DEFAULT 0,  -- 0: chưa phản hồi, 1: đạt, 2: không đạt,3: Sửa đổi nhỏ, sửa đổi lớn, 4 từ chối
    FOREIGN KEY (IDTapChiBaiViet) REFERENCES TapChiBaiViet(IDTapChiBaiViet),
    FOREIGN KEY (IDNguoiPhanBien) REFERENCES NguoiDung(IDNguoiDung)
);
GO


CREATE TABLE PhanCongBienTap (
    IDPhanCongBienTap INT IDENTITY(1,1) PRIMARY KEY,
    IDBienTapVien INT NOT NULL,
    IDTapChiBaiViet INT NOT NULL,
    NgayPhanCong DATE NOT NULL DEFAULT GETDATE(),
    GhiChu NVARCHAR(300) NULL,
	TrangThai INT DEFAULT 0,-- 0 chưa phản hồi, 1 nhận,2 từ chối
    FOREIGN KEY (IDBienTapVien) REFERENCES BienTapVien(IDBienTapVien),
    FOREIGN KEY (IDTapChiBaiViet) REFERENCES TapChiBaiViet(IDTapChiBaiViet)
);


CREATE TABLE LichSuChinhSua (
    ID INT IDENTITY PRIMARY KEY,
    IDTapChiBaiViet INT,
    NoiDungCu NVARCHAR(MAX),
    NoiDungMoi NVARCHAR(MAX),
    NgayChinhSua DATETIME DEFAULT GETDATE(),
	VongPhanBien INT DEFAULT 1,
    LanChinhSua INT DEFAULT 1,
    GhiChu NVARCHAR(500) NULL,
    DuongDanFile NVARCHAR(200) NULL, 
	IDNguoiChinhSua INT,
    constraint FK_LichSuChinhSua_TapChi FOREIGN KEY (IDTapChiBaiViet) REFERENCES TapChiBaiViet(IDTapChiBaiViet),
	CONSTRAINT FK_LichSuChinhSua_NguoiDung FOREIGN KEY (IDNguoiChinhSua) REFERENCES NguoiDung(IDNguoiDung)
);
CREATE TABLE SoTapChi (
    IDSoTapChi INT IDENTITY(1,1) PRIMARY KEY,
    TenSo NVARCHAR(100) NOT NULL,      -- Ví dụ: "Số 1/2024"
    ChuDe NVARCHAR(200) NULL,          -- Chủ đề chính của số tạp chí (nếu có)
    NgayPhatHanh DATE NOT NULL,
    MoTa NVARCHAR(300) NULL
);
-- Bảng Xuất Bản Tạp Chí
CREATE TABLE XuatBan (
    IDXuatBan INT IDENTITY(1,1) PRIMARY KEY,
    SoTapChi NVARCHAR(50) NOT NULL,
    NgayXuatBan DATE NOT NULL DEFAULT GETDATE(),
    IDTapChiBaiViet INT NOT NULL,
    IDBienTapVien INT NOT NULL,
	IDSoTapChi INT,
    FOREIGN KEY (IDTapChiBaiViet) REFERENCES TapChiBaiViet(IDTapChiBaiViet),
    FOREIGN KEY (IDBienTapVien) REFERENCES BienTapVien(IDBienTapVien),
	CONSTRAINT FK_XuatBan_SoTapChi
FOREIGN KEY (IDSoTapChi) REFERENCES SoTapChi(IDSoTapChi)
);
-- Chèn dữ liệu vào bảng VaiTro
/*INSERT INTO VaiTro (TenVaiTro) VALUES (N'Tác Giả');
INSERT INTO VaiTro (TenVaiTro) VALUES (N'Phản Biện');
*/
-- Kiểm tra dữ liệu đã được thêm vào chưa

select * from NguoiDung
select * from BienTapVien
select * from TapChiBaiViet
INSERT INTO SoTapChi (TenSo, ChuDe, NgayPhatHanh, MoTa)
VALUES (N'Số 1/2025', N'Khoa học và Công nghệ', '2025-01-01', N'Mô tả số 1');

USE QLTapChi;
GO

-- Insert into NguoiDung (Authors/Submitters)
INSERT INTO NguoiDung (HoTen, Email, MatKhau, SDT, DiaChi, QuocGia, ChucDanh, GioiTinh, ToChuc, PhanBien, IDLinhVuc)
VALUES 
    (N'Đặng Duy Thanh', 'dangduythanh@example.com', '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', '0123456781', N'Cần Thơ', N'Việt Nam', N'Giảng viên', 1, N'ĐH Nam Cần Thơ', 0, 1),
    (N'Đào Đình Kiên', 'daodinhkien@example.com', '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', '0123456782', N'Cần Thơ', N'Việt Nam', N'Nghiên cứu sinh', 1, N'ĐH Nam Cần Thơ', 0, 1),
    (N'Trần Thị D', 'tranthid@example.com', '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', '0123456783', N'Hồ Chí Minh', N'Việt Nam', N'Tiến sĩ', 0, N'ĐH Nông Lâm', 1, 1);

-- Insert into BienTapVien (Editors)
INSERT INTO BienTapVien (HoTen, Email, MatKhau, SDT, DiaChi, QuocGia, ChuyenNganh, LoaiBienTapVien)
VALUES 
    (N'Lê Thị B', 'lethib@example.com', '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', '0987654322', N'Hồ Chí Minh', N'Việt Nam', N'Nông nghiệp', N'BienTapVien'),
    (N'Phạm Văn C', 'phamvanc@example.com', '5994471abb01112afcc18159f6cc74b4f511b99806da59b3caf5a9c173cacfc5', '0987654323', N'Hà Nội', N'Việt Nam', N'Công nghệ', N'TongBienTap');

-- Insert into LinhVuc (Fields of Study)
INSERT INTO LinhVuc (TenLinhVuc)
VALUES 
    (N'Công nghệ Thông tin'),
    (N'Khoa học Môi trường');

-- Insert into SoTapChi (Journal Issues)
INSERT INTO SoTapChi (TenSo, ChuDe, NgayPhatHanh, MoTa)
VALUES 
    (N'Số 2/2025', N'Nông nghiệp và Phát triển Bền vững', '2025-03-01', N'Mô tả số 2'),
    (N'Số 3/2025', N'Công nghệ và Đổi mới', '2025-05-01', N'Mô tả số 3');

-- Insert into TapChiBaiViet (Articles)
-- Assuming IDLinhVuc=1 is 'Nông nghiệp', IDLinhVuc=3 is 'Công nghệ Thông tin', and NguoiDung IDs are auto-incremented starting from the last inserted ID.
INSERT INTO TapChiBaiViet (TieuDe, TacGia, DongTacGia, NoiDung, IDLinhVuc, TrangThai, NgayGui, TomTat, TuKhoa, IDNguoiGui, TrangThaiPhanBien)
VALUES 
    (N'Ứng dụng IoT trong nông nghiệp thông minh', 
     N'Đặng Duy Thanh', 
     N'Đặng Duy Thanh, Nguyễn Trúc Anh', 
     N'Content/BaiViet/iot_nongnghiep.pdf', 
     1, 
     3, -- Published
     '2025-02-01', 
     N'Bài báo nghiên cứu ứng dụng IoT để tối ưu hóa quy trình trồng trọt...', 
     N'IoT, nông nghiệp, công nghệ', 
     (SELECT MAX(IDNguoiDung) FROM NguoiDung WHERE Email = 'dangduythanh@example.com'), 
     2), -- Published after review
    (N'Phát triển hệ thống quản lý môi trường dựa trên AI', 
     N'Trần Thị D', 
     N'Trần Thị D, Đào Đình Kiên', 
     N'Content/BaiViet/ai_moitruong.pdf', 
     3, 
     3, -- Published
     '2025-04-01', 
     N'Bài báo đề xuất hệ thống AI để giám sát và quản lý chất lượng môi trường...', 
     N'AI, môi trường, công nghệ', 
     (SELECT MAX(IDNguoiDung) FROM NguoiDung WHERE Email = 'tranthid@example.com'), 
     2);

-- Insert into XuatBan (Publications)
-- Assuming BienTapVien IDs are auto-incremented, and SoTapChi IDs are from the newly inserted issues.
INSERT INTO XuatBan (SoTapChi, NgayXuatBan, IDTapChiBaiViet, IDBienTapVien, IDSoTapChi)
VALUES 
    (N'Số 2/2025', 
     '2025-03-01', 
     (SELECT MAX(IDTapChiBaiViet) FROM TapChiBaiViet WHERE TieuDe = N'Ứng dụng IoT trong nông nghiệp thông minh'), 
     (SELECT MAX(IDBienTapVien) FROM BienTapVien WHERE Email = 'lethib@example.com'), 
     (SELECT MAX(IDSoTapChi) FROM SoTapChi WHERE TenSo = N'Số 2/2025')),
    (N'Số 3/2025', 
     '2025-05-01', 
     (SELECT MAX(IDTapChiBaiViet) FROM TapChiBaiViet WHERE TieuDe = N'Phát triển hệ thống quản lý môi trường dựa trên AI'), 
     (SELECT MAX(IDBienTapVien) FROM BienTapVien WHERE Email = 'phamvanc@example.com'), 
     (SELECT MAX(IDSoTapChi) FROM SoTapChi WHERE TenSo = N'Số 3/2025'));