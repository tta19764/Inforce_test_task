using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Servicies;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;
using URLShortener.Services.Models;
using URLShortener.WebApi.Models.Dtos;
using URLShortener.WebApi.Models.Dtos.Read;

namespace URLShortener.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
    {
        private readonly IAuthService authService = authService ?? throw new ArgumentNullException(nameof(authService));
        private readonly IUserService userService = userService ?? throw new ArgumentNullException(nameof(userService));

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> Register(RegisterDto request)
        {
            try
            {
                var user = await userService.AddAsync(new UserModel()
                {
                    Username = request.Username,
                    Password = request.Password,
                    Nickname = request.Nickname,
                    AccountType = request.AccountType,
                });

                if (user is null)
                {
                    return BadRequest("Username already exists.");
                }

                return await Login(new UserDto()
                {
                    Username = request.Username,
                    Password = request.Password,
                });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest($"An error occurred while Registering a user: {ex}");
            }
        }
    }
}
