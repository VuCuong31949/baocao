using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Controllers
{
    public class TapChiController : Controller
    {
        // GET: TapChi
        QLTapChiEntities db = new QLTapChiEntities();
        public ActionResult DanhSachTapChi()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            int idNguoiDung = (int)Session["idUser"];
            var BaiBao = db.TapChiBaiViets.Where(x => x.IDNguoiGui == idNguoiDung).OrderByDescending(x => x.NgayGui).ToList();
            return View(BaiBao);
        }
        public ActionResult XemPhanBien(int id)
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy ID người dùng từ session
            int idNguoiDung = (int)Session["idUser"];

            // Tìm bài viết và xác minh quyền sở hữu
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id && b.IDNguoiGui == idNguoiDung);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết hoặc bạn không có quyền truy cập.";
                return RedirectToAction("DanhSachTapChi");
            }

            // Lấy danh sách phản biện cho bài viết
            var phanBien = db.PhanBiens.Where(p => p.IDTapChiBaiViet == id).ToList();

            // Tạo ViewModel
            var viewModel = new XemPhanBienViewModel
            {
                BaiViet = baiViet,
                DanhSachPhanBien = phanBien
            };

            return View(viewModel);
        }
        public ActionResult Add()
        {
            if (Session["idUser"] == null)
                return RedirectToAction("DangNhap", "TaiKhoan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(TapChiBaiViet model, HttpPostedFileBase File)
        {
            int idNguoiDung = (int)Session["idUser"];
            model.IDNguoiGui = idNguoiDung;
            model.TrangThai = 0;//chờ duyệt
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
                return RedirectToAction("DanhSachTapChi", "TapChi");
            }
            return View(model);
        }
        public ActionResult CapNhatTapChi(int id)
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
        public ActionResult CapNhatTapChi(TapChiBaiViet model, HttpPostedFileBase File)
        {
            var updateModel = db.TapChiBaiViets.Find(model.IDTapChiBaiViet);
            //2.Gán Giá Trị cho đối tượng
            updateModel.TieuDe = model.TieuDe;
            updateModel.TrangThai = model.TrangThai;
            updateModel.LinhVuc = model.LinhVuc;
            updateModel.GhiChu = model.GhiChu;


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
    }
}