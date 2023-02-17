using Ecommerce.BLL.ViewModels;
using Ecommerce.Data.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ecommerce.ViewComponents
{

    public class ContactMessageViewComponent : ViewComponent
    {
        private readonly ClothDbContext _clothDbContext;

        public ContactMessageViewComponent(ClothDbContext clothDbContext)
        {
            _clothDbContext = clothDbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var message = await _clothDbContext.ContactMessages
                .ToListAsync();

            var isALlReadMessages = message.All(x => x.IsStatus);

            return View(new ContactMessageReadViewModel
            {
                contactMessages = message,
                isReadAllMessage = isALlReadMessages
            });
        }
    }
}
