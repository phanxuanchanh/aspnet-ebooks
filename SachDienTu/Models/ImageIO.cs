using System.ComponentModel.DataAnnotations;

namespace SachDienTu.Models
{
    public class ImageIO
    {
        public long bookId { get; set; }

        [Display(Name = "Tên của sách")]
        public string bookName { get; set; }

        [Display(Name = "Hình ảnh")]
        public string imagesId { get; set; }
    }
}