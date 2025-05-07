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

            // Lấy danh sách phân công phản biện
            var phanCong = (from pc in db.PhanCongs
                            join nd in db.NguoiDungs on pc.IDNguoiPhanBien equals nd.IDNguoiDung
                            where pc.IDTapChiBaiViet == id
                            select new PhanCongViewModel
                            {
                                IDPhanCong = pc.IDPhanCong,
                                IDNguoiPhanBien = pc.IDNguoiPhanBien,
                                NguoiPhanBien = nd.HoTen,
                                EmailNguoiPhanBien = nd.Email,
                                TrangThaiPhanBien = pc.TrangThaiPhanBien,
                                VongPhanBien = pc.VongPhanBien,
                               
                            }).ToList();

            // Tạo ViewModel
            var viewModel = new XemPhanBienViewModel
            {
                BaiViet = baiViet,
                DanhSachPhanBien = phanBien,
                DanhSachPhanCong = phanCong
            };

            // Kiểm tra nếu bài viết bị từ chối vĩnh viễn
            if (baiViet.TrangThaiPhanBien == 4)
            {
                TempData["Error"] = "Bài viết đã bị từ chối sau 3 vòng phản biện. Vui lòng viết lại và nộp bài mới.";
            }

            return View(viewModel);
        }
        [HttpGet]
        public ActionResult ChinhSuaSauPhanBien(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idNguoiDung = (int)Session["idUser"];
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id && b.IDNguoiGui == idNguoiDung);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết hoặc bạn không có quyền truy cập.";
                return RedirectToAction("DanhSachTapChi");
            }

            // Kiểm tra xem bài viết có trạng thái cho phép chỉnh sửa không
            var danhSachPhanCong = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == id)
                .ToList();
            if (baiViet.TrangThaiPhanBien == 4 || !danhSachPhanCong.Any(pc => pc.TrangThaiPhanBien == 3 || pc.TrangThaiPhanBien == 4))
            {
                TempData["Error"] = "Bài viết không ở trạng thái cho phép chỉnh sửa.";
                return RedirectToAction("XemPhanBien", new { id = id });
            }

            // Gán danh sách lĩnh vực cho DropDownList
            var linhVucList = db.LinhVucs.ToList();
            if (linhVucList == null || !linhVucList.Any())
            {
                TempData["Error"] = "Không có lĩnh vực nào được định nghĩa. Vui lòng liên hệ quản trị viên.";
                // Tùy chọn: Có thể trả về một view khác hoặc redirect
                return RedirectToAction("DanhSachTapChi");
            }
            ViewBag.LinhVucList = linhVucList;

            return View(baiViet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult ChinhSuaSauPhanBien(TapChiBaiViet model, HttpPostedFileBase File, string GhiChu)
        //{
        //    if (Session["idUser"] == null)
        //    {
        //        TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
        //        return RedirectToAction("DangNhap", "TaiKhoan");
        //    }

        //    int idNguoiDung = (int)Session["idUser"];
        //    var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == model.IDTapChiBaiViet && b.IDNguoiGui == idNguoiDung);
        //    if (baiViet == null)
        //    {
        //        TempData["Error"] = "Không tìm thấy bài viết hoặc bạn không có quyền truy cập.";
        //        return RedirectToAction("DanhSachTapChi");
        //    }

        //    // Lấy vòng phản biện hiện tại
        //    var vongPhanBienHienTai = db.PhanCongs
        //        .Where(p => p.IDTapChiBaiViet == model.IDTapChiBaiViet)
        //        .Max(p => p.VongPhanBien) ?? 1;

        //    // Lưu lịch sử chỉnh sửa
        //    var lichSuChinhSua = new LichSuChinhSua
        //    {
        //        IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
        //        NoiDungCu = baiViet.NoiDung,
        //        NoiDungMoi = baiViet.NoiDung, // Sẽ cập nhật nếu có file mới
        //        NgayChinhSua = DateTime.Now,
        //        VongPhanBien = vongPhanBienHienTai,
        //        LanChinhSua = db.LichSuChinhSuas
        //            .Where(l => l.IDTapChiBaiViet == baiViet.IDTapChiBaiViet && l.VongPhanBien == vongPhanBienHienTai)
        //            .Count() + 1,
        //        GhiChu = GhiChu,
        //        IDNguoiChinhSua = idNguoiDung
        //    };

        //    // Cập nhật bài viết
        //    baiViet.TieuDe = model.TieuDe;
        //    baiViet.TacGia = model.TacGia;
        //    baiViet.IDLinhVuc = model.IDLinhVuc;
        //    baiViet.TuKhoa = model.TuKhoa;
        //    baiViet.DongTacGia = model.DongTacGia;

        //    if (File != null && File.ContentLength > 0)
        //    {
        //        string rootFolder = Server.MapPath("/Content/BaiViet/");
        //        string pathImage = rootFolder + File.FileName;
        //        File.SaveAs(pathImage);
        //        baiViet.NoiDung = "Content/BaiViet/" + File.FileName;
        //        lichSuChinhSua.NoiDungMoi = baiViet.NoiDung;
        //        lichSuChinhSua.DuongDanFile = baiViet.NoiDung;
        //    }

        //    // Thêm bản ghi lịch sử chỉnh sửa
        //    db.LichSuChinhSuas.Add(lichSuChinhSua);

        //    // Tạo phân công phản biện mới cho vòng tiếp theo
        //    var danhSachPhanCongHienTai = db.PhanCongs
        //        .Where(p => p.IDTapChiBaiViet == baiViet.IDTapChiBaiViet && p.VongPhanBien == vongPhanBienHienTai)
        //        .ToList();

        //    int newVongPhanBien = vongPhanBienHienTai + 1;
        //    foreach (var phanCong in danhSachPhanCongHienTai)
        //    {
        //        // Nếu chỉnh sửa nhỏ (TrangThaiPhanBien = 3) và có ít nhất một người đạt
        //        if (phanCong.TrangThaiPhanBien == 3 && danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 1))
        //        {
        //            if (phanCong.TrangThaiPhanBien == 3) // Chỉ phân công lại cho người yêu cầu chỉnh sửa nhỏ
        //            {
        //                db.PhanCongs.Add(new PhanCong
        //                {
        //                    IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
        //                    IDNguoiPhanBien = phanCong.IDNguoiPhanBien,
        //                    NgayPhanCong = DateTime.Now,
        //                    VongPhanBien = newVongPhanBien,
        //                    TrangThaiPhanBien = 0 // Chưa phản hồi
        //                });
        //            }
        //        }
        //        else // Trường hợp chỉnh sửa lớn hoặc không có ai đạt
        //        {
        //            db.PhanCongs.Add(new PhanCong
        //            {
        //                IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
        //                IDNguoiPhanBien = phanCong.IDNguoiPhanBien,
        //                NgayPhanCong = DateTime.Now,
        //                VongPhanBien = newVongPhanBien,
        //                TrangThaiPhanBien = 0 // Chưa phản hồi
        //            });
        //        }
        //    }

        //    // Cập nhật trạng thái bài viết
        //    baiViet.TrangThaiPhanBien = 1; // Đang phản biện
        //    db.SaveChanges();

        //    TempData["Success"] = "Chỉnh sửa bài viết thành công và đã gửi lại để phản biện.";
        //    return RedirectToAction("DanhSachTapChi");
        //}
        public ActionResult ChinhSuaSauPhanBien(TapChiBaiViet model, HttpPostedFileBase File, string GhiChu)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idNguoiDung = (int)Session["idUser"];
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == model.IDTapChiBaiViet && b.IDNguoiGui == idNguoiDung);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết hoặc bạn không có quyền truy cập.";
                return RedirectToAction("DanhSachTapChi");
            }

            // Lấy vòng phản biện hiện tại
            var vongPhanBienHienTai = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == model.IDTapChiBaiViet)
                .Max(p => p.VongPhanBien) ?? 1;

            // Lưu lịch sử chỉnh sửa
            var lichSuChinhSua = new LichSuChinhSua
            {
                IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
                NoiDungCu = baiViet.NoiDung,
                NoiDungMoi = baiViet.NoiDung, // Sẽ cập nhật nếu có file mới
                NgayChinhSua = DateTime.Now,
                VongPhanBien = vongPhanBienHienTai,
                LanChinhSua = db.LichSuChinhSuas
                    .Where(l => l.IDTapChiBaiViet == baiViet.IDTapChiBaiViet && l.VongPhanBien == vongPhanBienHienTai)
                    .Count() + 1,
                GhiChu = GhiChu,
                IDNguoiChinhSua = idNguoiDung
            };

            // Cập nhật bài viết
            baiViet.TieuDe = model.TieuDe;
            baiViet.TacGia = model.TacGia;
            baiViet.IDLinhVuc = model.IDLinhVuc;
            baiViet.TuKhoa = model.TuKhoa;
            baiViet.DongTacGia = model.DongTacGia;

            if (File != null && File.ContentLength > 0)
            {
                string rootFolder = Server.MapPath("/Content/BaiViet/");
                string pathImage = rootFolder + File.FileName;
                File.SaveAs(pathImage);
                baiViet.NoiDung = "Content/BaiViet/" + File.FileName;
                lichSuChinhSua.NoiDungMoi = baiViet.NoiDung;
                lichSuChinhSua.DuongDanFile = baiViet.NoiDung;
            }

            // Thêm bản ghi lịch sử chỉnh sửa
            db.LichSuChinhSuas.Add(lichSuChinhSua);

            // Tạo vòng phản biện mới
            var danhSachPhanCongHienTai = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == baiViet.IDTapChiBaiViet && p.VongPhanBien == vongPhanBienHienTai)
                .ToList();

            //int newVongPhanBien = vongPhanBienHienTai + 1;
            //bool coSuaDoiNho = danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 3);
            //bool coNguoiDat = danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 1);
            //bool coSuaDoiLon = danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 4);
            //foreach (var phanCong in danhSachPhanCongHienTai)
            //{
            //    // Nếu có "Sửa đổi nhỏ" và có ít nhất một người "Đạt", chỉ phân công lại cho người yêu cầu sửa đổi nhỏ
            //    if (coSuaDoiNho && coNguoiDat && phanCong.TrangThaiPhanBien == 3)
            //    {
            //        db.PhanCongs.Add(new PhanCong
            //        {
            //            IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
            //            IDNguoiPhanBien = phanCong.IDNguoiPhanBien,
            //            NgayPhanCong = DateTime.Now,
            //            VongPhanBien = newVongPhanBien,
            //            TrangThaiPhanBien = 0 // Chưa phản hồi
            //        });
            //    }
            //    // Nếu không, phân công lại cho tất cả người phản biện
            //    else if (!coSuaDoiNho || !coNguoiDat || phanCong.TrangThaiPhanBien == 4)
            //    {
            //        db.PhanCongs.Add(new PhanCong
            //        {
            //            IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
            //            IDNguoiPhanBien = phanCong.IDNguoiPhanBien,
            //            NgayPhanCong = DateTime.Now,
            //            VongPhanBien = newVongPhanBien,
            //            TrangThaiPhanBien = 0 // Chưa phản hồi
            //        });
            //    }
            //}
            bool coSuaDoiNho = danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 3);
            bool coNguoiDat = danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 1);
            bool coSuaDoiLon = danhSachPhanCongHienTai.Any(p => p.TrangThaiPhanBien == 4);

            // Trong trường hợp "Sửa đổi nhỏ", giữ trạng thái "Chờ chỉnh sửa" để tác giả có thể tiếp tục chỉnh sửa
            if (coSuaDoiNho && coNguoiDat && !coSuaDoiLon) // Chỉ có "sửa đổi nhỏ" và có ít nhất một người "đạt"
            {
                foreach (var phanCong in danhSachPhanCongHienTai)
                {
                    if (phanCong.TrangThaiPhanBien == 3) // Chỉ đặt lại trạng thái cho người yêu cầu sửa đổi nhỏ
                    {
                        phanCong.TrangThaiPhanBien = 5; // Chấp nhận phản biện, sẵn sàng đánh giá lại khi bài viết được gửi lại
                    }
                }
                // Giữ trạng thái "Chờ chỉnh sửa" (3) để tác giả tiếp tục chỉnh sửa nếu cần
                baiViet.TrangThaiPhanBien = 3; // Chờ chỉnh sửa
                baiViet.TrangThai = 2; // Đã phân công phản biện
            }
            else // Có "sửa đổi lớn" hoặc không có ai đạt
            {
                const int SO_VONG_PHAN_BIEN_TOI_DA = 3;
                int newVongPhanBien = vongPhanBienHienTai + 1;

                if (newVongPhanBien > SO_VONG_PHAN_BIEN_TOI_DA)
                {
                    baiViet.TrangThaiPhanBien = 4; // Từ chối vĩnh viễn
                    baiViet.TrangThai = 4;
                }
                else
                {
                    // Phân công lại cho tất cả người phản biện
                    foreach (var phanCong in danhSachPhanCongHienTai)
                    {
                        db.PhanCongs.Add(new PhanCong
                        {
                            IDTapChiBaiViet = baiViet.IDTapChiBaiViet,
                            IDNguoiPhanBien = phanCong.IDNguoiPhanBien,
                            NgayPhanCong = DateTime.Now,
                            VongPhanBien = newVongPhanBien,
                            TrangThaiPhanBien = 0 // Chưa phản hồi
                        });
                    }
                    baiViet.TrangThaiPhanBien = 1; // Đang phản biện
                    baiViet.TrangThai = 2; // Đã phân công phản biện
                }
            }

            //// Cập nhật trạng thái bài viết
            //baiViet.TrangThaiPhanBien = 1; // Đang phản biện
            //baiViet.TrangThai = 2; // Đã phân công phản biện

            // Gửi email thông báo cho người phản biện (chỉ khi gửi lại để phản biện, nhưng để ở đây để thử nghiệm)
            //foreach (var phanCong in danhSachPhanCongHienTai)
            //{
            //    var nguoiPhanBien = db.NguoiDungs.FirstOrDefault(nd => nd.IDNguoiDung == phanCong.IDNguoiPhanBien);
            //    if (nguoiPhanBien != null)
            //    {
            //        string filePath = HttpContext.Server.MapPath("~/Content/notify_phanbien.html");
            //        string content = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : "Thông báo: Bài viết đã được chỉnh sửa.";

            //        content = content.Replace("{{TieuDeBaiViet}}", baiViet.TieuDe);
            //        content = content.Replace("{{TacGia}}", baiViet.TacGia);

            //        SendMail.sendMail(
            //            name: "Hệ thống QLTapChi",
            //            subject: $"Bài viết #{baiViet.IDTapChiBaiViet} đã được chỉnh sửa",
            //            content: content,
            //            toMail: nguoiPhanBien.Email
            //        );
            //    }
            //}
            db.SaveChanges();

            TempData["Success"] = "Chỉnh sửa bài viết thành công và đã gửi lại để phản biện.";
            return RedirectToAction("DanhSachTapChi");
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
            return RedirectToAction("DanhSachTapChi");

        }
        public ActionResult XoaBaiBao(int id)
        {
            var model = db.TapChiBaiViets.Find(id);
            if (model != null)
            {
                db.TapChiBaiViets.Remove(model);
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachTapChi");
        }
        [HttpPost]
        public ActionResult GuiLaiPhanBien(int id)
        {
            if (Session["idUser"] == null)
            {
                TempData["Error"] = "Bạn chưa đăng nhập hoặc không có quyền truy cập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int idNguoiDung = (int)Session["idUser"];
            var baiViet = db.TapChiBaiViets.FirstOrDefault(b => b.IDTapChiBaiViet == id && b.IDNguoiGui == idNguoiDung);
            if (baiViet == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết hoặc bạn không có quyền truy cập.";
                return RedirectToAction("DanhSachTapChi");
            }

            // Kiểm tra xem bài viết có đang ở trạng thái "Chờ chỉnh sửa" không
            if (baiViet.TrangThaiPhanBien != 3)
            {
                TempData["Error"] = "Bài viết không ở trạng thái chờ chỉnh sửa.";
                return RedirectToAction("DanhSachTapChi");
            }

            // Lấy vòng phản biện hiện tại
            var vongPhanBienHienTai = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == id)
                .Max(p => p.VongPhanBien) ?? 1;

            var danhSachPhanCongHienTai = db.PhanCongs
                .Where(p => p.IDTapChiBaiViet == baiViet.IDTapChiBaiViet && p.VongPhanBien == vongPhanBienHienTai)
                .ToList();

            // Cập nhật trạng thái bài viết thành "Đang phản biện"
            baiViet.TrangThaiPhanBien = 1; // Đang phản biện
            baiViet.TrangThai = 2; // Đã phân công phản biện

            // Gửi email thông báo cho người phản biện
            foreach (var phanCong in danhSachPhanCongHienTai)
            {
                var nguoiPhanBien = db.NguoiDungs.FirstOrDefault(nd => nd.IDNguoiDung == phanCong.IDNguoiPhanBien);
                if (nguoiPhanBien != null)
                {
                    string filePath = HttpContext.Server.MapPath("~/Content/notify_phanbien.html");
                    string content = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : "Thông báo: Bài viết đã được chỉnh sửa.";

                    content = content.Replace("{{TieuDeBaiViet}}", baiViet.TieuDe);
                    content = content.Replace("{{TacGia}}", baiViet.TacGia);

                    SendMail.sendMail(
                        name: "Hệ thống QLTapChi",
                        subject: $"Bài viết #{baiViet.IDTapChiBaiViet} đã được chỉnh sửa",
                        content: content,
                        toMail: nguoiPhanBien.Email
                    );
                }
            }

            db.SaveChanges();

            TempData["Success"] = "Bài viết đã được gửi lại để phản biện.";
            return RedirectToAction("DanhSachTapChi");
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