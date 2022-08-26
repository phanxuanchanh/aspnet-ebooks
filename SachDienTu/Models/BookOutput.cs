using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SachDienTu.Models
{
    public class BookOutput
    {
        public Book book { get; set; }
        public Image image { get; set; }

        public List<Image> images { get; set; }

        public static List<BookOutput> GetBooks(List<BookOutput> bookOutput)
        {
            List<BookOutput> bookOutputs = new List<BookOutput>();
            foreach (BookOutput item in bookOutput)
            {
                bool check = bookOutputs.Any(b => b.book.name == item.book.name);
                if(check == false)
                {
                    bookOutputs.Add(new BookOutput
                    {
                        book = item.book,
                        images = bookOutput.Where(b => b.book.name == item.book.name)
                            .Select(b => new Image
                            {
                                ID = b.image.ID,
                                name = b.image.name,
                                source = b.image.source
                            }).ToList()
                    });
                }
            }
            return bookOutputs;
        }
    }
}