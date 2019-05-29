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
    public class HomeController : Controller
    {
        public IFitbitClient FitbitClient { get; }

        public HomeController(IFitbitClient fitbitClient)
        {
            FitbitClient = fitbitClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Index2()
        {
            var da = FitbitClient.Authentication.GetCodeGrantFlowWithPkceUrl(new PermissionsRequestType[]
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

            return Redirect(da);
        }

        public async Task<IActionResult> Privacy(string code)
        {
            await FitbitClient.Authentication.FinishCodeGrantFlowWithPkceAsync(code);

            var da = await FitbitClient.HeartRate.GetHeartRateTimeSeriesAsync("", "");

            return View();
        }

        [Authorize]
        public IActionResult KeyPairManagement()
        {
            //using (PGP pgp = new PGP())
            //{
            //    // Generate keys
            //    pgp.GenerateKey(@"C:\TEMP\keys\1.public.asc", @"C:\TEMP\keys\1.private.asc", "email@email.com", "password");
            //    pgp.GenerateKey(@"C:\TEMP\keys\2.public.asc", @"C:\TEMP\keys\2.private.asc", "email@email.com", "password");
            //    // Encrypt file
            //    pgp.EncryptFile(@"C:\TEMP\keys\content.txt", @"C:\TEMP\keys\content__encrypted.pgp", @"C:\TEMP\keys\1.public.asc", true, true);
            //    // Encrypt file with multiple keys
            //    string[] publicKeys = Directory.GetFiles(@"C:\TEMP\keys\", "*.public.asc");
            //    pgp.EncryptFile(@"C:\TEMP\keys\content.txt", @"C:\TEMP\keys\content__encrypted.pgp", publicKeys, true, true);
            //    // Encrypt and sign file
            //    pgp.EncryptFileAndSign(@"C:\TEMP\keys\content.txt", @"C:\TEMP\keys\content__encrypted_signed.pgp", @"C:\TEMP\keys\1.public.asc", @"C:\TEMP\keys\1.private.asc", "password", true, true);
            //    // Decrypt file
            //    pgp.DecryptFile(@"C:\TEMP\keys\content__encrypted.pgp", @"C:\TEMP\keys\content__decrypted.txt", @"C:\TEMP\keys\2.private.asc", "password");
            //    // Decrypt signed file
            //    pgp.DecryptFile(@"C:\TEMP\keys\content__encrypted_signed.pgp", @"C:\TEMP\keys\content__decrypted_signed.txt", @"C:\TEMP\keys\1.private.asc", "password");

            //    // Encrypt stream
            //    using (FileStream inputFileStream = new FileStream(@"C:\TEMP\keys\content.txt", FileMode.Open))
            //    using (Stream outputFileStream = System.IO.File.Create(@"C:\TEMP\keys\content__encrypted2.pgp"))
            //    using (Stream publicKeyStream = new FileStream(@"C:\TEMP\keys\1.public.asc", FileMode.Open))
            //        pgp.EncryptStream(inputFileStream, outputFileStream, publicKeyStream, true, true);

            //    // Decrypt stream
            //    using (FileStream inputFileStream = new FileStream(@"C:\TEMP\keys\content__encrypted2.pgp", FileMode.Open))
            //    using (Stream outputFileStream = System.IO.File.Create(@"C:\TEMP\keys\content__decrypted2.txt"))
            //    using (Stream privateKeyStream = new FileStream(@"C:\TEMP\keys\1.private.asc", FileMode.Open))
            //        pgp.DecryptStream(inputFileStream, outputFileStream, privateKeyStream, "password");
            //}
            var keyPairs = KeyPairManagementService.Instance.GetAllKeyPairs(User.Claims);

            return View(keyPairs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
