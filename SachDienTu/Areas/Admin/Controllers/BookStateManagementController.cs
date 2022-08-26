using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SachDienTu.Models;

namespace SachDienTu.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quan-tri")]
    [RoutePrefix("trang-thai-sach")]
    [Authorize(Roles = "Super Admin, Admin")]
    public class BookStateManagementController : Controller
    {
        private SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("tat-ca-trang-thai")]
        public async Task<ActionResult> Index()
        {
            try
            {
                List<BookState> bookState = await db.BookStates.ToListAsync();
                ViewBag.SiteName = "Danh sách trạng thái";
                return View(bookState);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xem-chi-tiet/{id?}")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id < 0 || id == null)
            {
                return RedirectToAction("Index");
            }
            try
            {
                BookState bookState = await db.BookStates.FindAsync(id);
                if (bookState == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chi tiết về trạng thái";
                return View(bookState);
            }
            catch(Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("tao-moi")]
        public ActionResult Create()
        {
            ViewBag.SiteName = "Chỉnh sửa trạng thái";
            return View();
        }

        [Route("tao-moi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,name,description")] BookState bookState)
        {
            try
            {
                int check = await db.BookStates.CountAsync(s => s.name == bookState.name);
                if (check > 0)
                {
                    return View("_AdminError", model: "Dữ liệu bị trùng");
                }
                if (ModelState.IsValid)
                {
                    db.BookStates.Add(bookState);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.SiteName = "Chỉnh sửa trạng thái";
                return View(bookState);
            }
            catch(Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id < 0 || id == null)
            {
                return RedirectToAction("Index");
            }
            try
            {
                BookState bookState = await db.BookStates.FindAsync(id);
                if (bookState == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chỉnh sửa trạng thái";
                return View(bookState);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,name,description")] BookState bookState)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(bookState).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.SiteName = "Chỉnh sửa trạng thái";
                return View(bookState);
            }
            catch(Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa/{id?}")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id < 0 || id == null)
            {
                return RedirectToAction("Index");
            }
            try
            {
                BookState bookState = await db.BookStates.FindAsync(id);
                if (bookState == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Xóa trạng thái";
                return View(bookState);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa/{id?}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                BookState bookState = await db.BookStates.FindAsync(id);
                db.BookStates.Remove(bookState);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                int count = await db.Books.CountAsync(b => b.stateId == id);
                if(count > 0)
                {
                    return View("_AdminError", model: $"Không thể xóa được dữ liệu, do có {count} nội dung liên quan tới trạng thái này!");
                }
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
