using System;
using System.Collections.Generic;

namespace RapChieuPhim.Models
{
    public class ChiTietDatVeViewModel
    {
        public string TenPhim { get; set; }           // Tên phim
        public TimeSpan GioChieu { get; set; }        // Giờ chiếu
        public string RapChieuPhim { get; set; }      // Rạp chiếu phim
        public decimal GiaVe { get; set; }            // Giá vé
        public List<string> SoGhe { get; set; }       // Danh sách số ghế
        public List<Ghe_ngoi> Ghe_ngoi { get; set; } // Sử dụng List để chứa nhiều ghế

    }
}