using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class BestSellerViewComponent:ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public BestSellerViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _clothDbContext
                .Products
                .Where(c => c.IsStatus)
                .Take(3)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(products);
        }
    }
}
