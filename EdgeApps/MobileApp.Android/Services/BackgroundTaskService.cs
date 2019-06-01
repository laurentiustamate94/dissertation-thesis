using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using MobileApp.Messages;
using Xamarin.Forms;

namespace MobileApp.Droid.Services
{
    [Service]
    public class BackgroundTaskService : Service
    {
        CancellationTokenSource cancellationTokenSource;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    // Shared code is invoked here
                    var sender = new MobileApp.Services.BackgroundTaskService();
                    sender.ProcessAsync(cancellationTokenSource.Token).Wait();
                }
                catch (OperationCanceledException)
                {
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

            }, cancellationTokenSource.Token);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                cancellationTokenSource.Cancel();
            }

            base.OnDestroy();
        }
    }
}