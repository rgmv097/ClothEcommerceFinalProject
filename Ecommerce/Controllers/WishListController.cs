using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;

namespace Ecommerce.Controllers
{
    public class WishListController : Controller
    {
        private readonly ClothDbContext _clothDbContext;
        private readonly UserManager<User> _userManager;

        public WishListController(ClothDbContext clothDbContext, UserManager<User> userManager)
        {
            _clothDbContext = clothDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<WishListViewModel> model = new();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var wishList = await _clothDbContext.WishList
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

                if (wishList is not null)
                {

                    foreach (var item in wishList.WishListProducts)
                    {
                        model.Add(new WishListViewModel
                        {
                            Id = item.ProductId,
                            Name = item.Product.Name,
                            Price = item.Product.Price,
                            Sku = item.Product.Sku,
                            Aviability = item.Product.Availability,
                            ImageUrl = item.Product.MainImageUrl,
                        });
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
                        var product = await _clothDbContext.Products
                            .Where(x => x.Id == productId)
                            .FirstOrDefaultAsync();

                        if (product is null) return BadRequest();

                        model.Add(new WishListViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Sku = product.Sku,
                            Aviability = product.Availability,
                            ImageUrl = product.MainImageUrl,
                        });
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWishlist(int? productId)
        {
            if (productId == null)
                return NotFound();


            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null) return BadRequest();

                var existWishList = await _clothDbContext
                    .WishList
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .FirstOrDefaultAsync();

                if (existWishList != null)
                {
                    var createdWishList = new WishList
                    {
                        UserId = user.Id,
                        WishListProducts = new List<WishListProduct>()
                    };

                    var existProduct = await _clothDbContext.Products.FindAsync(productId);

                    if (existProduct == null) return NotFound();

                    if (existWishList.WishListProducts.Any(x => x.ProductId == existProduct.Id))
                        return NoContent();

                    existWishList.WishListProducts.Add(new WishListProduct
                    {
                        WishlistId = createdWishList.Id,
                        ProductId = existProduct.Id
                    });

                    _clothDbContext.Update(existWishList);
                }
                else
                {
                    var createdWishList = new WishList
                    {
                        UserId = user.Id,
                        WishListProducts = new List<WishListProduct>()
                    };

                    var wishListProducts = new List<WishListProduct>();

                    var existProduct = await _clothDbContext.Products.FindAsync(productId);

                    if (existProduct == null) return NotFound();

                    wishListProducts.Add(new WishListProduct
                    {
                        WishlistId = createdWishList.Id,
                        ProductId = existProduct.Id
                    });

                    createdWishList.WishListProducts = wishListProducts;

                    await _clothDbContext.WishList.AddAsync(createdWishList);
                }

                await _clothDbContext.SaveChangesAsync();
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    if (productIdList.Contains(productId.Value))
                        return NoContent();

                    productIdList.Add(productId.Value);

                    var productIdListJson = JsonConvert.SerializeObject(productIdList);

                    Response.Cookies.Append(Constants.WISH_LIST_COOKIE_NAME, productIdListJson);
                }
                else
                {
                    var productIdListJson = JsonConvert.SerializeObject(new List<int> { productId.Value });

                    Response.Cookies.Append(Constants.WISH_LIST_COOKIE_NAME, productIdListJson);
                }
            }

            return NoContent();
        }
        [HttpPost]

        public async Task<IActionResult> DeleteProductFromWishList(int? productId)
        {
            if (productId == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null) return BadRequest();

                var wishList = await _clothDbContext.WishList.Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts).FirstOrDefaultAsync();

                var existProduct = await _clothDbContext.Products.FindAsync(productId);

                if (existProduct == null) return NotFound();

                var existWishListProduct = wishList.WishListProducts.FirstOrDefault(x => x.ProductId == existProduct.Id);

                wishList.WishListProducts.Remove(existWishListProduct);

                _clothDbContext.Update(wishList);

                await _clothDbContext.SaveChangesAsync();
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    productIdList.Remove(productId.Value);

                    var productIdListJson = JsonConvert.SerializeObject(productIdList);

                    Response.Cookies.Append(Constants.WISH_LIST_COOKIE_NAME, productIdListJson);
                }
            }

            return NoContent();
        }
    }
}
