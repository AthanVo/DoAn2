using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class DatChoController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/DatCho
        public ActionResult Index()
        {
            var datChoList = data.Dat_chos.ToList();
            return View(datChoList);
        }

        // Chi tiết đặt chỗ
        public ActionResult Details(int ghe_id, int suat_chieu_id)
        {
            var datCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == ghe_id && dc.suat_chieu_id == suat_chieu_id);
            if (datCho == null)
            {
                return HttpNotFound();
            }
            return View(datCho);
        }


        // Tạo mới đặt chỗ (GET)
        public ActionResult Create()
        {
            ViewBag.SuatChieuList = new SelectList(data.Suat_chieus, "suat_chieu_id", "gio_bat_dau");
            ViewBag.GheList = new SelectList(data.Ghe_ngois, "ghe_id", "so_ghe");
            return View();
        }

        // Tạo mới đặt chỗ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dat_cho datCho)
        {
            if (ModelState.IsValid)
            {
                // Thêm mới đặt chỗ
                data.Dat_chos.InsertOnSubmit(datCho);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SuatChieuList = new SelectList(data.Suat_chieus, "suat_chieu_id", "gio_bat_dau", datCho.suat_chieu_id);
            ViewBag.GheList = new SelectList(data.Ghe_ngois, "ghe_id", "so_ghe", datCho.ghe_id);
            return View(datCho);
        }

        // Sửa đặt chỗ (GET)
        public ActionResult Edit(int ghe_id, int suat_chieu_id)
        {
            var datCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == ghe_id && dc.suat_chieu_id == suat_chieu_id);
            if (datCho == null)
            {
                return HttpNotFound();
            }
            ViewBag.SuatChieuList = new SelectList(data.Suat_chieus, "suat_chieu_id", "gio_bat_dau", datCho.suat_chieu_id);
            ViewBag.GheList = new SelectList(data.Ghe_ngois, "ghe_id", "so_ghe", datCho.ghe_id);
            return View(datCho);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int ghe_id, int suat_chieu_id, Dat_cho updatedDatCho)
        {
            if (ModelState.IsValid)
            {
                var existingDatCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == ghe_id && dc.suat_chieu_id == suat_chieu_id);
                if (existingDatCho == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin
                existingDatCho.da_dat = updatedDatCho.da_dat;

                // Lưu thay đổi
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SuatChieuList = new SelectList(data.Suat_chieus, "suat_chieu_id", "gio_bat_dau", updatedDatCho.suat_chieu_id);
            ViewBag.GheList = new SelectList(data.Ghe_ngois, "ghe_id", "so_ghe", updatedDatCho.ghe_id);
            return View(updatedDatCho);
        }


        public ActionResult Delete(int ghe_id, int suat_chieu_id)
        {
            var datCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == ghe_id && dc.suat_chieu_id == suat_chieu_id);
            if (datCho == null)
            {
                return HttpNotFound();
            }
            return View(datCho);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int ghe_id, int suat_chieu_id)
        {
            var datCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == ghe_id && dc.suat_chieu_id == suat_chieu_id);
            if (datCho == null)
            {
                return HttpNotFound();
            }

            // Xóa khỏi cơ sở dữ liệu
            data.Dat_chos.DeleteOnSubmit(datCho);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }

    }
}
