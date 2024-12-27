using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class DatVeController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/DatVe
        public ActionResult Index()
        {
            var datVeList = data.Dat_ves.ToList();
            return View(datVeList);
        }

        // GET: Admin/DatVe/Details/5
        public ActionResult Details(int id)
        {
            var datVe = data.Dat_ves.SingleOrDefault(d => d.dat_ve_id == id);
            if (datVe == null)
            {
                return HttpNotFound();
            }
            return View(datVe);
        }

        // GET: Admin/DatVe/Edit/5
        public ActionResult Edit(int id)
        {
            var datVe = data.Dat_ves.SingleOrDefault(d => d.dat_ve_id == id);
            if (datVe == null)
            {
                return HttpNotFound();
            }
            return View(datVe);
        }

        // POST: Admin/DatVe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Dat_ve datVe)
        {
            if (ModelState.IsValid)
            {
                var existingDatVe = data.Dat_ves.SingleOrDefault(d => d.dat_ve_id == datVe.dat_ve_id);
                if (existingDatVe != null)
                {
                    existingDatVe.nguoi_dung_id = datVe.nguoi_dung_id;
                    existingDatVe.suat_chieu_id = datVe.suat_chieu_id;
                    existingDatVe.thoi_gian_dat = datVe.thoi_gian_dat;
                    existingDatVe.tong_tien = datVe.tong_tien;
                    // Update other fields as needed
                    data.SubmitChanges();
                }
                return RedirectToAction("Index");
            }
            return View(datVe);
        }

        // GET: Admin/DatVe/Delete/5
        public ActionResult Delete(int id)
        {
            var datVe = data.Dat_ves.SingleOrDefault(d => d.dat_ve_id == id);
            if (datVe == null)
            {
                return HttpNotFound();
            }
            return View(datVe);
        }

        // POST: Admin/DatVe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var datVe = data.Dat_ves.SingleOrDefault(d => d.dat_ve_id == id);
            if (datVe == null)
            {
                ViewBag.Error = "Không tìm thấy thông tin đặt vé!";
                return View("Delete", datVe); // Giữ nguyên trang "Delete"
            }

            try
            {
                // Xóa bản ghi đặt vé
                data.Dat_ves.DeleteOnSubmit(datVe);

                // Lưu thay đổi vào cơ sở dữ liệu
                data.SubmitChanges();

                // Thông báo thành công
                ViewBag.Success = "Xóa đặt vé thành công!";
            }
            catch (Exception ex)
            {
                // Log lỗi (nếu cần)
                System.IO.File.AppendAllText(Server.MapPath("~/App_Data/ErrorLog.txt"), ex.ToString());

                // Thông báo lỗi
                ViewBag.Error = "Bạn phải xóa các ràng buộc trước rồi mới được xóa đặt vé. Vui lòng thử lại.";
            }

            // Trả về trang "Delete" với thông báo lỗi hoặc thành công
            return View("Delete", datVe);
        }
    }
}
