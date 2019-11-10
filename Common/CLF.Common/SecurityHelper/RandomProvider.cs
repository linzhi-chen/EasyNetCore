using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CLF.Common.SecurityHelper
{
    public class RandomProvider
    {
        public static  string GenerateRandom(int length = 32)
        {
            var randomNumber = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
