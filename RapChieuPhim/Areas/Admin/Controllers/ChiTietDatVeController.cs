using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class ChiTietDatVeController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/ChiTietDatVe
        public ActionResult Index()
        {
            var chiTietDatVeList = data.Chi_tiet_dat_ves.ToList();
            return View(chiTietDatVeList);
        }

        // GET: Admin/ChiTietDatVe/Details/5
        public ActionResult Details(int id)
        {
            var chiTiet = data.Chi_tiet_dat_ves.SingleOrDefault(ct => ct.chi_tiet_dat_ve_id == id);
            if (chiTiet == null)
            {
                return HttpNotFound();
            }
            return View(chiTiet);
        }

        // GET: Admin/ChiTietDatVe/Edit/5
        public ActionResult Edit(int id)
        {
            var chiTiet = data.Chi_tiet_dat_ves.SingleOrDefault(ct => ct.chi_tiet_dat_ve_id == id);
            if (chiTiet == null)
            {
                return HttpNotFound();
            }
            return View(chiTiet);
        }

        // POST: Admin/ChiTietDatVe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Chi_tiet_dat_ve chiTiet)
        {
            if (ModelState.IsValid)
            {
                var existingChiTiet = data.Chi_tiet_dat_ves.SingleOrDefault(ct => ct.chi_tiet_dat_ve_id == chiTiet.chi_tiet_dat_ve_id);
                if (existingChiTiet != null)
                {
                    existingChiTiet.dat_ve_id = chiTiet.dat_ve_id;
                    existingChiTiet.gia_ve = chiTiet.gia_ve;
                    existingChiTiet.ghe_id = chiTiet.ghe_id;
                    // Update other fields as needed
                    data.SubmitChanges();
                }
                return RedirectToAction("Index");
            }
            return View(chiTiet);
        }

        // GET: Admin/ChiTietDatVe/Delete/5
        public ActionResult Delete(int id)
        {
            var chiTiet = data.Chi_tiet_dat_ves.SingleOrDefault(ct => ct.chi_tiet_dat_ve_id == id);
            if (chiTiet == null)
            {
                return HttpNotFound();
            }
            return View(chiTiet);
        }

        // POST: Admin/ChiTietDatVe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var chiTiet = data.Chi_tiet_dat_ves.SingleOrDefault(ct => ct.chi_tiet_dat_ve_id == id);
            if (chiTiet != null)
            {
                data.Chi_tiet_dat_ves.DeleteOnSubmit(chiTiet);
                data.SubmitChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
