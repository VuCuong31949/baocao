using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLTapChi.Models;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class BienTapViensController : Controller
    {
        private QLTapChiEntities db = new QLTapChiEntities();

        // GET: Admin/BienTapViens
        public ActionResult DanhSachBTV()
        {
            var tk = db.BienTapViens.ToList();
            return View(tk);

        }
        public ActionResult ThemBTV()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemBTV(BienTapVien _user, string MatKhauLai)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                bool checkTenDangNhap = db.BienTapViens.Any(s => s.HoTen == _user.HoTen);
                bool checkEmail = db.BienTapViens.Any(s => s.Email == _user.Email);
                bool checkSdt = db.BienTapViens.Any(s => s.SDT == _user.SDT);
                if (checkTenDangNhap || checkEmail || checkSdt)
                {
                    if (checkTenDangNhap)
                    {
                        ViewBag.errorTenDangNhap = "* Tên đăng nhập đã tồn tại!";
                    }
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
                _user.LoaiBienTapVien = "BienTapVien";
                

                // Mã hóa mật khẩu
                _user.MatKhau = Hashing.ToSHA256(_user.MatKhau);
                db.Configuration.ValidateOnSaveEnabled = false;
                db.BienTapViens.Add(_user);
                db.SaveChanges();
                //Session["VaiTro"] = _user.LoaiBienTapVien;
                return RedirectToAction("DanhSachBTV", "BientapViens");
            }
            return View();
        }

        public ActionResult CapNhatBTV(int id)
        {
            BienTapVien timkiemUser = db.BienTapViens.Find(id);
            return View(timkiemUser);
        }
        [HttpPost]
        public ActionResult CapNhatBTV(BienTapVien model)
        {
            BienTapVien EditUser = db.BienTapViens.Find(model.IDBienTapVien);
            // Kiểm tra tên đăng nhập trùng lặp
            var checkTenDangNhap = db.BienTapViens.Any(u => u.HoTen == model.HoTen && u.IDBienTapVien != model.IDBienTapVien);
            if (checkTenDangNhap)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(EditUser);
            }
            // Kiểm tra số điện thoại trùng lặp
            var checkSDT = db.BienTapViens.Any(u => u.SDT == model.SDT && u.IDBienTapVien != model.IDBienTapVien);
            if (checkSDT)
            {
                ModelState.AddModelError("SDT", "Số điện thoại đã được sử dụng.");
                return View(EditUser);
            }
            
            // Kiểm tra nếu mật khẩu đã được thay đổi, sau đó mã hóa mật khẩu
            if (!string.IsNullOrEmpty(model.MatKhau) && EditUser.MatKhau != model.MatKhau)
            {
                EditUser.MatKhau = Hashing.ToSHA256(model.MatKhau); // Mã hóa mật khẩu trước khi lưu
            }
            EditUser.Email = model.Email;
            EditUser.SDT = model.SDT;
            EditUser.HoTen = model.HoTen;
            EditUser.QuocGia = model.QuocGia;
            EditUser.ChuyenNganh = model.ChuyenNganh;
            EditUser.LoaiBienTapVien = model.LoaiBienTapVien;
           
            db.SaveChanges();
            
            return RedirectToAction("DanhSachBTV", "BienTapViens");
        }
        public ActionResult XoaBTV(int id)
        {
            // Tìm đối tượng xóa
            var deleteUser = db.BienTapViens.Find(id);
            // Xóa
            if (deleteUser != null)
            {
                db.BienTapViens.Remove(deleteUser);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachTaiKhoan");
        }
        
    }
}
