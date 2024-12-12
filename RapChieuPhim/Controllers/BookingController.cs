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

        public ActionResult Index()
        {
            var datChoList = data.Dat_chos.ToList();
            return View(datChoList);
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

            var gheTrong = data.Dat_chos
                .Where(dc => dc.suat_chieu_id == suatchieuId)
                .ToList();

            ViewBag.FilmId = filmId;
            ViewBag.SuatChieuId = suatchieuId;
            return View(gheTrong);
        }

        private string PrepareEmailContent(Dat_ve datVe, string selectedSeats)
        {
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/neworder.html"));

            var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();
            var seatNames = data.Ghe_ngois.Where(g => seatIds.Contains(g.ghe_id)).Select(g => g.so_ghe).ToList();

            content = content.Replace("{{Name}}", datVe.Suat_chieu.Phim.ten_phim);
            content = content.Replace("{{Date}}", DateTime.Now.ToString("dd/MM/yyyy"));
            content = content.Replace("{{Amount}}", seatIds.Count.ToString());
            content = content.Replace("{{CustomerName}}", datVe.Nguoi_dung.ho_ten);
            content = content.Replace("{{Phone}}", datVe.Nguoi_dung.so_dien_thoai);
            content = content.Replace("{{Email}}", datVe.Nguoi_dung.email);
            content = content.Replace("{{Giave}}", datVe.tong_tien.ToString());
            content = content.Replace("{{Mave}}", datVe.dat_ve_id.ToString());
            content = content.Replace("{{Suatchieu}}", datVe.Suat_chieu.gio_bat_dau.ToString());
            content = content.Replace("{{Tenrap}}", datVe.Suat_chieu.Rap_chieu_phim.ten_rap);
            content = content.Replace("{{Ghengoi}}", string.Join(", ", seatNames));

            return content;
        }

        private string GetCustomerEmail(Dat_ve datVe)
        {
            return datVe.Nguoi_dung.email;
        }

        public ActionResult Chitietdatve(int datVeId)
        {
            var datVe = data.Dat_ves.FirstOrDefault(dv => dv.dat_ve_id == datVeId);

            if (datVe == null)
            {
                return HttpNotFound("Không tìm thấy thông tin đặt vé.");
            }

            var gheList = data.Chi_tiet_dat_ves
                              .Where(ct => ct.dat_ve_id == datVeId)
                              .Select(ct => ct.Ghe_ngoi.so_ghe)
                              .ToList();

            ViewBag.GheCount = gheList.Count();
            string gheString = gheList.Any() ? string.Join(", ", gheList) : "Chưa có ghế nào";

            var suatChieu = data.Suat_chieus.FirstOrDefault(sc => sc.suat_chieu_id == datVe.suat_chieu_id);
            var phim = suatChieu?.Phim;

            ViewBag.NguoiDung = data.Nguoi_dungs.FirstOrDefault(nd => nd.nguoi_dung_id == datVe.nguoi_dung_id);
            ViewBag.GheList = gheString;
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

            var bookings = data.Dat_ves
                .Where(dv => dv.nguoi_dung_id == userId)
                .GroupBy(dv => dv.dat_ve_id)
                .Select(g => g.FirstOrDefault())
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        public ActionResult Thanhtoan(string selectedSeats, decimal totalPrice, int filmId, int suatchieuId, decimal? finalPrice)
        {
            PaymentInformationModel model = new PaymentInformationModel
            {
                SelectedSeats = selectedSeats,
                Amount = finalPrice ?? totalPrice, // Nếu finalPrice không có, sử dụng giá gốc
                FilmId = filmId,
                SuatChieuId = suatchieuId,
            };
            return View(model);
        }


        [HttpPost]
        public JsonResult UrlPayment(int TypePaymentVN, int suatchieuId, decimal finalPrice, int filmId, string selectedSeats)
        {
            try
            {
                // Tạo URL trả về từ VNPay
                string vnp_Returnurl = $"{ConfigurationManager.AppSettings["vnp_Returnurl"]}?suatchieuId={suatchieuId}&filmId={filmId}&selectedSeats={selectedSeats}&totalPrice={finalPrice}";
                string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
                string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

                if (string.IsNullOrEmpty(vnp_Returnurl) || string.IsNullOrEmpty(vnp_Url) ||
                    string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                {
                    return Json(new { error = "Cấu hình thanh toán không hợp lệ." }, JsonRequestBehavior.AllowGet);
                }

                VnPayLibrary vnpay = new VnPayLibrary();
                long Price = (long)(finalPrice * 100); // Giá cần nhân 100 để tính theo VNPay

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", Price.ToString());

                // Thêm mã ngân hàng dựa trên phương thức thanh toán
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

                string uniqueTxnRef = $"{suatchieuId}_{DateTime.Now.Ticks}";
                vnpay.AddRequestData("vnp_TxnRef", uniqueTxnRef);

                // Tạo URL thanh toán
                string urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                return Json(new { url = urlPayment }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Đã xảy ra lỗi khi tạo URL thanh toán: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult ConfirmBooking(int filmId, int suatchieuId, string selectedSeats, decimal finalPrice)
        {
            if (string.IsNullOrEmpty(selectedSeats))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Không có ghế nào được chọn.");
            }

            var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();
            var existingBookings = data.Dat_chos.Where(dc => dc.suat_chieu_id == suatchieuId && seatIds.Contains(dc.ghe_id) && dc.da_dat == true).ToList();

            if (existingBookings.Any())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, "Một hoặc nhiều ghế đã được đặt.");
            }

            if (Session["Nguoi_dung_id"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Bạn phải đăng nhập để đặt vé.");
            }

            int nguoiDungId = (int)Session["Nguoi_dung_id"];

            // Cập nhật trạng thái ghế
            foreach (var seatId in seatIds)
            {
                var datCho = data.Dat_chos.FirstOrDefault(dc => dc.ghe_id == seatId && dc.suat_chieu_id == suatchieuId);
                if (datCho != null)
                {
                    datCho.da_dat = true;
                }
            }

            data.SubmitChanges();

            // Lưu thông tin đặt vé
            var datVe = new Dat_ve
            {
                nguoi_dung_id = nguoiDungId,
                suat_chieu_id = suatchieuId,
                thoi_gian_dat = DateTime.Now,
                tong_tien = finalPrice // Tổng tiền sau giảm giá
            };

            data.Dat_ves.InsertOnSubmit(datVe);
            data.SubmitChanges();

            // Lưu thông tin chi tiết ghế
            foreach (var seatId in seatIds)
            {
                var chiTietDatVe = new Chi_tiet_dat_ve
                {
                    dat_ve_id = datVe.dat_ve_id,
                    ghe_id = seatId,
                    gia_ve = finalPrice / seatIds.Count
                };
                data.Chi_tiet_dat_ves.InsertOnSubmit(chiTietDatVe);
            }

            data.SubmitChanges();

            var content = PrepareEmailContent(datVe, selectedSeats);
            var customerEmail = GetCustomerEmail(datVe);
            new MailHelper().SendMail(customerEmail, "Xác nhận đặt vé", content, datVe.Nguoi_dung.ho_ten);

            return RedirectToAction("Chitietdatve", new { datVeId = datVe.dat_ve_id });
        }

        public ActionResult VnpayReturn(int? suatchieuId, int? filmId, string selectedSeats, decimal? totalPrice, decimal? finalPrice = null)
        {
            if (!suatchieuId.HasValue || !filmId.HasValue || string.IsNullOrEmpty(selectedSeats) || !totalPrice.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Thiếu thông tin thanh toán.");
            }

            try
            {
                decimal amount = finalPrice ?? totalPrice.Value;

                // Kiểm tra chữ ký bảo mật từ VNPay
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string key in vnpayData)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, vnpayData[key]);
                    }
                }

                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

                // Giao dịch thành công
                if (checkSignature && vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    int nguoiDungId = (int)Session["Nguoi_dung_id"];
                    var seatIds = selectedSeats.Split(',').Select(int.Parse).ToList();

                    var datVe = new Dat_ve
                    {
                        nguoi_dung_id = nguoiDungId,
                        suat_chieu_id = suatchieuId,
                        thoi_gian_dat = DateTime.Now,
                        tong_tien = amount
                    };

                    data.Dat_ves.InsertOnSubmit(datVe);
                    data.SubmitChanges();

                    foreach (var seatId in seatIds)
                    {
                        var chiTietDatVe = new Chi_tiet_dat_ve
                        {
                            dat_ve_id = datVe.dat_ve_id,
                            ghe_id = seatId,
                            gia_ve = amount / seatIds.Count
                        };
                        data.Chi_tiet_dat_ves.InsertOnSubmit(chiTietDatVe);
                    }

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
                    ViewBag.Message = "Xin lỗi, giao dịch không thành công. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}";
            }

            return View();
        }





        public JsonResult ApplyVoucher(string voucherCode, decimal originalPrice, string appliedVoucher)
        {
            try
            {
                if (!string.IsNullOrEmpty(appliedVoucher))
                {
                    return Json(new { success = false, message = "Bạn chỉ có thể áp dụng một mã giảm giá." }, JsonRequestBehavior.AllowGet);
                }

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

                // Tính tổng tiền sau khi giảm giá
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
