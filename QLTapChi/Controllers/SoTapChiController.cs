using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Controllers
{
    public class SoTapChiController : Controller
    {
        public QLTapChiEntities db = new QLTapChiEntities();

        // GET: SoTapChi/SoHienTai
        public ActionResult SoHienTai()
        {
            // Lấy số tạp chí hiện tại (số mới nhất)
            var soTapChiHienTai = db.SoTapChis
                .OrderByDescending(s => s.NgayPhatHanh)
                .FirstOrDefault();

            ViewBag.SoTapChiHienTai = soTapChiHienTai != null
                ? $"Số {soTapChiHienTai.TenSo} (Tập. 61 Số. 1 - 2025)"
                : "Chưa có số tạp chí";

            // Lấy bài viết thuộc số hiện tại (nếu có)
            var baiVietHienTai = soTapChiHienTai != null
                ? db.XuatBans
                    .Where(x => x.IDSoTapChi == soTapChiHienTai.IDSoTapChi)
                    .Join(db.TapChiBaiViets,
                        xuatBan => xuatBan.IDTapChiBaiViet,
                        baiViet => baiViet.IDTapChiBaiViet,
                        (xuatBan, baiViet) => new
                        {
                            baiViet.IDTapChiBaiViet,
                            baiViet.TieuDe,
                            baiViet.TacGia,
                            baiViet.DongTacGia,
                            baiViet.TomTat,
                            baiViet.NoiDung,
                            baiViet.TrangThai // Thêm TrangThai vào anonymous type
                        })
                    .Where(b => b.TrangThai == 3) // Chỉ lấy bài viết đã xuất bản
                    .ToList()
                    .Select(b => new TapChiBaiViet
                    {
                        IDTapChiBaiViet = b.IDTapChiBaiViet,
                        TieuDe = b.TieuDe,
                        TacGia = b.TacGia,
                        DongTacGia = b.DongTacGia,
                        TomTat = b.TomTat,
                        NoiDung = b.NoiDung
                    })
                    .ToList()
                : new List<TapChiBaiViet>();

            return View(baiVietHienTai);
        }

        // GET: SoTapChi/PublishAll
        // GET: SoTapChi/PublishAll
       public ActionResult PublishAll()
        {
            // Lấy tất cả số tạp chí, sắp xếp theo ngày phát hành giảm dần
            var soTapChis = db.SoTapChis
                .OrderByDescending(s => s.NgayPhatHanh)
                .ToList();
            return View(soTapChis);
        }

        // GET: SoTapChi/IssueDetails/5
        public ActionResult IssueDetails(int id)
        {
            // Lấy số tạp chí theo ID
            var soTapChi = db.SoTapChis
                .FirstOrDefault(s => s.IDSoTapChi == id);

            if (soTapChi == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách bài viết thuộc số tạp chí này
            var baiViets = db.XuatBans
                .Where(x => x.IDSoTapChi == id)
                .Join(db.TapChiBaiViets,
                    xuatBan => xuatBan.IDTapChiBaiViet,
                    baiViet => baiViet.IDTapChiBaiViet,
                    (xuatBan, baiViet) => new
                    {
                        baiViet.IDTapChiBaiViet,
                        baiViet.TieuDe,
                        baiViet.TacGia,
                        baiViet.DongTacGia,
                        baiViet.TomTat,
                        baiViet.NoiDung,
                        baiViet.TrangThai // Thêm TrangThai vào anonymous type
                    })
                .Where(b => b.TrangThai == 3) // Chỉ lấy bài viết đã xuất bản
                .ToList()
                .Select(b => new TapChiBaiViet
                {
                    IDTapChiBaiViet = b.IDTapChiBaiViet,
                    TieuDe = b.TieuDe,
                    TacGia = b.TacGia,
                    DongTacGia = b.DongTacGia,
                    TomTat = b.TomTat,
                    NoiDung = b.NoiDung
                })
                .ToList();

            // Truyền thông tin số tạp chí vào ViewBag
            ViewBag.SoTapChiHienTai = $"Số {soTapChi.TenSo} ({soTapChi.NgayPhatHanh.ToString("dd/MM/yyyy")})";

            return View(baiViets);
        }
    }
}