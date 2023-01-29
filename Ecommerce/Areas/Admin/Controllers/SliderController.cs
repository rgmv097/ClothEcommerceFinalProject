using Ecommerce.BLL.Extentions;
using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Areas.Admin.Controllers
{
    public class SliderController : BaseController
    {
        private readonly ClothDbContext _clothDbContext;

        public SliderController(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var sliderList = await _clothDbContext.Sliders.OrderByDescending(s=>s.Id).ToListAsync();
            return View(sliderList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
            var unicalFileName = await model.Image.GenerateFile(Constants.SliderPath);


            await _clothDbContext.Sliders.AddAsync(new Slider
            {
                Title = model.Title,
                Slogan = model.Slogan,
                Description = model.Description,
                ImageUrl = unicalFileName,
                IsStatus=model.IsStatus

            });
           
            await _clothDbContext.SaveChangesAsync();

           
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var slider = await _clothDbContext.Sliders.FindAsync(id);

            if (slider == null) return NotFound();

            return View(new SlideUpdateViewModel
            {
                ImageUrl = slider.ImageUrl,
                Title = slider.Title,
                Description=slider.Description,
                Slogan=slider.Slogan,
                IsStatus=slider.IsStatus
                
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, SlideUpdateViewModel model)
        {
            if (id == null) return NotFound();

            var Slider = await _clothDbContext.Sliders.FindAsync(id);

            if (Slider == null) return NotFound();

            if (Slider.Id != id) BadRequest();

            if (!ModelState.IsValid)
            {
                return View(new SlideUpdateViewModel
                {
                    ImageUrl = Slider.ImageUrl,

                });
            }
            if (model.Image != null)
            {

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "Choose a image format");

                    return View(new SlideUpdateViewModel
                    {
                        ImageUrl = Slider.ImageUrl,
                    });
                }

                if (!model.Image.IsAllowedSize(10))
                {
                    ModelState.AddModelError("Image", "The size of the image can be maximum 10 MB");

                    return View(new SlideUpdateViewModel
                    {
                        ImageUrl = Slider.ImageUrl,

                    });
                }
                var path = Path.Combine(Constants.SliderPath, Slider.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                var unicalFileName = await model.Image.GenerateFile(Constants.SliderPath);
                Slider.ImageUrl = unicalFileName;
            }
            Slider.Title = model.Title;
            Slider.Description = model.Description;
            Slider.Slogan = model.Slogan;
            Slider.IsStatus = model.IsStatus;


            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var slide = await _clothDbContext.Sliders.FindAsync(id);

            if (slide == null) return NotFound();

            if (slide.Id != id) BadRequest();

            var path = Path.Combine(Constants.SliderPath, slide.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

                _clothDbContext.Sliders.Remove(slide);
            await _clothDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
