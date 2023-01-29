
using Ecommerce.BLL.Services;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;

        public ProductController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = await _categoryService.GetCategories();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                LongDescription = model.LongDescription,
                DescriptionTitle = model.DescriptionTitle,
                Sku = model.Sku,
                Discount = model.DiscountPrice,
                Availability = model.Availability,
            };

            List<ProductOption> productOptions = new List<ProductOption>();

            model.ProductOptions.ForEach(productOptionVm =>
            {
                var productOption = new ProductOption
                {
                    Size = productOptionVm.Size,
                    Count = productOptionVm.Count,
                    Color = productOptionVm.Color,
                    IsStatus = model.IsStatus
                };

                productOptions.Add(productOption);
            });

            product.ProductOptions = productOptions;

            await _clothDbContext.Products.AddAsync(product);
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChildCategories(int parentCategoryId)
        {
            var parentCategory = await _clothDbContext.Categories
                .Where(c => c.IsStatus && c.isMain && c.Id == parentCategoryId)
                .Include(c => c.Children)
                .FirstOrDefaultAsync();


            var childCategoriesSelectListItems = new List<SelectListItem>();

            parentCategory?.Children.ToList()
                .ForEach(c => childCategoriesSelectListItems
          .Add(new SelectListItem(c.Name, c.Id.ToString())));

            return Json(childCategoriesSelectListItems);
        }
    }
}
