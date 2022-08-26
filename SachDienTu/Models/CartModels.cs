using System.Collections.Generic;
using System.Linq;

namespace SachDienTu.Models
{
    public class CartItem
    {

        public Book book { get; set; }

        public CartItem() { }
        public CartItem(Book book, int bookNumber)
        {
            this.book = book;
        }
    }

    public class CartModel
    {
        private List<CartItem> _List = new List<CartItem>();
        public List<CartItem> List => _List;

        public void Add(CartItem item)
        {
            var cartItem = _List.Find(p => p.book.ID == item.book.ID);
            if (cartItem == null)
                _List.Add(item);
        }

        public void Edit(int id, int bookNumber)
        {
            var edit = _List.Find(p => p.book.ID == id);
        }

        public void Delete(int id)
        {
            var delete = _List.Find(p => p.book.ID == id);
            _List.Remove(delete);
        }
        public void DeleteAll()
        {
            _List.Clear();
        }

        public int TotalProduct
        {
            get { return _List.Count; }
        }

        public double TotalMoney
        {
            get
            {
                double kq = 0;
                kq = _List.Sum(p => p.book.price);
                return kq;
            }
        }
    }
}