using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SachDienTu.Models;
using X.PagedList;
using SachDienTu.Common;

namespace SachDienTu.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quan-tri")]
    [RoutePrefix("hinh-anh")]
    [Authorize(Roles = "Super Admin, Admin")]
    public class ImageManagementController : Controller
    {
        private SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("tat-ca-hinh-anh/{page?}")]
        public async Task<ActionResult> Index(int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 10;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.Images.CountAsync();
                List<Image> onePageOfData = await db.Images
                    .OrderBy(p => p.ID).Skip(n).Take(pageSize).ToListAsync();
                StaticPagedList<Image> images = new StaticPagedList<Image>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.Images = images;
                ViewBag.SiteName = "Danh sách hình ảnh";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ImageListPartial");
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
                Image image = await db.Images.FindAsync(id);
                if (image == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chi tiết hình ảnh";
                return View(image);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("tao-moi")]
        public ActionResult Create()
        {
            return RedirectToAction("Upload");
        }

        [Route("chinh-sua/{id?}")]
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null || id < 1)
                return RedirectToAction("Index");
            try
            {
                Image image = await db.Images.FindAsync(id);
                if (image == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chỉnh sửa thông tin hình ảnh";
                return View(image);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,name,description")] Image image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Image img = await db.Images.FindAsync(image.ID);
                    TryUpdateModel(img, new string[] { "name", "description" });
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.SiteName = "Chỉnh sửa thông tin hình ảnh";
                return View(image);
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
                Image image = await db.Images.FindAsync(id);
                if (image == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Xóa hình ảnh";
                return View(image);
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
                Image image = await db.Images.FindAsync(id);
                db.Images.Remove(image);
                await db.SaveChangesAsync();

                string fullFileMapPath = Server.MapPath($"~/Photos/{image.source}");
                if (System.IO.File.Exists(fullFileMapPath))
                    System.IO.File.Delete(fullFileMapPath);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                long count = await db.ImageDistributions.CountAsync(i => i.imageId == id);
                if (count > 0)
                {
                    return View("_AdminError", model: $"Không thể xóa được dữ liệu, do có {count} nội dung sử dụng tới hình ảnh này!");
                }
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }

        }

        [Route("tai-len")]
        public ActionResult Upload()
        {
            ViewBag.SiteName = "Tải lên hình ảnh";
            return View();
        }

        [Route("tai-len")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase file, string name, string description = null)
        {
            try
            {
                if (file == null)
                {
                    ViewBag.UploadResult = "Không có tập tin tải lên";
                    return View();
                }

                string saveLocation = Server.MapPath("~/Photos/");
                string fileName = name.TextToUrl();
                Upload upload = new Upload(saveLocation, fileName);
                upload.FileUpload = file;
                UploadResult<string> uploadResult = upload.Complete();
                ViewBag.SiteName = "Tải lên hình ảnh";
                if (uploadResult.BoolStatus)
                {
                    ViewBag.UploadResult = uploadResult.StringStatus;
                    bool check = await db.Images.AnyAsync(i => i.name == name && i.source == uploadResult.Result);
                    if (check)
                    {
                        ViewBag.UploadResult = "Đã tồn tại hình này trong database";
                        return View();
                    }
                    Image image = new Image
                    {
                        name = name,
                        description = description,
                        source = uploadResult.Result.Substring(uploadResult.Result.LastIndexOf("/") + 1),
                        uploadTime = DateTime.Now
                    };
                    db.Images.Add(image);
                    await db.SaveChangesAsync();
                    return View();
                }
                ViewBag.UploadResult = uploadResult.StringStatus;
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
