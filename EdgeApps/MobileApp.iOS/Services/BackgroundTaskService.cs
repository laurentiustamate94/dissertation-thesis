using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using MobileApp.Messages;
using MobileApp.Services;
using UIKit;
using Xamarin.Forms;

namespace MobileApp.iOS.Services
{
    public class BackgroundTaskService
    {
        nint taskId;
        CancellationTokenSource cancellationTokenSource;

        public async Task Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask("LongRunningTask", OnExpiration);

            try
            {
                // Shared code is invoked here
                var sender = new MobileApp.Services.BackgroundTaskService();
                await sender.ProcessAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // only used to stop execution
            }
            finally
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    var message = new CancelledMessage();
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "CancelledMessage")
                    );
                }
            }

            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        void OnExpiration()
        {
            cancellationTokenSource.Cancel();
        }
    }
}