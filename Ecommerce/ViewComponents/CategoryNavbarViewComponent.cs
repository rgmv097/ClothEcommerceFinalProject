using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class CategoryNavbarViewComponent : ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public CategoryNavbarViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories= await _clothDbContext.Categories
                .Where(c=>c.isMain && c.IsStatus)
                .Include(c=>c.Children)
                .Include(p=>p.ProductCategories)
                .ThenInclude(p=>p.Product)
                .ToListAsync();
                

            return View(categories);
        }
    }
}
