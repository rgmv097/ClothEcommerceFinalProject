using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _userManager;

        public OrderController(ClothDbContext clothDbContext, UserManager<User> userManager)
        {
            _clothDbContext = clothDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OrderProduct()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("LogIn", "Account");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user is null) return BadRequest();

            var basket = await _clothDbContext
                    .Baskets
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.BasketProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

            if (basket is null) return NotFound();

            _clothDbContext.Baskets.Remove(basket);

            var order = new Order
            {
                UserId = user.Id,
                CreateTime = DateTime.Now,
                OrderItems = new List<OrderItem>()
            };

            foreach (var product in basket.BasketProducts)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.ProductId,
                    OrderId = order.Id,
                    Color= product.Color,
                    Size= product.Size,
                    Count = product.Count,
                });
            }

            await _clothDbContext.Orders.AddAsync(order);
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


    }
}
