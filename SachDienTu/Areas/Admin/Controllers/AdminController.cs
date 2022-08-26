using SachDienTu.Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SachDienTu.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quan-tri")]
    [RoutePrefix("tong-quan")]
    [Authorize]
    public class AdminController : Controller
    {
        SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("trang-trong")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Route("thong-ke")]
        [Authorize(Roles = "Super Admin")]
        public async Task<ActionResult> General()
        {
            try
            {
                ViewBag.BookNumber = await db.Books.CountAsync();
                ViewBag.BookAuthorNumber = await db.BookAuthors.CountAsync();
                ViewBag.CategoryNumber = await db.Categories.CountAsync();
                ViewBag.PublishingHouseNumber = await db.PublishingHouses.CountAsync();
                ViewBag.SiteName = "Trang tổng quan";
                return View();
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
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