using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Common.Helpers
{
    public static class DataEncrypterHelpers
    {
        public static string ToBase64Data(this double number)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(number));
        }

        public static string ToBase64Data(this string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
}
