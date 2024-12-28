using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class PhimController : Controller
    {
        // Kết nối với cơ sở dữ liệu
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/Phim
        public ActionResult Index()
        {
            var phimlist = data.Phims.ToList(); // Lấy danh sách phim
            return View(phimlist); // Truyền danh sách vào View
        }

        // Chi tiết phim
        public ActionResult Details(int id)
        {
            var phim = data.Phims.FirstOrDefault(p => p.phim_id == id);
            if (phim == null)
            {
                return HttpNotFound();
            }
            return View(phim); // Truyền đối tượng phim vào View
        }

        // Thêm phim (GET)
        public ActionResult Create()
        {
            var model = new Phim(); // Khởi tạo đối tượng Model
            return View(model); // Truyền Model vào View
        }

        // Thêm phim (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Phim phim, HttpPostedFileBase anh, HttpPostedFileBase video, HttpPostedFileBase anh_video)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload file ảnh
                if (anh != null && anh.ContentLength > 0)
                {
                    var anhFileName = System.IO.Path.GetFileName(anh.FileName);
                    var anhPath = System.IO.Path.Combine(Server.MapPath("/UploadedFiles/Images/"), anhFileName);
                    anh.SaveAs(anhPath);
                    phim.anh = "/UploadedFiles/Images/" + anhFileName; // Lưu đường dẫn ảnh
                }
                // Xử lý upload file video
                if (video != null && video.ContentLength > 0)
                {
                    var videoFileName = System.IO.Path.GetFileName(video.FileName);
                    var videoPath = System.IO.Path.Combine(Server.MapPath("/UploadedFiles/Videos/"), videoFileName);
                    video.SaveAs(videoPath);
                    phim.video = "/UploadedFiles/Videos/" + videoFileName; // Lưu đường dẫn video
                }
                // Xử lý upload file ảnh video
                if (anh_video != null && anh_video.ContentLength > 0)
                {
                    var anhVideoFileName = System.IO.Path.GetFileName(anh_video.FileName);
                    var anhVideoPath = System.IO.Path.Combine(Server.MapPath("/UploadedFiles/ImageVideos/"), anhVideoFileName);
                    anh_video.SaveAs(anhVideoPath);
                    phim.anh_video = "/UploadedFiles/ImageVideos/" + anhVideoFileName; // Lưu đường dẫn ảnh video
                }
                // Lưu thông tin phim vào database
                data.Phims.InsertOnSubmit(phim);
                data.SubmitChanges();

                return RedirectToAction("Index");
            }

            return View(phim);
        }

        // Sửa phim (GET)
        public ActionResult Edit(int id)
        {
            var phim = data.Phims.FirstOrDefault(p => p.phim_id == id);
            if (phim == null)
            {
                return HttpNotFound();
            }
            return View(phim); // Truyền đối tượng phim vào View sửa
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Phim phim, HttpPostedFileBase anh, HttpPostedFileBase video, HttpPostedFileBase anh_video)
        {
            if (ModelState.IsValid)
            {
                // Tìm phim trong cơ sở dữ liệu
                var existingPhim = data.Phims.FirstOrDefault(p => p.phim_id == phim.phim_id);
                if (existingPhim != null)
                {
                    // Cập nhật thông tin chung của phim
                    existingPhim.ten_phim = phim.ten_phim;
                    existingPhim.the_loai = phim.the_loai;
                    existingPhim.thoi_luong = phim.thoi_luong;
                    existingPhim.mo_ta = phim.mo_ta;
                    existingPhim.ngay_khoi_chieu = phim.ngay_khoi_chieu;
                    existingPhim.diem_danh_gia = phim.diem_danh_gia;
                    existingPhim.dao_dien = phim.dao_dien;
                    existingPhim.dien_vien_id = phim.dien_vien_id;
                    existingPhim.quoc_gia_id = phim.quoc_gia_id;
                    existingPhim.nha_san_xuat = phim.nha_san_xuat;

                    // Xử lý upload file ảnh
                    if (anh != null && anh.ContentLength > 0)
                    {
                        var fileName = System.IO.Path.GetFileName(anh.FileName);
                        var path = System.IO.Path.Combine(Server.MapPath("/UploadedFiles/Images/"), fileName);

                        // Xóa file ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(existingPhim.anh))
                        {
                            var oldPath = Server.MapPath(existingPhim.anh);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        anh.SaveAs(path);
                        existingPhim.anh = "/UploadedFiles/Images/" + fileName; // Lưu đường dẫn ảnh mới
                    }

                    // Xử lý upload file video
                    if (video != null && video.ContentLength > 0)
                    {
                        var videoFileName = System.IO.Path.GetFileName(video.FileName);
                        var videoPath = System.IO.Path.Combine(Server.MapPath("/UploadedFiles/Videos/"), videoFileName);

                        // Xóa file video cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(existingPhim.video))
                        {
                            var oldVideoPath = Server.MapPath(existingPhim.video);
                            if (System.IO.File.Exists(oldVideoPath))
                            {
                                System.IO.File.Delete(oldVideoPath);
                            }
                        }

                        video.SaveAs(videoPath);
                        existingPhim.video = "/UploadedFiles/Videos/" + videoFileName; // Lưu đường dẫn video mới
                    }

                    // Xử lý upload file ảnh video
                    if (anh_video != null && anh_video.ContentLength > 0)
                    {
                        var anhVideoFileName = System.IO.Path.GetFileName(anh_video.FileName);
                        var anhVideoPath = System.IO.Path.Combine(Server.MapPath("/UploadedFiles/ImageVideos/"), anhVideoFileName);

                        // Xóa file ảnh video cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(existingPhim.anh_video))
                        {
                            var oldAnhVideoPath = Server.MapPath(existingPhim.anh_video);
                            if (System.IO.File.Exists(oldAnhVideoPath))
                            {
                                System.IO.File.Delete(oldAnhVideoPath);
                            }
                        }

                        anh_video.SaveAs(anhVideoPath);
                        existingPhim.anh_video = "/UploadedFiles/ImageVideos/" + anhVideoFileName; // Lưu đường dẫn ảnh video mới
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    data.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(phim); // Trả lại View nếu có lỗi
        }

        // Xóa phim (GET)
        public ActionResult Delete(int id)
        {
            var phim = data.Phims.FirstOrDefault(p => p.phim_id == id);
            if (phim == null)
            {
                return HttpNotFound();
            }
            return View(phim); // Truyền đối tượng phim vào View xác nhận xóa
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var phim = data.Phims.FirstOrDefault(p => p.phim_id == id);
            if (phim == null)
            {
                return HttpNotFound(); // Nếu phim không tồn tại, trả về 404
            }

            try
            {
                // Xóa phim khỏi cơ sở dữ liệu
                data.Phims.DeleteOnSubmit(phim);
                data.SubmitChanges();
                ViewBag.Success = "Xóa phim thành công!";
                return RedirectToAction("Index"); // Chuyển về trang danh sách sau khi xóa thành công
            }
            catch (Exception ex)
            {
                // Bắt lỗi, ví dụ: Lỗi ràng buộc khóa ngoại
                ViewBag.Error = "Không thể xóa phim vì có dữ liệu liên quan. Hãy xóa các dữ liệu liên quan trước.";
                // Ghi log lỗi nếu cần
                System.IO.File.AppendAllText(Server.MapPath("~/App_Data/ErrorLog.txt"), ex.ToString());
                return View(phim); // Trả lại View "Delete" cùng với đối tượng phim và thông báo lỗi
            }
        }

    }

}

