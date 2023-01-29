using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class SlideUpdateViewModel:BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slogan { get; set; }
      
        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }
    }
}
