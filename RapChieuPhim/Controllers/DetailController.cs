using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Controllers
{
    public class DetailController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
        // GET: Detail
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int? id) // 'id' có thể null
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Hoặc xử lý lỗi theo cách khác
            }

            var phim = data.Phims.FirstOrDefault(s => s.phim_id == id);
            if (phim == null)
            {
                return HttpNotFound();
            }

            return View(phim);
        }

    }
}