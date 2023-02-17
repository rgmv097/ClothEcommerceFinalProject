using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class ContactController : Controller
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _usermanager;

        public ContactController(ClothDbContext clothDbContext, UserManager<User> usermanager)
        {
            _clothDbContext = clothDbContext;
            _usermanager = usermanager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ContactViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _usermanager.FindByNameAsync(User.Identity.Name);

                model.ContactMessage = new ContactMessageViewModel
                {
                    Name = user.UserName,
                    Email = user.Email,
                    Image=user.ProfileImage
                };


            }
            return View(model);
        }

        public async Task<IActionResult> SendMessage(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: nameof(Index), model);
            }
            


            var message = new ContactMessage
            {
                Name = model.ContactMessage.Name,
                Subject = model.ContactMessage.Subject,
                Email = model.ContactMessage.Email,
                Message = model.ContactMessage.Message,
                ProfileImage=model.ContactMessage.Image
            };

            await _clothDbContext.ContactMessages.AddAsync(message);

            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
