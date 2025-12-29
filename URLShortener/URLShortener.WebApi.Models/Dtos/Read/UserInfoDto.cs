using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.WebApi.Models.Dtos.Read
{
    public class UserInfoDto
    {
        public string UserId { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
