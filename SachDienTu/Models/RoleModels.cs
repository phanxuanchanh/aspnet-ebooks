using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SachDienTu.Models
{
    public class RoleModels
    {
        public string ID { get; set; }

        [Display(Name = "Tên vai trò")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string roleName { get; set; }
    }
}