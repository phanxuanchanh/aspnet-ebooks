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
    [RoutePrefix("nha-xuat-ban")]
    [Authorize(Roles = "Super Admin, Admin")]
    public class PublishingHouseManagementController : Controller
    {
        private SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("tat-ca-nha-xuat-ban")]
        public async Task<ActionResult> Index(int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 10;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.PublishingHouses.CountAsync();
                List<PublishingHouse> onePageOfData = await db.PublishingHouses
                    .OrderBy(p => p.ID).Skip(n).Take(pageSize).ToListAsync();
                StaticPagedList<PublishingHouse> publishingHouses = new StaticPagedList<PublishingHouse>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.PublishingHouses = publishingHouses;
                ViewBag.SiteName = "Danh sách nhà xuất bản";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_PublishingHouseListPartial");
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
                PublishingHouse publishingHouse = await db.PublishingHouses.FindAsync(id);
                if (publishingHouse == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chi tiết nhà xuất bản";
                return View(publishingHouse);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("tao-moi")]
        public ActionResult Create()
        {
            ViewBag.SiteName = "Tạo mới nhà xuất bản";
            return View();
        }

        [Route("tao-moi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,name,description,email,phoneNumber,location")] PublishingHouse publishingHouse)
        {
            try
            {
                int check = await db.PublishingHouses.CountAsync(p => p.ID == publishingHouse.ID);
                if(check > 0)
                {
                    return View("_AdminError", model: "Dữ liệu bị trùng");
                }
                if (ModelState.IsValid)
                {
                    publishingHouse.createAt = DateTime.Now;
                    publishingHouse.updateAt = DateTime.Now;
                    db.PublishingHouses.Add(publishingHouse);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.SiteName = "Tạo mới nhà xuất bản";
                return View(publishingHouse);
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
                PublishingHouse publishingHouse = await db.PublishingHouses.FindAsync(id);
                if (publishingHouse == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chỉnh sửa nhà xuất bản";
                return View(publishingHouse);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,name,description,email,phoneNumber,location")] PublishingHouse publishingHouse)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PublishingHouse pub = await db.PublishingHouses.FindAsync(publishingHouse.ID);
                    pub.updateAt = DateTime.Now;
                    TryUpdateModel(pub, new string[] { "ID", "name", "description", "email", "phoneNumber", "location", "updateAt" });
                    await db.SaveChangesAsync();
                    return RedirectToAction("Detail", new { id = publishingHouse.ID });
                }
                ViewBag.SiteName = "Chỉnh sửa nhà xuất bản";
                return View(publishingHouse);
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
                PublishingHouse publishingHouse = await db.PublishingHouses.FindAsync(id);
                if (publishingHouse == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Xóa nhà xuất bản";
                return View(publishingHouse);
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
                PublishingHouse publishingHouse = await db.PublishingHouses.FindAsync(id);
                db.PublishingHouses.Remove(publishingHouse);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                long count = await db.Books.CountAsync(b => b.publishingHouseId == id);
                if (count > 0)
                {
                    return View("_AdminError", model: $"Không thể xóa được dữ liệu, do có {count} nội dung sử dụng tới thể loại này!");
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
