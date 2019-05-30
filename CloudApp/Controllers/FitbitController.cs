using System.Linq;
using System.Threading.Tasks;
using CloudApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudApp.Controllers
{
    [Authorize]
    public class FitbitController : Controller
    {
        private IFitbitService FitbitService { get; }

        public FitbitController(IFitbitService fitbitService)
        {
            FitbitService = fitbitService;
        }

        public IActionResult Authorize()
        {
            var authorizationUrl = FitbitService.GetAuthorizationUrl();

            return Redirect(authorizationUrl);
        }

        public async Task<IActionResult> Callback(string code)
        {
            await FitbitService.FinishAuthorization(User, code);

            return RedirectToAction(nameof(EnvironmentController.Setup), "Environment");
        }

        public async Task PersistData()
        {
            // TODO will need to fetch access token from database
            await FitbitService.PersistData(User.Claims.First(c => c.Type == "id").Value);
        }
    }
}
