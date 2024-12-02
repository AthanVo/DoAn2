using RapChieuPhim.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;  // Đảm bảo import đúng thư viện ASP.NET Web API

[RoutePrefix("api/BookingApi")]
public class BookingApiController : ApiController

{
    private readonly DataDataContext _context;

    // Khởi tạo controller và khởi tạo DataContext từ chuỗi kết nối
    public BookingApiController()
    {
        _context = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
    }

    // API lấy danh sách ghế theo suất chiếu
    [HttpGet]
    [Route("GetRemainingSeats/{suatChieuId}")]
    public IHttpActionResult GetRemainingSeats(int suatChieuId)
    {
        var suatChieu = _context.Suat_chieus.FirstOrDefault(sc => sc.suat_chieu_id == suatChieuId);

        if (suatChieu == null)
        {
            return NotFound();
        }

        var remainingSeats = suatChieu.tong_so_ghe_con_lai;
        return Ok(new { remainingSeats });
    }



    // API đặt ghế
    [HttpPost]
    [Route("BookSeats")]
    public IHttpActionResult BookSeats([FromBody] BookingRequest request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            return BadRequest("Không có ghế nào được chọn.");
        }

        var seats = _context.Dat_chos
            .Where(d => request.SeatIds.Contains(d.ghe_id) && d.suat_chieu_id == request.SuatChieuId)
            .ToList();

        if (seats.Any(s => s.da_dat.HasValue && s.da_dat.Value))
        {
            return BadRequest("Một hoặc nhiều ghế đã được đặt.");
        }

        foreach (var seat in seats)
        {
            seat.da_dat = true;  // Đánh dấu ghế đã đặt
        }

        var suatChieu = _context.Suat_chieus.FirstOrDefault(sc => sc.suat_chieu_id == request.SuatChieuId);
        if (suatChieu != null)
        {
            suatChieu.tong_so_ghe_con_lai -= seats.Count;
        }

        _context.SubmitChanges();


        return Ok("Đặt vé thành công.");
    }
}



// Lớp để nhận dữ liệu đặt ghế từ client
public class BookingRequest
{
    public int SuatChieuId { get; set; }
    public List<int> SeatIds { get; set; }
}
