using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class RecentPostViewComponent:ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public RecentPostViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = await _clothDbContext
                .Blogs
                .Where(c => c.IsStatus)
                .Take(3)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(blogs);
        }
    }
}
