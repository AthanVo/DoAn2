using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class SuatchieuController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/Suatchieu
        public ActionResult Index()
        {
            // Lấy danh sách suất chiếu
            var suatChieuList = data.Suat_chieus.ToList(); // Lấy toàn bộ dữ liệu từ bảng Suat_chieu
            return View(suatChieuList); // Truyền danh sách vào View
        }

        // Hiển thị form thêm mới suất chiếu
        public ActionResult Create()
        {
            // Lấy danh sách phim, rạp, màn hình để hiển thị trong form
            ViewBag.Phims = data.Phims.ToList();
            ViewBag.Raps = data.Rap_chieu_phims.ToList();
            ViewBag.ManHinhs = data.Man_hinh_chieus.ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Suat_chieu suatChieu)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lịch suất chiếu
                var isConflict = data.Suat_chieus.Any(s =>
                    s.man_hinh_id == suatChieu.man_hinh_id &&
                    s.ngay_chieu == suatChieu.ngay_chieu &&
                    ((suatChieu.gio_bat_dau >= s.gio_bat_dau && suatChieu.gio_bat_dau < s.gio_ket_thuc) ||
                     (suatChieu.gio_ket_thuc > s.gio_bat_dau && suatChieu.gio_ket_thuc <= s.gio_ket_thuc))
                );

                if (isConflict)
                {
                    ModelState.AddModelError("", "Suất chiếu bị trùng thời gian với suất chiếu khác trong cùng màn hình!");
                    ViewBag.Phims = data.Phims.ToList();
                    ViewBag.Raps = data.Rap_chieu_phims.ToList();
                    ViewBag.ManHinhs = data.Man_hinh_chieus.ToList();
                    return View(suatChieu);
                }

                // Thêm suất chiếu mới
                data.Suat_chieus.InsertOnSubmit(suatChieu);
                data.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Gọi hàm tạo ghế mặc định cho suất chiếu
                TaoGheMacDinh(suatChieu.suat_chieu_id, suatChieu.rap_id.Value);

                return RedirectToAction("Index");
            }

            ViewBag.Phims = data.Phims.ToList();
            ViewBag.Raps = data.Rap_chieu_phims.ToList();
            ViewBag.ManHinhs = data.Man_hinh_chieus.ToList();
            return View(suatChieu);
        }



        // Hiển thị form chỉnh sửa suất chiếu
        public ActionResult Edit(int id)
        {
            var suatChieu = data.Suat_chieus.FirstOrDefault(s => s.suat_chieu_id == id);
            if (suatChieu == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách phim để hiển thị trong dropdown
            ViewBag.Phims = data.Phims.ToList();

            return View(suatChieu);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suat_chieu suatChieu)
        {
            if (ModelState.IsValid)
            {
                // Tìm suất chiếu trong cơ sở dữ liệu
                var existingSuatChieu = data.Suat_chieus.FirstOrDefault(s => s.suat_chieu_id == suatChieu.suat_chieu_id);
                if (existingSuatChieu != null)
                {
                    // Cập nhật các thông tin suất chiếu
                    existingSuatChieu.phim_id = suatChieu.phim_id; // Gán phim mới cho suất chiếu
                    existingSuatChieu.man_hinh_id = suatChieu.man_hinh_id;
                    existingSuatChieu.ngay_chieu = suatChieu.ngay_chieu;
                    existingSuatChieu.gio_bat_dau = suatChieu.gio_bat_dau;
                    existingSuatChieu.gio_ket_thuc = suatChieu.gio_ket_thuc;
                    existingSuatChieu.rap_id = suatChieu.rap_id;
                    existingSuatChieu.tong_so_ghe_con_lai = suatChieu.tong_so_ghe_con_lai;

                    data.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return RedirectToAction("Index");
                }
            }
            return View(suatChieu);
        }

        
        
        // Hiển thị chi tiết suất chiếu
        public ActionResult Details(int id)
        {
            // Tìm suất chiếu theo ID
            var suatChieu = data.Suat_chieus.FirstOrDefault(s => s.suat_chieu_id == id);
            if (suatChieu == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy suất chiếu, trả về lỗi 404
            }
            return View(suatChieu); // Trả về View với dữ liệu suất chiếu
        }

        public void TaoGheMacDinh(int suatChieuId, int rapId)
        {
            // Lấy danh sách ghế của rạp
            var gheNgois = data.Ghe_ngois.Where(g => g.rap_id == rapId).ToList();

            // Nếu ghế chưa tồn tại, thêm ghế vào Ghe_ngoi
            if (!gheNgois.Any())
            {
                char row = 'A';
                int soHang = 5; // Ví dụ: 5 hàng ghế
                int soGheMoiHang = 8; // Ví dụ: mỗi hàng có 8 ghế

                for (int hang = 1; hang <= soHang; hang++)
                {
                    for (int ghe = 1; ghe <= soGheMoiHang; ghe++)
                    {
                        var gheMoi = new Ghe_ngoi
                        {
                            man_hinh_id = 1, // Giả sử màn hình 1
                            so_ghe = $"{row}{ghe}",
                            loai_ghe_id = (hang <= 2) ? 2 : 1, // 2 hàng đầu là VIP
                            rap_id = rapId
                        };
                        data.Ghe_ngois.InsertOnSubmit(gheMoi);
                    }
                    row++;
                }
                data.SubmitChanges();
            }

            // Thêm ghế vào Dat_cho cho suất chiếu
            var gheIds = data.Ghe_ngois.Where(g => g.rap_id == rapId).Select(g => g.ghe_id).ToList();
            foreach (var gheId in gheIds)
            {
                var datCho = new Dat_cho
                {
                    ghe_id = gheId,
                    suat_chieu_id = suatChieuId,
                    da_dat = false // Trạng thái mặc định
                };
                data.Dat_chos.InsertOnSubmit(datCho);
            }
            data.SubmitChanges();
        }

        public ActionResult Delete(int id)
        {
            // Lấy suất chiếu theo ID
            var suatChieu = data.Suat_chieus.FirstOrDefault(s => s.suat_chieu_id == id);
            if (suatChieu == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy suất chiếu, trả về lỗi 404
            }
            return View(suatChieu); // Trả về View với dữ liệu suất chiếu
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            // Lấy suất chiếu cần xóa
            var suatChieu = data.Suat_chieus.FirstOrDefault(s => s.suat_chieu_id == id);
            if (suatChieu == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy suất chiếu, trả về lỗi 404
            }

            // Xóa các ghế liên quan trong bảng Dat_cho
            var datChos = data.Dat_chos.Where(dc => dc.suat_chieu_id == id).ToList();
            if (datChos.Any())
            {
                data.Dat_chos.DeleteAllOnSubmit(datChos);
            }

            // Xóa suất chiếu
            data.Suat_chieus.DeleteOnSubmit(suatChieu);

            // Lưu thay đổi vào cơ sở dữ liệu
            data.SubmitChanges();

            return RedirectToAction("Index");
        }


    }
}
