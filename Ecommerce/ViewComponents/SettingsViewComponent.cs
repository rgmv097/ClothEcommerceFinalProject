using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class SettingsViewComponent:ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public SettingsViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settings = await _clothDbContext.Settings
                .FirstOrDefaultAsync();

            return View(settings);

        }
    }
}
