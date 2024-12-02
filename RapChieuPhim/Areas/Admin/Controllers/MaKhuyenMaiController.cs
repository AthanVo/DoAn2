using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class MaKhuyenMaiController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // Hiển thị danh sách mã khuyến mãi
        public ActionResult Index()
        {
            var khuyenMaiList = data.Khuyen_mais.ToList(); // Lấy danh sách mã khuyến mãi
            return View(khuyenMaiList);
        }

        // Hiển thị chi tiết mã khuyến mãi
        public ActionResult Details(int id)
        {
            var khuyenMai = data.Khuyen_mais.FirstOrDefault(km => km.ma_khuyen_mai_id == id);
            if (khuyenMai == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(khuyenMai);
        }

        // Hiển thị form thêm mới mã khuyến mãi
        public ActionResult Create()
        {
            return View();
        }

        // Xử lý thêm mới mã khuyến mãi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Khuyen_mai khuyenMai, HttpPostedFileBase anh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (anh != null && anh.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(anh.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/Promotions/"), fileName);
                    anh.SaveAs(path);

                    // Lưu đường dẫn đầy đủ vào cơ sở dữ liệu
                    khuyenMai.anh = "/UploadedFiles/Promotions/" + fileName;
                }


                // Lưu thông tin mã khuyến mãi vào cơ sở dữ liệu
                data.Khuyen_mais.InsertOnSubmit(khuyenMai);
                data.SubmitChanges();

                return RedirectToAction("Index");
            }

            return View(khuyenMai);
        }


        // Hiển thị form chỉnh sửa mã khuyến mãi
        public ActionResult Edit(int id)
        {
            var khuyenMai = data.Khuyen_mais.FirstOrDefault(km => km.ma_khuyen_mai_id == id);
            if (khuyenMai == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(khuyenMai);
        }

        // Xử lý chỉnh sửa mã khuyến mãi
        // Xử lý chỉnh sửa mã khuyến mãi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Khuyen_mai khuyenMai, HttpPostedFileBase anh)
        {
            if (ModelState.IsValid)
            {
                // Tìm mã khuyến mãi trong cơ sở dữ liệu
                var existingKhuyenMai = data.Khuyen_mais.FirstOrDefault(km => km.ma_khuyen_mai_id == khuyenMai.ma_khuyen_mai_id);
                if (existingKhuyenMai != null)
                {
                    // Cập nhật thông tin
                    existingKhuyenMai.ten_khuyen_mai = khuyenMai.ten_khuyen_mai;
                    existingKhuyenMai.mo_ta = khuyenMai.mo_ta;
                    existingKhuyenMai.phan_tram_giam = khuyenMai.phan_tram_giam;
                    existingKhuyenMai.ngay_bat_dau = khuyenMai.ngay_bat_dau;
                    existingKhuyenMai.ngay_ket_thuc = khuyenMai.ngay_ket_thuc;
                    existingKhuyenMai.trang_thai = khuyenMai.trang_thai;
                    existingKhuyenMai.ma_giam_gia = khuyenMai.ma_giam_gia;

                    // Xử lý upload ảnh nếu có file ảnh mới
                    if (anh != null && anh.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(anh.FileName);
                        var path = System.IO.Path.Combine(Server.MapPath("~/UploadedFiles/Promotions/"), fileName);

                        // Xóa file cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(existingKhuyenMai.anh))
                        {
                            var oldPath = Server.MapPath(existingKhuyenMai.anh);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        anh.SaveAs(path);

                        // Cập nhật đường dẫn đầy đủ
                        existingKhuyenMai.anh = "/UploadedFiles/Promotions/" + fileName;
                    }


                    // Lưu thay đổi vào cơ sở dữ liệu
                    data.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(khuyenMai); // Trả lại View nếu có lỗi
        }


        // Xóa mã khuyến mãi
        public ActionResult Delete(int id)
        {
            var khuyenMai = data.Khuyen_mais.FirstOrDefault(km => km.ma_khuyen_mai_id == id);
            if (khuyenMai == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 404
            }
            return View(khuyenMai);
        }

        // Xử lý xóa mã khuyến mãi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var khuyenMai = data.Khuyen_mais.FirstOrDefault(km => km.ma_khuyen_mai_id == id);
            if (khuyenMai != null)
            {
                data.Khuyen_mais.DeleteOnSubmit(khuyenMai);
                data.SubmitChanges(); // Xóa khỏi cơ sở dữ liệu
            }
            return RedirectToAction("Index");
        }
    }
}
