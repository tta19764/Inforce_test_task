using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;

namespace URLShortener.Services.Services
{
    public sealed class UrlShorteningService : IUrlShorteningService
    {
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int CodeLength = 8;

        public string GenerateShortCode(string originalUrl)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(originalUrl);
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(originalUrl));
            var value = BitConverter.ToUInt64(hashBytes, 0);

            return EncodeBase62(value);
        }

        private static string EncodeBase62(ulong value)
        {
            var chars = new char[CodeLength];

            for (int i = CodeLength - 1; i >= 0; i--)
            {
                chars[i] = Alphabet[(int)(value % 62)];
                value /= 62;
            }

            return new string(chars);
        }
    }
}
