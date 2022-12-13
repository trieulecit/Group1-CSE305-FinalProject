using DeliveryAgentManagementApp.Areas.Identity.Pages.Account;
using DeliveryAgentManagementApp.Data;
using DeliveryAgentManagementApp.Models;
using DeliveryAgentManagementApp.Models.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using LoginModel = DeliveryAgentManagementApp.Models.BindingModels.LoginModel;

namespace DeliveryAgentManagementApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly DeliveryAgentManagementAppContext _context;

        public AccountsController(DeliveryAgentManagementAppContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, [FromServices] SignInManager<ApplicationUser> signInManager, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");


            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    return LocalRedirect(returnUrl);
                }
                
                if (result.IsLockedOut)
                {
                    //_logger.LogWarning("User account locked out.");
                    return RedirectToPage("/Identity/Account/Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            
            return View(model);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Register(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Register(Models.BindingModels.RegisterModel model, [FromServices] SignInManager<ApplicationUser> signInManager, [FromServices] UserManager<ApplicationUser> userManager, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
   
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.UserName = model.UserName;


                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    await userManager.AddToRoleAsync(user, "courier");

                    Courier courier = new Courier();
                    courier.UserId = user.Id;
                    courier.ShippingFee = 100;
                    courier.User = user;
                    courier.Name = "Courier " + (user.Id - 1);

                    _context.Add(courier);
                    await _context.SaveChangesAsync();

                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { username = model.UserName, returnUrl = returnUrl });
                    }
                    else
                    {
                        //await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                  
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return View(model);
        }

        private ApplicationUser CreateUser()
        {
            try
            {

                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public async Task<IActionResult> Logout([FromServices] SignInManager<ApplicationUser> signInManager,string returnUrl = null)
        {
            await signInManager.SignOutAsync();

  

            return RedirectToAction("Index", "Orders");
            
        }
    }
}
