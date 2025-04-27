using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class PhanCongController : Controller
    {
        // GET: Admin/PhanCong
        QLTapChiEntities db = new QLTapChiEntities();

        public ActionResult BaiVietChoDuyet()
        {
            // Kiểm tra đăng nhập và loại biên tập viên
            if (Session["idUser"] == null || Session["LoaiNguoiDung"] == null || Session["LoaiBienTapVien"].ToString() != "TongBienTap")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idBTV = (int)Session["idUser"];
            var bienTapVien = db.BienTapViens.FirstOrDefault(b => b.IDBienTapVien == idBTV);

            if (bienTapVien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            string chuyenNganh = bienTapVien.ChuyenNganh;

            // Lấy bài viết chờ duyệt theo chuyên ngành của biên tập viên
            var baiChoDuyet = db.TapChiBaiViets
                                .Where(b => b.TrangThai == 0 && b.LinhVuc.TenLinhVuc == chuyenNganh)
                                .OrderByDescending(b => b.NgayGui)
                                .ToList();
            int soBaiChoDuyet = baiChoDuyet.Count;
            ViewBag.SoBaiChoDuyet = soBaiChoDuyet;
            // Danh sách biên tập viên thuộc cùng chuyên ngành để phân công
            var bienTapVienTheoChuyenNganh = db.BienTapViens
                                               .Where(btv => btv.ChuyenNganh == chuyenNganh && btv.LoaiBienTapVien != "TongBienTap")
                                               .ToList();

            ViewBag.BienTapVien = new SelectList(bienTapVienTheoChuyenNganh, "IDBienTapVien", "HoTen");
            return View(baiChoDuyet);
        }

        // POST: Phân công biên tập viên chịu trách nhiệm
 
        [HttpPost]
        public ActionResult PhanCong(int idBaiViet, int idBienTapVien)
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null || Session["LoaiBienTapVien"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var userId = (int)Session["idUser"];
            var vaiTro = Session["LoaiBienTapVien"].ToString();

            // Kiểm tra vai trò Tổng Biên Tập
            if (vaiTro != "TongBienTap")
            {
                TempData["Error"] = "Bạn không có quyền phân công biên tập viên!";
                return RedirectToAction("BaiVietChoDuyet");
            }

            // Tìm bài viết
            var baiViet = db.TapChiBaiViets.Find(idBaiViet);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết!";
                return RedirectToAction("BaiVietChoDuyet");
            }

            // Kiểm tra đã duyệt chưa
            if (baiViet.TrangThai != 0)
            {
                TempData["Error"] = "Bài viết này đã được duyệt hoặc xử lý trước đó!";
                return RedirectToAction("BaiVietChoDuyet");
            }

            // Kiểm tra đã phân công chưa
            bool daPhanCong = db.PhanCongBienTaps.Any(p => p.IDTapChiBaiViet == idBaiViet);
            if (daPhanCong)
            {
                TempData["Error"] = "Bài viết đã được phân công cho biên tập viên khác.";
                return RedirectToAction("BaiVietChoDuyet");
            }

            // Cập nhật trạng thái bài viết
            baiViet.TrangThai = 1; // Đã duyệt
            db.SaveChanges();

            // Thêm bản ghi phân công
            var phanCong = new PhanCongBienTap
            {
                IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
                IDBienTapVien = idBienTapVien,
                NgayPhanCong = DateTime.Now
            };
            db.PhanCongBienTaps.Add(phanCong);
            db.SaveChanges();

            TempData["Success"] = "Phân công biên tập viên thành công.";
            return RedirectToAction("BaiVietChoDuyet");
        }
        public ActionResult BaiVietChoPhanBien()
        {
            int idBTV = (int)Session["idUser"];
            var bienTapVien = db.BienTapViens.FirstOrDefault(b => b.IDBienTapVien == idBTV);

            if (bienTapVien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            string chuyenNganh = bienTapVien.ChuyenNganh;

            var baiChoPhanBien = (from b in db.TapChiBaiViets
                                  join p in db.PhanCongBienTaps on b.IDTapChiBaiViet equals p.IDTapChiBaiViet
                                  where b.TrangThai == 1 && p.IDBienTapVien == idBTV
                                  orderby b.NgayGui descending
                                  select b).ToList();
            int soBaiChoPB = baiChoPhanBien.Count;
            ViewBag.SoBaiChoPB = soBaiChoPB;
            // Danh sách biên tập viên thuộc cùng chuyên ngành để phân công
            var PhanBien = db.NguoiDungs.Where(pb => pb.LinhVuc.TenLinhVuc == chuyenNganh && pb.PhanBien == true).ToList();

            ViewBag.PhanBien = new SelectList(PhanBien, "IDNguoiDung", "HoTen");
          
            return View( baiChoPhanBien);
        }
        [HttpPost]
        public ActionResult PhanCongPhanBien(int idBaiViet, int idPhanBien, DateTime ngayKetThuc)
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null || Session["LoaiBienTapVien"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Tìm bài viết
            var baiViet = db.TapChiBaiViets.Find(idBaiViet);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết!";
                return RedirectToAction("BaiVietChoPhanBien");
            }

            // Kiểm tra đã duyệt chưa
            if (baiViet.TrangThai != 1)
            {
                TempData["Error"] = "Bài viết này đã được duyệt hoặc xử lý trước đó!";
                return RedirectToAction("BaiVietChoPhanBien");
            }

            // Kiểm tra đã phân công chưa
            bool daPhanCong = db.PhanCongs.Any(p => p.IDTapChiBaiViet == idBaiViet);
            if (daPhanCong)
            {
                TempData["Error"] = "Bài viết đã được phân công cho người phản biện khác.";
                return RedirectToAction("BaiVietChoPhanBien");
            }

            // Kiểm tra ngày kết thúc có hợp lệ không (phải lớn hơn hoặc bằng ngày hiện tại)
            if (ngayKetThuc < DateTime.Today)
            {
                TempData["Error"] = "Ngày kết thúc phải từ hôm nay trở đi!";
                return RedirectToAction("BaiVietChoPhanBien");
            }

            // Cập nhật trạng thái bài viết
            baiViet.TrangThai = 2; // Đã phân công
            baiViet.TrangThaiPhanBien = 0;
            db.SaveChanges();

            // Thêm bản ghi phân công
            var phanCong = new PhanCong
            {
                IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
                IDNguoiPhanBien = idPhanBien, // Sử dụng idPhanBien từ form, không phải userId
                NgayPhanCong = DateTime.Now,
                NgayKetThuc = ngayKetThuc,
                VongPhanBien=1,
                TrangThaiPhanBien=0,
                
            };
            db.PhanCongs.Add(phanCong);
            db.SaveChanges();
            int soLuongPhanBien = db.PhanCongs.Count(p => p.IDTapChiBaiViet == idBaiViet);
            int maxPB = 2; // Số lượng tối đa người phản biện (có thể thay đổi)

            if (soLuongPhanBien >= maxPB)
            {
                baiViet.TrangThai = 2; // Đã phân công đủ người phản biện
                db.SaveChanges();
            }
            TempData["Success"] = "Phân công phản biện thành công.";

            // Lấy thông tin người phản biện
            var nguoiPhanBien = db.NguoiDungs.FirstOrDefault(nd => nd.IDNguoiDung == idPhanBien);
            var linhVuc = db.LinhVucs.FirstOrDefault(lv => lv.IDLinhVuc == baiViet.IDLinhVuc);

            if (nguoiPhanBien == null || linhVuc == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người phản biện hoặc lĩnh vực.";
                return RedirectToAction("BaiVietChoPhanBien");
            }

            // Đọc template email
            string content = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Content/templates/send_phanbien.html"));

            // Thay thế các placeholder trong template
            content = content.Replace("{{IDPhanCong}}", phanCong.IDPhanCong.ToString());
            content = content.Replace("{{TieuDeBaiViet}}", baiViet.TieuDe);
            content = content.Replace("{{TacGia}}", baiViet.TacGia);
            content = content.Replace("{{TenLinhVuc}}", linhVuc.TenLinhVuc);
            content = content.Replace("{{TenNguoiPhanBien}}", nguoiPhanBien.HoTen);
            content = content.Replace("{{Email}}", nguoiPhanBien.Email);
            content = content.Replace("{{NgayPhanCong}}", phanCong.NgayPhanCong.ToString("dd/MM/yyyy"));
            content = content.Replace("{{VongPhanBien}}", phanCong.VongPhanBien.ToString());
            //content = content.Replace("{{LinkPhanBien}}", $"{Request.Url.Scheme}://{Request.Url.Authority}/PhanBien/PhanBienBaiViet/{phanCong.IDPhanCong}");
            var link = Url.Action("DangNhap", "TaiKhoan");
            var fullLink = $"{Request.Url.Scheme}://{Request.Url.Authority}{link}";
            content = content.Replace("{{LinkPhanBien}}", fullLink);


            // Gửi email
            bool emailSent = SendMail.sendMail(
                name: "Biên tập viên QLTapChi",
                subject: $"Phân công phản biện: #{phanCong.IDPhanCong}",
                content: content,
                toMail: nguoiPhanBien.Email
            );

            if (!emailSent)
            {
                TempData["Warning"] = "Phân công thành công, nhưng gửi email thất bại.";
            }
            return RedirectToAction("BaiVietChoPhanBien");
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
            var phanCong = db.PhanCongBienTaps.FirstOrDefault(p => p.IDPhanCongBienTap == id);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phân công.";
                return RedirectToAction("DanhSachPhanCong");
            }

            // Kiểm tra trạng thái hiện tại
            if (phanCong.TrangThai != 0) // Chỉ cho phép từ chối nếu trạng thái là "chưa phản hồi"
            {
                TempData["Error"] = "Phân công này đã được xử lý.";
                return RedirectToAction("PhanCongPhanBien");
            }

            // Cập nhật trạng thái: 2 = từ chối
            phanCong.TrangThai = 2;
            db.SaveChanges();

            TempData["Success"] = "Bạn đã từ chối phân công thành công.";
            return RedirectToAction("DanhSachPhanCong");
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