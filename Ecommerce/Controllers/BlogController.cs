using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    
    public class BlogController : Controller
    {
        private readonly ClothDbContext _clothDbContext;

        public BlogController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _clothDbContext.Blogs
                .ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var blog=await _clothDbContext.Blogs
                .Where(b=>b.Id== id)
                .FirstOrDefaultAsync();

            return View(blog);
        }
    }
}
