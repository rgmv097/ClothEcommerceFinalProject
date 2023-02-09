using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class UserCreateViewModel
    {
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public IFormFile? Image { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Zipcode { get; set; }

        public string Role { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public List<SelectListItem>? Roles { get; set; }
    }
}
