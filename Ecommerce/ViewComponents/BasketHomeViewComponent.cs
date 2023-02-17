using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class BasketHomeViewComponent:ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _userManager;

        public BasketHomeViewComponent(ClothDbContext clothDbContext, UserManager<User> userManager)
        {
            _clothDbContext = clothDbContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var basket = await _clothDbContext
                .Baskets
                .Where(x => x.UserId == user.Id)
                .Include(bp=>bp.BasketProducts)
                .FirstOrDefaultAsync();

            return View(basket);
        }
    }
}
