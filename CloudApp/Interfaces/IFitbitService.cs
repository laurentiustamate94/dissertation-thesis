using System.Threading.Tasks;

namespace CloudApp.Interfaces
{
    public interface IFitbitService
    {
        Task<string> GetAuthorizationUrl();

        Task FinishAuthorization(string code);

        Task PersistData();
    }
}
