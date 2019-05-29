using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Common.Models;
using MobileApp.DataCollectors;
using MobileApp.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileApp.Services
{
    public class TelemetryCollectorService : ITelemetryCollectorService
    {
        private IDataCollector[] DataCollectors { get; }

        public TelemetryCollectorService()
        {
            this.DataCollectors = this.GetAvailableDataCollectors();
        }

        private IDataCollector[] GetAvailableDataCollectors()
        {
            return new IDataCollector[]
            {
                new AccelerometerCollector(),
                new BarometerCollector(),
                new BatteryUsageCollector(),
                new CompassCollector(),
                new MagnetometerCollector(),
            };
        }

        private async Task StartCollectingData()
        {
            foreach (var item in this.DataCollectors)
            {
                await item.Start();
            }
        }

        private async Task StopCollectingData()
        {
            foreach (var item in this.DataCollectors)
            {
                await item.Stop();
            }
        }

        public async Task<IEnumerable<DecryptedData>> GetCollectedData(int secondsToCollect)
        {
            var collectedData = new List<DecryptedData>();

            await this.StartCollectingData();
            await Task.Delay(secondsToCollect);
            await this.StopCollectingData();

            foreach (var item in this.DataCollectors)
            {
                var data = await item.Collect();

                if (data != null)
                {
                    collectedData.Add(data);
                }
            }

            return collectedData;
        }
    }
}
