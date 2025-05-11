using QLTapChi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLTapChi.Areas.Admin.Controllers
{
    public class LinhVucController : Controller
    {
        // GET: Admin/LinhVuc

        QLTapChiEntities db = new QLTapChiEntities();
        public ActionResult DanhSachLinhVuc()
        {
            var danhSach = db.LinhVucs.ToList();
            return View(danhSach);
        }
        public ActionResult createLV()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult createLV(LinhVuc model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    db.LinhVucs.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachLinhVuc");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi thêm Linh Vuc: " + ex.Message);
                }
            }
            return View(model);
            
        }
        public ActionResult Edit(int id)
        {
            var linhVuc = db.LinhVucs.Find(id);
            if (linhVuc == null)
            {
                TempData["Error"] = "Không tìm thấy lĩnh vực.";
                return RedirectToAction("DanhSachLinhVuc");
            }
            return View(linhVuc);
        }

        // POST: Admin/LinhVuc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LinhVuc model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var linhVuc = db.LinhVucs.Find(model.IDLinhVuc);
                    if (linhVuc == null)
                    {
                        TempData["Error"] = "Không tìm thấy lĩnh vực.";
                        return RedirectToAction("DanhSachLinhVuc");
                    }
                    linhVuc.TenLinhVuc = model.TenLinhVuc;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật lĩnh vực thành công!";
                    return RedirectToAction("DanhSachLinhVuc");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi cập nhật lĩnh vực: " + ex.Message;
                }
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var linhVuc = db.LinhVucs.Find(id);
            if (linhVuc == null)
            {
                TempData["Error"] = "Không tìm thấy lĩnh vực.";
                return RedirectToAction("DanhSachLinhVuc");
            }
            return View(linhVuc);
        }

        public ActionResult Delete(int id)
        {
            var linhVuc = db.LinhVucs.Find(id);
            if (linhVuc == null)
            {
                TempData["Error"] = "Không tìm thấy lĩnh vực.";
                return RedirectToAction("DanhSachLinhVuc");
            }
            try
            {
                db.LinhVucs.Remove(linhVuc);
                db.SaveChanges();
                TempData["Success"] = "Xóa lĩnh vực thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi xóa lĩnh vực: " + ex.Message;
            }
            return RedirectToAction("DanhSachLinhVuc");
        }
    }
}