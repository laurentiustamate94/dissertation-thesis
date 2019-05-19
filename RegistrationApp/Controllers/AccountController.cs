
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RegistrationApp.Models;

namespace RegistrationApp.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        private bool ValidateLogin(string userName, string password)
        {
            // For this sample, all logins are successful.
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberPassword, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return View("Error", new ErrorViewModel
                {
                    Reason = "Email and password cannot be null",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }

            // Normally Identity handles sign in, but you can do it directly
            if (ValidateLogin(email, password))
            {
                var claims = new List<Claim>
                {
                    new Claim("user", email),
                    new Claim("role", "Member")
                };

                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/");
                }
            }

            return View();
        }

        public IActionResult SignUp(string returnUrl = null)
        {
            return View();
        }

        public IActionResult ForgotPassword(string returnUrl = null)
        {
            return View();
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
