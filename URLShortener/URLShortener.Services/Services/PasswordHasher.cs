using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;

namespace URLShortener.Services.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> hasher = new();

        public string Hash(string password)
            => hasher.HashPassword(null!, password);

        public bool Verify(string password, string hash)
            => hasher.VerifyHashedPassword(null!, hash, password)
               == PasswordVerificationResult.Success;
    }
}
