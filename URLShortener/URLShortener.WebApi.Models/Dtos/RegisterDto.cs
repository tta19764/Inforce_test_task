using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Enums;

namespace URLShortener.WebApi.Models.Dtos
{
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nickname { get; set; } = string.Empty;
        public AccountLevel AccountType { get; set; } = AccountLevel.Regular;
    }
}
