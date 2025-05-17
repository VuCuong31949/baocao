using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Nếu Session không tồn tại mà Cookie có tồn tại, khôi phục lại Session từ Cookie
            //if (Session["UserName"] == null && Request.Cookies["UserName"] != null)
            //{
            //    Session["UserName"] = Request.Cookies["UserName"].Value;
            //    Session["idUser"] = Request.Cookies["UserId"]?.Value;  // Dùng ? để tránh lỗi nếu cookie không tồn tại
            //    Session["LoaiNguoiDung"] = Request.Cookies["LoaiNguoiDung"]?.Value;
            //    // Nếu là Biên tập viên thì khôi phục thêm
            //    if (Request.Cookies["LoaiBienTapVien"] != null)
            //    {
            //        Session["LoaiBienTapVien"] = Request.Cookies["LoaiBienTapVien"].Value;
            //    }
            //}
            // Điều hướng tự động nếu là Biên tập viên
            if (Session["LoaiNguoiDung"] != null && Session["LoaiNguoiDung"].ToString() == "BienTapVien")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { controller = "BienTapViens", action = "DanhSachBTV", area = "Admin" })
                );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}