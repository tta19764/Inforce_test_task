using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.JWT;

namespace URLShortener.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> LoginAsync(UserDto request, CancellationToken cancellationToken = default);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);

        Task<bool> LogoutAsync(LogoutRequestDto logoutRequestDto, CancellationToken cancellationToken = default);
    }
}
