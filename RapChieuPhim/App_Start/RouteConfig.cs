using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace RapChieuPhim
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route cho MVC
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "ApplyVoucher",
            url: "Booking/ApplyVoucher",
            defaults: new { controller = "Booking", action = "ApplyVoucher" }
             );

        }
    }

    // Cấu hình cho Web API
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Cấu hình Web API routes sử dụng attribute routing
            config.MapHttpAttributeRoutes();

            // Cấu hình route mặc định cho Web API nếu không có attribute routing
            config.Routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
