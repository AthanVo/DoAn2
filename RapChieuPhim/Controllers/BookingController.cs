using Common;
using RapChieuPhim.Models;
using RapChieuPhim.Models.Payments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace RapChieuPhim.Controllers
{
    public class BookingController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Booking
        public ActionResult Index()
        {
            var datChoList = data.Dat_chos.ToList(); // Lấy dữ liệu từ bảng Dat_cho
            return View(datChoList); // Truyền vào view dưới dạng danh sách Dat_cho
        }


        public ActionResult Datve(int filmId)
        {
            var suatchieu = data.Suat_chieus.Where(sc => sc.phim_id == filmId).ToList();
            if (suatchieu == null || suatchieu.Count == 0)
            {
                return HttpNotFound("Không có suất chiếu nào cho phim này.");
            }
            return View(suatchieu);
        }

        public ActionResult Datghe(int filmId, int suatchieuId)
        {
            var suatChieu = data.Suat_chieus.FirstOrDefault(sc => sc.phim_id == filmId && sc.suat_chieu_id == suatchieuId);
            if (suatChieu == null)
            {
                return HttpNotFound("Không tìm thấy thông tin về suất chiếu.");
            }

            // Truy vấn tất cả các ghế của suất chiếu từ bảng Dat_cho
            var gheTrong = data.Dat_chos
                .Where(dc => dc.suat_chieu_id == suatchieuId)
                .ToList(); // Đảm bảo trả về danh sách Dat_cho

            ViewBag.FilmId = filmId;
            ViewBag.SuatChieuId = suatchieuId;
            return View(gheTrong); // Truyền danh sách Dat_cho vào view
        }

        // Chuẩn bị nội dung email
        private string PrepareEmailContent(Dat_ve datVe, string selectedSeats)
        {
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/neworder.html"));

            // Giả sử selectedSeats chứa các id của ghế đã chọn, tách và chuyển thành danh sách id
            var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();
            // Lấy tên ghế (số ghế) dựa trên các id ghế
            var seatNames = data.Ghe_ngois.Where(g => seatIds.Contains(g.ghe_id)).Select(g => g.so_ghe).ToList();

            // Replace các giá trị trong template
            content = content.Replace("{{Name}}", datVe.Suat_chieu.Phim.ten_phim);
            content = content.Replace("{{Date}}", DateTime.Now.ToString("dd/MM/yyyy"));
            content = content.Replace("{{Amount}}", seatIds.Count.ToString());
            content = content.Replace("{{CustomerName}}", datVe.Nguoi_dung.ho_ten);
            content = content.Replace("{{Phone}}", datVe.Nguoi_dung.so_dien_thoai);
            content = content.Replace("{{Email}}", datVe.Nguoi_dung.email);
            content = content.Replace("{{Giave}}", datVe.tong_tien.ToString());
            content = content.Replace("{{Mave}}", datVe.dat_ve_id.ToString());
            content = content.Replace("{{Suatchieu}}",datVe.Suat_chieu.gio_bat_dau.ToString());
            content = content.Replace("{{Tenrap}}", datVe.Suat_chieu.Rap_chieu_phim.ten_rap);
            // Dùng string.Join để nối các số ghế lại
            content = content.Replace("{{Ghengoi}}", string.Join(", ", seatNames));

            return content;
        }


        private string GetCustomerEmail(Dat_ve datVe)
        {
            return datVe.Nguoi_dung.email;
        }


        public ActionResult Chitietdatve(int datVeId)
        {
            // Lấy thông tin vé đặt
            var datVe = data.Dat_ves
                                .FirstOrDefault(dv => dv.dat_ve_id == datVeId);

            if (datVe == null)
            {
                return HttpNotFound("Không tìm thấy thông tin đặt vé.");
            }

            // Lấy thông tin người dùng
            var nguoiDung = data.Nguoi_dungs.FirstOrDefault(nd => nd.nguoi_dung_id == datVe.nguoi_dung_id);

            // Lấy danh sách ghế đã đặt thông qua bảng Chi_tiet_dat_ve
            var gheList = data.Chi_tiet_dat_ves
                              .Where(ct => ct.dat_ve_id == datVeId)  // Lấy các ghế liên kết với dat_ve_id
                              .Select(ct => ct.Ghe_ngoi.so_ghe)     // Lấy số ghế từ bảng Ghe_ngoi
                              .ToList();

            // Tính số lượng ghế
            ViewBag.GheCount = gheList.Count(); // Truyền số lượng ghế vào ViewBag

            // Chỉ lấy ghế đầu tiên nếu có ghế đã đặt
            string gheString = gheList.Any() ? string.Join(", ", gheList) : "Chưa có ghế nào";  // Trả về "Chưa có ghế nào" nếu không có ghế

            // Lấy thông tin phim và suất chiếu
            var suatChieu = data.Suat_chieus.FirstOrDefault(sc => sc.suat_chieu_id == datVe.suat_chieu_id);
            var phim = suatChieu?.Phim; // Đảm bảo thông tin phim không null

            // Truyền dữ liệu sang View
            ViewBag.NguoiDung = nguoiDung;
            ViewBag.GheList = gheString;  // Chỉ truyền ghế đại diện
            ViewBag.SuatChieu = suatChieu;
            ViewBag.Phim = phim;

            return View(datVe);
        }


        public ActionResult Lichsu()
        {
            if (Session["Nguoi_dung_id"] == null)
            {
                return RedirectToAction("Dangnhap", "Login");
            }

            int userId = (int)Session["Nguoi_dung_id"];

            // Lấy danh sách đặt vé duy nhất
            var bookings = data.Dat_ves
                .Where(dv => dv.nguoi_dung_id == userId)
                .GroupBy(dv => dv.dat_ve_id) // Nhóm theo ID đặt vé
                .Select(g => g.FirstOrDefault()) // Chỉ lấy một bản ghi đại diện cho mỗi ID
                .ToList();

            return View(bookings);
        }


        [HttpPost]
        public ActionResult Thanhtoan(string selectedSeats, decimal totalPrice, int filmId, int suatchieuId)
        {
            PaymentInformationModel model = new PaymentInformationModel
            {
                SelectedSeats = selectedSeats,
                Amount = totalPrice,
                FilmId = filmId,
                SuatChieuId = suatchieuId,

            };
            return View(model);
        }

        //Thanh toán vnpay
        [HttpPost]
        public JsonResult UrlPayment(int TypePaymentVN, int suatchieuId, decimal totalPrice, int filmId, string selectedSeats)
        {
            try
            {
                //string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
                string vnp_Returnurl = $"{ConfigurationManager.AppSettings["vnp_Returnurl"]}?suatchieuId={suatchieuId}&filmId={filmId}&selectedSeats={selectedSeats}&totalPrice={totalPrice}";

                string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
                string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
                // Đảm bảo bạn lấy mã này từ cấu hình và nó khớp với mã trong email hoặc tài khoản VNPAY của bạn
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];


                if (string.IsNullOrEmpty(vnp_Returnurl) || string.IsNullOrEmpty(vnp_Url) ||
                    string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                {
                    return Json(new { error = "Cấu hình thanh toán không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }

                VnPayLibrary vnpay = new VnPayLibrary();
                long Price = (long)totalPrice * 100; // Sử dụng `totalPrice` truyền vào từ ConfirmBooking

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", Price.ToString());

                // Xác định mã ngân hàng dựa trên TypePaymentVN
                if (TypePaymentVN == 1)
                {
                    vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
                }
                else if (TypePaymentVN == 2)
                {
                    vnpay.AddRequestData("vnp_BankCode", "VNBANK");
                }
                else if (TypePaymentVN == 3)
                {
                    vnpay.AddRequestData("vnp_BankCode", "INTCARD");
                }
                else
                {
                    return Json(new { error = "Loại thanh toán không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }

                var createdDate = DateTime.Now;
                vnpay.AddRequestData("vnp_CreateDate", createdDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán suất chiếu {suatchieuId} của phim {filmId}");
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

                //vnpay.AddRequestData("vnp_TxnRef", uniqueTxnRef);
                string uniqueTxnRef = $"{suatchieuId}_{DateTime.Now.Ticks}";
                vnpay.AddRequestData("vnp_TxnRef", uniqueTxnRef);

                string urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                Console.WriteLine("Generated Payment URL: " + urlPayment); // Hoặc dùng Debug.WriteLine
                return Json(new { url = urlPayment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Đã xảy ra lỗi khi tạo URL thanh toán: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ConfirmBooking(int filmId, int suatchieuId, string selectedSeats)
        {
            if (string.IsNullOrEmpty(selectedSeats))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Không có ghế nào được chọn.");
            }

            var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();

            decimal totalPrice = 0; // Tổng tiền

            // Kiểm tra trùng lặp đặt vé
            var existingBookings = data.Dat_chos.Where(dc => dc.suat_chieu_id == suatchieuId && seatIds.Contains(dc.ghe_id) && dc.da_dat == true).ToList();

            if (existingBookings.Any())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Một hoặc nhiều ghế đã được đặt.");
            }

            // Lấy ID của người dùng từ session
            if (Session["Nguoi_dung_id"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Bạn phải đăng nhập để đặt vé.");
            }

            int nguoiDungId = (int)Session["Nguoi_dung_id"];

            Dat_ve datVe = null;

            // Cập nhật trạng thái ghế đã đặt và tính tổng tiền cho tất cả ghế đã chọn
            foreach (var seatId in seatIds)
            {
                var datCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == seatId && dc.suat_chieu_id == suatchieuId);
                if (datCho != null)
                {
                    datCho.da_dat = true; // Cập nhật trạng thái ghế đã đặt
                    totalPrice += Convert.ToDecimal(datCho.Ghe_ngoi.Loai_ghe.gia_ve);
                }
            }

            // Lưu các thay đổi vào cơ sở dữ liệu (cập nhật thông tin ghế và trạng thái)
            data.SubmitChanges();

            // Lưu thông tin đặt vé chung (chỉ lưu 1 lần sau khi tất cả ghế đã được đặt)
            datVe = new Dat_ve
            {
                nguoi_dung_id = nguoiDungId,
                suat_chieu_id = suatchieuId,
                thoi_gian_dat = DateTime.Now,
                tong_tien = totalPrice
            };

            data.Dat_ves.InsertOnSubmit(datVe);
            data.SubmitChanges();

            // Lưu các chi tiết đặt vé (với tất cả các ghế đã chọn)
            foreach (var seatId in seatIds)
            {
                var chiTietDatVe = new Chi_tiet_dat_ve
                {
                    dat_ve_id = datVe.dat_ve_id,
                    ghe_id = seatId,
                    gia_ve = totalPrice // Giá vé có thể tính lại sau khi lưu tất cả
                };
                data.Chi_tiet_dat_ves.InsertOnSubmit(chiTietDatVe);
            }

            // Lưu lại vào cơ sở dữ liệu
            data.SubmitChanges();

            // Gửi email chỉ một lần sau khi tất cả các ghế đã được đặt và thông tin đã lưu vào cơ sở dữ liệu
            var content = PrepareEmailContent(datVe, selectedSeats);
            var customerEmail = GetCustomerEmail(datVe);
            new MailHelper().SendMail(customerEmail, "Xác nhận đặt vé", content, datVe.Nguoi_dung.ho_ten);

            // Chuyển hướng người dùng đến trang chi tiết đặt vé
            return RedirectToAction("Chitietdatve", new { datVeId = datVe.dat_ve_id });
        }




        public ActionResult VnpayReturn(int? suatchieuId, int? filmId, string selectedSeats, decimal? totalPrice)
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }

                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                if (checkSignature && vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    if (Session["Nguoi_dung_id"] == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Bạn phải đăng nhập để đặt vé.");
                    }

                    int nguoiDungId = (int)Session["Nguoi_dung_id"];
                    var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();
                    decimal totalTransactionPrice = 0; // Khởi tạo biến tổng tiền
                    Dat_ve datVe = null; // Khởi tạo ở ngoài vòng lặp
                    // Cập nhật trạng thái ghế và tính tổng tiền
                    foreach (var seatId in seatIds)
                    {
                        var datCho = data.Dat_chos.FirstOrDefault(dc => dc.suat_chieu_id == suatchieuId && dc.ghe_id == seatId);
                        if (datCho != null)
                        {
                            datCho.da_dat = true; // Đánh dấu ghế đã đặt
                            totalTransactionPrice += Convert.ToDecimal(datCho.Ghe_ngoi.Loai_ghe.gia_ve); // Tính tổng tiền
                        }

                        // Lưu chi tiết đặt vé cho từng ghế (không cần tạo Dat_ve trong vòng lặp)
                        var chiTietDatVe = new Chi_tiet_dat_ve
                        {
                            ghe_id = seatId,
                            gia_ve = totalTransactionPrice // Giá vé có thể tính lại sau khi lưu tất cả
                        };
                        data.Chi_tiet_dat_ves.InsertOnSubmit(chiTietDatVe);
                    }

                    // Lưu thông tin đặt vé chung (chỉ lưu 1 lần sau khi tất cả ghế đã được xử lý)
                    datVe = new Dat_ve
                    {
                        nguoi_dung_id = nguoiDungId,
                        suat_chieu_id = suatchieuId,
                        thoi_gian_dat = DateTime.Now,
                        tong_tien = totalTransactionPrice // Sử dụng tổng tiền sau khi tính cho tất cả ghế
                    };
                    data.Dat_ves.InsertOnSubmit(datVe);

                    // Lưu lại vào cơ sở dữ liệu (cập nhật trạng thái và các chi tiết)
                    data.SubmitChanges();

                    // Gửi email chỉ một lần sau khi tất cả ghế đã được đặt và thông tin đã lưu vào cơ sở dữ liệu
                    var content = PrepareEmailContent(datVe, selectedSeats);
                    var customerEmail = GetCustomerEmail(datVe);
                    new MailHelper().SendMail(customerEmail, "Xác nhận đặt vé", content, datVe.Nguoi_dung.ho_ten);

                    ViewBag.Message = "Giao dịch thành công!";
                    ViewBag.OrderId = datVe.dat_ve_id;
                }
                else
                {
                    ViewBag.Message = "Xin lỗi, có lỗi xảy ra trong quá trình thanh toán.";
                }
            }
            return View();
        }

        public JsonResult ApplyVoucher(string voucherCode, decimal originalPrice, string appliedVoucher)
        {
            try
            {
                // Kiểm tra nếu đã có mã giảm giá được áp dụng
                if (!string.IsNullOrEmpty(appliedVoucher))
                {
                    return Json(new { success = false, message = "Bạn chỉ có thể áp dụng một mã giảm giá." }, JsonRequestBehavior.AllowGet);
                }

                // Tìm mã giảm giá dựa vào mã voucher
                var voucher = data.Khuyen_mais.FirstOrDefault(v =>
                    v.ma_giam_gia == voucherCode &&
                    v.ngay_bat_dau <= DateTime.Now &&
                    v.ngay_ket_thuc >= DateTime.Now &&
                    v.trang_thai
                );

                if (voucher == null)
                {
                    return Json(new { success = false, message = "Mã khuyến mãi không hợp lệ hoặc đã hết hạn." }, JsonRequestBehavior.AllowGet);
                }

                // Tính giá sau khi giảm
                decimal finalPrice = originalPrice * (1 - (decimal)voucher.phan_tram_giam / 100m);


                return Json(new { success = true, finalPrice = finalPrice, appliedVoucher = voucherCode }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}

