using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CloudApp.Interfaces;
using Communication.Common;
using Communication.Common.Interfaces;
using Communication.Common.Models;
using Fitbit.Api;
using Fitbit.Api.Abstractions;
using Fitbit.Api.Abstractions.Models.Authentication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CloudApp.Services
{
    public class FitbitService : IFitbitService
    {
        private IConfiguration Configuration { get; }

        private IFitbitClient FitbitClient { get; }

        private IAccountService AccountService { get; }

        private IDataPersistor DataPersistor { get; }

        public FitbitService(
            IConfiguration configuration, 
            IAccountService accountService,
            IDataPersistor dataPersistor)
        {
            Configuration = configuration;
            AccountService = accountService;
            DataPersistor = dataPersistor;

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
            await FitbitClient.Authentication.FinishCodeGrantFlowWithPkceAsync(code);
            await AccountService.LoginAgainWithClaim(user, new Claim("fitbit", "yes"));
        }

        public async Task PersistData(string userId)
        {
            var decryptedData = await this.CollectData();

            await DataPersistor.HandleDecryptedData(decryptedData.ToArray());
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
