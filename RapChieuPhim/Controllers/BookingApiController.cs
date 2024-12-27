using RapChieuPhim.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

[RoutePrefix("api/BookingApi")]
public class BookingApiController : ApiController
{
    private readonly DataDataContext _context;

    // Khởi tạo controller
    public BookingApiController()
    {
        _context = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
    }

    // API lấy số ghế còn trống theo suất chiếu
    [HttpGet]
    [Route("GetRemainingSeats/{suatChieuId}")]
    public IHttpActionResult GetRemainingSeats(int suatChieuId)
    {
        var suatChieu = _context.Suat_chieus.FirstOrDefault(sc => sc.suat_chieu_id == suatChieuId);

        if (suatChieu == null)
        {
            return NotFound();
        }

        // Tổng số ghế trống dựa trên số lượng ghế chưa được đặt
        var remainingSeats = _context.Ghe_ngois.Count(g => !_context.Dat_chos.Any(dc => dc.ghe_id == g.ghe_id && dc.suat_chieu_id == suatChieuId));
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

        // Kiểm tra suất chiếu
        var suatChieu = _context.Suat_chieus.FirstOrDefault(sc => sc.suat_chieu_id == request.SuatChieuId);
        if (suatChieu == null)
        {
            return BadRequest("Suất chiếu không tồn tại.");
        }

        // Kiểm tra trạng thái ghế
        var alreadyBookedSeats = _context.Dat_chos
            .Where(dc => request.SeatIds.Contains(dc.ghe_id) && dc.suat_chieu_id == request.SuatChieuId && dc.da_dat == true)
            .Select(dc => dc.ghe_id)
            .ToList();

        if (alreadyBookedSeats.Any())
        {
            return BadRequest($"Các ghế sau đã được đặt: {string.Join(", ", alreadyBookedSeats)}");
        }

        // Đặt ghế
        foreach (var seatId in request.SeatIds)
        {
            var booking = new Dat_cho
            {
                ghe_id = seatId,
                suat_chieu_id = request.SuatChieuId,
                da_dat = true
            };
            _context.Dat_chos.InsertOnSubmit(booking);
        }

        // Cập nhật số ghế còn lại trong suất chiếu
        suatChieu.tong_so_ghe_con_lai -= request.SeatIds.Count;

        _context.SubmitChanges();

        return Ok("Đặt vé thành công.");
    }

    // API giải phóng ghế (khi hết thời gian giữ)
    [HttpPost]
    [Route("ReleaseSeats")]
    public IHttpActionResult ReleaseSeats([FromBody] ReleaseRequest request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            return BadRequest("Không có ghế nào được giải phóng.");
        }

        var seatsToRelease = _context.Dat_chos
            .Where(dc => request.SeatIds.Contains(dc.ghe_id) && dc.suat_chieu_id == request.SuatChieuId && dc.da_dat == false)
            .ToList();

        if (!seatsToRelease.Any())
        {
            return BadRequest("Không tìm thấy ghế nào để giải phóng.");
        }

        foreach (var seat in seatsToRelease)
        {
            _context.Dat_chos.DeleteOnSubmit(seat);
        }

        _context.SubmitChanges();

        return Ok("Giải phóng ghế thành công.");
    }
}

// Lớp nhận dữ liệu yêu cầu đặt ghế
public class BookingRequest
{
    public int SuatChieuId { get; set; }
    public List<int> SeatIds { get; set; }
}

// Lớp nhận dữ liệu yêu cầu giải phóng ghế
public class ReleaseRequest
{
    public int SuatChieuId { get; set; }
    public List<int> SeatIds { get; set; }
}
