using System.Diagnostics;
using System.Threading.Tasks;
using CloudApp.Interfaces;
using CloudApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CloudApp.Controllers
{
    public class AccountController : Controller
    {
        private IAccountService AccountService { get; }

        public AccountController(IAccountService accountService)
        {
            AccountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberPassword, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return View(
                    "Error",
                    new ErrorViewModel
                    {
                        Reason = "Email and password cannot be null",
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    });
            }

            var isValidLogin = await AccountService.ValidateLogin(email, password);
            if (!isValidLogin)
            {
                return View();
            }

            await AccountService.Login(email, password);

            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : Redirect("/");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult SignUp(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult ForgotPassword(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
