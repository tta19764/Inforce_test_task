using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;
using URLShortener.Services.Models;
using URLShortener.WebApi.Controllers;
using URLShortener.WebApi.Models.Dtos;

namespace URLShortener.Tests.Controllers
{
    public class URLShortenerAuthControllerTests
    {
        private readonly Mock<IAuthService> authServiceMock = new();
        private readonly Mock<IUserService> userServiceMock = new();

        private AuthController CreateController()
            => new(authServiceMock.Object, userServiceMock.Object);

        [Fact]
        public async Task Login_InvalidCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<UserDto>()))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = CreateController();

            // Act
            var result = await controller.Login(new UserDto());

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldReturnOk()
        {
            // Arrange
            authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(CreateToken());

            var controller = CreateController();

            // Act
            var result = await controller.Login(new UserDto());

            // Assert
            var ok = result.Result.ShouldBeOfType<OkObjectResult>();
            ok.Value.ShouldBeAssignableTo<TokenResponseDto>();
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            authServiceMock
                .Setup(s => s.RefreshTokensAsync(It.IsAny<RefreshTokenRequestDto>()))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = CreateController();

            // Act
            var result = await controller.RefreshToken(new RefreshTokenRequestDto());

            // Assert
            result.Result.ShouldBeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task RefreshToken_ValidToken_ShouldReturnOk()
        {
            // Arrange
            authServiceMock
                .Setup(s => s.RefreshTokensAsync(It.IsAny<RefreshTokenRequestDto>()))
                .ReturnsAsync(CreateToken());

            var controller = CreateController();

            // Act
            var result = await controller.RefreshToken(new RefreshTokenRequestDto());

            // Assert
            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ShouldReturnBadRequest()
        {
            // Arrange
            userServiceMock
                .Setup(s => s.AddAsync(It.IsAny<UserModel>()))
                .ThrowsAsync(new InvalidOperationException());

            var controller = CreateController();

            // Act
            var result = await controller.Register(new RegisterDto());

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_LoginFailsAfterCreate_ShouldReturnBadRequest()
        {
            // Arrange
            userServiceMock
                .Setup(s => s.AddAsync(It.IsAny<UserModel>()))
                .ReturnsAsync(new UserModel());

            authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<UserDto>()))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = CreateController();

            // Act
            var result = await controller.Register(new RegisterDto());

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_Success_ShouldReturnOk()
        {
            // Arrange
            userServiceMock
                .Setup(s => s.AddAsync(It.IsAny<UserModel>()))
                .ReturnsAsync(new UserModel());

            authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<UserDto>()))
                .ReturnsAsync(CreateToken());

            var controller = CreateController();

            // Act
            var result = await controller.Register(new RegisterDto());

            // Assert
            result.Result.ShouldBeOfType<OkObjectResult>();
        }

        private static TokenResponseDto CreateToken()
            => new()
            {
                AccessToken = "access",
                RefreshToken = "refresh"
            };
    }
}
