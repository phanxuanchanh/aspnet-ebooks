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
    [RoutePrefix("sach")]
    [Authorize(Roles = "Super Admin, Admin")]
    public class BookManagementController : Controller
    {
        private SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("tat-ca-sach/{page?}")]
        [Authorize(Roles = "Super Admin, Admin, SEOer")]
        public async Task<ActionResult> Index(int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 10;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.Books.CountAsync();
                List<Book> onePageOfData = await db.Books
                    .Include(b => b.Category).Include(b => b.PublishingHouse).Include(b => b.BookState)
                    .OrderBy(p => p.ID).Skip(n).Take(pageSize).ToListAsync();
                StaticPagedList<Book> books = new StaticPagedList<Book>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.Books = books;
                ViewBag.SiteName = "Danh sách sách";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_BookListPartial");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xem-chi-tiet/{id?}")]
        [Authorize(Roles = "Super Admin, Admin, SEOer")]
        public async Task<ActionResult> Detail(long? id)
        {
            if (id == null || id < 1)
                return RedirectToAction("Index");
            try
            {
                Book book = await db.Books.Include(b => b.Category).Include(b => b.PublishingHouse).SingleOrDefaultAsync(b => b.ID == id);
                if (book == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                List<AuthorContribute> authorContributes = await db.AuthorContributes
                   .Include(c => c.BookAuthor).Where(c => c.bookId == id).ToListAsync();
                List<ImageDistribution> imageDistributions = await db.ImageDistributions
                   .Include(i => i.Image).Where(c => c.bookId == id).ToListAsync();
                ViewBag.ImageDistributions = imageDistributions;
                ViewBag.AuthorContributes = authorContributes;
                ViewBag.SiteName = "Chi tiết sách";
                book.description = HttpUtility.HtmlDecode(book.description);
                return View(book);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("tao-moi")]
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.Categories, "ID", "name");
            ViewBag.publishingHouseId = new SelectList(db.PublishingHouses, "ID", "name");
            ViewBag.bookStateId = new SelectList(db.BookStates, "ID", "name");
            ViewBag.SiteName = "Tạo mới sách";
            return View();
        }

        [Route("tao-moi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,name,price,categoryId,publishingHouseId,description,stateId")] Book book)
        {
            try
            {
                int check = await db.Books.CountAsync(b => b.name == book.name);
                if (check > 0)
                {
                    return View("_AdminError", model: "Dữ liệu bị trùng");
                }
                if (ModelState.IsValid)
                {
                    book.createAt = DateTime.Now;
                    book.updateAt = DateTime.Now;
                    book.views = 0;
                    book.url = book.name.TextToUrl();
                    db.Books.Add(book);
                    await db.SaveChangesAsync();
                    return RedirectToAction("AddAuthor", new { id = book.ID });
                }
                ViewBag.categoryId = new SelectList(db.Categories, "ID", "name", book.categoryId);
                ViewBag.publishingHouseId = new SelectList(db.PublishingHouses, "ID", "name", book.publishingHouseId);
                ViewBag.bookStateId = new SelectList(db.BookStates, "ID", "name", book.stateId);
                ViewBag.SiteName = "Tạo mới sách";
                return View(book);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("them-pdf/{id?}")]
        public async Task<ActionResult> AddPdf(long? id)
        {
            if (id == null || id < 0)
                return RedirectToAction("Index");
            try
            {
                Book book = await db.Books.SingleOrDefaultAsync(b => b.ID == id);
                if (book == null)
                    return View("_AdminError", model: "Not Found");

                ViewBag.SiteName = "Thêm pdf vào sách";
                return View(book);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("them-pdf/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPdf(long id, HttpPostedFileBase file)
        {
            try
            {
                Book book = await db.Books.SingleOrDefaultAsync(b => b.ID == id);
                if (book == null)
                    return View("_AdminError", model: "Not Found");

                ViewBag.SiteName = "Thêm pdf vào sách";
                if (file == null)
                {
                    ViewBag.Result = "Không có tập tin tải lên";
                    return View(book);
                }

                FileUpload fileUpload = new FileUpload();
                string filePath = null;
                FileUpload.UploadState uploadState = fileUpload.UploadPDF(file, ref filePath);
                string uploadResult;
                if (uploadState == FileUpload.UploadState.Success)
                {
                    book.updateAt = DateTime.Now;
                    book.pdf = filePath;
                    TryUpdateModel(book, new string[] { "pdf" });
                    await db.SaveChangesAsync();
                    uploadResult = "Đã tải lên file PDF thành công";
                }
                else if (uploadState == FileUpload.UploadState.Failed_InvalidFile)
                {
                    uploadResult = "Đã tải lên thất bại, lý do định dạng file không hợp lệ";
                }
                else if (uploadState == FileUpload.UploadState.Failed_AlreadyExist)
                {
                    uploadResult = "Đã tải lên thất bại, lý do đã tồn tại file này trên hệ thống";
                }
                else
                {
                    uploadResult = "Đã tải lên thất bại, không rõ nguyên nhân";
                }
                ViewBag.Result = uploadResult;
                return View(book);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa-pdf/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePdf(long id)
        {
            try
            {
                Book book = await db.Books.SingleOrDefaultAsync(b => b.ID == id);
                if (book == null)
                    return View("_AdminError", model: "Not Found");

                book.updateAt = DateTime.Now;

                FileUpload fileUpload = new FileUpload();
                fileUpload.RemovePDF(book.pdf);
                book.pdf = null;
                TryUpdateModel(book, new string[] { "pdf" });
                await db.SaveChangesAsync();

                TempData["deletePdf"] = "Đã xóa file PDF thành công";
                return RedirectToAction("AddPdf");
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("them-tac-gia/{id?}")]
        public async Task<ActionResult> AddAuthor(long? id)
        {
            if (id == null || id < 0)
                return RedirectToAction("Index");
            try
            {
                AuthorContributeIO authorContributeIO = await db.Books.Where(b => b.ID == id)
                    .Select(b => new AuthorContributeIO
                    {
                        bookId = b.ID,
                        bookName = b.name,
                        authorId = 0,
                        role = null
                    }).SingleOrDefaultAsync();
                if (authorContributeIO == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.bookAuthorId = new SelectList(db.BookAuthors, "ID", "name");
                ViewBag.SiteName = "Thêm tác giả vào sách";
                return View(authorContributeIO);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("them-tac-gia/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAuthor([Bind(Include = "ID, bookName, bookId, authorId, role")] AuthorContributeIO authorContributeIO)
        {
            try
            {
                int check = await db.AuthorContributes.CountAsync(c => c.bookId == authorContributeIO.bookId && c.authorId == authorContributeIO.authorId);
                if (check > 0)
                {
                    TempData["addAuthor"] = $"Đã tồn tại tác giả với vai trò {authorContributeIO.role} trong sách {authorContributeIO.bookName}";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        AuthorContribute authorContribute = new AuthorContribute
                        {
                            bookId = authorContributeIO.bookId,
                            authorId = authorContributeIO.authorId,
                            role = authorContributeIO.role
                        };
                        db.AuthorContributes.Add(authorContribute);
                        await db.SaveChangesAsync();
                        TempData["addAuthor"] = $"Đã thêm thành công tác giả vào sách: {authorContributeIO.bookName}";
                    }
                    if (!ModelState.IsValid)
                    {
                        TempData["addAuthor"] = $"Thêm thất bại tác giả vào sách: {authorContributeIO.bookName}";
                    }
                }
                ViewBag.bookAuthorId = new SelectList(db.BookAuthors, "ID", "name");
                ViewBag.SiteName = "Thêm tác giả vào sách";
                return View(authorContributeIO);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua-tac-gia/{id?}")]
        public async Task<ActionResult> EditAuthor(long? id)
        {
            if (id == null || id < 0)
                return RedirectToAction("Index");
            try
            {
                Book book = await db.Books.FindAsync(id);
                if (book == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                List<AuthorContribute> authorContributes = await db.AuthorContributes
                  .Include(c => c.BookAuthor).Where(c => c.bookId == id).ToListAsync();
                ViewBag.bookName = book.name;
                ViewBag.SiteName = "Chỉnh sửa tác giả của sách";
                return View(authorContributes);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua-tac-gia/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAuthor(AuthorContribute authorContribute, string authorName)
        {
            try
            {
                Book book = await db.Books.FindAsync(authorContribute.bookId);
                if (ModelState.IsValid)
                {
                    db.Entry(authorContribute).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["editAuthor"] = $"Chỉnh sửa thành công cho sách {book.name} và tác giả {authorName} với vai trò {authorContribute.role}";
                    return RedirectToAction("EditAuthor", new { id = book.ID });
                }
                List<AuthorContribute> authorContributes = await db.AuthorContributes
                  .Include(ac => ac.BookAuthor).Where(ac => ac.bookId == authorContribute.bookId).ToListAsync();
                TempData["editAuthor"] = "Chỉnh sửa thất bại";
                ViewBag.bookName = book.name;
                ViewBag.SiteName = "Chỉnh sửa tác giả của sách";
                return View(authorContributes);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa-tac-gia/{bookId?}/{authorId?}")]
        public async Task<ActionResult> DeleteAuthor(int? bookId, int? authorId)
        {
            if ((bookId == null || bookId < 1) && (authorId == null || authorId < 1))
                return RedirectToAction("EditAuthor");
            try
            {
                AuthorContribute authorContribute = await db.AuthorContributes.SingleOrDefaultAsync(ac => ac.bookId == bookId && ac.authorId == authorId);
                if (authorContribute == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                db.AuthorContributes.Remove(authorContribute);
                await db.SaveChangesAsync();
                return RedirectToAction("EditAuthor", new { id = bookId });
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("them-hinh-anh/{id?}")]
        public async Task<ActionResult> AddImage(long? id)
        {
            if (id == null || id < 0)
                return RedirectToAction("Index");
            try
            {
                ImageIO imageIO = await db.Books.Where(b => b.ID == id)
                    .Select(b => new ImageIO
                    {
                        bookId = b.ID,
                        bookName = b.name
                    }).SingleOrDefaultAsync();
                if (imageIO == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.Images = await db.Images.ToListAsync();
                ViewBag.SiteName = "Thêm hình ảnh vào sách";
                return View(imageIO);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("them-hinh-anh/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddImage(ImageIO imageIO)
        {
            try
            {
                string status = null;
                if (ModelState.IsValid)
                {
                    string[] imagesId = imageIO.imagesId.Split(';');
                    foreach (string imageId in imagesId)
                    {
                        long id = int.Parse(imageId);
                        bool check = await db.ImageDistributions.AnyAsync(i => i.bookId == imageIO.bookId && i.imageId == id);
                        string imageName = db.Images.Find(id).name;
                        //string bookName = db.Books.Find(imageIO.bookId).name;
                        if (!check)
                        {
                            ImageDistribution imageDistribution = new ImageDistribution
                            {
                                bookId = imageIO.bookId,
                                imageId = id
                            };
                            db.ImageDistributions.Add(imageDistribution);
                            await db.SaveChangesAsync();
                            status += $"Thêm thành công :{imageName} -- ";
                        }
                        else
                        {
                            status += $"Đã tồn tại: {imageName} -- ";
                        }
                    }
                    TempData["addImage"] = status;
                    return RedirectToAction("AddImage");
                }
                TempData["addImage"] = "Thêm ảnh thất bại";
                return RedirectToAction("AddImage");
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("hinh-anh-duoc-dung/{id?}")]
        [Authorize(Roles = "Super Admin, Admin, SEOer")]
        public async Task<ActionResult> ImagesUsedInBook(long? id)
        {
            if (id == null || id < 0)
                return RedirectToAction("Index");
            try
            {
                Book book = await db.Books.FindAsync(id);
                if (book == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                List<ImageDistribution> imageDistributions = await db.ImageDistributions
                    .Include(i => i.Image).Where(i => i.bookId == id).ToListAsync();
                ViewBag.bookName = book.name;
                ViewBag.SiteName = "Hình ảnh đã được sử dụng trong sách";
                return View(imageDistributions);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("xoa-hinh-anh")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteImage(long? bookId, long? imageId)
        {
            if ((bookId == null || bookId < 1) && (imageId == null || imageId < 1))
                return RedirectToAction("Index");

            try
            {
                ImageDistribution imageDistribution = await db.ImageDistributions.SingleOrDefaultAsync(i => i.bookId == bookId && i.imageId == imageId);
                if (imageDistribution == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                db.ImageDistributions.Remove(imageDistribution);
                await db.SaveChangesAsync();
                return RedirectToAction("ImagesUsedInBook", new { id = bookId });
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        [Authorize(Roles = "Super Admin, Admin, SEOer")]
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null || id < 1)
                return RedirectToAction("Index");
            try
            {
                Book book = await db.Books.FindAsync(id);
                if (book == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Chỉnh sửa sách";
                ViewBag.categoryId = new SelectList(db.Categories, "ID", "name", book.categoryId);
                ViewBag.publishingHouseId = new SelectList(db.PublishingHouses, "ID", "name", book.publishingHouseId);
                ViewBag.bookStateId = new SelectList(db.BookStates, "ID", "name", book.stateId);
                book.description = HttpUtility.HtmlDecode(book.description);
                return View(book);
            }
            catch (Exception ex)
            {
                return View("_AdminError", model: $"Lỗi: {ex.Message}");
            }
        }

        [Route("chinh-sua/{id?}")]
        [Authorize(Roles = "Super Admin, Admin, SEOer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,name,price,categoryId,publishingHouseId,description,stateId")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Book bk = await db.Books.FindAsync(book.ID);
                    bk.updateAt = DateTime.Now;
                    bk.description = HttpUtility.HtmlEncode(book.description);
                    TryUpdateModel(bk, new string[] { "name", "price", "categoryId", "publishingHouseId", "description", "stateId" });
                    await db.SaveChangesAsync();
                    return RedirectToAction("Detail", new { id = book.ID });
                }
                ViewBag.SiteName = "Chỉnh sửa sách";
                ViewBag.categoryId = new SelectList(db.Categories, "ID", "name", book.categoryId);
                ViewBag.publishingHouseId = new SelectList(db.PublishingHouses, "ID", "name", book.publishingHouseId);
                ViewBag.bookStateId = new SelectList(db.BookStates, "ID", "name", book.stateId);
                return View(book);
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
                Book book = await db.Books.Include(b => b.Category).Include(b => b.PublishingHouse).SingleOrDefaultAsync(b => b.ID == id);
                if (book == null)
                {
                    return View("_AdminError", model: "Not Found");
                }
                ViewBag.SiteName = "Xóa sách";
                return View(book);
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
                Book book = await db.Books.FindAsync(id);
                db.Books.Remove(book);

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
