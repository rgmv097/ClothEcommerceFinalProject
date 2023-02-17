using Ecommerce.BLL.Extentions;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;

        public SettingsController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {
         
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SettingsCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View();
            if (!model.Logo.IsImage())
            {
                ModelState.AddModelError("Image", "Choose a image format");
                return View();
            }
            if (!model.Logo.IsImage())
            {
                ModelState.AddModelError("Image", "Choose a image format");
                return View();
            }

            if (!model.Logo.IsAllowedSize(10))
            {
                ModelState.AddModelError("Image", "The size of the image can be maximum 10 MB");
                return View();
            }
            var unicalFile = await model.Logo.GenerateFile(Constants.LogoPath);
            

            await _clothDbContext.Settings.AddAsync(new Settings
            {
                LogoUrl= unicalFile,
                Address=model.Address,
                Phone=model.Phone,
                Description=model.Description,
                Email=model.Email,

            });
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction("index","dashboard");
        }
    }
}
