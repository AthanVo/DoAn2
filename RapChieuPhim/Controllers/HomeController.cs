using RapChieuPhim.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
namespace RapChieuPhim.Controllers
{
    public class HomeController : Controller
    {

        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
        [ActionName("Home")]
        public ActionResult Index()
        {
            var phimList = data.Phims.ToList(); // Lấy danh sách sách từ cơ sở dữ liệu
            return View(phimList); // Trả về view với mô hình là danh sách các sách
        }

        private List<Phim> Layphimmoi(int count)
        {
            return data.Phims.OrderByDescending(a => a.ngay_khoi_chieu).Take(count).ToList();
        }

        public ActionResult Index(int? page)
        {
            int pageSize = 16;
            int pageNum = (page ?? 1);

            var phimmoi = Layphimmoi(15);
            return View(phimmoi.ToPagedList(pageNum, pageSize));
        }


        public ActionResult Search(string query, int? page)
        {
            // Đảm bảo chuỗi tìm kiếm không phân biệt độ dài, tìm kiếm cả khi có ít ký tự
            var searchResults = data.Phims
                                    .Where(p => p.ten_phim.Contains(query) || p.ten_phim.StartsWith(query) || p.ten_phim.EndsWith(query)) // Sử dụng thêm StartsWith và EndsWith để mở rộng phạm vi tìm kiếm
                                    .ToList();

            // Phân trang kết quả tìm kiếm
            int pageSize = 10; // Số phần tử trên mỗi trang
            int pageNumber = (page ?? 1);
            var pagedList = searchResults.ToPagedList(pageNumber, pageSize);

            // Trả về view tìm kiếm với kết quả phân trang
            return View("SearchResult", pagedList);
        }

    }
}