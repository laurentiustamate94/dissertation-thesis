using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudApp.Interfaces
{
    public interface IFitbitService
    {
        string GetAuthorizationUrl();

        Task FinishAuthorization(ClaimsPrincipal user, string code);

        Task PersistData(string userId);
    }
}
