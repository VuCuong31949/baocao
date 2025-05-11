using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class SoTapChiController : Controller
    {
        // GET: Admin/SoTapChi
        private QLTapChiEntities db = new QLTapChiEntities();

        // GET: Admin/SoTapChi/DanhSach
        public ActionResult DanhSach()
        {
            // Kiểm tra đăng nhập và vai trò
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
            {
                TempData["Error"] = "Bạn không có quyền truy cập danh sách này.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            var danhSachSoTapChi = db.SoTapChis.OrderByDescending(s => s.NgayPhatHanh).ToList();
            return View(danhSachSoTapChi);
        }

        // GET: Admin/SoTapChi/Them
        public ActionResult Them()
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
            {
                TempData["Error"] = "Bạn không có quyền thêm số tạp chí.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            return View();
        }

        // POST: Admin/SoTapChi/Them
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Them(SoTapChi model)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.SoTapChis.Add(model);
                    db.SaveChanges();
                    TempData["Success"] = "Thêm số tạp chí thành công.";
                    return RedirectToAction("DanhSach");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return View(model);
        }

        // GET: Admin/SoTapChi/Sua
        public ActionResult Sua(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (Session["LoaiBienTapVien"] == null || (Session["LoaiBienTapVien"].ToString() != "TongBienTap" && Session["LoaiBienTapVien"].ToString() != "BienTapVien"))
            {
                TempData["Error"] = "Bạn không có quyền sửa số tạp chí.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            var soTapChi = db.SoTapChis.Find(id);
            if (soTapChi == null)
            {
                TempData["Error"] = "Không tìm thấy số tạp chí.";
                return RedirectToAction("DanhSach");
            }

            return View(soTapChi);
        }

        // POST: Admin/SoTapChi/Sua
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(SoTapChi model)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var soTapChi = db.SoTapChis.Find(model.IDSoTapChi);
                    if (soTapChi == null)
                    {
                        TempData["Error"] = "Không tìm thấy số tạp chí.";
                        return RedirectToAction("DanhSach");
                    }

                    soTapChi.TenSo = model.TenSo;
                    soTapChi.ChuDe = model.ChuDe;
                    soTapChi.NgayPhatHanh = model.NgayPhatHanh;
                    soTapChi.MoTa = model.MoTa;

                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật số tạp chí thành công.";
                    return RedirectToAction("DanhSach");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return View(model);
        }

        // POST: Admin/SoTapChi/Xoa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Xoa(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            try
            {
                var soTapChi = db.SoTapChis.Find(id);
                if (soTapChi == null)
                {
                    TempData["Error"] = "Không tìm thấy số tạp chí.";
                    return RedirectToAction("DanhSach");
                }

                // Kiểm tra xem số tạp chí có đang được sử dụng trong bảng XuatBan
                if (db.XuatBans.Any(xb => xb.IDSoTapChi == id))
                {
                    TempData["Error"] = "Không thể xóa số tạp chí này vì đã có bài viết xuất bản.";
                    return RedirectToAction("DanhSach");
                }

                db.SoTapChis.Remove(soTapChi);
                db.SaveChanges();
                TempData["Success"] = "Xóa số tạp chí thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("DanhSach");
        }
    }
}