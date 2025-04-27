using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        QLTapChiEntities db = new QLTapChiEntities();
        public ActionResult DanhSachTaiKhoan()
        {
            var tk = db.NguoiDungs.ToList();
            return View(tk);

        }
        public ActionResult ThemTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemTaiKhoan(NguoiDung _user, string MatKhauLai)
        {
            if (ModelState.IsValid)
            {
               
                bool checkEmail = db.NguoiDungs.Any(s => s.Email == _user.Email);
                bool checkSdt = db.NguoiDungs.Any(s => s.SDT == _user.SDT);
                if ( checkEmail || checkSdt)
                {
                    
                    if (checkEmail)
                    {
                        ViewBag.erroremail = "* Email đã tồn tại!";
                    }
                    if (checkSdt)
                    {
                        ViewBag.errorsdt = "* Số điện thoại đã được đăng ký!";
                    }
                    return View();
                }
                // Kiểm tra mật khẩu khớp
                if (_user.MatKhau != MatKhauLai)
                {
                    ViewBag.errorMatKhauLai = "* Mật khẩu nhập lại không khớp!";
                    return View();
                }
               
                // Mã hóa mật khẩu
                _user.MatKhau = Hashing.ToSHA256(_user.MatKhau);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.NguoiDungs.Add(_user);
                db.SaveChanges(); 

                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            return View();
        }

        public ActionResult TKCaNhan(int id)
        {
            NguoiDung timkiemUser = db.NguoiDungs.Find(id);
            return View(timkiemUser);
        }
        public ActionResult CapNhatTaiKhoan(int id)
        {
            NguoiDung timkiemUser = db.NguoiDungs.Find(id);
            return View(timkiemUser);
        }
        [HttpPost]
        public ActionResult CapNhatTaiKhoan(NguoiDung model)
        {
            NguoiDung EditUser = db.NguoiDungs.Find(model.IDNguoiDung);
            // Kiểm tra tên đăng nhập trùng lặp
            var checkTenDangNhap = db.NguoiDungs.Any(u => u.HoTen == model.HoTen && u.IDNguoiDung != model.IDNguoiDung);
            if (checkTenDangNhap)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(EditUser);
            }
            // Kiểm tra số điện thoại trùng lặp
            var checkSDT = db.NguoiDungs.Any(u => u.SDT == model.SDT && u.IDNguoiDung != model.IDNguoiDung);
            if (checkSDT)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được sử dụng.");
                return View(EditUser);
            }
            EditUser.HoTen = model.HoTen;
            // Kiểm tra nếu mật khẩu đã được thay đổi, sau đó mã hóa mật khẩu
            if (!string.IsNullOrEmpty(model.MatKhau) && EditUser.MatKhau != model.MatKhau)
            {
                EditUser.MatKhau = Hashing.ToSHA256(model.MatKhau); // Mã hóa mật khẩu trước khi lưu
            }
           EditUser.DiaChi = model.DiaChi;
            EditUser.Email = model.Email;
            EditUser.SDT = model.SDT;
            EditUser.IDLinhVuc = model.IDLinhVuc;
            EditUser.ChucDanh = model.ChucDanh;
            EditUser.GioiTinh = model.GioiTinh;
            EditUser.QuocGia = model.QuocGia;
            EditUser.ToChuc = model.ToChuc;
            EditUser.PhanBien = model.PhanBien;
            db.SaveChanges();
            return RedirectToAction("TKCaNhan", "TaiKhoan", new { id = model.IDNguoiDung });
        }
        public ActionResult XoaTaiKhoan(int id)
        {
            // Tìm đối tượng xóa
            var deleteUser = db.NguoiDungs.Find(id);
            // Xóa
            if (deleteUser != null)
            {
                db.NguoiDungs.Remove(deleteUser);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachTaiKhoan");
        }

        public ActionResult DangNhap()
        {
            return View("DangNhap");
        }
        //[HttpPost]
        //public ActionResult DangNhap(string email, string matkhau)
        //{
        //    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau))
        //    {
        //        ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
        //        return View();
        //    }

        //    var f_matkhau = Hashing.ToSHA256(matkhau);

        //    // 1. Kiểm tra Biên Tập Viên
        //    var btv = db.BienTapViens.FirstOrDefault(s => s.Email.Trim() == email.Trim() && s.MatKhau.Equals(f_matkhau));
        //    if (btv != null)
        //    {
        //        Session["UserName"] = btv.HoTen;
        //        Session["idUser"] = btv.IDBienTapVien;
        //        Session["LoaiNguoiDung"] = "BienTapVien"; // Có thể dùng để phân quyền view
        //        Session["LoaiBienTapVien"] = btv.LoaiBienTapVien; // Tổng hoặc PhuTrach
        //        CookieHelper.create("UserName", btv.HoTen, DateTime.Now.AddDays(1));
        //        CookieHelper.create("UserId", btv.IDBienTapVien.ToString(), DateTime.Now.AddDays(1));
        //        CookieHelper.create("LoaiNguoiDung", "BienTapVien", DateTime.Now.AddDays(1));
        //        CookieHelper.create("LoaiBienTapVien", btv.LoaiBienTapVien, DateTime.Now.AddDays(1));
        //        return RedirectToAction("DanhSachBTV", "BienTapViens", new { area = "Admin" });
        //    }

        //    // 2. Kiểm tra Người Dùng (Tác giả, phản biện,...)
        //    var nguoiDung = db.NguoiDungs.FirstOrDefault(s => s.Email.Trim() == email.Trim() && s.MatKhau.Equals(f_matkhau));
        //    var loaiND = nguoiDung.PhanBien;
        //    if (nguoiDung != null)
        //    {
        //        Session["UserName"] = nguoiDung.HoTen;
        //        Session["idUser"] = nguoiDung.IDNguoiDung;
        //        string loaiNguoiDung = nguoiDung.PhanBien == true ? "PhanBien" : "NguoiDung";
        //        Session["LoaiNguoiDung"] = loaiNguoiDung;
        //        CookieHelper.create("UserName", nguoiDung.HoTen, DateTime.Now.AddDays(1));
        //        CookieHelper.create("UserId", nguoiDung.IDNguoiDung.ToString(), DateTime.Now.AddDays(1));
        //        CookieHelper.create("LoaiNguoiDung", loaiNguoiDung, DateTime.Now.AddDays(1));
        //        return RedirectToAction("TKCaNhan", "TaiKhoan", new { id = nguoiDung.IDNguoiDung });
        //    }

        //    // Sai tài khoản hoặc mật khẩu
        //    ViewBag.Error = "* Tài khoản hoặc mật khẩu không đúng !";
        //    return View();
        //}
        [HttpPost]
        public ActionResult DangNhap(string email, string matkhau)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var f_matkhau = Hashing.ToSHA256(matkhau);

            // 1. Kiểm tra Biên Tập Viên
            var btv = db.BienTapViens.FirstOrDefault(s => s.Email.Trim() == email.Trim() && s.MatKhau.Equals(f_matkhau));
            if (btv != null)
            {
                Session["UserName"] = btv.HoTen;
                Session["idUser"] = btv.IDBienTapVien;
                Session["LoaiNguoiDung"] = "BienTapVien"; // Có thể dùng để phân quyền view
                Session["LoaiBienTapVien"] = btv.LoaiBienTapVien; // Tổng hoặc PhuTrach
                CookieHelper.create("UserName", btv.HoTen, DateTime.Now.AddDays(1));
                CookieHelper.create("UserId", btv.IDBienTapVien.ToString(), DateTime.Now.AddDays(1));
                CookieHelper.create("LoaiNguoiDung", "BienTapVien", DateTime.Now.AddDays(1));
                CookieHelper.create("LoaiBienTapVien", btv.LoaiBienTapVien, DateTime.Now.AddDays(1));
                return RedirectToAction("DanhSachBTV", "BienTapViens", new { area = "Admin" });
            }

            // 2. Kiểm tra Người Dùng (Tác giả, phản biện,...)
            var nguoiDung = db.NguoiDungs.FirstOrDefault(s => s.Email.Trim() == email.Trim() && s.MatKhau.Equals(f_matkhau));
            if (nguoiDung != null)
            {
                var loaiND = nguoiDung.PhanBien; // Di chuyển dòng này vào trong if
                Session["UserName"] = nguoiDung.HoTen;
                Session["idUser"] = nguoiDung.IDNguoiDung;
                string loaiNguoiDung = nguoiDung.PhanBien == true ? "PhanBien" : "NguoiDung";
                Session["LoaiNguoiDung"] = loaiNguoiDung;
                CookieHelper.create("UserName", nguoiDung.HoTen, DateTime.Now.AddDays(1));
                CookieHelper.create("UserId", nguoiDung.IDNguoiDung.ToString(), DateTime.Now.AddDays(1));
                CookieHelper.create("LoaiNguoiDung", loaiNguoiDung, DateTime.Now.AddDays(1));
                return RedirectToAction("TKCaNhan", "TaiKhoan", new { id = nguoiDung.IDNguoiDung });
            }

            // Sai tài khoản hoặc mật khẩu
            ViewBag.Error = "* Tài khoản hoặc mật khẩu không đúng !";
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return View("DangNhap");
        }

    }
}