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
    public class CategoryUpdateViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public IFormFile? Icon { get; set; }
        public string? IconUrl { get; set; }
        public bool IsMain { get; set; }
        public int? ParentId { get; set; }
        public List<SelectListItem> ParentCategories { get; set; } = new();
        public List<Category> Children { get; set; } = new List<Category>();
    }
}
