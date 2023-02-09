using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class CategoryCreateViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public string? Icon { get; set; }
        public bool IsMain { get; set; }
        public int ParentId { get; set; }
        public List<SelectListItem> ParentCategories { get; set; } = new();
    }
}
