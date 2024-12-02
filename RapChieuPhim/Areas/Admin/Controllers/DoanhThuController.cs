using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class DoanhThuController : Controller
    {
        // GET: Admin/DoanhThu
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
        public ActionResult Index()
        {
            // Tổng doanh thu trong năm hiện tại
            decimal tongDoanhThu = data.Dat_ves
                .Where(hd => hd.thoi_gian_dat.HasValue && hd.thoi_gian_dat.Value.Year == DateTime.Now.Year)
                .Sum(hd => hd.tong_tien ?? 0);

            // Doanh thu theo tháng
            var doanhThuTheoThang = Enumerable.Range(1, 12)
    .Select(thang => new {
        Thang = thang,
        DoanhThu = data.Dat_ves
                    .Where(hd => hd.thoi_gian_dat.HasValue
                                 && hd.thoi_gian_dat.Value.Year == DateTime.Now.Year
                                 && hd.thoi_gian_dat.Value.Month == thang)
                    .ToList() // Lấy toàn bộ danh sách vé
    })
    .Select(d => new {
        d.Thang,
        DoanhThu = d.DoanhThu.Sum(hd => hd.tong_tien ?? 0) // Tính tổng sau khi xử lý null
    })
    .ToList();


            // Số lượng các đối tượng liên quan
            int soLuongPhim = data.Phims.Count();
            int soLuongPhongChieu = data.Rap_chieu_phims.Count();
            int soLuongKhachHang = data.Nguoi_dungs.Count(u => u.role == "User");
            int soLuongDatVe = data.Dat_ves.Count();

            // Truyền dữ liệu sang View
            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.DoanhThuTheoThang = doanhThuTheoThang.Select(dt => dt.DoanhThu).ToList();
            ViewBag.SoLuongPhim = soLuongPhim;
            ViewBag.SoLuongPhongChieu = soLuongPhongChieu;
            ViewBag.SoLuongKhachHang = soLuongKhachHang;
            ViewBag.SoLuongDatVe = soLuongDatVe;

            return View();
        }

    }
}