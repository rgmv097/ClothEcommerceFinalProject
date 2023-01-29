using Ecommerce.BLL.ViewModels;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.Services
{
    public class CategoryService
    {
        private readonly ClothDbContext _clothDbContext;

        public CategoryService(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<ProductCreateViewModel> GetCategories()
        {
            var categories = await _clothDbContext.Categories
                .Where(c => c.IsStatus && c.isMain)
                .Include(c => c.Children)
                .ToListAsync();

            var parentCategoriesSelectListItems = new List<SelectListItem>();
            var childCategoriesSelectListItems = new List<SelectListItem>();

            categories.ForEach(c => parentCategoriesSelectListItems
            .Add(new SelectListItem(c.Name, c.Id.ToString())));

            categories[0].Children.ToList()
                .ForEach(c => childCategoriesSelectListItems
          .Add(new SelectListItem(c.Name, c.Id.ToString())));

            var model = new ProductCreateViewModel
            {
                ParentCategories = parentCategoriesSelectListItems,
                ChildCategories = childCategoriesSelectListItems
            };
            return model;
        }
    }
}
