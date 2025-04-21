using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class PhanBienController : Controller
    {
        QLTapChiEntities db = new QLTapChiEntities();

        // Action: Lấy danh sách các phân công phản biện đã xử lý
        public ActionResult DanhSachDaPhanBien()
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
    }
}