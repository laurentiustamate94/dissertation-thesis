using System;
using System.Threading;
using System.Threading.Tasks;
using MobileApp.Interfaces;
using Xamarin.Forms;

namespace MobileApp.Services
{
    public class BackgroundTaskService
    {
        private ITelemetryCollectorService TelemetryCollectorService { get; }

        private ITelemetryEncrypterService TelemetryEncrypterService { get; }

        private ITelemetrySenderService TelemetrySenderService { get; }

        private const int SECONDS_TO_WAIT = 300; // 5 minutes

        private const int SECONDS_TO_COLLECT = 10;

        public BackgroundTaskService()
        {
            this.TelemetryCollectorService = DependencyService.Resolve<ITelemetryCollectorService>();
            this.TelemetryEncrypterService = DependencyService.Resolve<ITelemetryEncrypterService>();
            this.TelemetrySenderService = DependencyService.Resolve<ITelemetrySenderService>();
        }

        public async Task ProcessAsync(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        // rescan every 30 minutes
                        await this.TelemetrySenderService.ScanForFogApps(i % 6 == 0);

                        var collectedData = await this.TelemetryCollectorService.GetCollectedData(SECONDS_TO_COLLECT * 1000);
                        var encryptedData = await this.TelemetryEncrypterService.GetDataContracts(collectedData);

                        await this.TelemetrySenderService.SendData(encryptedData);

                        MessagingCenter.Send<object, string>(this, "UpdateLabel", "Latest sync time - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));

                        await Task.Delay(SECONDS_TO_WAIT * 1000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }, token);
        }
    }
}
