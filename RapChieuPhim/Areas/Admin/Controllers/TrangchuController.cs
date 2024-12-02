using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RapChieuPhim.Areas.Admin.Views.Controllers
{
    public class TrangchuController : Controller
    {
        // GET: Admin/Trangchu
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
        public ActionResult Index()
        {
            // Tính toán tổng doanh thu trong năm (Ví dụ)
            decimal tongDoanhThu = data.Dat_ves
                                    .Where(hd => hd.thoi_gian_dat.HasValue && hd.thoi_gian_dat.Value.Year == DateTime.Now.Year) // Kiểm tra null và lọc theo năm
                                    .Sum(hd => hd.tong_tien ?? 0); // Tính tổng và xử lý null

            ViewBag.TongDoanhThu = tongDoanhThu;


            // Đếm số lượng phòng chiếu
            var soLuongChiTietDatVe = data.Chi_tiet_dat_ves.Count();

            // Đếm số lượng vé đặt
            var soLuongDatVe = data.Dat_ves.Count();

            // Đếm số lượng phòng chiếu
            var soLuongPhongChieu = data.Rap_chieu_phims.Count();

            // Đếm số lượng phim
            var soLuongPhim = data.Phims.Count();

            // Đếm số suất chiếu
            var soLuongSuatChieu = data.Suat_chieus.Count();

            // Đếm số suất chiếu
            var soLuongMaKhuyenMai = data.Khuyen_mais.Count();

            // Đếm số lượng ghế
            var soLuongGhe = data.Ghe_ngois.Count();

            // Đếm số lượng loại ghế ngồi
            var soLuongLoaiGhe = data.Loai_ghes.Count();

            // Đếm số lượng khách hàng
            var soLuongKhachHang = data.Nguoi_dungs.Where(u => u.role == "User").Count();

            // Truyền dữ liệu đến View thông qua ViewBag hoặc Model
            ViewBag.SoLuongPhongChieu = soLuongPhongChieu;
            ViewBag.SoLuongPhim = soLuongPhim;
            ViewBag.SoLuongKhachHang = soLuongKhachHang;
            ViewBag.SoLuongSuatChieu = soLuongSuatChieu;
            ViewBag.SoLuongGheNgoi = soLuongGhe;
            ViewBag.SoLuongMaKhuyenMai = soLuongMaKhuyenMai;
            ViewBag.SoLuongLoaiGhe = soLuongLoaiGhe;
            ViewBag.SoLuongDatVe = soLuongDatVe;
            ViewBag.SoLuongChiTietDatVe = soLuongChiTietDatVe;

            return View();
        }
        public ActionResult Trangchu()
        {
            return View();
        }

        public ActionResult Search(string query)
        {
            var results = new List<SearchResult>();

            // Tìm kiếm trong bảng MaKhuyenMai
            results.AddRange(data.Khuyen_mais
                .Where(p => p.ten_khuyen_mai != null && p.ten_khuyen_mai.ToLower().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.ma_khuyen_mai_id,
                    Name = p.ten_khuyen_mai,
                    Category = "MaKhuyenMai"
                }));


            // Tìm kiếm trong bảng LoaiGhe
            results.AddRange(data.Loai_ghes
                .Where(p => p.ten_loai_ghe != null && p.ten_loai_ghe.ToLower().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.loai_ghe_id,
                    Name = p.ten_loai_ghe,
                    Category = "LoaiGhe"
                }));

            // Tìm kiếm trong bảng GheNgoi
            results.AddRange(data.Ghe_ngois
                .Where(p => p.so_ghe != null && p.so_ghe.ToString().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.ghe_id,
                    Name = p.so_ghe.ToString(),
                    Category = "GheNgoi"
                }));

            // Tìm kiếm trong bảng DatVe
            results.AddRange(data.Dat_ves
                .Where(p => p.thoi_gian_dat != null && p.thoi_gian_dat.ToString().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.dat_ve_id,
                    Name = p.thoi_gian_dat.ToString(),
                    Category = "DatVe"
                }));



            // Tìm kiếm trong bảng ChiTietDatVe
            results.AddRange(data.Chi_tiet_dat_ves
                .Where(p => p.dat_ve_id != null && p.dat_ve_id.ToString().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.chi_tiet_dat_ve_id,
                    Name = p.dat_ve_id.ToString(),
                    Category = "ChiTietDatVe"
                }));

            // Tìm kiếm trong bảng Suất chiếu
            results.AddRange(data.Suat_chieus
                .Where(p => p.ngay_chieu != null && p.ngay_chieu.ToString().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.suat_chieu_id,
                    Name = p.ngay_chieu.ToString(),
                    Category = "Suatchieu"
                }));

            // Tìm kiếm trong bảng Phim
            results.AddRange(data.Phims
                .Where(p => p.ten_phim != null && p.ten_phim.ToLower().Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.phim_id,
                    Name = p.ten_phim,
                    Category = "Phim"
                }));

            // Tìm kiếm trong bảng Người dùng
            results.AddRange(data.Nguoi_dungs
                .Where(kh => kh.ho_ten != null && kh.ho_ten.ToLower().Contains(query))
                .Select(kh => new SearchResult
                {
                    Id = kh.nguoi_dung_id,
                    Name = kh.ho_ten,
                    Category = "KhachHang"
                }));

            // Tìm kiếm trong bảng Rạp chiếu phim
            results.AddRange(data.Rap_chieu_phims
                .Where(r => r.ten_rap != null && r.ten_rap.ToLower().Contains(query))
                .Select(r => new SearchResult
                {
                    Id = r.rap_id,
                    Name = r.ten_rap,
                    Category = "SoLuongPhongChieu"
                }));

            // Nếu không có kết quả
            if (!results.Any())
            {
                ViewBag.Message = "Không tìm thấy kết quả nào phù hợp.";
            }

            return View(results.OrderBy(r => r.Name).ToList());
        }

    }
}