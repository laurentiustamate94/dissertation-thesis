
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Communication.Common.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using CloudApp.DbModels;
using CloudApp.Models;
using CloudApp.Services;

namespace CloudApp.Controllers
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

#if DEBUG
        [HttpGet]
        public async Task<IActionResult> LoginDebug(string email, string password, string id)
        {
            var claims = GenerateClaims(email, password, id);

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

            return Redirect("/");
        }
#endif

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

            // Normally Identity handles sign in, but you can do it directly1
            if (ValidateLogin(email, password))
            {
                var claims = GenerateClaims(email, password);

                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

                return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : Redirect("/");
            }

            return View();
        }

        private IEnumerable<Claim> GenerateClaims(string email, string password, string id = null)
        {
            var iid = id ?? new UniqueIdGenerationService().GenerateRandomId();
            var passwordHash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));

            using (var dbContext = HttpContext.RequestServices.GetService(typeof(DissertationThesisContext)) as DissertationThesisContext)
            {
                var user = dbContext.Users.FirstOrDefault(x => x.User == email && x.PasswordHash == passwordHash);

                if (user == null)
                {
                    dbContext.Users.Add(new Users
                    {
                        Id = iid,
                        User = email,
                        PasswordHash = passwordHash,
                        Role = "Member"
                    });
                    dbContext.SaveChanges();
                }

                iid = user.Id;
            }

            return new List<Claim>
            {
                new Claim("id", iid),
                new Claim("user", email),
                new Claim("hash", passwordHash),
                new Claim("role", "Member")
            };
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

        private string GenerateSignature(string key, string content)
        {
            var hmac = new HMACSHA1();
            hmac.Key = Encoding.UTF8.GetBytes(key);
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var signature = hmac.ComputeHash(contentBytes);

            return Convert.ToBase64String(signature);
        }
    }
}
