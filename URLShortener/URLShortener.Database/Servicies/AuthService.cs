using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;

namespace URLShortener.Services.Database.Servicies
{
    public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration config) : IAuthService
    {
        private readonly IUserRepository userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IPasswordHasher passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        private readonly IConfiguration configuration = config ?? throw new ArgumentNullException(nameof(config));
        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await this.userRepository.GetUserByLoginAsync(request.Username);
            if (user is null)
            {
                return null;
            }

            if (!this.passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return null;
            }

            var user = await ValidateRefreshTokenAsync(int.Parse(request.UserId), request.RefreshToken);
            if (user is null)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await this.userRepository.GetByIdAsync(userId);
            if (user is null || passwordHasher.Verify(refreshToken, user.RefreshToken!)
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = passwordHasher.Hash(refreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await this.userRepository.UpdateAsync(user);
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.AccountType.TypeName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this.configuration["AppSettings:Token"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: this.configuration["AppSettings:Issuer"],
                audience: this.configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
