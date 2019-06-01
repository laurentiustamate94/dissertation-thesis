using System;
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
    public class MagnetometerCollector : IDataCollector
    {
        private MagnetometerData MagnetometerData { get; set; }
        private readonly SensorSpeed speed = SensorSpeed.UI;

        public MagnetometerCollector()
        {
            Magnetometer.ReadingChanged += Magnometer_ReadingChanged;
        }

        public Task<DecryptedData> Collect()
        {
            if (MagnetometerData == null)
            {
                return Task.FromResult<DecryptedData>(null);
            }

            return Task.FromResult(new DecryptedData()
            {
                DataSource = DataSourceType.Magnetometer,
                DataType = DataType.Json,
                PlatformType = Device.RuntimePlatform.ToPlatformType(),
                Base64Data = JsonConvert.SerializeObject(new
                {
                    AngularVelocityAxisX = MagnetometerData.MagneticField.X,
                    AngularVelocityAxisY = MagnetometerData.MagneticField.Y,
                    AngularVelocityAxisZ = MagnetometerData.MagneticField.Z
                }).ToBase64Data()
            });
        }

        private void Magnometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            this.MagnetometerData = e.Reading;
        }

        public Task Start()
        {
            try
            {
                Magnetometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                Console.WriteLine(fnsEx.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            try
            {
                Magnetometer.Stop();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                Console.WriteLine(fnsEx.Message);
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
