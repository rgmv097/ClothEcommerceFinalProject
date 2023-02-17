using Ecommerce.BLL.Helpers;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ecommerce.Controllers
{
    public class ShopController : Controller
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _userManager;

        public ShopController(ClothDbContext clothDbContext, UserManager<User> userManager)
        {
            _clothDbContext = clothDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var products = await _clothDbContext.Products
                .Include(p => p.ProductOptions)
                .Include(c => c.ProductCategories)
                .ThenInclude(c => c.Category)
                .Include(i => i.ProductImages)
                .ToListAsync();

            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var wishList = await _clothDbContext
                    .WishList
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

                if (wishList != null)
                {
                    foreach (var WishlisProducts in wishList.WishListProducts)
                    {
                        var product = await _clothDbContext.Products.
                            Where(p => p.Id == WishlisProducts.Product.Id)
                            .FirstOrDefaultAsync();

                        if (product is not null)
                        {
                            product.IsLike = true;
                        }
                    }
                }
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    foreach (var productId in productIdList)
                    {
                        var product = await _clothDbContext.Products.
                            Where(p => p.Id == productId)
                            .FirstOrDefaultAsync();

                        if (product is not null)
                        {
                            product.IsLike = true;
                        }
                    }
                }
            }

            if (products is null) return NotFound();

            int perPage = 3;
            int pageCount = (int)Math.Ceiling((double)products.Count() / perPage);

            if (page <= 0) page = 1;
            if (page > pageCount) page = pageCount;

            ViewBag.CurrentPage = page;
            ViewBag.PageCount = pageCount;

            return View(products.Skip((page - 1) * perPage).Take(perPage).ToList());

        }

        public async Task<IActionResult> Details(int? id)
        {
            var existProduct = await _clothDbContext.Products
               .Where(p => p.Id == id)
               .Include(p => p.ProductOptions)
               .Include(c => c.ProductCategories)
               .ThenInclude(c => c.Category)
               .Include(i => i.ProductImages)
               .FirstOrDefaultAsync();

            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var wishList = await _clothDbContext
                    .WishList
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

                if (wishList != null)
                {
                    foreach (var WishlisProducts in wishList.WishListProducts)
                    {
                        var product = await _clothDbContext.Products.
                            Where(p => p.Id == WishlisProducts.Product.Id)
                            .FirstOrDefaultAsync();

                        if (product is not null)
                        {
                            product.IsLike = true;
                        }
                    }
                }
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    foreach (var productId in productIdList)
                    {
                        var product = await _clothDbContext.Products.
                            Where(p => p.Id == productId)
                            .FirstOrDefaultAsync();

                        if (product is not null)
                        {
                            product.IsLike = true;
                        }
                    }
                }
            }

            return View(existProduct);
        }

        public async Task<IActionResult> Search(string? searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return NoContent();

            var products = await _clothDbContext.Products
                .Where(product => product.Name.ToLower().StartsWith(searchText.ToLower()))
                .ToListAsync();

            var model = new List<Product>();

            products.ForEach(product => model.Add(new Product
            {
                Id = product.Id,
                Name = product.Name,
                MainImageUrl = product.MainImageUrl,
            }));

            return PartialView("_ProductSearchPartial", products);
        }

        public async Task<IActionResult> CategoryShop(int? id, int page = 1)
        {
            if (id is null) return BadRequest();
            var products = await _clothDbContext.ProductCategories
                .Where(c => c.CategoryId == id)
                .Include(c => c.Product)               
                .ToListAsync();

            if (products is null) return NotFound();

            int perPage = 2;
            int pageCount = (int)Math.Ceiling((double)products.Count() / perPage);

            if (page <= 0) page = 1;
            if (page > pageCount) page = pageCount;

            ViewBag.CurrentPage = page;
            ViewBag.PageCount = pageCount;

            return View(products.Skip((page - 1) * perPage).Take(perPage).ToList());
        }
    }
}
