using Ecommerce.BLL.Extentions;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {

        private readonly ClothDbContext _clothDbContext;

        public CategoryController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var categories= await _clothDbContext.Categories
                .Where(c=>c.isMain)
                .Include(s=>s.Children)                          
                .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _clothDbContext.Categories
                .Where(x => x.IsStatus && x.isMain).ToListAsync();

            var categoryListItem = new List<SelectListItem>
            {
                new SelectListItem("Select Parent Category", "0")
            };

            categories.ForEach(x => categoryListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));

            var model = new CategoryCreateViewModel()
            {
                ParentCategories = categoryListItem,
                IsMain=false,
                IsStatus = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            
            var parentCategories = await _clothDbContext.Categories.Where(x => x.IsStatus && x.isMain).Include(x => x.Children).ToListAsync();

            var categoryListItems = new List<SelectListItem>
            {
                new SelectListItem("--Select parent category--", "0")
            };
            parentCategories.ForEach(x => categoryListItems.Add(new SelectListItem(x.Name, x.Id.ToString())));

            var viewModel = new CategoryCreateViewModel()
            {
                ParentCategories = categoryListItems
            };

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var createdCategory = new Category();

            if (model.IsMain)
            {
                if (model.Icon is null)
                {
                    ModelState.AddModelError("", "Pls Choose image");
                    return View(viewModel);
                }
                if (!model.Icon.IsImage())
                {
                    ModelState.AddModelError("", "Pls Select Image");
                    return View(viewModel);
                }
               

                if (!model.Icon.IsAllowedSize(10))
                {
                    ModelState.AddModelError("", "The image cannot be more than 10 MB");
                    return View(viewModel);
                }

                if (parentCategories.Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                {
                    ModelState.AddModelError("", "This name has a parent category");
                    return View(viewModel);
                }

                var unicalName = await model.Icon.GenerateFile(Constants.CategoryIconPath);
                createdCategory.Icon = unicalName;
            }
            else
            {
                if (model.ParentId == 0)
                {
                    ModelState.AddModelError("", "Pls Select Parent Category");
                    return View(viewModel);
                }

                var parentCategory = parentCategories.FirstOrDefault(x => x.Id == model.ParentId);
                if (parentCategory is null)
                {
                    ModelState.AddModelError("", "Pls Select Parent Category");
                    return View(viewModel);
                }

                if (parentCategory.Children.Any(x => x.Name.ToLower().Trim().Equals(model.Name.ToLower().Trim())))
                {
                    ModelState.AddModelError("", "This name has a parent category");
                    return View(viewModel);
                }
               

                createdCategory.ParentId = model.ParentId;
            }

            createdCategory.Name = model.Name;
            createdCategory.isMain = model.IsMain;
            createdCategory.IsStatus = model.IsStatus;

            await _clothDbContext.Categories.AddAsync(createdCategory);
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult>Update(int? id)
        {
            if (id is null) return BadRequest();

            var categories = await _clothDbContext.Categories
               .Include(x => x.Children).Include(c => c.Parent)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (categories is null) return NotFound();

            var parentCategories = await _clothDbContext.Categories.Where(c => c.isMain).ToListAsync();

            var categoryListItem = new List<SelectListItem>();

            parentCategories.ForEach(x => categoryListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));

            var categoryViewModel = new CategoryUpdateViewModel
            {
                Name = categories.Name,
                IsMain = false,              
                ParentCategories = categoryListItem,
                ParentId = categories.ParentId
            };

            return View(categoryViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, CategoryUpdateViewModel model)
        {
            if (id is null) return BadRequest();

            var dbCategory = await _clothDbContext.Categories
              .Where(x => x.Id == id)
              .FirstOrDefaultAsync();

            if (dbCategory is null) return NotFound();
            var parentCategories = await _clothDbContext.Categories.Where(c => c.isMain).Include(x => x.Children).ToListAsync();

            var categoryListItem = new List<SelectListItem>();

            parentCategories.ForEach(x => categoryListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));

            var viewModel = new CategoryUpdateViewModel()
            {
                ParentCategories = categoryListItem,
            };

            if (!ModelState.IsValid) return View(viewModel);

            var createdCategory = new Category();

            if (model.IsMain)
            {
                if (parentCategories.Any(x => x.Name.ToLower().Trim().Equals(model.Name.ToLower().Trim())))
                {
                    ModelState.AddModelError("", "There is a category with this name");
                    return View(viewModel);
                }
            }
            else
            {
                if (model.ParentId == 0)
                {
                    ModelState.AddModelError("", "Must be choose Parent Category");
                    return View(viewModel);
                }

                var parentCategory = parentCategories.FirstOrDefault(c => c.Id == model.ParentId && c.isMain);

                if (parentCategory is not null)
                {
                    if (parentCategory.Children.Any(x => x.Name.ToLower().Trim().Equals(model.Name.ToLower().Trim())))
                    {
                        ModelState.AddModelError("", "There is a subcategory with this name");
                        return View(viewModel);
                    }
                }

                dbCategory.ParentId = model.ParentId;
            }

            dbCategory.Name = model.Name;
            dbCategory.isMain = model.IsMain;
            dbCategory.IsStatus = model.IsStatus;
            dbCategory.Children = model.Children;

            await _clothDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var parentCategories = await _clothDbContext.Categories
              .Include(x => x.Children).FirstOrDefaultAsync(c => c.Id == id);

            if (parentCategories is null) return NotFound();

            if (parentCategories.Id != id) return BadRequest();

            _clothDbContext.Remove(parentCategories);
            await _clothDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
