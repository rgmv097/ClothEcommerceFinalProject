using Ecommerce.BLL.Helpers;
using Ecommerce.BLL.ViewModels;
using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net.Mail;
using Ecommerce.BLL.Services;
using System.Net;
using Ecommerce.Data.Data;

namespace Ecommerce.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMailService _mailManager;
        private readonly IConfiguration _config;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMailService mailManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailManager = mailManager;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName=model.Username,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Country = model.Country,
                City = model.City,
                Address= model.Address,
                ZipCode = model.ZipCode,
            };

            string token = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = token;

            IdentityResult identityResult = await _userManager.CreateAsync(user, model.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, "User");

            var link = Url.Action(nameof(VerifyEmail), "Account", new { id = user.Id, token }, Request.Scheme, Request.Host.ToString());

            EmailViewModel email = _config.GetSection("Email").Get<EmailViewModel>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(user.Email);
            mail.Subject = "VerifyEmail";
            string body = "";
            using (StreamReader reader = new StreamReader("wwwroot/template/verifyemail.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{link}", link);
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);

            return RedirectToAction(nameof(EmailVerification));

        }
        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            if (user.EmailConfirmationToken != token)
            {
                return BadRequest();
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

            if (result.Succeeded)
            {
                string newToken = Guid.NewGuid().ToString();
                user.EmailConfirmationToken = newToken;
                await _userManager.UpdateAsync(user);

                return View();
            }

            return BadRequest();
        }
        public IActionResult EmailVerification() => View();
        public IActionResult Login(string? returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
           
            if(!ModelState.IsValid) return View(model);

            

            var existUser = await _userManager.FindByNameAsync(model.Username);

            if (existUser == null) 
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(existUser))
            {
                ModelState.AddModelError("", "Please verify your account first!");
                return View(model);
            }
            var signResult = await _signInManager.PasswordSignInAsync(existUser, model.Password, model.RememberMe, false);

            if(!signResult.Succeeded)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.ReturnUrl)) return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {

            if (!User.Identity.IsAuthenticated) return RedirectToAction(nameof(Login));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (existUser is null) return BadRequest();

            var result = await _userManager.ChangePasswordAsync(existUser, model.CurrentPassword, model.NewPassword);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }

            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Must be write email address");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                ModelState.AddModelError("", "So the email is not available");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(nameof(ResetPassword), "Account", new { id = user.Id, token }, Request.Scheme, Request.Host.ToString());

            EmailViewModel email = _config.GetSection("Email").Get<EmailViewModel>();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email.SenderEmail, email.SenderName);
            mail.To.Add(model.Email);
            mail.Subject = "Reset Password";
            mail.Body = $"<a href=\"{link}\">Reset Password</a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = email.Server;
            smtp.Port = email.Port;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(email.SenderEmail, email.Password);
            smtp.Send(mail);

            return RedirectToAction(nameof(Login));

           
        }

        public IActionResult ResetPassword(string email,string token)
        {
            return View(new ResetPasswordViewModel { Email=email,Token=token});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByEmailAsync(model.Email);

            if (existUser is null) return BadRequest();

            var result= await _userManager.ResetPasswordAsync(existUser,model.Token,model.Password);

            if (result.Succeeded) return RedirectToAction(nameof(Login));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return View();
            }

            return View();
        }
    }
}
