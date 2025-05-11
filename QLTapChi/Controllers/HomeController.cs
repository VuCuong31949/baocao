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

        // GET: Home/Index
        //public ActionResult Index()
        //{
        //    // Lấy tất cả bài viết đã xuất bản (TrangThai = 3) và sắp xếp theo NgayXuatBan
        //    var baiVietDaXuatBan = db.TapChiBaiViets
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
        //        .Where(b => b.baiViet.TrangThai == 3) // Bài viết đã xuất bản
        //        .OrderByDescending(b => b.xuatBan.NgayXuatBan) // Sắp xếp theo ngày xuất bản giảm dần
        //        .Select(b => new
        //        {
        //            b.baiViet.IDTapChiBaiViet,
        //            b.baiViet.TieuDe,
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
        //        DongTacGia = b.DongTacGia,
        //        TomTat = b.TomTat,
        //        NoiDung = b.NoiDung // Đường dẫn file PDF
        //    }).ToList();

        //    return View(model);
        //}
        public ActionResult Index(string search = null)
        {
            // Lấy tất cả bài viết đã xuất bản (TrangThai = 3)
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
                        so.TenSo
                    })
                .Where(b => b.baiViet.TrangThai == 3); // Bài viết đã xuất bản

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
                    b.TenSo
                })
                .ToList();

            // Thông tin số tạp chí hiện tại (số mới nhất)
            var soTapChiHienTai = db.SoTapChis
                .OrderByDescending(s => s.NgayPhatHanh)
                .FirstOrDefault();

            ViewBag.SoTapChiHienTai = soTapChiHienTai != null
                ? $"Số {soTapChiHienTai.TenSo} (Tập. 61 Số. 1 - 2025)"
                : "Chưa có số tạp chí";

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
          

            // Lấy thông tin bài viết theo ID, sử dụng DTO để tránh lỗi
            var baiVietDTO = db.TapChiBaiViets
                .Where(bv => bv.IDTapChiBaiViet == id && bv.TrangThai == 3)
                .Select(bv => new TapChiBaiVietDTO
                {
                    IDTapChiBaiViet = bv.IDTapChiBaiViet,
                    TieuDe = bv.TieuDe,
                    TacGia = bv.TacGia,
                    DongTacGia = bv.DongTacGia,
                    TomTat = bv.TomTat,
                    TuKhoa = bv.TuKhoa,
                    NoiDung = bv.NoiDung,
                    NgayGui = bv.NgayGui
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
                TomTat = baiVietDTO.TomTat,
                TuKhoa = baiVietDTO.TuKhoa,
                NoiDung = baiVietDTO.NoiDung,
                NgayGui = baiVietDTO.NgayGui
            };

            return View(baiViet);
        }
        
    }
}