using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudApp.Interfaces
{
    public interface IAccountService
    {
        Task<bool> ValidateLogin(string email, string password);

        Task Login(string email, string password);

        Task LoginAgainWithClaim(ClaimsPrincipal user, Claim claim);
    }
}
