using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class ProductRelatedViewComponent:ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public ProductRelatedViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _clothDbContext
                .Products
                .Where(c => c.IsStatus)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(products);
        }
    }
}
