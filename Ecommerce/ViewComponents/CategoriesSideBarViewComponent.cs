using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class CategoriesSideBarViewComponent:ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public CategoriesSideBarViewComponent(ClothDbContext clotDbContext)
        {
            _clothDbContext = clotDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _clothDbContext.Categories
                .Where(p => p.IsStatus && p.isMain)
                .Include(p=>p.ProductCategories)
                .ThenInclude(p=>p.Product)
                .ToListAsync();

            return View(categories);
        }
    }
}
