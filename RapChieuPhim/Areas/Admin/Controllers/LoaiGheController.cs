using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class LoaiGheController : Controller
    {
        // GET: Admin/LoaiGhe
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
        public ActionResult Index()
        {
            var loaiGheList = data.Loai_ghes.ToList();
            return View(loaiGheList);
        }


        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Loai_ghe loaiGhe)
        {
            if (ModelState.IsValid)
            {
                // Thêm loại ghế vào cơ sở dữ liệu
                data.Loai_ghes.InsertOnSubmit(loaiGhe);
                data.SubmitChanges();
                return RedirectToAction("Index"); // Chuyển về danh sách loại ghế
            }
            return View(loaiGhe); // Quay lại form nếu có lỗi
        }

        public ActionResult Edit(int id)
        {
            // Lấy loại ghế cần sửa
            var loaiGhe = data.Loai_ghes.FirstOrDefault(lg => lg.loai_ghe_id == id);
            if (loaiGhe == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(loaiGhe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Loai_ghe loaiGhe)
        {
            if (ModelState.IsValid)
            {
                // Tìm loại ghế trong cơ sở dữ liệu
                var existingLoaiGhe = data.Loai_ghes.FirstOrDefault(lg => lg.loai_ghe_id == loaiGhe.loai_ghe_id);
                if (existingLoaiGhe != null)
                {
                    // Cập nhật thông tin loại ghế
                    existingLoaiGhe.ten_loai_ghe = loaiGhe.ten_loai_ghe;
                    existingLoaiGhe.gia_ve = loaiGhe.gia_ve;

                    data.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return RedirectToAction("Index");
                }
            }
            return View(loaiGhe); // Quay lại form nếu có lỗi
        }

        public ActionResult Delete(int id)
        {
            // Lấy loại ghế cần xóa
            var loaiGhe = data.Loai_ghes.FirstOrDefault(lg => lg.loai_ghe_id == id);
            if (loaiGhe == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(loaiGhe);
        }

        public ActionResult Details(int id)
        {
            // Lấy loại ghế cần hiển thị
            var loaiGhe = data.Loai_ghes.FirstOrDefault(lg => lg.loai_ghe_id == id);
            if (loaiGhe == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(loaiGhe);
        }

    }

}