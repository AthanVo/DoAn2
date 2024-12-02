using System;

namespace RapChieuPhim.Models
{
    public class PaymentInformationModel
    {
        public int GheId { get; set; } // ID của ghế
        public string SelectedSeats { get; set; } // Ghế đã chọn
        public decimal Amount { get; set; } // Tổng giá trị thanh toán
        public int FilmId { get; set; } // ID của phim
        public int Film { get; set; } // tên của phim
        public int SuatChieuId { get; set; } // ID suất chiếu
        public int DatVeId { get; set; } // Thêm thuộc tính này
        public DateTime NgayDat { get; set; } // Ngày đặt vé
        public string ReturnUrl { get; set; } // Đường dẫn trả về sau khi thanh toán

    }
}