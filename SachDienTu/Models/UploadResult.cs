using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SachDienTu.Models
{
    public class UploadResult<T>
    {
        public bool BoolStatus { get; set; }
        public string StringStatus { get; set; }
        public T Result { get; set; }
    }
}