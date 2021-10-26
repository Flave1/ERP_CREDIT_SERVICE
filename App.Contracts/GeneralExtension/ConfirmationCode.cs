using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Banking.Contracts.GeneralExtension
{
    public static class ConfirmationCode
    {
        private static Random random = new Random();
        public static string Generate()
        {
            const string chars = "!@#$//||%&*ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class CustomEncoder
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
