using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CloudApp.DbModels;
using CloudApp.Interfaces;
using Communication.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CloudApp.Services
{
    public class AccountService : IAccountService
    {
        private IUniqueIdGenerationService UniqueIdGenerationService { get; }

        private IHttpContextAccessor HttpContextAccessor { get; }

        public AccountService(
            IUniqueIdGenerationService uniqueIdGenerationService,
            IHttpContextAccessor httpContextAccessor)
        {
            UniqueIdGenerationService = uniqueIdGenerationService;
            HttpContextAccessor = httpContextAccessor;
        }

        public async Task Login(string email, string password)
        {
            var claims = GenerateClaims(email, password);

            await HttpContextAccessor.HttpContext
                .SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
        }

        public Task<bool> ValidateLogin(string username, string password)
        {
            // For this sample, all logins are successful.
            return Task.FromResult(true);
        }

        private IEnumerable<Claim> GenerateClaims(string email, string password)
        {
            var userClaimId = UniqueIdGenerationService.GenerateRandomId();
            var passwordHash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));

            using (var dbContext = HttpContextAccessor.HttpContext.RequestServices.GetService(typeof(DissertationThesisContext)) as DissertationThesisContext)
            {
                var user = dbContext.Users.FirstOrDefault(x => x.User == email && x.PasswordHash == passwordHash);

                if (user == null)
                {
                    dbContext.Users.Add(new Users
                    {
                        Id = userClaimId,
                        User = email,
                        PasswordHash = passwordHash,
                        Role = "Member"
                    });

                    dbContext.SaveChanges();
                }

                userClaimId = user.Id;
            }

            return new List<Claim>
            {
                new Claim("id", userClaimId),
                new Claim("user", email),
                new Claim("hash", passwordHash),
                new Claim("role", "Member")
            };
        }

        public async Task LoginAgainWithClaim(ClaimsPrincipal user, Claim claim)
        {
            if (user.HasClaim("fitbit", "yes"))
            {
                return;
            }

            var claims = user.Claims.ToList();
            claims.Add(new Claim("fitbit", "yes"));
            var newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role"));

            await HttpContextAccessor.HttpContext.SignOutAsync();
            await HttpContextAccessor.HttpContext.SignInAsync(newPrincipal);
        }
    }
}
