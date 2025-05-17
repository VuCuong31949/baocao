using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class BaiBaoController : Controller
    {
        // GET: Admin/BaiBao
        QLTapChiEntities db = new QLTapChiEntities();
        public ActionResult Index()
        {
            var BaiBao = db.TapChiBaiViets.OrderByDescending(x =>x.IDTapChiBaiViet).ToList();
            return View(BaiBao);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(TapChiBaiViet model, HttpPostedFileBase File)
        {
            model.TrangThai = 0;
            model.NgayGui = DateTime.Now;
            if (File != null && File.ContentLength > 0)
            {
                string rootFolder = Server.MapPath("/Content/BaiViet/");
                string pathImage = rootFolder + File.FileName;
                File.SaveAs(pathImage);
                //Lưu thuộc tính url
                model.NoiDung = "Content/BaiViet/" + File.FileName;
                db.TapChiBaiViets.Add(model);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult CapNhatBaiBao(int id)
        {
            var baiBao = db.TapChiBaiViets.FirstOrDefault(s => s.IDTapChiBaiViet == id);
            if (baiBao == null)
            {
                return HttpNotFound();
            }

            return View(baiBao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatBaiBao(TapChiBaiViet model, HttpPostedFileBase File)
        {
            var updateModel = db.TapChiBaiViets.Find(model.IDTapChiBaiViet);
            //2.Gán Giá Trị cho đối tượng
            updateModel.TieuDe = model.TieuDe;
            updateModel.TrangThai = model.TrangThai;
            updateModel.LinhVuc = model.LinhVuc;
            updateModel.TomTat = model.TomTat;
           
            
            if (File != null && File.ContentLength > 0)
            {
                string rootFolder = Server.MapPath("/Content/BaiViet/");
                string pathImage = rootFolder + File.FileName;
                File.SaveAs(pathImage);
                // Lưu thuộc tính url
                updateModel.NoiDung = "Content/BaiViet/" + File.FileName;

            }

             db.SaveChanges();
                return RedirectToAction("Index");
           
        }
        public ActionResult XoaBaiBao(int id)
        {
            var model = db.TapChiBaiViets.Find(id);
            if (model != null)
            {
                db.TapChiBaiViets.Remove(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult DownloadFile(int id)
        {
            // Tìm bài báo theo ID
            var baiBao = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);

            // Nếu không tìm thấy bài báo, trả về 404
            if (baiBao == null)
            {
                return HttpNotFound();
            }

            // Lấy đường dẫn tới file (trong trường hợp là "NoiDung" là đường dẫn file)
            string filePath = Server.MapPath("~/" + baiBao.NoiDung);

            // Kiểm tra file có tồn tại không
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            // Lấy tên file
            string fileName = Path.GetFileName(filePath);

            // Trả về file dưới dạng download
            return File(filePath, "application/octet-stream", fileName);
        }
        public ActionResult CapNhatBTV(int id)
        {
            // Check if the user is logged in
            if (Session["idUser"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            // Ensure the id matches the logged-in user's ID (for security)
            int loggedInUserId;
            if (!int.TryParse(Session["idUser"]?.ToString(), out loggedInUserId) || id != loggedInUserId)
            {
                return HttpNotFound(); // Or redirect to an error page
            }

            // Fetch the BienTapVien record
            BienTapVien timkiemUser = db.BienTapViens.Find(id);
            if (timkiemUser == null)
            {
                return HttpNotFound(); // Return 404 if the record is not found
            }

            // Pass the LinhVuc list to the view (to avoid creating DbContext in the view)
            ViewBag.LinhVucList = db.LinhVucs.ToList();

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
          

            db.SaveChanges();

            return RedirectToAction("Index", "BaiBao");
        }

    }
}