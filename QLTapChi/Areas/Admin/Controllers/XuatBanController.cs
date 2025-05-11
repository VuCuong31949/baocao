using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class XuatBanController : Controller
    {
        // GET: Admin/XuatBan

        QLTapChiEntities db = new QLTapChiEntities();

        // Action: Lấy danh sách các bài viết đã phản biện để biên tập viên xem xét xuất bản
        //public ActionResult DanhSachDaPhanBien()
        //{
        //    // Kiểm tra đăng nhập
        //    if (Session["idUser"] == null)
        //    {
        //        TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
        //        return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        //    }

        //    // Kiểm tra vai trò: Chỉ cho phép biên tập viên truy cập
        //    if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
        //    {
        //        TempData["Error"] = "Bạn không có quyền truy cập danh sách này.";
        //        return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        //    }

        //    // Truy vấn danh sách các bài viết đã được phản biện
        //    var danhSachDaPhanBien = (from tcbv in db.TapChiBaiViets
        //                              join lv in db.LinhVucs on tcbv.IDLinhVuc equals lv.IDLinhVuc
        //                              where tcbv.TrangThaiPhanBien.HasValue && tcbv.TrangThaiPhanBien != 0
        //                              select new DanhSachXuatBanViewModel
        //                              {
        //                                  IDTapChiBaiViet = tcbv.IDTapChiBaiViet,
        //                                  TieuDe = tcbv.TieuDe,
        //                                  TacGia = tcbv.TacGia,
        //                                  TenLinhVuc = lv.TenLinhVuc,
        //                                  TrangThaiPhanBien = tcbv.TrangThaiPhanBien ?? 0,
        //                                  VongPhanBien = (from pc in db.PhanCongs
        //                                                  where pc.IDTapChiBaiViet == tcbv.IDTapChiBaiViet
        //                                                  select pc.VongPhanBien).Max() ?? 0,
        //                                  NgayPhanBienMoiNhat = db.PhanBiens
        //                                                      .Where(pb => pb.IDTapChiBaiViet == tcbv.IDTapChiBaiViet)
        //                                                      .Select(pb => (DateTime?)pb.NgayPhanBien)
        //                                                      .DefaultIfEmpty()
        //                                                      .Max()
        //                              })
        //                   .OrderByDescending(x => x.NgayPhanBienMoiNhat)
        //                   .ToList();

        //    // Tổng hợp kết quả phản biện và chuyển đổi trạng thái
        //    foreach (var item in danhSachDaPhanBien)
        //    {
        //        // Lấy tất cả trạng thái phản biện trong vòng mới nhất
        //        var trangThaiPhanBiens = db.PhanCongs
        //            .Where(pc => pc.IDTapChiBaiViet == item.IDTapChiBaiViet && pc.VongPhanBien == item.VongPhanBien)
        //            .Select(pc => pc.TrangThaiPhanBien)
        //            .ToList();

        //        // Tổng hợp kết quả phản biện
        //        if (trangThaiPhanBiens.All(t => t == 1))
        //            item.KetQuaPhanBien = "Tất cả đạt";
        //        else if (trangThaiPhanBiens.Any(t => t == 3))
        //            item.KetQuaPhanBien = "Có sửa đổi nhỏ";
        //        else if (trangThaiPhanBiens.Any(t => t == 4))
        //            item.KetQuaPhanBien = "Có sửa đổi lớn";
        //        else if (trangThaiPhanBiens.Any(t => t == 2))
        //            item.KetQuaPhanBien = "Có không đạt";
        //        else
        //            item.KetQuaPhanBien = "Chưa hoàn tất";

        //        // Chuyển đổi trạng thái tổng quát
        //        item.TrangThaiPhanBienText = GetTrangThaiPhanBienText(item.TrangThaiPhanBien);
        //    }

        //    return View(danhSachDaPhanBien);
        //}

        //// GET: Admin/PhanBien/XuatBan
        //[HttpGet]
        //public ActionResult XuatBan(int id)
        //{
        //    // Kiểm tra đăng nhập và vai trò
        //    if (Session["idUser"] == null)
        //    {
        //        TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
        //        return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        //    }

        //    if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
        //    {
        //        TempData["Error"] = "Bạn không có quyền xuất bản bài viết.";
        //        return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        //    }

        //    // Tìm bài viết
        //    var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
        //    if (baiViet == null)
        //    {
        //        TempData["Error"] = "Không tìm thấy bài viết.";
        //        return RedirectToAction("DanhSachDaPhanBien");
        //    }

        //    // Kiểm tra trạng thái bài viết: Chỉ cho phép xuất bản nếu TrangThaiPhanBien = 2 (Đạt, chờ xuất bản)
        //    if (baiViet.TrangThaiPhanBien != 2)
        //    {
        //        TempData["Error"] = "Bài viết chưa đạt trạng thái để xuất bản.";
        //        return RedirectToAction("DanhSachDaPhanBien");
        //    }

        //    // Lấy danh sách số tạp chí để hiển thị trong form
        //    ViewBag.DanhSachSoTapChi = db.SoTapChis
        //        .Select(s => new SelectListItem
        //        {
        //            Value = s.IDSoTapChi.ToString(),
        //            Text = s.TenSo
        //        })
        //        .ToList();

        //    ViewBag.IDTapChiBaiViet = id;
        //    return View();
        //}

        //// POST: Admin/PhanBien/XuatBan
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult XuatBan(int IDTapChiBaiViet, int IDSoTapChi)
        //{
        //    // Kiểm tra đăng nhập và vai trò
        //    if (Session["idUser"] == null)
        //    {
        //        TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
        //        return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        //    }

        //    int idBienTapVien = (int)Session["idUser"];

        //    // Tìm bài viết
        //    var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == IDTapChiBaiViet);
        //    if (baiViet == null)
        //    {
        //        TempData["Error"] = "Không tìm thấy bài viết.";
        //        return RedirectToAction("DanhSachDaPhanBien");
        //    }

        //    // Kiểm tra trạng thái bài viết
        //    if (baiViet.TrangThaiPhanBien != 2)
        //    {
        //        TempData["Error"] = "Bài viết chưa đạt trạng thái để xuất bản.";
        //        return RedirectToAction("DanhSachDaPhanBien");
        //    }

        //    // Tìm số tạp chí
        //    var soTapChi = db.SoTapChis.FirstOrDefault(s => s.IDSoTapChi == IDSoTapChi);
        //    if (soTapChi == null)
        //    {
        //        TempData["Error"] = "Không tìm thấy số tạp chí.";
        //        return RedirectToAction("DanhSachDaPhanBien");
        //    }

        //    // Tạo bản ghi xuất bản
        //    var xuatBan = new XuatBan
        //    {
        //        SoTapChi = soTapChi.TenSo,
        //        NgayXuatBan = DateTime.Now,
        //        IDTapChiBaiViet = IDTapChiBaiViet,
        //        IDBienTapVien = idBienTapVien,
        //        IDSoTapChi = IDSoTapChi
        //    };

        //    // Cập nhật trạng thái bài viết
        //    baiViet.TrangThai = 4; // Xuất bản
        //    baiViet.TrangThaiPhanBien = 2; // Giữ trạng thái "Đạt, chờ xuất bản"

        //    // Lưu thay đổi
        //    db.XuatBans.Add(xuatBan);
        //    db.SaveChanges();

        //    TempData["Success"] = "Xuất bản bài viết thành công.";
        //    return RedirectToAction("DanhSachDaPhanBien");
        //}

        // Hàm chuyển đổi trạng thái phản biện sang văn bản

        public ActionResult DanhSachDaPhanBien()
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            // Kiểm tra vai trò: Chỉ cho phép biên tập viên truy cập
            if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
            {
                TempData["Error"] = "Bạn không có quyền truy cập danh sách này.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            // Truy vấn danh sách các bài viết đã được phản biện
            var danhSachDaPhanBien = (from tcbv in db.TapChiBaiViets
                                      join lv in db.LinhVucs on tcbv.IDLinhVuc equals lv.IDLinhVuc
                                      where tcbv.TrangThaiPhanBien.HasValue && tcbv.TrangThaiPhanBien != 0
                                      select new DanhSachXuatBanViewModel
                                      {
                                          IDTapChiBaiViet = tcbv.IDTapChiBaiViet,
                                          TieuDe = tcbv.TieuDe,
                                          TacGia = tcbv.TacGia,
                                          TenLinhVuc = lv.TenLinhVuc,
                                          TrangThaiPhanBien = tcbv.TrangThaiPhanBien ?? 0,
                                          VongPhanBien = (from pc in db.PhanCongs
                                                          where pc.IDTapChiBaiViet == tcbv.IDTapChiBaiViet
                                                          select pc.VongPhanBien).Max() ?? 0,
                                          NgayPhanBienMoiNhat = db.PhanBiens
                                                              .Where(pb => pb.IDTapChiBaiViet == tcbv.IDTapChiBaiViet)
                                                              .Select(pb => (DateTime?)pb.NgayPhanBien)
                                                              .DefaultIfEmpty()
                                                              .Max(),
                                          SoTapChi = db.XuatBans
                                                      .Where(xb => xb.IDTapChiBaiViet == tcbv.IDTapChiBaiViet)
                                                      .Select(xb => xb.SoTapChi)
                                                      .FirstOrDefault()
                                      })
                           .OrderByDescending(x => x.NgayPhanBienMoiNhat)
                           .ToList();

            // Tổng hợp kết quả phản biện và chuyển đổi trạng thái
            foreach (var item in danhSachDaPhanBien)
            {
                // Lấy tất cả trạng thái phản biện trong vòng mới nhất
                var trangThaiPhanBiens = db.PhanCongs
                    .Where(pc => pc.IDTapChiBaiViet == item.IDTapChiBaiViet && pc.VongPhanBien == item.VongPhanBien)
                    .Select(pc => pc.TrangThaiPhanBien)
                    .ToList();

                // Tổng hợp kết quả phản biện
                if (trangThaiPhanBiens.All(t => t == 1))
                    item.KetQuaPhanBien = "Tất cả đạt";
                else if (trangThaiPhanBiens.Any(t => t == 3))
                    item.KetQuaPhanBien = "Có sửa đổi nhỏ";
                else if (trangThaiPhanBiens.Any(t => t == 4))
                    item.KetQuaPhanBien = "Có sửa đổi lớn";
                else if (trangThaiPhanBiens.Any(t => t == 2))
                    item.KetQuaPhanBien = "Có không đạt";
                else
                    item.KetQuaPhanBien = "Chưa hoàn tất";

                // Chuyển đổi trạng thái tổng quát
                item.TrangThaiPhanBienText = GetTrangThaiPhanBienText(item.TrangThaiPhanBien);
            }

            // Thêm ViewBag.DanhSachSoTapChi để hỗ trợ modal chọn số tạp chí
            ViewBag.DanhSachSoTapChi = db.SoTapChis
                .Select(s => new SelectListItem
                {
                    Value = s.IDSoTapChi.ToString(),
                    Text = s.TenSo
                })
                .ToList();

            return View(danhSachDaPhanBien);
        }

        public ActionResult XuatBan(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
            {
                TempData["Error"] = "Bạn không có quyền xuất bản bài viết.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            if (baiViet.TrangThaiPhanBien != 2)
            {
                TempData["Error"] = "Bài viết chưa đạt trạng thái để xuất bản.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            if (db.XuatBans.Any(xb => xb.IDTapChiBaiViet == id))
            {
                TempData["Error"] = "Bài viết đã được xuất bản.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            ViewBag.DanhSachSoTapChi = db.SoTapChis
                .Select(s => new SelectListItem
                {
                    Value = s.IDSoTapChi.ToString(),
                    Text = s.TenSo
                })
                .ToList();

            ViewBag.SelectedArticleId = id;
            return View("DanhSachDaPhanBien");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XuatBanNhieuBai(string baiVietIds, int IDSoTapChi)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
            {
                TempData["Error"] = "Bạn không có quyền xuất bản bài viết.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (string.IsNullOrEmpty(baiVietIds))
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một bài viết.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            var ids = baiVietIds.Split(',').Select(int.Parse).ToArray();
            if (!ids.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một bài viết.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            var soTapChi = db.SoTapChis.FirstOrDefault(s => s.IDSoTapChi == IDSoTapChi);
            if (soTapChi == null)
            {
                TempData["Error"] = "Không tìm thấy số tạp chí.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            int idBienTapVien = (int)Session["idUser"];
            foreach (var id in ids)
            {
                var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
                if (baiViet != null && baiViet.TrangThaiPhanBien == 2 && !db.XuatBans.Any(xb => xb.IDTapChiBaiViet == id))
                {
                    var xuatBan = new XuatBan
                    {
                        SoTapChi = soTapChi.TenSo,
                        NgayXuatBan = DateTime.Now,
                        IDTapChiBaiViet = id,
                        IDBienTapVien = idBienTapVien,
                        IDSoTapChi = IDSoTapChi
                    };

                    baiViet.TrangThai = 3; // Xuất bản
                    baiViet.TrangThaiPhanBien = 5; // Đã xuất bản

                    db.XuatBans.Add(xuatBan);
                }
            }

            db.SaveChanges();
            TempData["Success"] = "Xuất bản nhiều bài viết thành công.";
            return RedirectToAction("DanhSachDaPhanBien");
        }

        private string GetTrangThaiPhanBienText(int? trangThai)
        {
            if (!trangThai.HasValue) return "Chưa xác định";
            switch (trangThai.Value)
            {
                case 0: return "Chờ phản biện";
                case 1: return "Đang phản biện";
                case 2: return "Đạt, chờ xuất bản";
                case 3: return "Không đạt, chờ chỉnh sửa";
                case 4: return "Từ chối";
                case 5: return "Đã xuất bản"; // Thêm trạng thái 5
                default: return "Không xác định";
            }
        }
        // GET: Admin/PhanBien/DownloadFile
        public ActionResult DownloadFile(int id)
        {
            var baiBao = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiBao == null)
            {
                return HttpNotFound();
            }

            string filePath = Server.MapPath("~/" + baiBao.NoiDung);
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            string fileName = Path.GetFileName(filePath);
            return File(filePath, "application/octet-stream", fileName);
        }

    }
}