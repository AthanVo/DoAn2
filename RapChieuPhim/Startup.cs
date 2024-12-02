using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(RapChieuPhim.Startup))]

namespace RapChieuPhim
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình SignalR sử dụng OWIN
            app.MapSignalR();

        }
    }
}
