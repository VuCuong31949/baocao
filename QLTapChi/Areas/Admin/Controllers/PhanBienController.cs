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
                                    where  pc.TrangThaiPhanBien != 0
                                    //where pc.IDNguoiPhanBien == idPB && pc.TrangThaiPhanBien != 0
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
        [HttpGet]
        public ActionResult SendTestMail(string toMail)
        {
            // Kiểm tra email người nhận
            if (string.IsNullOrWhiteSpace(toMail) || !IsValidEmail(toMail))
            {
                return Json(new { success = false, message = "Vui lòng cung cấp địa chỉ email hợp lệ." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Mẫu HTML nhúng
                string content = @"
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"">
                        <tbody>
                            <tr>
                                <td align=""center"" valign=""top"">
                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color:#ffffff;border:1px solid #dedede;border-radius:3px"">
                                        <tbody>
                                            <tr>
                                                <td align=""center"" valign=""top"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color:#007bff;color:#ffffff;border-bottom:0;font-weight:bold;line-height:100%;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;border-radius:3px 3px 0 0"">
                                                        <tbody>
                                                            <tr>
                                                                <td style=""padding:36px 48px;display:block"">
                                                                    <h1 style=""font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:30px;font-weight:300;line-height:150%;margin:0;text-align:left;color:#ffffff;background-color:inherit"">Phân công phản biện thử nghiệm: #TEST</h1>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align=""center"" valign=""top"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                                                        <tbody>
                                                            <tr>
                                                                <td valign=""top"" style=""background-color:#ffffff"">
                                                                    <table border=""0"" cellpadding=""20"" cellspacing=""0"" width=""100%"">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td valign=""top"" style=""padding:48px 48px 32px"">
                                                                                    <div style=""color:#636363;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:14px;line-height:150%;text-align:left"">
                                                                                        <p style=""margin:0 0 16px"">Kính gửi {{TenNguoiPhanBien}},</p>
                                                                                        <p style=""margin:0 0 16px"">Đây là email thử nghiệm để kiểm tra hệ thống gửi email. Vui lòng bỏ qua nếu bạn nhận được email này.</p>
                                                                                        <h2 style=""color:#007bff;display:block;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:18px;font-weight:bold;line-height:130%;margin:0 0 18px;text-align:left"">
                                                                                            <a href=""{{LinkPhanBien}}"" style=""font-weight:normal;text-decoration:underline;color:#007bff"" target=""_blank"">[Phân công #TEST]</a> ({{NgayPhanCong}})
                                                                                        </h2>
                                                                                        <div style=""margin-bottom:40px"">
                                                                                            <table cellspacing=""0"" cellpadding=""6"" border=""1"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;width:100%;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif"">
                                                                                                <thead>
                                                                                                    <tr>
                                                                                                        <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Tiêu đề bài viết</th>
                                                                                                        <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Tác giả</th>
                                                                                                        <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Lĩnh vực</th>
                                                                                                        <th scope=""col"" style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">Vòng phản biện</th>
                                                                                                    </tr>
                                                                                                </thead>
                                                                                                <tbody>
                                                                                                    <tr>
                                                                                                        <td style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">{{TieuDeBaiViet}}</td>
                                                                                                        <td style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">{{TacGia}}</td>
                                                                                                        <td style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">{{TenLinhVuc}}</td>
                                                                                                        <td style=""color:#636363;border:1px solid #e5e5e5;vertical-align:middle;padding:12px;text-align:left"">{{VongPhanBien}}</td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                        <p style=""margin:0 0 16px"">Vui lòng truy cập đường link sau để kiểm tra:</p>
                                                                                        <p style=""margin:0 0 16px"">
                                                                                            <a href=""{{LinkPhanBien}}"" style=""background-color:#007bff;color:#ffffff;padding:10px 20px;text-decoration:none;border-radius:5px;"" target=""_blank"">Truy cập trang thử nghiệm</a>
                                                                                        </p>
                                                                                        <table cellspacing=""0"" cellpadding=""0"" border=""0"" style=""width:100%;vertical-align:top;margin-bottom:40px;padding:0"">
                                                                                            <tbody>
                                                                                                <tr>
                                                                                                    <td valign=""top"" width=""50%"" style=""text-align:left;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;border:0;padding:0"">
                                                                                                        <h2 style=""color:#007bff;display:block;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;font-size:18px;font-weight:bold;line-height:130%;margin:0 0 18px;text-align:left"">Thông tin người nhận</h2>
                                                                                                        <address style=""padding:12px;color:#636363;border:1px solid #e5e5e5"">
                                                                                                            {{TenNguoiPhanBien}}<br>
                                                                                                            <a href=""mailto:{{Email}}"" style=""color:#007bff;font-weight:normal;text-decoration:underline"" target=""_blank"">{{Email}}</a>
                                                                                                        </address>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </tbody>
                                                                                        </table>
                                                                                        <p style=""margin:0 0 16px"">Trân trọng,<br>Biên tập viên QLTapChi</p>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>";

                // Thay thế các placeholder với giá trị giả lập
                content = content.Replace("{{IDPhanCong}}", "TEST");
                content = content.Replace("{{TieuDeBaiViet}}", "Bài viết thử nghiệm");
                content = content.Replace("{{TacGia}}", "Tác giả thử nghiệm");
                content = content.Replace("{{TenLinhVuc}}", "Lĩnh vực thử nghiệm");
                content = content.Replace("{{TenNguoiPhanBien}}", "Người nhận thử nghiệm");
                content = content.Replace("{{Email}}", toMail);
                content = content.Replace("{{NgayPhanCong}}", DateTime.Now.ToString("dd/MM/yyyy"));
                content = content.Replace("{{VongPhanBien}}", "1");
                string fullLink = "https://yourdomain.com/TaiKhoan/DangNhap"; // Thay bằng domain của bạn
                if (Request?.Url != null)
                {
                    var link = Url.Action("DangNhap", "TaiKhoan", null, Request.Url.Scheme);
                    fullLink = link;
                }
                content = content.Replace("{{LinkPhanBien}}", fullLink);

                // Gửi email
                bool emailSent = SendMail.sendMail(
                    name: "Biên tập viên QLTapChi",
                    subject: "Phân công phản biện thử nghiệm: #TEST",
                    content: content,
                    toMail: toMail
                );

                if (emailSent)
                {
                    return Json(new { success = true, message = "Email thử nghiệm đã được gửi thành công!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Gửi email thử nghiệm thất bại. Vui lòng kiểm tra log." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi gửi email: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Hàm kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}