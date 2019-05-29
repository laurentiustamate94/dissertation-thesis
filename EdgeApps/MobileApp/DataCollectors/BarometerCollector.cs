using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Communication.Common;
using Communication.Common.Helpers;
using Communication.Common.Models;
using MobileApp.Helpers;
using MobileApp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApp.DataCollectors
{
    public class BarometerCollector : IDataCollector
    {
        private BarometerData BarometerData { get; set; }
        private readonly SensorSpeed speed = SensorSpeed.UI;

        public BarometerCollector()
        {
            Barometer.ReadingChanged += Barometer_ReadingChanged;
        }

        public Task<DecryptedData> Collect()
        {
            if (BarometerData == null)
            {
                return Task.FromResult<DecryptedData>(null);
            }

            return Task.FromResult(new DecryptedData()
            {
                DataSource = DataSourceType.Barometer,
                PlatformType = Device.RuntimePlatform.ToPlatformType(),
                DataType = DataType.Json,
                Base64Data = JsonConvert.SerializeObject(new
                {
                    PressureInHectopascals = BarometerData.PressureInHectopascals
                }).ToBase64Data()
            });
        }

        private void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            this.BarometerData = e.Reading;
        }

        public Task Start()
        {
            try
            {
                Barometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            try
            {
                Barometer.Stop();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }

            return Task.CompletedTask;
        }
    }
}
