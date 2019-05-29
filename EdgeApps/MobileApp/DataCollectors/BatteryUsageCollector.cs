using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Communication.Common;
using Communication.Common.Helpers;
using Communication.Common.Models;
using MobileApp.Helpers;
using MobileApp.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApp.DataCollectors
{
    public class BatteryUsageCollector : IDataCollector
    {
        public Task<DecryptedData> Collect()
        {
            return Task.FromResult(new DecryptedData()
            {
                DataSource = DataSourceType.Battery,
                PlatformType = Device.RuntimePlatform.ToPlatformType(),
                DataType = DataType.Number,
                Base64Data = Battery.ChargeLevel.ToBase64Data()
            });
        }

        public Task Start()
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }
    }
}
