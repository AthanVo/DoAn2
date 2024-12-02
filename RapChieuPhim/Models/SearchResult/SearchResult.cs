
namespace RapChieuPhim.Models

{
    public class SearchResult
    {
        public int Id { get; set; } // Mặc định là -1 nếu không có giá trị
        public string Name { get; set; } // Mặc định là "Không có" nếu không có giá trị
        public string Category { get; set; } // Loại bảng, ví dụ: "Phim", "Người dùng"
    }

}
