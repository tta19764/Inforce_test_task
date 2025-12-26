using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.JWT
{
    public class LogoutRequestDto
    {
        public int UserId { get; set; }

        public string AccessToken { get; set; } = string.Empty;
    }
}
