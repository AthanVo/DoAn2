using System.Web.Mvc;

namespace RapChieuPhim.Filters
{
    public class AuthorizeAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy vai trò của người dùng từ Session
            var role = filterContext.HttpContext.Session["Role"]?.ToString();

            // Kiểm tra nếu người dùng chưa đăng nhập hoặc không phải admin
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                // Chuyển hướng về trang đăng nhập
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Login" }, // Tên controller xử lý đăng nhập
                    { "action", "Dangnhap" },  // Tên action đăng nhập
                    { "area", "" } // Nếu dùng Areas, cần chỉ định đúng Area
                });
            }

            // Gọi lớp cha để tiếp tục xử lý
            base.OnActionExecuting(filterContext);
        }
    }
}
