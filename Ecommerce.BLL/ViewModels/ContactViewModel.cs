using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class ContactViewModel
    {
        public ContactMessageViewModel ContactMessage { get; set; } = new();
    }
    public class ContactMessageViewModel
    {
        public string Name { get; set; }
        public string? Image { get; set; }
        public string? Subject { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
