using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fitbit.Api.Abstractions;
using Fitbit.Api.Abstractions.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PgpCore;
using CloudApp.Models;
using CloudApp.Services;

namespace CloudApp.Controllers
{
    [Authorize]
    public class EnvironmentController : Controller
    {
        private IFitbitClient FitbitClient { get; }

        public EnvironmentController(IFitbitClient fitbitClient)
        {
            FitbitClient = fitbitClient;
        }

        public IActionResult Setup()
        {
            return View("Setup");
        }

        public IActionResult AuthorizeFitbit()
        {
            var authorizationUrl = FitbitClient.Authentication.GetCodeGrantFlowWithPkceUrl(new PermissionsRequestType[]
            {
                PermissionsRequestType.Activity,
                PermissionsRequestType.HeartRate,
                PermissionsRequestType.Location,
                PermissionsRequestType.Nutrition,
                PermissionsRequestType.Profile,
                PermissionsRequestType.Settings,
                PermissionsRequestType.Sleep,
                PermissionsRequestType.Social,
                PermissionsRequestType.Weight
            });

            return Redirect(authorizationUrl);
        }

        public async Task<IActionResult> FitbitCallback(string code)
        {
            await FitbitClient.Authentication.FinishCodeGrantFlowWithPkceAsync(code);

            if (!User.HasClaim("fitbit", "yes"))
            {
                var claims = User.Claims.ToList();
                claims.Add(new Claim("fitbit", "yes"));
                var newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role"));

                await HttpContext.SignOutAsync();
                await HttpContext.SignInAsync(newPrincipal);
            }

            var da = await FitbitClient.HeartRate.GetHeartRateTimeSeriesAsync("", "");

            return Setup();
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
