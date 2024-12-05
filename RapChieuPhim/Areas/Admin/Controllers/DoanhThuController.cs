using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Controllers
{
    public class DoanhThuController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Admin/DoanhThu
        public ActionResult Index()
        {
            // Tổng doanh thu trong năm hiện tại
            decimal tongDoanhThu = data.Dat_ves
                .Where(hd => hd.thoi_gian_dat.HasValue && hd.thoi_gian_dat.Value.Year == DateTime.Now.Year)
                .Sum(hd => hd.tong_tien ?? 0);

            // Doanh thu theo tháng
            var doanhThuTheoThang = Enumerable.Range(1, 12)
                .Select(thang => new
                {
                    Thang = thang,
                    DoanhThu = data.Dat_ves
                                .Where(hd => hd.thoi_gian_dat.HasValue
                                             && hd.thoi_gian_dat.Value.Year == DateTime.Now.Year
                                             && hd.thoi_gian_dat.Value.Month == thang)
                                .ToList() // Lấy toàn bộ danh sách vé
                })
                .Select(d => new
                {
                    d.Thang,
                    DoanhThu = d.DoanhThu.Sum(hd => hd.tong_tien ?? 0) // Tính tổng sau khi xử lý null
                })
                .ToList();
            ViewBag.DoanhThuTheoThang = doanhThuTheoThang; // Truyền vào ViewBag

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

        // Action mới để xem doanh thu theo ngày của một tháng
        public ActionResult DoanhThuNgay(int? thang)
        {
            if (!thang.HasValue)
            {
                thang = DateTime.Now.Month;
            }

            int currentYear = DateTime.Now.Year;

            var datVes = data.Dat_ves
                .Where(hd => hd.thoi_gian_dat.HasValue
                             && hd.thoi_gian_dat.Value.Year == currentYear
                             && hd.thoi_gian_dat.Value.Month == thang)
                .ToList();

            var doanhThuTheoNgay = datVes
                .GroupBy(hd => hd.thoi_gian_dat.Value.Day)
                .Select(group => new
                {
                    Ngay = group.Key,
                    DoanhThu = group.Sum(hd => hd.tong_tien ?? 0)
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.ThangHienTai = thang;
            ViewBag.DoanhThuTheoNgay = doanhThuTheoNgay.Select(d => d.DoanhThu).ToList();
            ViewBag.Ngay = doanhThuTheoNgay.Select(d => d.Ngay).ToList();

            // Truyền doanh thu theo tháng vào ViewBag (Dữ liệu cho biểu đồ các tháng)
            var doanhThuTheoThang = Enumerable.Range(1, 12)
                 .Select(thang => new
                 {
                     Thang = thang,
                     DoanhThu = data.Dat_ves
                                 .Where(hd => hd.thoi_gian_dat.HasValue
                                              && hd.thoi_gian_dat.Value.Year == DateTime.Now.Year
                                              && hd.thoi_gian_dat.Value.Month == thang)
                                 .ToList() // Lấy toàn bộ danh sách vé
                 })
                 .Select(d => new
                 {
                     d.Thang,
                     DoanhThu = d.DoanhThu.Sum(hd => hd.tong_tien ?? 0) // Tính tổng sau khi xử lý null
                 })
                 .ToList();

            ViewBag.DoanhThuTheoThang = doanhThuTheoThang.Select(dt => dt.DoanhThu).ToList();

            return View();
        }

    }


}