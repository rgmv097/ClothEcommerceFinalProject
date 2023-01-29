using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ecommerce.BLL.ViewModels
{
    public class SliderCreateViewModel:BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slogan { get; set; }

        public IFormFile Image { get; set; }

    }
}
