using RapChieuPhim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RapChieuPhim.Controllers
{
    public class PromotionController : Controller
    {
        DataDataContext data = new DataDataContext("DESKTOP-GIKAS1S\\MSSQLSERVER01");
        // GET: Promotion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Khuyenmai()
        {
            var khuyenmai = data.Khuyen_mais.ToList();
            return View(khuyenmai);
        }

        public ActionResult Chitietkhuyenmai(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Hoặc xử lý lỗi theo cách khác
            }

            var khuyenmai = data.Khuyen_mais.FirstOrDefault(s => s.ma_khuyen_mai_id == id);
            if (khuyenmai == null)
            {
                return HttpNotFound();
            }

            return View(khuyenmai);
        }

    }
}