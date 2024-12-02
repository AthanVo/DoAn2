using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        // Kết nối đến database
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/KhachHang
        public ActionResult Index()
        {
            var khachHangList = data.Nguoi_dungs.ToList();
            return View(khachHangList);
        }

        // GET: Admin/KhachHang/Edit/5
        public ActionResult Edit(int id)
        {
            var khachHang = data.Nguoi_dungs.FirstOrDefault(k => k.nguoi_dung_id == id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách các vai trò
            var roles = new List<SelectListItem>
    {
        new SelectListItem { Text = "Admin", Value = "Admin" },
        new SelectListItem { Text = "User", Value = "User" },
    };

            // Truyền danh sách vai trò vào ViewBag
            ViewBag.RoleList = roles;

            return View(khachHang);
        }


        // POST: Admin/KhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Nguoi_dung updatedKhachHang)
        {
            try
            {
                var khachHang = data.Nguoi_dungs.FirstOrDefault(k => k.nguoi_dung_id == id);
                if (khachHang == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật dữ liệu
                khachHang.ten_dang_nhap = updatedKhachHang.ten_dang_nhap;
                khachHang.email = updatedKhachHang.email;
                khachHang.so_dien_thoai = updatedKhachHang.so_dien_thoai;
                khachHang.role = updatedKhachHang.role; // Cập nhật quyền nếu có thay đổi

                // Lưu thay đổi
                data.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        // GET: Admin/KhachHang/Details/5
        public ActionResult Details(int id)
        {
            // Tìm người dùng theo ID
            var khachHang = data.Nguoi_dungs.FirstOrDefault(k => k.nguoi_dung_id == id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }

            return View(khachHang); // Trả về View hiển thị thông tin chi tiết
        }


        // GET: Admin/KhachHang/Delete/5
        public ActionResult Delete(int id)
        {
            var khachHang = data.Nguoi_dungs.FirstOrDefault(k => k.nguoi_dung_id == id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                var khachHang = data.Nguoi_dungs.FirstOrDefault(k => k.nguoi_dung_id == id);
                if (khachHang == null)
                {
                    return HttpNotFound();
                }

                // Xóa khách hàng
                data.Nguoi_dungs.DeleteOnSubmit(khachHang);

                // Lưu thay đổi
                data.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }


        }
    }
}
