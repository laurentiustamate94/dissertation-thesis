using System.ComponentModel;
using MobileApp.Messages;
using Xamarin.Forms;

namespace MobileApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object, string>(this, "UpdateLabel", (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    BackgroundServiceLabel.Text = e;
                });
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, "CancelledMessage", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    BackgroundServiceLabel.Text = "Not sending data anymore! You have to restart the application!";
                });
            });
        }

        protected override void OnAppearing()
        {
            var message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(message, "StartLongRunningTaskMessage");

            base.OnAppearing();
        }
    }
}
