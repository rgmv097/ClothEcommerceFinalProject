using Ecommerce.BLL.Extentions;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

		public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ClothDbContext clothDbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_clothDbContext = clothDbContext;
		}
		public async Task<IActionResult> Index()
        {
            var model = new List<UserViewModel>();

            var users = await _userManager.Users.ToListAsync();
            var roles= await _roleManager.Roles.ToListAsync();
            var userRoles= await _clothDbContext.UserRoles.ToListAsync();

            foreach (var user in users)
            {
                var userRole = userRoles.FirstOrDefault(x => x.UserId == user.Id);
                var role = roles.FirstOrDefault(x => x.Id == userRole.RoleId).Name;

                model.Add(new UserViewModel
                {
                    Firstname=user.Firstname,
                    Lastname=user.Lastname,
                    UserName=user.UserName,
                    Email=user.Email,
                    Role=role,
                    Id=user.Id,
                    ProfileImage=user.ProfileImage,
                });

            }
            return View(model);
        }

        public IActionResult Create()
        {
            var roles = new List<string>{ Constants.AdminRole, Constants.UserRole};
            var roleSelectList=new List<SelectListItem>();
            roles.ForEach(x => roleSelectList.Add(new SelectListItem(x, x)));

            var model = new UserCreateViewModel
            {
                Roles=roleSelectList,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            var roles = new List<string> { Constants.AdminRole, Constants.UserRole };
            var roleSelectList = new List<SelectListItem>();
            roles.ForEach(x => roleSelectList.Add(new SelectListItem(x, x)));

            var viewModel = new UserCreateViewModel
            {
                Roles = roleSelectList,
            };

            if(!ModelState.IsValid) return View(viewModel);

            var user = new User
            {
                UserName= model.UserName,
                Email=model.Email,
                Firstname=model.Firstname,
                Lastname=model.Lastname,
                Country=model.Country,
                City=model.City,
                Address=model.Address,
                ZipCode=model.Zipcode,
            };

            if (model.Image is not null)
            {

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "Choose a image format");
                    return View(model);
                }

                if (!model.Image.IsAllowedSize(10))
                {
                    ModelState.AddModelError("Image", "The size of the image can be maximum 10 MB");
                    return View(model);
                }
                var unicalFileName = await model.Image.GenerateFile(Constants.UserImage);
                user.ProfileImage = unicalFileName;
            }

            var result= await _userManager.CreateAsync(user);
            if (!result.Succeeded) 
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(viewModel);
            }

            var createdUser= await _userManager.FindByNameAsync(model.UserName);

            if (createdUser is null)
            {
                ModelState.AddModelError("", "Not Created User");
                return View(viewModel);
            }

            result = await _userManager.AddToRoleAsync(createdUser, model.Role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            if (user.Id != id) BadRequest();

            if(user.ProfileImage is not null)
            {
                var path = Path.Combine(Constants.UserImage, user.ProfileImage);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

           await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
