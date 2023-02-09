using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class ShopController : Controller
    {
        private readonly ClothDbContext _clothDbContext;

        public ShopController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _clothDbContext.Products
                .Include(p => p.ProductOptions)
                .Include(c => c.ProductCategories)
                .ThenInclude(c => c.Category)
                .Include(i => i.ProductImages)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var product = await _clothDbContext.Products
               .Where(p => p.Id == id)
               .Include(p => p.ProductOptions)
               .Include(c => c.ProductCategories)
               .ThenInclude(c => c.Category)
               .Include(i => i.ProductImages)
               .FirstOrDefaultAsync();

            return View(product);
        }
    }
}
