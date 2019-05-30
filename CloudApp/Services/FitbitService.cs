using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudApp.Interfaces;
using Fitbit.Api;
using Fitbit.Api.Abstractions;
using Fitbit.Api.Abstractions.Models.Authentication;
using Microsoft.Extensions.Configuration;

namespace CloudApp.Services
{
    public class FitbitService : IFitbitService
    {
        private IConfiguration Configuration { get; }

        private IFitbitClient FitbitClient { get; }

        private IAccountService AccountService { get; }

        public FitbitService(IConfiguration configuration, IAccountService accountService)
        {
            Configuration = configuration;
            AccountService = accountService;

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

        public async Task PersistData()
        {
            var da = await FitbitClient.HeartRate.GetHeartRateTimeSeriesAsync("", "");
        }
    }
}
