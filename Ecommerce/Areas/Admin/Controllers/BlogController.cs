using Ecommerce.BLL.Extentions;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{

   
    public class BlogController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;

        public BlogController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {

            var blogList = await _clothDbContext.Blogs.OrderByDescending(x => x.Id).ToListAsync();
            return View(blogList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("Image", "Choose a image format");
                return View();
            }

            if (!model.Image.IsAllowedSize(10))
            {
                ModelState.AddModelError("Image", "The size of the image can be maximum 10 MB");
                return View();
            }
            var unicalFileName = await model.Image.GenerateFile(Constants.BlogPath);
            await _clothDbContext.Blogs.AddAsync(new Blog
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl= unicalFileName,
                Author = model.Author,
                IsStatus = model.IsStatus,
                Date = DateTime.Now,
            });
            await _clothDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _clothDbContext.Blogs.FindAsync(id);

            if (blog == null) return NotFound();

            return View(new BlogUpdateViewModel
            {
                Title = blog.Title,
                Description = blog.Description,
                ImageUrl = blog.ImageUrl,
                Author = blog.Author,
                IsStatus = blog.IsStatus
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, BlogUpdateViewModel model)
        {
            if (id == null) return NotFound();
            var blog = await _clothDbContext.Blogs.FindAsync(id);
            if (blog == null) return NotFound();
            if (blog.Id != id) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(new BlogUpdateViewModel
                {
                    ImageUrl = blog.ImageUrl,
                });
            }
            if (model.Image != null)
            {

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "Choose a image format");

                    return View(new BlogUpdateViewModel
                    {
                        ImageUrl = blog.ImageUrl,
                    });
                }

                if (!model.Image.IsAllowedSize(10))
                {
                    ModelState.AddModelError("Image", "The size of the image can be maximum 10 MB");

                    return View(new BlogUpdateViewModel
                    {
                        ImageUrl = blog.ImageUrl,
                    });
                }
                var path = Path.Combine(Constants.BlogPath, blog.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                var unicalFileName = await model.Image.GenerateFile(Constants.BlogPath);
                blog.ImageUrl = unicalFileName;
            }
            blog.Description = model.Description;
            blog.Title = model.Title;
            blog.Author = model.Author;
            blog.Date = DateTime.Now;
            blog.IsStatus=model.IsStatus;


            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var blog = await _clothDbContext.Blogs.FindAsync(id);

            if (blog == null) return NotFound();

            if (blog.Id != id) BadRequest();

            var path = Path.Combine(Constants.BlogPath, blog.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _clothDbContext.Blogs.Remove(blog);
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
