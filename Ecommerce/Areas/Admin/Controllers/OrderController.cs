using Ecommerce.BLL.Services;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net.Mail;
using System.Net;

namespace Ecommerce.Areas.Admin.Controllers
{
	public class OrderController : BaseController
	{
		private readonly ClothDbContext _clothDbContext;
		private readonly UserManager<User> _userManager;
        private readonly IMailService _mailManager;
        private readonly IConfiguration _config;

        public OrderController(ClothDbContext clothDbContext, UserManager<User> userManager, IMailService mailManager, IConfiguration config)
        {
            _clothDbContext = clothDbContext;
            _userManager = userManager;
            _mailManager = mailManager;
            _config = config;
        }

        public async Task<IActionResult> Index()
		{
			var orders = await _clothDbContext.Orders
				.Include(order => order.OrderItems)
				.ThenInclude(order => order.Product)
				.OrderByDescending(o=>o.Id)
				.ToListAsync();

			var model = new List<OrderViewModel>();

			if (orders is not null)
			{
				foreach (var order in orders)
				{

					var user = await _userManager.FindByIdAsync(order.UserId);

					int count = 0;
					decimal TotalPrice = 0;
					foreach (var item in order.OrderItems)
					{
						count += item.Count;
						TotalPrice += item.Product.Price;
					}


					model.Add(new OrderViewModel
					{
						Name = user.UserName,
						Id = order.Id,
						Time = order.CreateTime,
						Status = order.Status,
						Items = order.OrderItems,
						TotalCount = count,
						Amount = TotalPrice,

					});
				}
               
            }
            return View(model);

        }

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var order = await _clothDbContext.Orders.Where(order => order.Id == id)
			  .Include(order => order.OrderItems)
			  .ThenInclude(order => order.Product)
			  .ThenInclude(c => c.ProductCategories)
			  .ThenInclude(c => c.Category)
			  .FirstOrDefaultAsync();

			return View(order);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OrderStatus(int? id, bool? Status)
		{
			if(id == null) return NotFound();

			var order = await _clothDbContext.Orders
		     .Where(order => order.Id == id)
			 .FirstOrDefaultAsync();

			var user = await _userManager.FindByIdAsync(order.UserId);

			if (order == null) return NotFound();

			order.Status= Status;

            EmailViewModel email = _config.GetSection("Email").Get<EmailViewModel>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(user.Email);
			if(Status == false)
			{
                mail.Subject = $"Canceled";
                mail.Body = $"Dear {user.UserName.ToUpper()},Your Order is Cancel";
			}
			else
			{
                mail.Subject = "Success";
                mail.Body = $"Dear {user.UserName.ToUpper()}, Your Order is Success";
            }
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);

            await _clothDbContext.SaveChangesAsync();

			return(RedirectToAction(nameof(Index)));
		}
	}
}
