using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudApp.Controllers
{
    [Authorize]
    public class EnvironmentController : Controller
    {
        public IActionResult Setup()
        {
            return View("Setup");
        }

        public IActionResult DownloadForApple()
        {
            // Packaging and simple installation of the iOS application is out of the scope of this paper
            return Setup();
        }

        public IActionResult DownloadForGoogle()
        {
            // Packaging and simple installation of the Android application is out of the scope of this paper
            return Setup();
        }
    }
}
