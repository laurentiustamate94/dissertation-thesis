using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fitbit.Api.Abstractions;
using Fitbit.Api.Abstractions.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PgpCore;
using CloudApp.Models;
using CloudApp.Services;

namespace CloudApp.Controllers
{
    [Authorize]
    public class KeyPairController : Controller
    {
        public IActionResult Management()
        {
            var keyPairs = KeyPairManagementService.Instance.GetAllKeyPairs(User.Claims);

            return View("Management", keyPairs);
        }

        public IActionResult GenerateKey(string keyPurpose)
        {
            KeyPairManagementService.Instance.GenerateKeyPair(User.Claims, keyPurpose);

            return Management();
        }

        public IActionResult DeleteKey(string id)
        {
            KeyPairManagementService.Instance.DeleteKeyPair(User.Claims, id);

            return Management();
        }

        public IActionResult DownloadPublicKey(string id)
        {
            var fileResult = KeyPairManagementService.Instance.DownloadKey(User.Claims, id, "public");

            return fileResult;
        }

        public IActionResult DownloadPrivateKey(string id)
        {
            var fileResult = KeyPairManagementService.Instance.DownloadKey(User.Claims, id, "private");

            return fileResult;
        }
    }
}
