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
                                 where p.TrangThaiPhanBien == 5 && p.IDNguoiPhanBien == idPB
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
            phanCong.TrangThaiPhanBien = 4;
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
                NhanXet = NhanXet,
                NgayPhanBien = DateTime.Now,
                IDTapChiBaiViet = IDTapChiBaiViet,
                IDNguoiPhanBien = idPB
            };

            // Xử lý file upload nếu có
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string rootFolder = Server.MapPath("/Content/PhanBien/");
                string pathFile = rootFolder + fileUpload.FileName;
                fileUpload.SaveAs(pathFile);
                phanBien.filePB = "Content/PhanBien/" + fileUpload.FileName;
            }

            // Thêm phản biện vào DB
            db.PhanBiens.Add(phanBien);

            //// Cập nhật trạng thái tổng quát của bài viết
            //var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == IDTapChiBaiViet);
            //if (baiViet != null)
            //{
            //    var phanCongs = db.PhanCongs
            //        .Where(p => p.IDTapChiBaiViet == IDTapChiBaiViet && p.VongPhanBien == phanCong.VongPhanBien)
            //        .ToList();

            //    // Kiểm tra xem tất cả người phản biện đã gửi phản biện chưa
            //    if (phanCongs.All(p => p.TrangThaiPhanBien != 0))
            //    {
            //        var trangThaiPhanBiens = phanCongs.Select(p => p.TrangThaiPhanBien).ToList();
            //        int vongPhanBienHienTai = phanCong.VongPhanBien ?? 0;
            //        const int SO_VONG_PHAN_BIEN_TOI_DA = 3;

            //        if (trangThaiPhanBiens.All(t => t == 1)) // Tất cả đều "Đạt"
            //        {
            //            baiViet.TrangThaiPhanBien = 2; // Đạt, chờ xuất bản
            //            baiViet.TrangThai = 4; // Xuất bản
            //        }
            //        else if (trangThaiPhanBiens.Any(t => t == 2)) // Có "Không đạt"
            //        {
            //            if (vongPhanBienHienTai >= SO_VONG_PHAN_BIEN_TOI_DA)
            //            {
            //                baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
            //                baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
            //            }
            //            else
            //            {
            //                baiViet.TrangThaiPhanBien = 3; // Không đạt, chờ chỉnh sửa
            //            }
            //        }
            //        else if (trangThaiPhanBiens.Any(t => t == 3 || t == 4)) // Có "Sửa đổi nhỏ" hoặc "Sửa đổi lớn"
            //        {
            //            if (vongPhanBienHienTai >= SO_VONG_PHAN_BIEN_TOI_DA)
            //            {
            //                baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
            //                baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
            //            }
            //            else
            //            {
            //                baiViet.TrangThaiPhanBien = 3; // Không đạt, chờ chỉnh sửa
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // Nếu chưa đủ phản biện, giữ trạng thái "Đang phản biện"
            //        baiViet.TrangThaiPhanBien = 1;
            //    }
            //}

            // Cập nhật trạng thái tổng quát của bài viết
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == IDTapChiBaiViet);
            if (baiViet != null)
            {
                var phanCongs = db.PhanCongs
                    .Where(p => p.IDTapChiBaiViet == IDTapChiBaiViet && p.VongPhanBien == phanCong.VongPhanBien)
                    .ToList();

                // [Chỉnh sửa 2]: Thêm logic tạo vòng phản biện mới khi có "Sửa đổi lớn"
                if (phanCongs.All(p => p.TrangThaiPhanBien != 0)) // Tất cả đã phản hồi
                {
                    var trangThaiPhanBiens = phanCongs.Select(p => p.TrangThaiPhanBien).ToList();
                    int vongPhanBienHienTai = phanCong.VongPhanBien ?? 0;
                    const int SO_VONG_PHAN_BIEN_TOI_DA = 3;

                    if (trangThaiPhanBiens.All(t => t == 1)) // Tất cả đều "Đạt"
                    {
                        baiViet.TrangThaiPhanBien = 2; // Đạt, chờ xuất bản
                        baiViet.TrangThai = 4; // Xuất bản
                    }
                    else if (trangThaiPhanBiens.Any(t => t == 4)) // Có "Sửa đổi lớn"
                    {
                        int newVongPhanBien = vongPhanBienHienTai + 1;
                        if (newVongPhanBien <= SO_VONG_PHAN_BIEN_TOI_DA)
                        {
                            foreach (var pc in phanCongs)
                            {
                                db.PhanCongs.Add(new PhanCong
                                {
                                    IDTapChiBaiViet = IDTapChiBaiViet,
                                    IDNguoiPhanBien = pc.IDNguoiPhanBien,
                                    NgayPhanCong = DateTime.Now,
                                    VongPhanBien = newVongPhanBien,
                                    TrangThaiPhanBien = 0 // Chưa phản hồi
                                });
                            }
                            baiViet.TrangThaiPhanBien = 1; // Đang phản biện
                            baiViet.TrangThai = 2; // Đã phân công phản biện
                        }
                        else
                        {
                            baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
                            baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
                        }
                    }
                    else if (trangThaiPhanBiens.Any(t => t == 2)) // Có "Không đạt"
                    {
                        if (vongPhanBienHienTai >= SO_VONG_PHAN_BIEN_TOI_DA)
                        {
                            baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
                            baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
                        }
                        else
                        {
                            baiViet.TrangThaiPhanBien = 3; // Không đạt, chờ chỉnh sửa
                        }
                    }
                    else if (trangThaiPhanBiens.Any(t => t == 3)) // Có "Sửa đổi nhỏ"
                    {
                        if (vongPhanBienHienTai >= SO_VONG_PHAN_BIEN_TOI_DA)
                        {
                            baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
                            baiViet.TrangThai = 4; // Đánh dấu bài viết đã bị từ chối
                        }
                        else
                        {
                            baiViet.TrangThaiPhanBien = 3; // Không đạt, chờ chỉnh sửa
                        }
                    }
                }
                else
                {
                    baiViet.TrangThaiPhanBien = 1; // Vẫn đang phản biện
                }
            }
            // Lưu thay đổi
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