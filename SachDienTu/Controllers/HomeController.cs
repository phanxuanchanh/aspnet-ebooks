using SachDienTu.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

/*
 * Phần quản trị được viết trong thư mục Areas
 */

namespace SachDienTu.Controllers
{
    //[RoutePrefix("")]

    public class HomeController : Controller
    {
        SachDienTuDBContext db = new SachDienTuDBContext();

        public ActionResult Index()
        {
            return RedirectToAction("MainPage");
        }

        [Route("trang-chu")]
        public async Task<ActionResult> MainPage()
        {
            List<Category> categories = await db.Categories.Include(c => c.Books).Where(c => c.Books.Count != 0).ToListAsync();
            ViewBag.SiteName = "Trang chủ";
            return View(categories);
        }

        [Route("gioi-thieu")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Route("lien-lac")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}