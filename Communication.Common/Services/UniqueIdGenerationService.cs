using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Communication.Common.Interfaces;

namespace Communication.Common.Services
{
    public class UniqueIdGenerationService : IUniqueIdGenerationService
    {
        private readonly RNGCryptoServiceProvider generator = null;
        private const string dictionary = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
        private const int maxLength = 64;

        public UniqueIdGenerationService()
        {
            generator = new RNGCryptoServiceProvider();
        }

        private int GetNext(int minimumValue, int maximumValue)
        {
            var randomNumber = new byte[1];
            generator.GetBytes(randomNumber);
            var asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001, 
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            var multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            var range = maximumValue - minimumValue + 1;
            var randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }

        public string GenerateRandomId()
        {
            var ret = new StringBuilder();
            var max = GetNext(1, maxLength);

            for (int i = 0; i < max; i++)
            {
                ret.Append(dictionary[GetNext(0, maxLength - 1)]);
            }

            return ret.ToString();
        }
    }
}
