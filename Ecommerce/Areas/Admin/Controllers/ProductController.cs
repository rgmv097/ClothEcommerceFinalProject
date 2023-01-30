
using Ecommerce.BLL.Extentions;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.Services;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;
        public ProductController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;

        }

        public async Task<IActionResult> Index()
        {
            var products = await _clothDbContext.Products
                .Include(p => p.ProductOptions)

                .Include(c => c.ProductCategories)
                .ThenInclude(c => c.Category)
                .Include(i => i.ProductImages)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _clothDbContext.Categories
                .Where(c => c.IsStatus && c.isMain)
                .Include(c => c.Children)
                .ToListAsync();

            var parentCategoriesSelectListItems = new List<SelectListItem>();
            var childCategoriesSelectListItems = new List<SelectListItem>();

            categories.ForEach(c => parentCategoriesSelectListItems
            .Add(new SelectListItem(c.Name, c.Id.ToString())));

            categories[0].Children.ToList()
                .ForEach(c => childCategoriesSelectListItems
          .Add(new SelectListItem(c.Name, c.Id.ToString())));

            var model = new ProductCreateViewModel
            {
                ParentCategories = parentCategoriesSelectListItems,
                ChildCategories = childCategoriesSelectListItems,
                IsStatus = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var categories = await _clothDbContext.Categories
                .Where(c => c.IsStatus && c.isMain)
                .Include(c => c.Children)
                .ToListAsync();

            var productSKU = await _clothDbContext.Products
              .FirstOrDefaultAsync(s => s.Sku.ToLower().Trim() == model.Sku.ToLower().Trim());

            var parentCategoriesSelectListItems = new List<SelectListItem>();
            var childCategoriesSelectListItems = new List<SelectListItem>();

            categories.ForEach(c => parentCategoriesSelectListItems
            .Add(new SelectListItem(c.Name, c.Id.ToString())));

            categories[0].Children.ToList()
                .ForEach(c => childCategoriesSelectListItems
          .Add(new SelectListItem(c.Name, c.Id.ToString())));

            var viewModel = new ProductCreateViewModel
            {
                ParentCategories = parentCategoriesSelectListItems,
                ChildCategories = childCategoriesSelectListItems,
                IsStatus = true
            };

            if (!ModelState.IsValid) return View(viewModel);

            if (model.Price < 0 || model.DiscountPrice < 0)
            {
                ModelState.AddModelError("", "Money can not be lower than 0.");
                return View(viewModel);
            }

            if (model.DiscountPrice >= model.Price)
            {
                ModelState.AddModelError("DiscountPrice", "Discount price cannot be equal to or greater than price.");
                return View(viewModel);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                LongDescription = model.LongDescription,
                DescriptionTitle = model.DescriptionTitle,
                Discount = model.DiscountPrice,
                IsStatus = model.IsStatus,
                Availability = model.Availability,
                Price = model.Price,
                ProductImages = new List<ProductImage>(),
                ProductCategories = new List<ProductCategory>(),
                ProductOptions = new List<ProductOption>()
            };
            List<ProductImage> productImages = new List<ProductImage>();

            foreach (var image in model.Images)
            {
                if (!image.IsImage())
                {
                    ModelState.AddModelError("", "Must be selected image");
                    return View(model);
                }

                if (!image.IsAllowedSize(8))
                {
                    ModelState.AddModelError("", "Image size can be max 8 mb");
                    return View(model);
                }

                var unicalNames = await image.GenerateFile(Constants.ProductImages);

                productImages.Add(new ProductImage
                {
                    Image = unicalNames,
                    ProductId = product.Id,
                });
            }

            product.ProductImages.AddRange(productImages);

            List<ProductOption> productOptions = new List<ProductOption>();

            foreach (var option in model.ProductOptions)
            {
                if (option.Size == 0)
                {
                    ModelState.AddModelError("", "Pls Select Size");
                    return View(viewModel);
                }

                if (option.Color == 0)
                {

                    ModelState.AddModelError("", "Pls Select Color");
                    return View(viewModel);
                }

                if (option.Count == 0)
                {
                    ModelState.AddModelError("", "Pls Select Count");
                    return View(viewModel);
                }

                var ProductOptionVM = new ProductOption
                {
                    Size = option.Size,
                    Count = option.Count,
                    Color = option.Color,
                    IsStatus = true
                };
                productOptions.Add(ProductOptionVM);
            }
            product.ProductOptions = productOptions;

            //model.ProductOptions.ForEach(productOptionVm =>
            //{

            //    var productOption = new ProductOption
            //    {
            //        Size = productOptionVm.Size,
            //        Count = productOptionVm.Count,
            //        Color = productOptionVm.Color,
            //        IsStatus = model.IsStatus
            //    };

            //    productOptions.Add(productOption);
            //});

            //product.ProductOptions = productOptions;

            var parentCategory = await _clothDbContext.Categories
                .Where(c => c.IsStatus && c.isMain && c.Id == model.ParentCategoryId)
                .Include(c => c.Children)
                .FirstOrDefaultAsync();

            if (parentCategory is null) return NotFound();

            var childCategory = parentCategory.Children
               .FirstOrDefault(c => c.Id == model.ChildCategoryId);

            if (childCategory is null)
            {
                var productcategories = new List<ProductCategory>
                {
                    new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = parentCategory.Id
                    }
                };
                product.ProductCategories.AddRange(productcategories);
            }
            else
            {
                var productcategories = new List<ProductCategory>
                {
                    new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = parentCategory.Id
                    },

                    new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = childCategory.Id
                    }
                };
                product.ProductCategories.AddRange(productcategories);
            }

            if (productSKU is not null)
            {
                ModelState.AddModelError("", "This product code already exists! Try again..");
                return View(model);
            }
            else
            {
                product.Sku = model.Sku;
            }

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

        public async Task<IActionResult> Update(int? id)
        {

            if (id == null) return BadRequest();

            var product = await _clothDbContext.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductOptions)
                .Include(p => p.ProductCategories)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            var productUpdateViewModel = new ProductUpdateViewModel();

            var parentCategories = new List<SelectListItem>();
            var childCategories = new List<SelectListItem>();

            var parentCategory = product.ProductCategories
                 .Where(c => c.Category.isMain)
                 .First();

            var childCategory = product.ProductCategories
                .Where(c => !c.Category.isMain)
                .FirstOrDefault();

            var categories = await _clothDbContext.Categories
                .Where(c => c.isMain && c.IsStatus)
                .Include(c => c.Children)
                .ToListAsync();

            categories
                .ForEach(c => parentCategories.Add(new SelectListItem(c.Name, c.Id.ToString())));

            parentCategory.Category.Children
                .ToList()
                .ForEach(c => childCategories.Add(new SelectListItem(c.Name, c.Id.ToString())));

            if (childCategory is null)
            {
                productUpdateViewModel.ChildCategoryId = 0;
            }
            else
            {
                productUpdateViewModel.ChildCategoryId = childCategory.Id;
            }

            productUpdateViewModel = new ProductUpdateViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                DescriptionTitle = product.DescriptionTitle,
                LongDescription = product.LongDescription,
                Price = product.Price,
                Sku = product.Sku,
                DiscountPrice = product.Discount,
                ProductOptions = product.ProductOptions,
                ParentCategories = parentCategories,
                ParentCategoryId = parentCategory.CategoryId,
                ChildCategories = childCategories,
            };

            List<ProductOption> productOptions = new List<ProductOption>();

            foreach (var option in product.ProductOptions)
            {


                var ProductOptionVM = new ProductOption
                {
                    Size = option.Size,
                    Count = option.Count,
                    Color = option.Color,
                    IsStatus = true
                };
                productOptions.Add(ProductOptionVM);
            }
            productOptions = productUpdateViewModel.ProductOptions;

            return View(productUpdateViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return NotFound();

            var product = await _clothDbContext.Products.FindAsync(id);

            if (product == null) return NotFound();

            if (product.Id != id) BadRequest();

            var productImages = await _clothDbContext.ProductImages
                .Where(i => i.ProductId == id)
                .ToListAsync();

            foreach (var image in productImages)
            {
                var path = Path.Combine(Constants.ProductImages, image.Image);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }


            _clothDbContext.Products.Remove(product);
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
