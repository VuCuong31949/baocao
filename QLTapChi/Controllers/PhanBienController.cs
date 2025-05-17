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
                                  where b.TrangThai == 2 && p.IDNguoiPhanBien == idPB && p.TrangThaiPhanBien == 0
                               orderby b.NgayGui descending
                                  select b).ToList();
            //int soBaiChoPB = baiPhanBien.Count;          
            //ViewBag.PhanBien = new SelectList(baiPhanBien, "IDNguoiDung", "HoTen");

            return View(baiPhanBien);
        }
        public ActionResult DSDaPhanBien()
        {
            // Kiểm tra đăng nhập
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy ID người phản biện từ session
            int idPB = (int)Session["idUser"];

            // Truy vấn danh sách phân công phản biện đã xử lý (TrangThaiPhanBien != 0)
            var danhSachPhanCong = (from pc in db.PhanCongs
                                    join tcbv in db.TapChiBaiViets on pc.IDTapChiBaiViet equals tcbv.IDTapChiBaiViet
                                    join nd in db.NguoiDungs on pc.IDNguoiPhanBien equals nd.IDNguoiDung
                                    join lv in db.LinhVucs on tcbv.IDLinhVuc equals lv.IDLinhVuc                                    
                                    where pc.IDNguoiPhanBien == idPB && pc.TrangThaiPhanBien != 0
                                    select new PhanCongViewModel
                                    {
                                        IDPhanCong = pc.IDPhanCong,
                                        TieuDeBaiViet = tcbv.TieuDe,
                                        TacGia = tcbv.TacGia,
                                        TenLinhVuc = lv.TenLinhVuc,
                                        NguoiPhanBien = nd.HoTen,
                                        EmailNguoiPhanBien = nd.Email,
                                        TrangThaiPhanBien = pc.TrangThaiPhanBien,
                                        NgayPhanCong = pc.NgayPhanCong,
                                        VongPhanBien = pc.VongPhanBien
                                    })
                                   .OrderByDescending(pc => pc.NgayPhanCong)
                                   .ToList();

            // Chuyển trạng thái phản biện sang văn bản dễ hiểu
            foreach (var item in danhSachPhanCong)
            {
                item.TrangThaiPhanBienText = GetTrangThaiPhanBienText(item.TrangThaiPhanBien);
            }

            // Trả về view với danh sách phân công
            return View(danhSachPhanCong);
        }

        // Hàm chuyển đổi trạng thái phản biện sang văn bản
        private string GetTrangThaiPhanBienText(int? trangThai)
        {
            switch (trangThai)
            {
                case 1: return "Đạt";
                case 2: return "Không đạt";
                case 3: return "Sửa đổi nhỏ";
                case 4: return "Sửa đổi lớn";
                default: return "Không xác định";
            }
        }
      
        public ActionResult DanhSachDaChapNhan()
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idPB = (int)Session["idUser"];

            // Lấy danh sách bài viết đã chấp nhận phản biện, chỉ lấy vòng phản biện mới nhất
            var baiDaChapNhan = (from b in db.TapChiBaiViets
                                 join p in db.PhanCongs on b.IDTapChiBaiViet equals p.IDTapChiBaiViet
                                 where p.IDNguoiPhanBien == idPB && (p.TrangThaiPhanBien == 5 || p.TrangThaiPhanBien == 3 || p.TrangThaiPhanBien == 4)
                                 group p by p.IDTapChiBaiViet into g
                                 let maxVongPhanBien = g.Max(p => p.VongPhanBien)
                                 from p in g
                                 where p.VongPhanBien == maxVongPhanBien
                                 orderby p.NgayPhanCong descending
                                 select p.TapChiBaiViet).ToList();

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
            // Cập nhật trạng thái phân công: 5 = chấp nhận phản biện
            phanCong.TrangThaiPhanBien = 5; // Chấp nhận phản biện
            phanCong.NgayPhanCong = DateTime.Now;

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
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy ID người phản biện từ session
            int idPB = (int)Session["idUser"];

            // Tìm phân công dựa trên ID bài viết và ID người phản biện
            var phanCong = db.PhanCongs.FirstOrDefault(p => p.IDTapChiBaiViet == id && p.IDNguoiPhanBien == idPB);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phân công.";
                return RedirectToAction("PhanBien");
            }

            // Kiểm tra trạng thái hiện tại
            if (phanCong.TrangThaiPhanBien != 0) // Chỉ cho phép từ chối nếu trạng thái là "chưa phản hồi"
            {
                TempData["Error"] = "Phân công này đã được xử lý.";
                return RedirectToAction("PhanBien");
            }

            // Lấy thông tin bài viết
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("PhanBien");
            }
            baiViet.TrangThai = 1;
            // Lấy thông tin người phản biện
            var nguoiPhanBien = db.NguoiDungs.FirstOrDefault(nd => nd.IDNguoiDung == idPB);
            if (nguoiPhanBien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin người phản biện.";
                return RedirectToAction("PhanBien");
            }

            // Lấy thông tin biên tập viên từ bảng PhanCongBienTap
            var phanCongBienTap = db.PhanCongBienTaps.FirstOrDefault(pc => pc.IDTapChiBaiViet == id);
            var bienTapVien = phanCongBienTap != null ? db.BienTapViens.FirstOrDefault(btv => btv.IDBienTapVien == phanCongBienTap.IDBienTapVien) : null;
            if (bienTapVien == null)
            {
                TempData["Error"] = "Không tìm thấy biên tập viên để gửi thông báo.";
                return RedirectToAction("PhanBien");
            }

            // Gửi email thông báo cho biên tập viên
            string filePath = HttpContext.Server.MapPath("~/Content/TuChoiPB.html");
            string content;
            if (System.IO.File.Exists(filePath))
            {
                content = System.IO.File.ReadAllText(filePath);
            }
            else
            {
                TempData["Error"] = "Không tìm thấy file template email.";
                return RedirectToAction("PhanBien");
            }

            // Thay thế các placeholder trong template
            content = content.Replace("{{IDPhanCong}}", phanCong.IDPhanCong.ToString());
            content = content.Replace("{{TenNguoiPhanBien}}", nguoiPhanBien.HoTen);
            content = content.Replace("{{TieuDeBaiViet}}", baiViet.TieuDe);
            content = content.Replace("{{TacGia}}", baiViet.TacGia);
            content = content.Replace("{{TenLinhVuc}}", db.LinhVucs.FirstOrDefault(lv => lv.IDLinhVuc == baiViet.IDLinhVuc)?.TenLinhVuc ?? "Không xác định");
            content = content.Replace("{{NgayPhanCong}}", phanCong.NgayPhanCong.ToString("dd/MM/yyyy"));
            var link = Url.Action("DangNhap", "TaiKhoan", new { area = "" }, protocol: Request.Url.Scheme);
            content = content.Replace("{{LinkPhanBien}}", link);
            content = content.Replace("{{EmailPhanBien}}", nguoiPhanBien.Email);

            // Gửi email
            bool emailSent = SendMail.sendMail(
                name: "Hệ thống QLTapChi",
                subject: $"Người phản biện đã từ chối nhiệm vụ phản biện #{phanCong.IDPhanCong}",
                content: content,
                toMail: bienTapVien.Email
            );

            if (!emailSent)
            {
                TempData["Warning"] = "Phân công đã được từ chối, nhưng gửi email thất bại.";
            }

            // Xóa bản ghi phân công
            db.PhanCongs.Remove(phanCong);

            // Kiểm tra số lượng người phản biện còn lại
            int soLuongPhanBien = db.PhanCongs.Count(p => p.IDTapChiBaiViet == id);
            if (soLuongPhanBien < 2 && baiViet.TrangThai == 2)
            {
                // Nếu chưa đủ 2 người phản biện, đưa bài viết về trạng thái chờ phân công
                baiViet.TrangThai = 1; // Chưa phân công phản biện
                baiViet.TrangThaiPhanBien = 0; // Chờ phản biện
            }

            // Lưu thay đổi
            db.SaveChanges();
            TempData["Success"] = "Bài viết đã từ chối phản biện ";

            return RedirectToAction("PhanBien");
        }


        public ActionResult GuiPhanBien(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idPB = (int)Session["idUser"];
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("PhanBien");
            }
            baiViet.TomTat = HttpUtility.HtmlDecode(baiViet.TomTat);
            // Lấy phân công ở vòng mới nhất
            var phanCong = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == id && p.IDNguoiPhanBien == idPB)
                .OrderByDescending(p => p.VongPhanBien)
                .FirstOrDefault();

            if (phanCong == null)
            {
                TempData["Error"] = "Bạn không được phân công phản biện bài viết này.";
                return RedirectToAction("PhanBien");
            }

            // Kiểm tra trạng thái phân công
            if (phanCong.TrangThaiPhanBien != 0 && phanCong.TrangThaiPhanBien != 5 && phanCong.TrangThaiPhanBien != 3 && phanCong.TrangThaiPhanBien != 4)
            {
                TempData["Error"] = "Bạn không thể phản biện bài viết này ở trạng thái hiện tại.";
                return RedirectToAction("PhanBien");
            }

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

            var phanCong = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == IDTapChiBaiViet && p.IDNguoiPhanBien == idPB)
                .OrderByDescending(p => p.VongPhanBien)
                .FirstOrDefault();

            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy phân công.";
                return RedirectToAction("PhanBien");
            }

            phanCong.TrangThaiPhanBien = TrangThaiPhanBien;
            phanCong.NgayPhanCong = DateTime.Now;

            var phanBien = new PhanBien
            {
                NhanXet = NhanXet,
                NgayPhanBien = DateTime.Now,
                IDTapChiBaiViet = IDTapChiBaiViet,
                IDNguoiPhanBien = idPB
            };

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string rootFolder = Server.MapPath("/Content/PhanBien/");
                string pathFile = rootFolder + fileUpload.FileName;
                fileUpload.SaveAs(pathFile);
                phanBien.filePB = "Content/PhanBien/" + fileUpload.FileName;
            }

            db.PhanBiens.Add(phanBien);

            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == IDTapChiBaiViet);
            baiViet.TomTat = HttpUtility.HtmlDecode(baiViet.TomTat);
            if (baiViet != null)
            {
                var phanCongs = db.PhanCongs
                    .Where(p => p.IDTapChiBaiViet == IDTapChiBaiViet && p.VongPhanBien == phanCong.VongPhanBien)
                    .ToList();

                int vongPhanBienHienTai = phanCong.VongPhanBien ?? 0;
                const int SO_NGUOI_PHAN_BIEN_TOI_THIEU = 2;
                const int SO_VONG_PHAN_BIEN_TOI_DA = 3;

                if (phanCongs.All(p => p.TrangThaiPhanBien != 0)) // Tất cả đã phản hồi
                {
                    var soNguoiDatVongHienTai = phanCongs.Count(p => p.TrangThaiPhanBien == 1);

                    if (soNguoiDatVongHienTai == SO_NGUOI_PHAN_BIEN_TOI_THIEU) // Cả hai đều "Đạt"
                    {
                        baiViet.TrangThaiPhanBien = 2; // Đạt, chờ xuất bản
                        baiViet.TrangThai = 4; // Xuất bản
                    }
                    else if (phanCongs.Any(p => p.TrangThaiPhanBien == 2)) // Có "Không đạt"
                    {
                        if (vongPhanBienHienTai >= SO_VONG_PHAN_BIEN_TOI_DA)
                        {
                            baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
                            baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
                        }
                        else
                        {
                            baiViet.TrangThaiPhanBien = 3; // Chờ chỉnh sửa
                        }
                    }
                    else if (phanCongs.Any(p => p.TrangThaiPhanBien == 3) || phanCongs.Any(p => p.TrangThaiPhanBien == 4)) // Có "Sửa đổi nhỏ" hoặc "Sửa đổi lớn"
                    {
                        if (vongPhanBienHienTai >= SO_VONG_PHAN_BIEN_TOI_DA)
                        {
                            baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
                            baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
                        }
                        else if (phanCongs.Any(p => p.TrangThaiPhanBien == 4)) // Có "Sửa đổi lớn"
                        {
                            baiViet.TrangThaiPhanBien = 3; // Chờ chỉnh sửa, không tạo vòng mới ngay
                        }
                        else
                        {
                            baiViet.TrangThaiPhanBien = 3; // Chờ chỉnh sửa (chỉ có "Sửa đổi nhỏ")
                        }
                    }
                }
                else
                {
                    baiViet.TrangThaiPhanBien = 1; // Vẫn đang phản biện
                }
            }

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