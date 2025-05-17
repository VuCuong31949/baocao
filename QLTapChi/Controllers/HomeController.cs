using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QLTapChi.Controllers
{
    public class HomeController : BaseController
    {
        public QLTapChiEntities db = new QLTapChiEntities();


        //public ActionResult Index(string search = null)
        //{
        //    // Lấy tất cả bài viết đã xuất bản (TrangThai = 3)
        //    var query = db.TapChiBaiViets
        //        .Join(db.XuatBans,
        //            baiViet => baiViet.IDTapChiBaiViet,
        //            xuatBan => xuatBan.IDTapChiBaiViet,
        //            (baiViet, xuatBan) => new { baiViet, xuatBan })
        //        .Join(db.SoTapChis,
        //            b => b.xuatBan.IDSoTapChi,
        //            so => so.IDSoTapChi,
        //            (b, so) => new
        //            {
        //                b.baiViet,
        //                b.xuatBan,
        //                so.TenSo
        //            })
        //        .Where(b => b.baiViet.TrangThai == 3); // Bài viết đã xuất bản

        //    // Nếu có từ khóa tìm kiếm, lọc bài viết theo TacGia, TieuDe, hoặc TomTat (không phân biệt hoa thường)
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        search = search.ToLower(); // Chuyển từ khóa tìm kiếm thành chữ thường
        //        query = query.Where(b => b.baiViet.TacGia.ToLower().Contains(search) ||
        //                                 b.baiViet.TieuDe.ToLower().Contains(search) ||
        //                                 b.baiViet.TomTat.ToLower().Contains(search));
        //    }

        //    var baiVietDaXuatBan = query
        //        .OrderByDescending(b => b.xuatBan.NgayXuatBan) // Sắp xếp theo ngày xuất bản giảm dần
        //        .Select(b => new
        //        {
        //            b.baiViet.IDTapChiBaiViet,
        //            b.baiViet.TieuDe,
        //            b.baiViet.TacGia,
        //            b.baiViet.DongTacGia,
        //            b.baiViet.TomTat,
        //            b.baiViet.NoiDung, // Đường dẫn file PDF
        //            b.TenSo
        //        })
        //        .ToList();

        //    // Thông tin số tạp chí hiện tại (số mới nhất)
        //    var soTapChiHienTai = db.SoTapChis
        //        .OrderByDescending(s => s.NgayPhatHanh)
        //        .FirstOrDefault();

        //    ViewBag.SoTapChiHienTai = soTapChiHienTai != null
        //        ? $"Số {soTapChiHienTai.TenSo} (Tập. 61 Số. 1 - 2025)"
        //        : "Chưa có số tạp chí";

        //    // Chuyển dữ liệu sang View
        //    var model = baiVietDaXuatBan.Select(b => new TapChiBaiViet
        //    {
        //        IDTapChiBaiViet = b.IDTapChiBaiViet,
        //        TieuDe = b.TieuDe,
        //        TacGia = b.TacGia,
        //        DongTacGia = b.DongTacGia,
        //        TomTat = b.TomTat,
        //        NoiDung = b.NoiDung // Đường dẫn file PDF
        //    }).ToList();

        //    // Truyền từ khóa tìm kiếm vào ViewBag
        //    ViewBag.Search = search;

        //    return View(model);
        //}
        //public ActionResult Index(string search = null)
        //{
        //    // Lấy ngày hiện tại
        //    DateTime ngayHienTai = DateTime.Now;

        //    // Lấy tất cả bài viết đã xuất bản (TrangThai = 3) và kiểm tra ngày xuất bản
        //    var query = db.TapChiBaiViets
        //        .Join(db.XuatBans,
        //            baiViet => baiViet.IDTapChiBaiViet,
        //            xuatBan => xuatBan.IDTapChiBaiViet,
        //            (baiViet, xuatBan) => new { baiViet, xuatBan })
        //        .Join(db.SoTapChis,
        //            b => b.xuatBan.IDSoTapChi,
        //            so => so.IDSoTapChi,
        //            (b, so) => new
        //            {
        //                b.baiViet,
        //                b.xuatBan,
        //                so.TenSo
        //            })
        //        .Where(b => b.baiViet.TrangThai == 3 && b.xuatBan.NgayXuatBan <= ngayHienTai); // Chỉ lấy bài viết đã đến ngày xuất bản

        //    // Nếu có từ khóa tìm kiếm, lọc bài viết theo TacGia, TieuDe, hoặc TomTat (không phân biệt hoa thường)
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        search = search.ToLower(); // Chuyển từ khóa tìm kiếm thành chữ thường
        //        query = query.Where(b => b.baiViet.TacGia.ToLower().Contains(search) ||
        //                                 b.baiViet.TieuDe.ToLower().Contains(search) ||
        //                                 b.baiViet.TomTat.ToLower().Contains(search));
        //    }

        //    var baiVietDaXuatBan = query
        //        .OrderByDescending(b => b.xuatBan.NgayXuatBan) // Sắp xếp theo ngày xuất bản giảm dần
        //        .Select(b => new
        //        {
        //            b.baiViet.IDTapChiBaiViet,
        //            b.baiViet.TieuDe,
        //            b.baiViet.TacGia,
        //            b.baiViet.DongTacGia,
        //            b.baiViet.TomTat,
        //            b.baiViet.NoiDung, // Đường dẫn file PDF
        //            b.TenSo
        //        })
        //        .ToList();

        //    // Thông tin số tạp chí hiện tại (số mới nhất)
        //    var soTapChiHienTai = db.SoTapChis
        //        .OrderByDescending(s => s.NgayPhatHanh)
        //        .FirstOrDefault();

        //    ViewBag.SoTapChiHienTai = soTapChiHienTai != null
        //        ? $"Số {soTapChiHienTai.TenSo} "
        //        : "Chưa có số tạp chí";

        //    // Chuyển dữ liệu sang View
        //    var model = baiVietDaXuatBan.Select(b => new TapChiBaiViet
        //    {
        //        IDTapChiBaiViet = b.IDTapChiBaiViet,
        //        TieuDe = b.TieuDe,
        //        TacGia = b.TacGia,
        //        DongTacGia = b.DongTacGia,
        //        TomTat = b.TomTat,
        //        NoiDung = b.NoiDung // Đường dẫn file PDF
        //    }).ToList();

        //    // Truyền từ khóa tìm kiếm vào ViewBag
        //    ViewBag.Search = search;

        //    return View(model);
        //}
        public ActionResult Index(string search = null)
        {
            // Lấy ngày hiện tại
            DateTime ngayHienTai = DateTime.Now; // 07:23 AM +07, ngày 17/05/2025

            // Lấy tất cả bài viết đã xuất bản (TrangThai = 3) và kiểm tra ngày xuất bản
            var query = db.TapChiBaiViets
                .Join(db.XuatBans,
                    baiViet => baiViet.IDTapChiBaiViet,
                    xuatBan => xuatBan.IDTapChiBaiViet,
                    (baiViet, xuatBan) => new { baiViet, xuatBan })
                .Join(db.SoTapChis,
                    b => b.xuatBan.IDSoTapChi,
                    so => so.IDSoTapChi,
                    (b, so) => new
                    {
                        b.baiViet,
                        b.xuatBan,
                        SoTapChi = so // Giữ nguyên đối tượng SoTapChi
                    })
                .Where(b => b.baiViet.TrangThai == 3 && b.xuatBan.NgayXuatBan <= ngayHienTai);

            // Nếu có từ khóa tìm kiếm, lọc bài viết theo TacGia, TieuDe, hoặc TomTat (không phân biệt hoa thường)
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower(); // Chuyển từ khóa tìm kiếm thành chữ thường
                query = query.Where(b => b.baiViet.TacGia.ToLower().Contains(search) ||
                                         b.baiViet.TieuDe.ToLower().Contains(search) ||
                                         b.baiViet.TomTat.ToLower().Contains(search));
            }

            var baiVietDaXuatBan = query
                .OrderByDescending(b => b.xuatBan.NgayXuatBan) // Sắp xếp theo ngày xuất bản giảm dần
                .Select(b => new
                {
                    b.baiViet.IDTapChiBaiViet,
                    b.baiViet.TieuDe,
                    b.baiViet.TacGia,
                    b.baiViet.DongTacGia,
                    b.baiViet.TomTat,
                    b.baiViet.NoiDung, // Đường dẫn file PDF
                    TenSo = b.SoTapChi.TenSo // Trích xuất TenSo từ SoTapChi
                })
                .ToList();

            // Lấy số tạp chí hiện tại dựa trên các bài viết đã xuất bản, chỉ lấy số đã phát hành
            var soTapChiHienTai = query
                .Select(b => b.SoTapChi) // Trích xuất đối tượng SoTapChi
                .Distinct() // Loại bỏ các số trùng lặp
                .Where(s => s.NgayPhatHanh <= ngayHienTai) // Chỉ lấy số đã phát hành
                .OrderByDescending(s => s.NgayPhatHanh) // Lấy số gần nhất đã phát hành
                .FirstOrDefault();

            ViewBag.SoTapChiHienTai = soTapChiHienTai != null
                ? $"Số {soTapChiHienTai.TenSo} (Phát hành ngày {soTapChiHienTai.NgayPhatHanh.ToString("dd-MM-yyyy")})"
                : "Chưa có số tạp chí đã phát hành";

            // Chuyển dữ liệu sang View
            var model = baiVietDaXuatBan.Select(b => new TapChiBaiViet
            {
                IDTapChiBaiViet = b.IDTapChiBaiViet,
                TieuDe = b.TieuDe,
                TacGia = b.TacGia,
                DongTacGia = b.DongTacGia,
                TomTat = b.TomTat,
                NoiDung = b.NoiDung // Đường dẫn file PDF
            }).ToList();

            // Truyền từ khóa tìm kiếm vào ViewBag
            ViewBag.Search = search;

            return View(model);
        }
        public ActionResult Details(int id)
        {
            // Kiểm tra đăng nhập (nếu cần)
            // if (Session["idUser"] == null) { ... }

            // Lấy thông tin bài viết theo ID, sử dụng DTO để tránh lỗi
            var baiVietDTO = (from bv in db.TapChiBaiViets
                              join xb in db.XuatBans on bv.IDTapChiBaiViet equals xb.IDTapChiBaiViet into xbGroup
                              from xb in xbGroup.DefaultIfEmpty()
                              where bv.IDTapChiBaiViet == id && bv.TrangThai == 3
                              select new TapChiBaiVietDTO
                              {
                                  IDTapChiBaiViet = bv.IDTapChiBaiViet,
                                  TieuDe = bv.TieuDe,
                                  TacGia = bv.TacGia,
                                  DongTacGia = bv.DongTacGia,
                                  TomTat = bv.TomTat,
                                  TuKhoa = bv.TuKhoa,
                                  NoiDung = bv.NoiDung,
                                  NgayGui = bv.NgayGui,
                                  NgayXuatBan = xb != null ? xb.NgayXuatBan : (DateTime?)null // Lấy ngày xuất bản nếu có
                              })
                              .FirstOrDefault();

            if (baiVietDTO == null)
            {
                return HttpNotFound();
            }

            // Map DTO sang TapChiBaiViet để sử dụng trong View (nếu cần)
            var baiViet = new TapChiBaiViet
            {
                IDTapChiBaiViet = baiVietDTO.IDTapChiBaiViet,
                TieuDe = baiVietDTO.TieuDe,
                TacGia = baiVietDTO.TacGia,
                DongTacGia = baiVietDTO.DongTacGia,
                TomTat = HttpUtility.HtmlDecode(baiVietDTO.TomTat),
                TuKhoa = baiVietDTO.TuKhoa,
                NoiDung = baiVietDTO.NoiDung,
                NgayGui = baiVietDTO.NgayGui
            };

            // Truyền thêm ngày xuất bản vào ViewBag nếu có
            ViewBag.NgayXuatBan = baiVietDTO.NgayXuatBan?.ToString("dd-MM-yyyy");

            return View(baiViet);
        }
        public ActionResult HoiDongBienTap()
        {
            var bienTapViens = db.BienTapViens.ToList();

            var tongBienTap = bienTapViens.FirstOrDefault(b => b.LoaiBienTapVien == "TongBienTap");
            var phoTongBienTap = bienTapViens.FirstOrDefault(b => b.LoaiBienTapVien == "PhoTongBienTap");

            var chuyenNganhs = bienTapViens
                .GroupBy(b => b.ChuyenNganh)
                .Select(g => new ChuyenNganhViewModel
                {
                    TenChuyenNganh = g.Key,
                    ThanhViens = g.OrderBy(b => b.HoTen).ToList()
                })
                .OrderBy(c => c.TenChuyenNganh)
                .ToList();

            var viewModel = new HoiDongBienTapViewModel
            {
                TongBienTap = tongBienTap,
                PhoTongBienTap = phoTongBienTap,
                ChuyenNganhs = chuyenNganhs
            };

            return View(viewModel);
        }

    }
}