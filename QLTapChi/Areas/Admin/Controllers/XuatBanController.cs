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
                                                              .Max()
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

            return View(danhSachDaPhanBien);
        }

        // GET: Admin/PhanBien/XuatBan
        [HttpGet]
        public ActionResult XuatBan(int id)
        {
            // Kiểm tra đăng nhập và vai trò
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

            // Tìm bài viết
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            // Kiểm tra trạng thái bài viết: Chỉ cho phép xuất bản nếu TrangThaiPhanBien = 2 (Đạt, chờ xuất bản)
            if (baiViet.TrangThaiPhanBien != 2)
            {
                TempData["Error"] = "Bài viết chưa đạt trạng thái để xuất bản.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            // Lấy danh sách số tạp chí để hiển thị trong form
            ViewBag.DanhSachSoTapChi = db.SoTapChis
                .Select(s => new SelectListItem
                {
                    Value = s.IDSoTapChi.ToString(),
                    Text = s.TenSo
                })
                .ToList();

            ViewBag.IDTapChiBaiViet = id;
            return View();
        }

        // POST: Admin/PhanBien/XuatBan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XuatBan(int IDTapChiBaiViet, int IDSoTapChi)
        {
            // Kiểm tra đăng nhập và vai trò
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            int idBienTapVien = (int)Session["idUser"];

            // Tìm bài viết
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == IDTapChiBaiViet);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            // Kiểm tra trạng thái bài viết
            if (baiViet.TrangThaiPhanBien != 2)
            {
                TempData["Error"] = "Bài viết chưa đạt trạng thái để xuất bản.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            // Tìm số tạp chí
            var soTapChi = db.SoTapChis.FirstOrDefault(s => s.IDSoTapChi == IDSoTapChi);
            if (soTapChi == null)
            {
                TempData["Error"] = "Không tìm thấy số tạp chí.";
                return RedirectToAction("DanhSachDaPhanBien");
            }

            // Tạo bản ghi xuất bản
            var xuatBan = new XuatBan
            {
                SoTapChi = soTapChi.TenSo,
                NgayXuatBan = DateTime.Now,
                IDTapChiBaiViet = IDTapChiBaiViet,
                IDBienTapVien = idBienTapVien,
                IDSoTapChi = IDSoTapChi
            };

            // Cập nhật trạng thái bài viết
            baiViet.TrangThai = 4; // Xuất bản
            baiViet.TrangThaiPhanBien = 2; // Giữ trạng thái "Đạt, chờ xuất bản"

            // Lưu thay đổi
            db.XuatBans.Add(xuatBan);
            db.SaveChanges();

            TempData["Success"] = "Xuất bản bài viết thành công.";
            return RedirectToAction("DanhSachDaPhanBien");
        }

        // Hàm chuyển đổi trạng thái phản biện sang văn bản
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
                default: return "Không xác định";
            }
        }

        // Các action khác của PhanBienController (giữ nguyên)
        // GET: Admin/PhanBien/PhanBien
        public ActionResult PhanBien()
        {
            if (Session["idUser"] == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }
            int idPB = (int)Session["idUser"];
            var phanbien = db.NguoiDungs.FirstOrDefault(b => b.IDNguoiDung == idPB);

            if (phanbien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            var baiPhanBien = (from b in db.TapChiBaiViets
                               join p in db.PhanCongs on b.IDTapChiBaiViet equals p.IDTapChiBaiViet
                               where b.TrangThai == 2 && p.IDNguoiPhanBien == idPB
                               orderby b.NgayGui descending
                               select b).ToList();

            return View(baiPhanBien);
        }

        // GET: Admin/PhanBien/DanhSachDaChapNhan
        public ActionResult DanhSachDaChapNhan()
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            int idPB = (int)Session["idUser"];

            var baiDaChapNhan = (from b in db.TapChiBaiViets
                                 join p in db.PhanCongs on b.IDTapChiBaiViet equals p.IDTapChiBaiViet
                                 where b.TrangThaiPhanBien == 1 && p.IDNguoiPhanBien == idPB
                                 orderby b.NgayGui descending
                                 select b).ToList();

            return View(baiDaChapNhan);
        }

        // POST: Admin/PhanBien/ChapNhanPhanBien
        [HttpPost]
        public ActionResult ChapNhanPhanBien(int idBaiViet)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            int idPB = (int)Session["idUser"];

            var phanCong = db.PhanCongs.FirstOrDefault(p => p.IDTapChiBaiViet == idBaiViet && p.IDNguoiPhanBien == idPB);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phân công.";
                return RedirectToAction("PhanBien");
            }

            if (phanCong.TrangThaiPhanBien != 0)
            {
                TempData["Error"] = "Phân công này đã được xử lý.";
                return RedirectToAction("PhanBien");
            }

            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == idBaiViet);
            if (baiViet != null)
            {
                baiViet.TrangThaiPhanBien = 1; // Đang phản biện
            }

            db.SaveChanges();

            TempData["Success"] = "Bạn đã chấp nhận phân công phản biện thành công.";
            return RedirectToAction("PhanBien");
        }

        // GET: Admin/PhanBien/TuChoiPhanBien
        public ActionResult TuChoiPhanBien(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            var phanCong = db.TapChiBaiViets.FirstOrDefault(p => p.IDTapChiBaiViet == id);
            if (phanCong == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin phân công.";
                return RedirectToAction("PhanBien");
            }

            if (phanCong.TrangThaiPhanBien != 0)
            {
                TempData["Error"] = "Phân công này đã được xử lý.";
                return RedirectToAction("PhanBien");
            }

            phanCong.TrangThai = 4;
            db.SaveChanges();

            TempData["Success"] = "Bạn đã từ chối phân công thành công.";
            return RedirectToAction("PhanBien");
        }

        // GET: Admin/PhanBien/GuiPhanBien
        public ActionResult GuiPhanBien(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            int idPB = (int)Session["idUser"];

            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết.";
                return RedirectToAction("PhanBien");
            }

            var phanCong = db.PhanCongs.FirstOrDefault(p => p.IDTapChiBaiViet == id && p.IDNguoiPhanBien == idPB);
            if (phanCong == null)
            {
                TempData["Error"] = "Bạn không được phân công phản biện bài viết này.";
                return RedirectToAction("PhanBien");
            }

            return View(baiViet);
        }

        // POST: Admin/PhanBien/GuiPhanBien
        [HttpPost]
        public ActionResult GuiPhanBien(int IDTapChiBaiViet, string NhanXet, int TrangThaiPhanBien, HttpPostedFileBase fileUpload)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
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

            // Cập nhật trạng thái tổng quát của bài viết
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == IDTapChiBaiViet);
            if (baiViet != null)
            {
                var phanCongs = db.PhanCongs
                    .Where(p => p.IDTapChiBaiViet == IDTapChiBaiViet && p.VongPhanBien == phanCong.VongPhanBien)
                    .ToList();

                // Kiểm tra xem tất cả người phản biện đã gửi phản biện chưa
                if (phanCongs.All(p => p.TrangThaiPhanBien != 0))
                {
                    var trangThaiPhanBiens = phanCongs.Select(p => p.TrangThaiPhanBien).ToList();
                    int vongPhanBienHienTai = phanCong.VongPhanBien ?? 0;
                    const int SO_VONG_PHAN_BIEN_TOI_DA = 3;

                    if (trangThaiPhanBiens.All(t => t == 1)) // Tất cả đều "Đạt"
                    {
                        baiViet.TrangThaiPhanBien = 2; // Đạt, chờ xuất bản
                        baiViet.TrangThai = 4; // Xuất bản
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
                    else if (trangThaiPhanBiens.Any(t => t == 3 || t == 4)) // Có "Sửa đổi nhỏ" hoặc "Sửa đổi lớn"
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
                    // Nếu chưa đủ phản biện, giữ trạng thái "Đang phản biện"
                    baiViet.TrangThaiPhanBien = 1;
                }
            }

            // Lưu thay đổi
            db.SaveChanges();

            TempData["Success"] = "Gửi phản biện thành công.";
            return RedirectToAction("PhanBien");
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