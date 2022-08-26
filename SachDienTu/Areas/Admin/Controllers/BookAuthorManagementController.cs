using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SachDienTu.Models;
using X.PagedList;

namespace SachDienTu.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quan-tri")]
    [RoutePrefix("tac-gia-sach")]
    [Authorize(Roles = "Super Admin, Admin")]
    public class BookAuthorManagementController : Controller
    {
        private SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("tat-ca-tac-gia/{page?}")]
        public async Task<ActionResult> Index(int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 10;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.BookAuthors.CountAsync();
                List<BookAuthor> onePageOfData = await db.BookAuthors.OrderBy(p => p.ID).Skip(n).Take(pageSize).ToListAsync();
                StaticPagedList<BookAuthor> bookAuthors = new StaticPagedList<BookAuthor>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.BookAuthors = bookAuthors;
                ViewBag.SiteName = "Danh sách tác giả";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_BookAuthorListPartial");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xem-chi-tiet/{id?}")]
        public async Task<ActionResult> Detail(long? id)
        {
            if (id == null || id < 1)
                return RedirectToAction("Index");
            try
            {
                BookAuthor bookAuthor = await db.BookAuthors.FindAsync(id);
                if (bookAuthor == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chi tiết tác giả";
                return View(bookAuthor);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("tao-moi")]
        public ActionResult Create()
        {
            ViewBag.SiteName = "Tạo mới tác giả";
            return View();
        }

        [Route("tao-moi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,name,email,phoneNumber,description,location")] BookAuthor bookAuthor)
        {
            try
            {
                int check = await db.BookAuthors.CountAsync(a => a.name == bookAuthor.name && a.email == bookAuthor.email && a.phoneNumber == bookAuthor.phoneNumber);
                if (check > 0)
                {
                    return View("_AdminError", model: "Dữ liệu bị trùng");
                }
                if (ModelState.IsValid)
                {
                    bookAuthor.createAt = DateTime.Now;
                    bookAuthor.updateAt = DateTime.Now;
                    db.BookAuthors.Add(bookAuthor);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.SiteName = "Tạo mới tác giả";
                return View(bookAuthor);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null || id < 1)
                return RedirectToAction("Index");
            try
            {
                BookAuthor bookAuthor = await db.BookAuthors.FindAsync(id);
                if (bookAuthor == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chi tiết tác giả";
                return View(bookAuthor);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,name,email,phoneNumber,description,location")] BookAuthor bookAuthor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BookAuthor auth = await db.BookAuthors.FindAsync(bookAuthor.ID);
                    auth.updateAt = DateTime.Now;
                    TryUpdateModel(auth, new string[] { "name", "email", "phoneNumber", "description", "location", "updateAt"});
                    await db.SaveChangesAsync();
                    return RedirectToAction("Detail", new { id = bookAuthor.ID });
                }
                ViewBag.SiteName = "Chỉnh sửa tác giả";
                return View(bookAuthor);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa/{id?}")]
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null || id < 1)
                return RedirectToAction("Index");
            try
            {
                BookAuthor bookAuthor = await db.BookAuthors.FindAsync(id);
                if (bookAuthor == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Xóa tác giả";
                return View(bookAuthor);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa/{id?}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            try
            {
                BookAuthor bookAuthor = await db.BookAuthors.FindAsync(id);
                db.BookAuthors.Remove(bookAuthor);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                long count = await db.AuthorContributes.CountAsync(c => c.authorId == id);
                if (count > 0)
                {
                    return View("_AdminError", model: $"Không thể xóa được dữ liệu, do có {count} nội dung liên quan tới tác giả này!");
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
