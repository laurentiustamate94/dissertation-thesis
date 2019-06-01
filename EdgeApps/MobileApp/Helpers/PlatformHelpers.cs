using Communication.Common;
using Xamarin.Forms;

namespace MobileApp.Helpers
{
    public static class PlatformHelpers
    {
        public static PlatformType ToPlatformType(this string data)
        {
            switch (data)
            {
                case Device.Android:
                    return PlatformType.Android;
                case Device.iOS:
                    return PlatformType.iPhone;
                default:
                    return PlatformType.Unknown;
            }
        }
    }
}
