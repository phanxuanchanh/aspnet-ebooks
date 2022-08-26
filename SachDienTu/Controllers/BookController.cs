using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SachDienTu.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace SachDienTu.Controllers
{
    [RoutePrefix("sach")]
    public class BookController : Controller
    {
        SachDienTuDBContext db = new SachDienTuDBContext();

        public List<BookOutput> GetBookOutput(List<Book> books)
        {
            List<BookOutput> bookOutputs = new List<BookOutput>();
            foreach (Book item in books)
            {
                bool check = bookOutputs.Any(b => b.book.name == item.name);
                if (check == false)
                {
                    List<ImageDistribution> imageDistributions = db.ImageDistributions
                            .Include(i => i.Image)
                            .Where(i => i.bookId == item.ID)
                            .ToList();

                    List<Image> imgs = imageDistributions.Select(i => new Image
                    {
                        ID = i.Image.ID,
                        name = i.Image.name,
                        source = i.Image.source
                    }).ToList();

                    bookOutputs.Add(new BookOutput
                    {
                        book = item,
                        images = imgs
                    });
                }
            }
            return bookOutputs;
        }

        [Route("xem-chi-tiet/{url}/{id}")]
        [Authorize]
        public async Task<ActionResult> Detail(long id, string url)
        {
            try
            {
                bool check = await db.Books.AnyAsync(b => b.ID == id);
                if (!check)
                {
                    return RedirectToAction("Index", "Home");
                }
                Book book = await db.Books.Include(b => b.Category).SingleOrDefaultAsync(b => b.ID == id);
                book.views += 1;
                book.description = HttpUtility.HtmlDecode(book.description);
                TryUpdateModel(book, new string[] { "views" });
                await db.SaveChangesAsync();
                List<ImageDistribution> imageDistributions = await db.ImageDistributions
                   .Include(i => i.Image).Where(c => c.bookId == id).ToListAsync();

                string isEnableAccess = "disable";
                string username = User.Identity.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    using (ApplicationDbContext appDb = new ApplicationDbContext())
                    {
                        UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appDb));
                        ApplicationUser applicationUser = await userManager.FindByNameAsync(username);

                        if (await db.BookDistributions
                            .AnyAsync(b => b.bookId == book.ID && b.customerId == applicationUser.Id))
                            isEnableAccess = "enable";
                    }
                }
                ViewBag.IsEnableAccess = isEnableAccess;
                ViewBag.ImageDistributions = imageDistributions;
                ViewBag.SiteName = book.name;
                return View(book);
            }
            catch (Exception)
            {
                return View("_Error", model: "Không thể truy cập dữ liệu");
            }
        }

        [Route("chia-se-voi-facebook/{url}/{id}")]
        public async Task<ActionResult> ShareWithFacebook(long id, string url)
        {
            try
            {
                bool check = await db.Books.AnyAsync(b => b.ID == id);
                if (!check)
                    return RedirectToAction("Index", "Home");
                Book book = await db.Books.SingleOrDefaultAsync(b => b.ID == id);
                string fbLink = "https://www.facebook.com/sharer/sharer.php";
                string link = HttpUtility.UrlEncode(Url.Action("Detail", "Book", new { id = id, url = url }, Request.Url.Scheme));
                string title = HttpUtility.UrlEncode(book.name);
                return Redirect($"{fbLink}?u={link}&t={title}");
            }
            catch (Exception)
            {
                return View("_Error", model: "Không thể truy cập dữ liệu");
            }
        }

        [Route("xem-sach-truc-tuyen/{url}/{id}")]
        public async Task<ActionResult> WatchOnline(long id, string url)
        {
            try
            {
                bool check = await db.Books.AnyAsync(b => b.ID == id);
                if (!check)
                    return RedirectToAction("Index", "Home");
                Book book = await db.Books.SingleOrDefaultAsync(b => b.ID == id);
                if (string.IsNullOrEmpty(book.pdf))
                    book.pdf = "default.pdf";

                return View(book);
            }
            catch (Exception)
            {
                return View("_Error", model: "Không thể truy cập dữ liệu");
            }
        }

        [Route("tat-ca-sach")]
        public ActionResult GetBooks()
        {
            try
            {
                List<BookOutput> bookOutputs_raw = db.ImageDistributions
                    .Include(i => i.Book).Include(i => i.Image)
                    .OrderByDescending(i => i.Book.views)
                    .Select(i => new BookOutput
                    {
                        book = i.Book,
                        image = i.Image
                    }).ToList();

                List<BookOutput> bookOutputs = BookOutput.GetBooks(bookOutputs_raw);
                ViewBag.SiteName = "Xem tất cả sách";
                return View(bookOutputs);
            }
            catch (Exception)
            {
                return View("_Error", model: "Lỗi");
            }
        }

        [Route("sach-da-mua/{page?}")]
        [Authorize]
        public async Task<ActionResult> GetMyBooks(int? page)
        {
            try
            {
                string username = User.Identity.Name;
                using (ApplicationDbContext appDb = new ApplicationDbContext())
                {
                    UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appDb));
                    ApplicationUser applicationUser = await userManager.FindByNameAsync(username);

                    int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                    int pageSize = 10;
                    int n = (pageNumber - 1) * pageSize;
                    int totalItemCount = await (from b in db.Books
                                                join bd in db.BookDistributions
                                                on b.ID equals bd.bookId
                                                where bd.customerId == applicationUser.Id
                                                orderby b.views descending
                                                select b).CountAsync();

                    List<Book> books = await (from b in db.Books
                                              join bd in db.BookDistributions
                                              on b.ID equals bd.bookId
                                              where bd.customerId == applicationUser.Id
                                              orderby b.views descending
                                              select b)
                        .Skip(n).Take(pageSize).ToListAsync();

                    List<BookOutput> onePageOfData = GetBookOutput(books);
                    StaticPagedList<BookOutput> bookOutputs = new StaticPagedList<BookOutput>(onePageOfData, pageNumber, pageSize, totalItemCount);
                    ViewBag.BookOutputs = bookOutputs;
                    ViewBag.SiteName = "Xem tất cả sách mà bạn đã mua";

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_MyBookListPartial");
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                return View("_Error", model: "Lỗi");
            }
        }

        [Route("sach-theo-luot-xem/{page?}")]
        public async Task<ActionResult> BooksByView(int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 10;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.Books.CountAsync();
                List<Book> books = await db.Books.OrderByDescending(p => p.views).Skip(n).Take(pageSize).ToListAsync();
                List<BookOutput> onePageOfData = GetBookOutput(books);
                StaticPagedList<BookOutput> bookOutputs = new StaticPagedList<BookOutput>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.BookOutputs = bookOutputs;
                ViewBag.SiteName = "Xem tất cả sách dựa theo xếp hạng lượt xem";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_BookListPartial");
                }
                return View();
            }
            catch (Exception)
            {
                return View("_Error", model: "Lỗi");
            }
        }

        [ChildActionOnly]
        public PartialViewResult _BooksByView(int? bookNumber)
        {
            try
            {
                int bkNumber = (bookNumber == null || bookNumber < 1) ? 5 : bookNumber.Value;
                List<Book> books = db.Books.OrderByDescending(b => b.views).Take(bkNumber).ToList();
                List<BookOutput> bookOutputs = GetBookOutput(books);
                return PartialView(bookOutputs);
            }
            catch (Exception)
            {
                return PartialView("_ErrorPartial");
            }
        }


        [ChildActionOnly]
        public PartialViewResult _Latest(int? bookNumber)
        {
            try
            {
                int bkNumber = (bookNumber == null || bookNumber < 1) ? 4 : bookNumber.Value;

                List<BookOutput> bookOutputs = new List<BookOutput>();
                List<Book> books = db.Books.OrderByDescending(b => b.createAt).Take(bkNumber).ToList();
                foreach (Book item in books)
                {
                    bool check = bookOutputs.Any(b => b.book.name == item.name);
                    if (check == false)
                    {
                        List<ImageDistribution> imageDistributions = db.ImageDistributions
                                .Include(i => i.Image)
                                .Where(i => i.bookId == item.ID)
                                .ToList();

                        List<Image> imgs = imageDistributions.Select(i => new Image
                        {
                            ID = i.Image.ID,
                            name = i.Image.name,
                            source = i.Image.source
                        }).ToList();

                        bookOutputs.Add(new BookOutput
                        {
                            book = item,
                            images = imgs
                        });
                    }
                }
                return PartialView(bookOutputs);
            }
            catch (Exception)
            {
                return PartialView("_ErrorPartial");
            }
        }

        [Route("sach-theo-the-loai/{id}")]
        public async Task<ActionResult> ListByCategory(int id, int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 10;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.Books.Where(b => b.categoryId == id).CountAsync();
                List<Book> books = await db.Books.Where(b => b.categoryId == id).OrderBy(b => b.name).Skip(n).Take(pageSize).ToListAsync();
                List<BookOutput> onePageOfData = GetBookOutput(books);
                StaticPagedList<BookOutput> bookOutputs = new StaticPagedList<BookOutput>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.BookOutputs = bookOutputs;
                Category category = await db.Categories.FindAsync(id);
                ViewBag.ID = id;
                ViewBag.SiteName = $"Xem tất cả sách thuộc thể loại {category.name}";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ListByCategoryPartial");
                }
                return View();
            }
            catch (Exception)
            {
                return View("_Error", model: "Lỗi");
            }
        }

        [ChildActionOnly]
        public PartialViewResult _ListByCategory(int id, int? bookNumber, string mode = null)
        {
            try
            {
                int bkNumber = (bookNumber == null || bookNumber < 1) ? 10 : bookNumber.Value;
                bool check1 = db.Categories.Any(c => c.ID == id);
                if (!check1)
                {
                    return PartialView();
                }
                List<Book> books = db.Books.Where(b => b.categoryId == id).Take(bkNumber).ToList();
                List<BookOutput> bookOutputs = GetBookOutput(books);
                ViewBag.CategoryName = db.Categories.Find(id).name;
                ViewBag.CategoryId = id;
                ViewBag.Mode = mode;
                return PartialView(bookOutputs);
            }
            catch (Exception)
            {
                return PartialView("_ErrorPartial");
            }
        }

        [Route("tim-kiem")]
        public async Task<ActionResult> Search(string keyword)
        {
            try
            {
                List<Book> books = await db.Books.Where(b => b.name.Contains(keyword)).ToListAsync();

                List<BookOutput> bookOutputs = GetBookOutput(books);
                if (bookOutputs.Count == 0)
                {
                    return View(model: "Không tìm thấy dữ liệu mà bạn yêu cầu");
                }
                ViewBag.SiteName = $"Tìm kiếm với từ khóa \"{keyword}\"";
                return View(bookOutputs);
            }
            catch (Exception)
            {
                return View("_Error", model: "Lỗi");
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