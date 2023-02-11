using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _userManager;

        public OrderController(ClothDbContext clothDbContext, UserManager<User> userManager)
        {
            _clothDbContext = clothDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _clothDbContext.Orders
                .Include(order => order.OrderItems)
                .ThenInclude(order => order.Product)
                .ToListAsync();

            var model = new List<OrderViewModel>();

            if (orders is not null)
            {
                foreach (var order in orders)
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    decimal amount = 0;
                    foreach (var product in order.OrderItems)
                    {
                        amount = amount + (product.Count * product.Product.Price);
                    }

                    model.Add(new OrderViewModel
                    {
                        Name = user.UserName,
                        Id = order.Id,
                        Time = order.CreateTime,
                        Amount = amount,
                        Status = order.Status,
                    });
                }
            }

            return View(model);
        }
    }
}
