using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class SoLuongPhongChieuController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // Hiển thị danh sách rạp chiếu
        public ActionResult Index()
        {
            var rapList = data.Rap_chieu_phims.ToList(); // Lấy danh sách rạp chiếu phim
            return View(rapList);
        }

        // Hiển thị chi tiết rạp chiếu
        public ActionResult Details(int id)
        {
            var rap = data.Rap_chieu_phims.FirstOrDefault(r => r.rap_id == id);
            if (rap == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(rap);
        }

        // Hiển thị form thêm mới rạp chiếu
        public ActionResult Create()
        {
            return View();
        }

        // Xử lý thêm mới rạp chiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rap_chieu_phim rap)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin rạp chiếu vào cơ sở dữ liệu
                data.Rap_chieu_phims.InsertOnSubmit(rap);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }

            return View(rap);
        }

        // Hiển thị form chỉnh sửa rạp chiếu
        public ActionResult Edit(int id)
        {
            var rap = data.Rap_chieu_phims.FirstOrDefault(r => r.rap_id == id);
            if (rap == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(rap);
        }

        // Xử lý chỉnh sửa rạp chiếu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rap_chieu_phim rap)
        {
            if (ModelState.IsValid)
            {
                var existingRap = data.Rap_chieu_phims.FirstOrDefault(r => r.rap_id == rap.rap_id);
                if (existingRap != null)
                {
                    // Cập nhật thông tin
                    existingRap.ten_rap = rap.ten_rap;
                    existingRap.dia_chi = rap.dia_chi;
                    existingRap.tong_so_ghe = rap.tong_so_ghe;

                    data.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return RedirectToAction("Index");
                }
            }
            return View(rap);
        }

        // Hiển thị form xóa rạp chiếu
        public ActionResult Delete(int id)
        {
            var rap = data.Rap_chieu_phims.FirstOrDefault(r => r.rap_id == id);
            if (rap == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(rap);
        }

        // Xử lý xóa rạp chiếu
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var rap = data.Rap_chieu_phims.FirstOrDefault(r => r.rap_id == id);
            if (rap != null)
            {
                data.Rap_chieu_phims.DeleteOnSubmit(rap);
                data.SubmitChanges(); // Xóa khỏi cơ sở dữ liệu
            }
            return RedirectToAction("Index");
        }
    }
}
