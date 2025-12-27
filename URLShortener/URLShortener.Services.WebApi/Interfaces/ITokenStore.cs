using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.JWT;

namespace URLShortener.Services.WebApi.Interfaces
{
    public interface ITokenStore
    {
        string? AccessToken { get; }
        string? RefreshToken { get; }
        string? UserId { get; }

        void Save(TokenResponseDto tokens, string userId);
        void Clear();
    }
}
