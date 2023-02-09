using Ecommerce.BLL.ViewModels;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClothDbContext _clothDbContext;

        public HomeController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {

            var sliders = await _clothDbContext.Sliders
                .Where(x => x.IsStatus)
                .ToListAsync();

            var categories= await _clothDbContext.Categories
                .Where(c=>c.IsStatus)
                .Include(s=>s.Children)
                .ToListAsync();

            var products = await _clothDbContext.Products
                .Include(p => p.ProductOptions)
                .Include(c => c.ProductCategories)
                .ThenInclude(c => c.Category)
                .Include(i => i.ProductImages)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {

                Sliders= sliders,
                Categories= categories,
                Products= products
            };

            return View(viewModel);
        }
    }
}