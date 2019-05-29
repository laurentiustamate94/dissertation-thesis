using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Communication.Common;
using Communication.Common.Models;
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
