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
    public class AccelerometerCollector : IDataCollector
    {
        private AccelerometerData AccelerometerData { get; set; }
        private readonly SensorSpeed speed = SensorSpeed.UI;

        public AccelerometerCollector()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        public Task<DecryptedData> Collect()
        {
            if (AccelerometerData == null)
            {
                return Task.FromResult<DecryptedData>(null);
            }

            return Task.FromResult(new DecryptedData()
            {
                DataSource = DataSourceType.Accelerometer,
                PlatformType = Device.RuntimePlatform.ToPlatformType(),
                DataType = DataType.Json,
                Base64Data = JsonConvert.SerializeObject(new
                {
                    AccelerationAxisX = AccelerometerData.Acceleration.X,
                    AccelerationAxisY = AccelerometerData.Acceleration.Y,
                    AccelerationAxisZ = AccelerometerData.Acceleration.Z,
                }).ToBase64Data()
            });
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            this.AccelerometerData = e.Reading;
        }

        public Task Start()
        {
            try
            {
                Accelerometer.Start(speed);
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
                Accelerometer.Stop();
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
