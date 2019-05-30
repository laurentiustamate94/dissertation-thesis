using CloudApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudApp.Controllers
{
    [Authorize]
    public class KeyPairController : Controller
    {
        private IKeyPairManagementService KeyPairManagementService { get; }

        public KeyPairController(IKeyPairManagementService keyPairManagementService)
        {
            KeyPairManagementService = keyPairManagementService;
        }

        public IActionResult Management()
        {
            var keyPairs = KeyPairManagementService.GetAllKeyPairs(User.Claims);

            return View("Management", keyPairs);
        }

        public IActionResult GenerateKey(string keyPurpose)
        {
            KeyPairManagementService.GenerateKeyPair(User.Claims, keyPurpose);

            return RedirectToAction(nameof(Management));
        }

        public IActionResult DeleteKey(string id)
        {
            KeyPairManagementService.DeleteKeyPair(User.Claims, id);

            return RedirectToAction(nameof(Management));
        }

        public IActionResult DownloadPublicKey(string id)
        {
            var fileResult = KeyPairManagementService.DownloadKey(User.Claims, id, "public");

            return fileResult;
        }

        public IActionResult DownloadPrivateKey(string id)
        {
            var fileResult = KeyPairManagementService.DownloadKey(User.Claims, id, "private");

            return fileResult;
        }
    }
}
