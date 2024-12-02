using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class GheNgoiController : Controller
    {
        // Kết nối đến database
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/GheNgoi
        public ActionResult Index()
        {
            var gheNgoiList = data.Ghe_ngois.ToList();
            return View(gheNgoiList);
        }

        // GET: Admin/GheNgoi/Details/5
        public ActionResult Details(int id)
        {
            var gheNgoi = data.Ghe_ngois.FirstOrDefault(g => g.ghe_id == id);
            if (gheNgoi == null)
            {
                return HttpNotFound();
            }
            return View(gheNgoi);
        }

        // GET: Admin/GheNgoi/Edit/5
        public ActionResult Edit(int id)
        {
            var gheNgoi = data.Ghe_ngois.FirstOrDefault(g => g.ghe_id == id);
            if (gheNgoi == null)
            {
                return HttpNotFound();
            }
            return View(gheNgoi);
        }

        // POST: Admin/GheNgoi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Ghe_ngoi updatedGheNgoi)
        {
            try
            {
                var gheNgoi = data.Ghe_ngois.FirstOrDefault(g => g.ghe_id == id);
                if (gheNgoi == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin ghế
                gheNgoi.so_ghe = updatedGheNgoi.so_ghe;
                gheNgoi.man_hinh_id = updatedGheNgoi.man_hinh_id;
                gheNgoi.loai_ghe_id = updatedGheNgoi.loai_ghe_id;
                gheNgoi.rap_id = updatedGheNgoi.rap_id;

                // Lưu thay đổi
                data.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/GheNgoi/Delete/5
        public ActionResult Delete(int id)
        {
            var gheNgoi = data.Ghe_ngois.FirstOrDefault(g => g.ghe_id == id);
            if (gheNgoi == null)
            {
                return HttpNotFound();
            }
            return View(gheNgoi);
        }

        // POST: Admin/GheNgoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                var gheNgoi = data.Ghe_ngois.FirstOrDefault(g => g.ghe_id == id);
                if (gheNgoi == null)
                {
                    return HttpNotFound();
                }

                // Xóa ghế
                data.Ghe_ngois.DeleteOnSubmit(gheNgoi);

                // Lưu thay đổi
                data.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/GheNgoi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/GheNgoi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ghe_ngoi newGheNgoi)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Thêm ghế mới vào database
                    data.Ghe_ngois.InsertOnSubmit(newGheNgoi);

                    // Lưu thay đổi
                    data.SubmitChanges();

                    return RedirectToAction("Index");
                }
                return View(newGheNgoi);
            }
            catch
            {
                return View();
            }
        }

    }
}
