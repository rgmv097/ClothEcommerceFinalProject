using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ecommerce.ViewComponents
{
    public class CategoryPopupViewComponent : ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public CategoryPopupViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _clothDbContext
                .Categories
                .Where(c => c.IsStatus && c.isMain)
                .Include(s=>s.Children)
                .ToListAsync();

            return View(categories);
        }
    }
}
