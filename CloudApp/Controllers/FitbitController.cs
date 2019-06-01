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

        public async Task<IActionResult> Authorize()
        {
            var authorizationUrl = await FitbitService.GetAuthorizationUrl();
            if (string.IsNullOrWhiteSpace(authorizationUrl))
            {
                return RedirectToAction(nameof(EnvironmentController.Setup), "Environment");
            }

            return Redirect(authorizationUrl);
        }

        public async Task<IActionResult> Callback(string code)
        {
            await FitbitService.FinishAuthorization(code);

            return RedirectToAction(nameof(EnvironmentController.Setup), "Environment");
        }

        [AllowAnonymous]
        public async Task PersistData()
        {
            await FitbitService.PersistData();
        }
    }
}
