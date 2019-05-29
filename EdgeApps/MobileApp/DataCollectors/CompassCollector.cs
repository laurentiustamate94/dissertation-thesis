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
    public class CompassCollector : IDataCollector
    {
        private CompassData CompassData { get; set; }
        private readonly SensorSpeed speed = SensorSpeed.UI;

        public CompassCollector()
        {
            Compass.ReadingChanged += Compass_ReadingChanged;
        }

        public Task<DecryptedData> Collect()
        {
            if (CompassData == null)
            {
                return Task.FromResult<DecryptedData>(null);
            }

            return Task.FromResult(new DecryptedData()
            {
                DataSource = DataSourceType.Compass,
                DataType = DataType.Json,
                PlatformType = Device.RuntimePlatform.ToPlatformType(),
                Base64Data = JsonConvert.SerializeObject(new
                {
                    HeadingMagneticNorth = CompassData.HeadingMagneticNorth
                }).ToBase64Data()
            });
        }

        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            this.CompassData = e.Reading;
        }

        public Task Start()
        {
            try
            {
                Compass.Start(speed);
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
                Compass.Stop();
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
