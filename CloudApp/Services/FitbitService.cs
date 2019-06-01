using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CloudApp.DbModels;
using CloudApp.Interfaces;
using Communication.Common;
using Communication.Common.Models;
using Fitbit.Api;
using Fitbit.Api.Abstractions;
using Fitbit.Api.Abstractions.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CloudApp.Services
{
    public class FitbitService : IFitbitService
    {
        private IConfiguration Configuration { get; }

        private IFitbitClient FitbitClient { get; set; }

        private IAccountService AccountService { get; }

        private IDataPersistor DataPersistor { get; }

        private IHttpContextAccessor HttpContextAccessor { get; }

        public FitbitService(
            IConfiguration configuration,
            IAccountService accountService,
            IDataPersistor dataPersistor,
            IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            AccountService = accountService;
            DataPersistor = dataPersistor;
            HttpContextAccessor = httpContextAccessor;

            var fitbitConfiguration = Configuration.GetSection("FitbitConfiguration");
            FitbitClient = new FitbitClient(
                fitbitConfiguration.GetSection("FitbitClientId").Value,
                fitbitConfiguration.GetSection("FitbitClientClientSecret").Value,
                fitbitConfiguration.GetSection("FitbitRedirectUri").Value);
        }

        public string GetAuthorizationUrl()
        {
            return FitbitClient.Authentication.GetCodeGrantFlowWithPkceUrl(
                new PermissionsRequestType[]
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
        }

        public async Task FinishAuthorization(ClaimsPrincipal user, string code)
        {
            var response = await FitbitClient.Authentication.FinishCodeGrantFlowWithPkceAsync(code);
            await SaveAuthenticationResponse(
                JsonConvert.SerializeObject(response),
                user.Claims.First(c => c.Type == "id").Value);

            await AccountService.LoginAgainWithClaim(user, new Claim("fitbit", "yes"));
        }

        public async Task PersistData()
        {
            using (var dbContext = HttpContextAccessor.HttpContext.RequestServices.GetService(typeof(DissertationThesisContext)) as DissertationThesisContext)
            {
                var fitbitUsers = dbContext.Users
                    .Where(u => u.FitbitAuthenticationResponseAsJson != null);

                foreach (var user in fitbitUsers)
                {
                    FitbitClient = new FitbitClient(JsonConvert.DeserializeObject<AuthenticationResponse>(user.FitbitAuthenticationResponseAsJson));

                    var decryptedData = await this.CollectData();
                    await DataPersistor.HandleDecryptedData(decryptedData.ToArray());
                }
            }
        }

        private async Task SaveAuthenticationResponse(string authenticationResponse, string userId)
        {
            using (var dbContext = HttpContextAccessor.HttpContext.RequestServices.GetService(typeof(DissertationThesisContext)) as DissertationThesisContext)
            {
                dbContext.Users.First(x => x.Id == userId)
                    .FitbitAuthenticationResponseAsJson = authenticationResponse;

                await dbContext.SaveChangesAsync();
            }
        }

        private async Task<IEnumerable<DecryptedData>> CollectData()
        {
            // For the purpose of this thesis, we only collect heart rate data
            var heartRate = await FitbitClient.HeartRate.GetHeartRateTimeSeriesAsync("today", PeriodType.OneDay);

            return new List<DecryptedData>
            {
                new DecryptedData
                {
                    DataType = DataType.Json,
                    DataSource = DataSourceType.HeartRate,
                    PlatformType = PlatformType.Fitbit,
                    Base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(heartRate)))
                }
            };
        }
    }
}
