using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class ProductCreateViewModel : BaseViewModel
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionTitle { get; set; }
        public string LongDescription { get; set; }
        public string Availability { get; set; }
        public string Sku { get; set; }
        public IFormFile[] Images { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public List<SelectListItem> ParentCategories { get; set; }
        public int ParentCategoryId { get; set; }
        public List<SelectListItem>? ChildCategories { get; set; }
        public int? ChildCategoryId { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductOptionCreateViewModel> ProductOptions { get; set; }
    }
}
