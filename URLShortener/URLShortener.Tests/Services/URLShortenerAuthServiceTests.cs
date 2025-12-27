using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Database.Servicies;
using URLShortener.Services.Interfaces;
using URLShortener.Services.JWT;

namespace URLShortener.Tests.Services
{
    public class URLShortenerAuthServiceTests
    {
        private readonly Mock<IUserRepository> userRepoMock = new();
        private readonly Mock<IPasswordHasher> passwordHasherMock = new();
        private readonly IConfiguration configuration;

        public URLShortenerAuthServiceTests()
        {
            var settings = new Dictionary<string, string>
            {
                ["AppSettings:Token"] = "MySuperSecureAndRandomKeyThatLooksJustAwesomeAndNeedsToBeVeryVeryLong!!!111oneeleven",
                ["AppSettings:Issuer"] = "UrlShortenerApp",
                ["AppSettings:Audience"] = "UrlShortenerApp"
            };

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();
        }

        private AuthService CreateService()
            => new(userRepoMock.Object, passwordHasherMock.Object, configuration);

        [Fact]
        public async Task LoginAsync_UserNotFound_ShouldReturnNull()
        {
            // Arrange
            userRepoMock.Setup(r => r.GetUserByLoginAsync("test"))
                .ReturnsAsync((User?)null);

            var service = CreateService();

            // Act
            var result = await service.LoginAsync(new UserDto
            {
                Username = "test",
                Password = "123"
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var user = CreateUser();

            userRepoMock.Setup(r => r.GetUserByLoginAsync(user.Username))
                .ReturnsAsync(user);

            passwordHasherMock.Setup(h =>
                h.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var service = CreateService();

            // Act
            var result = await service.LoginAsync(new UserDto
            {
                Username = user.Username,
                Password = "wrong"
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ShouldReturnTokens()
        {
            // Arrange
            var user = CreateUser();

            userRepoMock.Setup(r => r.GetUserByLoginAsync(user.Username))
                .ReturnsAsync(user);

            userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            passwordHasherMock.Setup(h =>
                h.Verify(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var service = CreateService();

            // Act
            var result = await service.LoginAsync(new UserDto
            {
                Username = user.Username,
                Password = "correct"
            });

            // Assert
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task RefreshTokensAsync_InvalidRefreshToken_ShouldReturnNull()
        {
            // Arrange
            var user = CreateUser();
            user.RefreshToken = "stored-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            userRepoMock.Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var service = CreateService();

            // Act
            var result = await service.RefreshTokensAsync(new RefreshTokenRequestDto
            {
                UserId = user.Id,
                RefreshToken = "invalid-token"
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task RefreshTokensAsync_ValidRefreshToken_ShouldReturnNewTokens()
        {
            // Arrange
            var user = CreateUser();
            user.RefreshToken = "valid-token";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            userRepoMock.Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            var service = CreateService();

            // Act
            var result = await service.RefreshTokensAsync(new RefreshTokenRequestDto
            {
                UserId = user.Id,
                RefreshToken = "valid-token"
            });

            // Assert
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNullOrWhiteSpace();
            result.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        }

        private static User CreateUser()
        {
            return new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = "hash",
                AccountType = new AccountType { TypeName = "User" }
            };
        }
    }
}
