using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SachDienTu.Models
{
    [MetadataType(typeof(BookAuthor.BookAuthorMetadata))]
    public partial class BookAuthor
    {
        class BookAuthorMetadata
        {
            [Display(Name = "ID của tác giả")]
            public long ID { get; set; }

            [Display(Name = "Tên tác giả")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string name { get; set; }

            [Display(Name = "Địa chỉ Email")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [EmailAddress(ErrorMessage = "{0} không hợp lệ")]
            public string email { get; set; }

            [Display(Name = "Số điện thoại")]
            [Phone(ErrorMessage = "{0} không hợp lệ")]
            public string phoneNumber { get; set; }

            [Display(Name = "Mô tả về tác giả")]
            public string description { get; set; }

            [Display(Name = "Địa chỉ của tác giả")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string location { get; set; }

            [Display(Name = "Ngày tạo tác giả")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime createAt { get; set; }

            [Display(Name = "Ngày chỉnh sửa tác giả")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public Nullable<System.DateTime> updateAt { get; set; }
        }
    }

    [MetadataType(typeof(Book.BookMetadata))]
    public partial class Book
    {
        class BookMetadata
        {
            [Display(Name = "ID của sách")]
            public long ID { get; set; }

            [Display(Name = "Tên sách")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string name { get; set; }

            [Display(Name = "Giá bán")]
            [DataType(DataType.Currency, ErrorMessage = "{0} không hợp lệ")]
            public double price { get; set; }

            [Display(Name = "ID thể loại")]
            public long categoryId { get; set; }

            [Display(Name = "ID nhà xuất bản")]
            public long publishingHouseId { get; set; }

            [Display(Name = "Mô tả về sách")]
            [AllowHtml]
            public string description { get; set; }

            [Display(Name = "ID trạng thái của sách")]
            public int stateId { get; set; }

            [Display(Name = "URL của sách")]
            //[Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Url, ErrorMessage = "{0} không hợp lệ")]
            public string url { get; set; }

            [Display(Name = "Lượt xem sách")]
            public long views { get; set; }

            [Display(Name = "File PDF")]
            public string pdf { get; set; }

            [Display(Name = "Ngày tạo sách")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime createAt { get; set; }

            [Display(Name = "Ngày tạo sách")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public Nullable<System.DateTime> updateAt { get; set; }
        }
    }

    [MetadataType(typeof(Category.CategoryMetadata))]
    public partial class Category
    {
        class CategoryMetadata
        {
            [Display(Name = "ID của thể loại")]
            public long ID { get; set; }

            [Display(Name = "Tên thể loại")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string name { get; set; }

            [Display(Name = "Ngày tạo thể loại")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime createAt { get; set; }

            [Display(Name = "Ngày chỉnh sửa thể loại")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime updateAt { get; set; }
        }
    }

    [MetadataType(typeof(AuthorContribute.AuthorContributeMetadata))]
    public partial class AuthorContribute
    {
        class AuthorContributeMetadata
        {
            public long ID { get; set; }

            [Display(Name = "ID của sách")]
            public long bookId { get; set; }

            [Display(Name = "ID của tác giả")]
            public long authorId { get; set; }

            [Display(Name = "Vai trò")]
            public string role { get; set; }
        }
    }

    [MetadataType(typeof(ImageDistribution.ImageDistributionMetadata))]
    public partial class ImageDistribution
    {
        class ImageDistributionMetadata
        {
            public long ID { get; set; }

            [Display(Name = "ID của sách")]
            public Nullable<long> bookId { get; set; }

            [Display(Name = "ID của hình ảnh")]
            public long imageId { get; set; }
        }
    }

    [MetadataType(typeof(Image.ImageMetadata))]
    public partial class Image
    {
        class ImageMetadata
        {
            [Display(Name = "ID của hình ảnh")]
            public long ID { get; set; }

            [Display(Name = "Tên hình ảnh")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string name { get; set; }

            [Display(Name = "Mô tả hình ảnh")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string description { get; set; }

            [Display(Name = "URL hình ảnh")]
            [DataType(DataType.Url, ErrorMessage = "{0} không hợp lệ")]
            public string source { get; set; }

            [Display(Name = "Ngày tạo hình ảnh")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime uploadTime { get; set; }
        }
    }

    [MetadataType(typeof(InvoiceDetail.InvoiceDetailMetadata))]
    public partial class InvoiceDetail
    {
        class InvoiceDetailMetadata
        {
            [Display(Name = "ID của hóa đơn chi tiết")]
            public long invoiceId { get; set; }

            [Display(Name = "ID của sách")]
            public long bookId { get; set; }

            [Display(Name = "Đơn giá")]
            public double unitPrice { get; set; }

            [Display(Name = "Thành tiền")]
            public double intoMoney { get; set; }
        }
    }

    [MetadataType(typeof(Invoice.InvoiceMetadata))]
    public partial class Invoice
    {
        class InvoiceMetadata
        {
            [Display(Name = "ID của hóa đơn")]
            public long ID { get; set; }

            [Display(Name = "ID của khách hàng")]
            public string customerId { get; set; }

            [Display(Name = "Họ tên khách hàng")]
            [Required(ErrorMessage = "{0} không được để trống")]
            public string customerName { get; set; }

            [Display(Name = "Địa chỉ Email của khách hàng")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [EmailAddress(ErrorMessage = "{0} không hợp lệ")]
            public string email { get; set; }

            [Display(Name = "Ngày đặt hàng")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime timeOrder { get; set; }

            [Display(Name = "Phương thức thanh toán")]
            public string paymentMethod { get; set; }

            [Display(Name = "Tổng tiền của hóa đơn")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Currency, ErrorMessage = "{0} không hợp lệ")]
            public double totalMoney { get; set; }
        }
    }

    [MetadataType(typeof(PublishingHouse.PublishingHouseMetadata))]
    public partial class PublishingHouse
    {
        class PublishingHouseMetadata
        {
            [Display(Name = "ID của nhà xuất bản")]
            public long ID { get; set; }

            [Display(Name = "Tên của nhà xuất bản")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string name { get; set; }

            [Display(Name = "Mô tả của nhà xuất bản")]
            [DataType(DataType.Text, ErrorMessage = "{0} không hợp lệ")]
            public string description { get; set; }

            [Display(Name = "Email của nhà xuất bản")]
            [EmailAddress(ErrorMessage = "{0} không hơp lệ")]
            public string email { get; set; }

            [Display(Name = "Số điện thoại của nhà xuất bản")]
            [Phone(ErrorMessage = "{0} không hợp lệ")]
            public string phoneNumber { get; set; }

            [Display(Name = "Địa chỉ của nhà xuất bản")]
            public string location { get; set; }

            [Display(Name = "Ngày tạo nhà xuất bản")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime createAt { get; set; }

            [Display(Name = "Ngày chỉnh sửa nhà xuất bản")]
            [DataType(DataType.DateTime, ErrorMessage = "{0} không hợp lệ")]
            public System.DateTime updateAt { get; set; }
        }
    }

    [MetadataType(typeof(BookState.BookStateMetadata))]
    public partial class BookState
    {
        class BookStateMetadata
        {
            [Display(Name = "ID của trạng thái")]
            public int ID { get; set; }

            [Display(Name = "Trạng thái của sách")]
            public string name { get; set; }

            [Display(Name = "Mô tả về trạng thái")]
            public string description { get; set; }
        }
    }
}