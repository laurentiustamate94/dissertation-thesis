using System;
using System.Net;
using Communication.Common;
using Communication.Common.Interfaces;
using Communication.Common.Services;
using MobileApp.Interfaces;
using MobileApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApp
{
    public partial class App : Application
    {
        public static readonly string USER_ID = "___YOUR_USER_ID___";

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            DependencyService.Register<ITelemetryCollectorService, TelemetryCollectorService>();
            DependencyService.Register<ITelemetryEncrypterService, TelemetryEncrypterService>();
            DependencyService.Register<ITelemetrySenderService, TelemetrySenderService>();
            DependencyService.Register<IPingService, PingService>();

            DependencyService.Register<IUniqueIdGenerationService, UniqueIdGenerationService>();
            DependencyService.Register<IDataProtector, SecureClient>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
