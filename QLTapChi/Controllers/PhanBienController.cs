using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Controllers
{
    public class PhanBienController : Controller
    {
        QLTapChiEntities db = new QLTapChiEntities();
        // GET: PhanBien
        public ActionResult PhanBien()
        {
            if (Session["idUser"] == null )
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            int idPB = (int)Session["idUser"];
            var phanbien = db.NguoiDungs.FirstOrDefault(b => b.IDNguoiDung == idPB);

            if (phanbien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            //string chuyenNganh = bienTapVien.ChuyenNganh;

            var baiPhanBien = (from b in db.TapChiBaiViets
                                  join p in db.PhanCongs on b.IDTapChiBaiViet equals p.IDTapChiBaiViet
                                  where b.TrangThai == 2 && p.IDNguoiPhanBien == idPB
                                  orderby b.NgayGui descending
                                  select b).ToList();
            //int soBaiChoPB = baiPhanBien.Count;          
            //ViewBag.PhanBien = new SelectList(baiPhanBien, "IDNguoiDung", "HoTen");

            return View(baiPhanBien);
        }
        public ActionResult DanhSachDaChapNhan()
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy ID người phản biện từ session
            int idPB = (int)Session["idUser"];

            // Lấy danh sách bài viết đã chấp nhận phản biện
            var baiDaChapNhan = (from b in db.TapChiBaiViets
                                 join p in db.PhanCongs on b.IDTapChiBaiViet equals p.IDTapChiBaiViet
                                 where b.TrangThaiPhanBien == 1 && p.IDNguoiPhanBien == idPB
                                 orderby b.NgayGui descending
                                 select b).ToList();

            return View(baiDaChapNhan);
        }
        [HttpPost]
        public ActionResult ChapNhanPhanBien(int idBaiViet)
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy ID người dùng từ session
            int idPB = (int)Session["idUser"];

            // Tìm phân công liên quan đến bài viết và người phản biện
            var phanCong = db.PhanCongs.FirstOrDefault(p => p.IDTapChiBaiViet == idBaiViet && p.IDNguoiPhanBien == idPB);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phân công.";
                return RedirectToAction("PhanBien");
            }

            // Kiểm tra trạng thái phân công
            if (phanCong.TrangThaiPhanBien != 0) // Chỉ cho phép chấp nhận nếu trạng thái là "chưa phản hồi"
            {
                TempData["Error"] = "Phân công này đã được xử lý.";
                return RedirectToAction("PhanBien");
            }

            // Cập nhật trạng thái phân công: 1 = chấp nhận (đạt)
            //phanCong.TrangThaiPhanBien = 1;
            //phanCong.NgayPhanCong = DateTime.Now; // Cập nhật ngày phân công nếu cần

            // Cập nhật trạng thái bài viết: 1 = Đang phản biện
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == idBaiViet);
            if (baiViet != null)
            {
                baiViet.TrangThaiPhanBien = 1; // Đang phản biện
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            TempData["Success"] = "Bạn đã chấp nhận phân công phản biện thành công.";
            return RedirectToAction("PhanBien");
        }
        public ActionResult TuChoiPhanBien(int id)
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null || Session["LoaiBienTapVien"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Tìm phân công
            var phanCong = db.TapChiBaiViets.FirstOrDefault(p => p.IDTapChiBaiViet == id);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phân công.";
                return RedirectToAction("DanhSachPhanCong");
            }

            // Kiểm tra trạng thái hiện tại
            if (phanCong.TrangThaiPhanBien != 0) // Chỉ cho phép từ chối nếu trạng thái là "chưa phản hồi"
            {
                TempData["Error"] = "Phân công này đã được xử lý.";
                return RedirectToAction("PhanCongPhanBien");
            }

            // Cập nhật trạng thái: 4 = từ chối
            phanCong.TrangThai = 4;
            db.SaveChanges();

            TempData["Success"] = "Bạn đã từ chối phân công thành công.";
            return RedirectToAction("PhanBien");
        }

        public ActionResult GuiPhanBien(int id)
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy ID người phản biện từ session
            int idPB = (int)Session["idUser"];

            // Tìm bài viết theo ID
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("PhanBien");
            }

            // Kiểm tra xem người dùng có được phân công phản biện bài này không
            var phanCong = db.PhanCongs.FirstOrDefault(p => p.IDTapChiBaiViet == id && p.IDNguoiPhanBien == idPB);
            if (phanCong == null)
            {
                TempData["Error"] = "Bạn không được phân công phản biện bài viết này.";
                return RedirectToAction("PhanBien");
            }

            // Trả về view với thông tin bài viết
            return View(baiViet);
        }
        [HttpPost]
        public ActionResult GuiPhanBien(int IDTapChiBaiViet, string NhanXet, int TrangThaiPhanBien, HttpPostedFileBase fileUpload)
        {

                    if (Session["idUser"] == null)
                    {
                        TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                        return RedirectToAction("DangNhap", "TaiKhoan");
                    }

                    int idPB = (int)Session["idUser"];
                    var phanCong = db.PhanCongs.FirstOrDefault(p => p.IDTapChiBaiViet == IDTapChiBaiViet && p.IDNguoiPhanBien == idPB);
                    if (phanCong == null)
                    {
                        TempData["Error"] = "Không tìm thấy phân công.";
                        return RedirectToAction("PhanBien");
                    }

                    // Cập nhật trạng thái phân công
                    phanCong.TrangThaiPhanBien = TrangThaiPhanBien;
                    phanCong.NgayPhanCong = DateTime.Now;

                    // Lưu nội dung phản biện vào bảng PhanBien
                    var phanBien = new PhanBien
                    {
                        NhanXet = NhanXet.ToString(),
                        NgayPhanBien = DateTime.Now,
                        IDTapChiBaiViet = IDTapChiBaiViet,
                        IDNguoiPhanBien = idPB
                    };
                    // Xử lý file upload nếu có
                    if (fileUpload != null && fileUpload.ContentLength > 0)
                    {
                        string rootFolder = Server.MapPath("/Content/PhanBien/");
                        string pathfile = rootFolder + fileUpload.FileName;
                        //string pathfile = Path.Combine(rootFolder, fileName);
                        fileUpload.SaveAs(pathfile);
                        phanBien.filePB = "Content/PhanBien/" + fileUpload.FileName;
                    }

                    // Thêm vào DB và lưu
                    db.PhanBiens.Add(phanBien);
                    db.SaveChanges();
                    TempData["Success"] = "Gửi phản biện thành công.";
                    return RedirectToAction("PhanBien");

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
        public ActionResult DanhSachdaPhanBien()
        {
            var ds = db.PhanBiens.ToList();
            return View(ds);
        }
    }
}