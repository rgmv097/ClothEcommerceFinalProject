using Ecommerce.Data.Data;


namespace Ecommerce.BLL.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(RequestEmail requestEmail);
    }
}
