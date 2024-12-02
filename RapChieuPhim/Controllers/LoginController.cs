using RapChieuPhim.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Controllers
{
    public class LoginController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, Nguoi_dung nd)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhatnhaplai = collection["Matkhaunhaplai"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];

            bool hasError = false;

            // Giữ lại dữ liệu trong form
            ViewData["HotenKH"] = hoten;
            ViewData["TenDN"] = tendn;
            ViewData["Email"] = email;
            ViewData["Dienthoai"] = dienthoai;

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống";
                hasError = true;
            }

            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
                hasError = true;
            }
            else
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                var existingUser = data.Nguoi_dungs.SingleOrDefault(u => u.ten_dang_nhap == tendn);
                if (existingUser != null)
                {
                    ViewData["Loi2"] = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.";
                    hasError = true;
                }
            }

            if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
                hasError = true;
            }

            if (string.IsNullOrEmpty(matkhatnhaplai))
            {
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
                hasError = true;
            }
            else if (matkhau != matkhatnhaplai)
            {
                ViewData["Loi4"] = "Mật khẩu nhập lại không khớp";
                hasError = true;
            }

            if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được bỏ trống";
                hasError = true;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ViewData["Loi5"] = "Email không đúng định dạng";
                hasError = true;
            }
            else
            {
                // Kiểm tra email đã tồn tại chưa
                var existingEmail = data.Nguoi_dungs.SingleOrDefault(u => u.email == email);
                if (existingEmail != null)
                {
                    ViewData["Loi5"] = "Email đã tồn tại. Vui lòng sử dụng email khác.";
                    hasError = true;
                }
            }

            if (string.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập điện thoại";
                hasError = true;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(dienthoai, @"^\d{10,11}$"))
            {
                ViewData["Loi6"] = "Số điện thoại phải là 10 hoặc 11 chữ số";
                hasError = true;
            }

            // Nếu không có lỗi, tiến hành lưu
            if (!hasError)
            {
                try
                {
                    nd.ho_ten = hoten;
                    nd.ten_dang_nhap = tendn;
                    nd.mat_khau = HashPassword(matkhau); // Mã hóa mật khẩu
                    nd.email = email;
                    nd.so_dien_thoai = dienthoai;
                    nd.ngay_tao = DateTime.Now; // Gán ngày tạo hiện tại
                    nd.role = "User"; // Thiết lập vai trò mặc định là User

                    data.Nguoi_dungs.InsertOnSubmit(nd);
                    data.SubmitChanges();

                    // Chuyển hướng đến trang đăng nhập
                    return RedirectToAction("Dangnhap");
                }
                catch (Exception ex)
                {
                    ViewData["Loi9"] = $"Có lỗi xảy ra khi tạo tài khoản. Chi tiết: {ex.Message}";
                }
            }

            // Nếu có lỗi, quay lại trang đăng ký
            return View("Dangky");
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];

            if (!string.IsNullOrEmpty(tendn) && !string.IsNullOrEmpty(matkhau))
            {
                var hashedPassword = HashPassword(matkhau);
                var user = data.Nguoi_dungs.SingleOrDefault(u => u.ten_dang_nhap == tendn && u.mat_khau == hashedPassword);

                if (user != null)
                {
                    // Lấy cookie nếu đã tồn tại
                    var loginCookie = Request.Cookies["LoginToken"];
                    string token;
                    int loginCount;

                    if (loginCookie != null && loginCookie.Value.Contains("|"))
                    {
                        var parts = loginCookie.Value.Split('|');
                        if (parts.Length == 2)
                        {
                            // Giữ nguyên token cũ và tăng số lần đăng nhập
                            token = parts[0];
                            loginCount = int.Parse(parts[1]) + 1;
                        }
                        else
                        {
                            // Cookie không hợp lệ, tạo mới
                            token = Guid.NewGuid().ToString();
                            loginCount = 1;
                        }
                    }
                    else
                    {
                        // Không có cookie, tạo mới token và thiết lập số lần đăng nhập ban đầu
                        token = Guid.NewGuid().ToString();
                        loginCount = 1;
                    }

                    // Lưu token và số lần đăng nhập vào cookie
                    var authCookie = new HttpCookie("LoginToken", $"{token}|{loginCount}")
                    {
                        HttpOnly = true, // Chỉ truy cập được từ server, ngăn JavaScript đọc
                        Secure = true,   // Chỉ gửi qua HTTPS
                        Expires = DateTime.Now.AddDays(30) // Token và số lần đăng nhập tồn tại trong 30 ngày
                    };

                    Response.Cookies.Add(authCookie);

                    // Lưu thông tin người dùng vào Session
                    Session["Nguoi_dung_id"] = user.nguoi_dung_id;
                    Session["ho_ten"] = user.ho_ten;
                    Session["Role"] = user.role;
                    Session["ten_nguoi_dung"] = user.ten_dang_nhap;
                    Session["so_dien_thoai"] = user.so_dien_thoai;
                    Session["Email"] = user.email;


                    // Điều hướng dựa trên vai trò
                    if (user.role == "Admin")
                    {
                        return RedirectToAction("Index", "Trangchu", new { area = "Admin" });
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng.";
                }
            }
            else
            {
                ViewBag.Thongbao = "Vui lòng nhập đầy đủ thông tin.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult CheckLoginStatus()
        {
            bool loggedIn = Session["Nguoi_dung_id"] != null;
            string role = loggedIn ? Session["role"].ToString() : null;

            return Json(new { loggedIn = loggedIn, role = role }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult Thongtinnguoidung()
        {
            if (Session["Nguoi_dung_id"] == null)
            {
                return RedirectToAction("Dangnhap");
            }
            return View();
        }

        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuenMatKhau(string email)
        {
            // Kiểm tra email người dùng
            var user = data.Nguoi_dungs.SingleOrDefault(u => u.email == email);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại trong hệ thống";
                return View();
            }

            // Tạo mã xác nhận hoặc token để đặt lại mật khẩu
            string resetToken = Guid.NewGuid().ToString();
            user.reset_token = resetToken;
            user.reset_token_expiration = DateTime.Now.AddHours(1); // Token hết hạn sau 1 giờ

            // Cập nhật token vào CSDL
            data.SubmitChanges();

            // Gửi email với đường dẫn đặt lại mật khẩu
            string resetLink = Url.Action("DatLaiMatKhau", "Login", new { token = resetToken }, Request.Url.Scheme);
            string emailContent = $"Nhấn vào liên kết sau để đặt lại mật khẩu của bạn: <a href='{resetLink}'>Đặt lại mật khẩu</a>";

            // Gửi email (sử dụng một phương thức gửi email mà bạn đã cài đặt)
            SendEmail(user.email, "Đặt lại mật khẩu", emailContent);

            ViewBag.Message = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.";
            return View();
        }


        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("ducthinh25042003@gmail.com", "Athan"); // Địa chỉ email gửi
                var toAddress = new MailAddress(toEmail); // Địa chỉ email nhận
                string fromPassword = "fwcgokjijwcpyssk"; // Mật khẩu ứng dụng của email

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // SMTP server cho Gmail
                    Port = 587,  // Cổng SMTP
                    EnableSsl = true,  // Bật SSL để bảo mật kết nối
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword) // Sử dụng tài khoản và mật khẩu
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Nếu bạn muốn gửi email có định dạng HTML
                })
                {
                    smtp.Send(message); // Gửi email
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ra console hoặc file
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                throw; // Ném lỗi nếu cần thiết để xử lý phía trên
            }
        }
        public ActionResult DatLaiMatKhau(string token)
        {
            // Kiểm tra token hợp lệ
            var user = data.Nguoi_dungs.SingleOrDefault(u => u.reset_token == token && u.reset_token_expiration > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return View("Loi");
            }

            return View();
        }

        [HttpPost]
        public ActionResult DatLaiMatKhau(string token, string matkhau, string matkhaunhaplai)
        {
            // Kiểm tra token hợp lệ
            var user = data.Nguoi_dungs.SingleOrDefault(u => u.reset_token == token && u.reset_token_expiration > DateTime.Now);
            if (user == null)
            {
                ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
                return View("Loi");
            }

            if (matkhau != matkhaunhaplai)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            // Cập nhật mật khẩu mới
            user.mat_khau = HashPassword(matkhau);
            user.reset_token = null; // Xóa token sau khi mật khẩu đã được đặt lại
            user.reset_token_expiration = null;
            data.SubmitChanges();

            ViewBag.Message = "Mật khẩu đã được đặt lại thành công!";
            return RedirectToAction("Dangnhap");
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public ActionResult Logout()
        {
            // Xóa cookie chứa token
            var authCookie = Request.Cookies["LoginToken"];
            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now.AddDays(-1); // Hết hạn cookie
                Response.Cookies.Add(authCookie);
            }

            // Xóa Session
            Session.Clear();

            return RedirectToAction("Dangnhap", "Login");
        }

        public ActionResult SomeAction()
        {
            var loginCookie = Request.Cookies["LoginToken"];
            if (loginCookie != null && loginCookie.Value.Contains("|"))
            {
                var parts = loginCookie.Value.Split('|');
                var token = parts[0];
                var loginCount = parts[1];

                // Kiểm tra token hoặc sử dụng loginCount nếu cần
                if (!string.IsNullOrEmpty(token))
                {
                    ViewBag.Message = $"Token hợp lệ. Số lần đăng nhập: {loginCount}";
                }
                else
                {
                    return RedirectToAction("Dangnhap", "Login");
                }
            }
            else
            {
                return RedirectToAction("Dangnhap", "Login");
            }

            return View();
        }

        // GET: Thay đổi mật khẩu
        public ActionResult DoiMatKhau()
        {
            if (Session["Nguoi_dung_id"] == null)
            {
                return RedirectToAction("Dangnhap");
            }

            return View();
        }

        [HttpPost]
        public ActionResult DoiMatKhau(string matkhauCu, string matkhauMoi, string matkhaunhaplai)
        {
            if (Session["Nguoi_dung_id"] == null)
            {
                return RedirectToAction("Dangnhap");
            }

            int userId = (int)Session["Nguoi_dung_id"];
            var user = data.Nguoi_dungs.SingleOrDefault(u => u.nguoi_dung_id == userId);

            if (user != null)
            {
                // Kiểm tra mật khẩu cũ
                if (user.mat_khau != HashPassword(matkhauCu))
                {
                    ViewBag.Error = "Mật khẩu cũ không đúng.";
                    return View();
                }

                // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới
                if (matkhauMoi != matkhaunhaplai)
                {
                    ViewBag.Error = "Mật khẩu mới và mật khẩu nhập lại không khớp.";
                    return View();
                }

                // Cập nhật mật khẩu mới
                user.mat_khau = HashPassword(matkhauMoi);
                data.SubmitChanges();

                // Xóa session và cookie (đăng xuất người dùng)
                Session.Clear(); // Xóa toàn bộ session
                var authCookie = Request.Cookies["LoginToken"];
                if (authCookie != null)
                {
                    authCookie.Expires = DateTime.Now.AddDays(-1); // Hết hạn cookie
                    Response.Cookies.Add(authCookie); // Đảm bảo cookie đã được xóa
                }

                // Thông báo thành công
                ViewBag.Message = "Mật khẩu đã được thay đổi thành công. Bạn sẽ bị đăng xuất khỏi tài khoản hiện tại.";

                // Chuyển hướng đến trang đăng nhập
                return RedirectToAction("Dangnhap", "Login");
            }

            ViewBag.Error = "Không tìm thấy người dùng.";
            return View();
        }





    }
}
