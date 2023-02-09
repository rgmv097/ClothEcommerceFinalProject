using Ecommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders = new List<Slider>();
        public List<Category> Categories = new List<Category>();
        public List<Product> Products = new List<Product>();
    }
}
