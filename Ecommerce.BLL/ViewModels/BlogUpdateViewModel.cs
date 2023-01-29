using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class BlogUpdateViewModel:BaseViewModel
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
    }
}
