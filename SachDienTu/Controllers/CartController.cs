using System;
using System.Web.Mvc;
using SachDienTu.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Text;

namespace SachDienTu.Controllers
{
    [RoutePrefix("gio-hang")]
    public class CartController : Controller
    {
        SachDienTuDBContext db = new SachDienTuDBContext();

        [Route("cac-san-pham")]
        [Authorize]
        public ActionResult Index()
        {
            var cart = Session["cart"] as CartModel;
            ViewBag.ShoppingCartAct = "active";
            ViewBag.cart = cart;
            ViewBag.SiteName = "Danh sách giỏ hàng";
            if (Request.IsAjaxRequest())
            {
                return PartialView("_IndexPartial");
            }
            return View();
        }

        [Route("them-vao-gio-hang")]
        [HttpPost]
        [Authorize]
        public ActionResult AddToCart(int bookId, int bookNumber = 1)
        {
            var cart = Session["cart"] as CartModel;
            if (cart == null)
            {
                cart = new CartModel();
                Session["cart"] = cart;
            }
            Book book = db.Books.Find(bookId);
            var item = new CartItem(book, bookNumber);
            cart.Add(item);
            return RedirectToAction("Index");
        }

        [Route("xoa-san-pham")]
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int bookId)
        {
            var cart = Session["cart"] as CartModel;
            cart.Delete(bookId);
            return RedirectToAction("Index");
        }

        [Route("dat-hang")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Order()
        {
            try
            {
                ViewBag.SiteName = "Đặt hàng";
                string username = User.Identity.Name;
                Invoice invoice;
                using (ApplicationDbContext appDb = new ApplicationDbContext())
                {
                    UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appDb));
                    ApplicationUser applicationUser = await userManager.FindByNameAsync(username);
                    invoice = new Invoice
                    {
                        customerId = applicationUser.Id,
                        customerName = $"{applicationUser.LastName} {applicationUser.FirstName}",
                        email = applicationUser.Email,
                    };
                }
                return View(invoice);
            }
            catch (Exception)
            {
                return View("_Error", model: "Đã có lỗi xảy ra");
            }
        }

        [Route("dat-hang")]
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Order([Bind(Include = "ID,email,customerId,customerName")] Invoice invoice)
        {
            var cart = Session["cart"] as CartModel;
            if (cart == null || cart.TotalProduct == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    invoice.timeOrder = DateTime.Now;
                    invoice.totalMoney = cart.TotalMoney;
                    db.Invoices.Add(invoice);
                    StringBuilder invoiceDetailStrBuliders = new StringBuilder("\nDanh sách sản phẩm:\n");
                    foreach (var item in cart.List)
                    {
                        InvoiceDetail invoiceDetail = new InvoiceDetail
                        {
                            invoiceId = invoice.ID,
                            bookId = item.book.ID,
                            unitPrice = item.book.price,
                            intoMoney = item.book.price
                        };
                        db.InvoiceDetails.Add(invoiceDetail);

                        BookDistribution bookDistribution = new BookDistribution
                        {
                            customerId = invoice.customerId,
                            bookId = item.book.ID
                        };
                        db.BookDistributions.Add(bookDistribution);

                        invoiceDetailStrBuliders.Append($"\t+ Tên sách: \"{item.book.name}\"; ");
                        invoiceDetailStrBuliders.Append($"Giá: {item.book.price.ToString("#, ##0 VNĐ")}; ");
                        invoiceDetailStrBuliders.Append($"Thành tiền: {item.book.price.ToString("#, ##0 VNĐ")}\n");
                    }
                    await db.SaveChangesAsync();
                    cart.DeleteAll();

                    EmailService emailService = new EmailService();
                    IdentityMessage identityMessage = new IdentityMessage();
                    identityMessage.Destination = invoice.email;
                    identityMessage.Subject = "Xác nhận sách đã mua";

                    StringBuilder emailStrBuilder = new StringBuilder("HÓA ĐƠN MUA SÁCH ĐIỆN TỬ\n\n");
                    emailStrBuilder.Append(invoiceDetailStrBuliders.ToString());
                    emailStrBuilder.Append($"\nTên khách hàng: {invoice.customerName}\n");
                    emailStrBuilder.Append($"Email: {invoice.email}\n");
                    emailStrBuilder.Append($"\nThời gian: {invoice.timeOrder}\n");
                    emailStrBuilder.Append($"Tổng tiền: {invoice.totalMoney.ToString("#, ##0 VNĐ")}");
                    identityMessage.Body = emailStrBuilder.ToString();
                    await emailService.SendAsync(identityMessage);

                    ViewBag.SiteName = "Đặt mua thành công";
                    return View("OrderSuccess", invoice);
                }
                ViewBag.SiteName = "Đặt hàng";
                return View();
            }
            catch (Exception ex)
            {
                TempData["LoiDatHang"] = "Đặt mua không thành công.<br>" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [ChildActionOnly]
        public int TotalProduct()
        {
            var cart = Session["cart"] as CartModel;
            if (cart == null)
                return 0;
            return cart.TotalProduct;
        }
    }
}