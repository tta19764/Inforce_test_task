using Microsoft.EntityFrameworkCore;
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
using URLShortener.Services.Enums;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;

namespace URLShortener.Tests.Services
{
    public class URLShortenerUserServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IPasswordHasher> passwordHasherMock;

        public URLShortenerUserServiceTests()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            passwordHasherMock = new Mock<IPasswordHasher>();
        }

        private UserService CreateService()
            => new(userRepositoryMock.Object, passwordHasherMock.Object);


        [Fact]
        public async Task AddAsync_EmptyUsername_ThrowsArgumentException()
        {
            // Arrnge
            var service = CreateService();

            var model = new UserModel
            {
                Username = "",
                Password = "pwd"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(model));
        }

        [Fact]
        public async Task AddAsync_NullUsername_ThrowsArgumentBullException()
        {
            // Arrnge
            var service = CreateService();

            var model = new UserModel
            {
                Username = null,
                Password = "pwd"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAsync(model));
        }

        [Fact]
        public async Task AddAsync_EmptyPassword_ThrowsArgumentException()
        {
            // Arrange
            var service = CreateService();

            var model = new UserModel
            {
                Username = "user",
                Password = ""
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(model));
        }


        [Fact]
        public async Task AddAsync_NullPassword_ThrowsArgumentNullException()
        {
            // Arrange
            var service = CreateService();

            var model = new UserModel
            {
                Username = "user",
                Password = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddAsync(model));
        }

        [Fact]
        public async Task AddAsync_ValidUser_ReturnsCreatedUser()
        {
            // Arrange
            var service = CreateService();

            passwordHasherMock
                .Setup(p => p.Hash("pwd"))
                .Returns("hashed");

            userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) =>
                {
                    u.Id = 1;
                    u.AccountType = new AccountType() { TypeName = "Regular" };
                    return u;
                });

            var model = new UserModel
            {
                Username = "user",
                Password = "pwd",
                Nickname = "nick",
                AccountType = AccountLevel.Regular
            };

            // Act
            var result = await service.AddAsync(model);

            //Assert
            result.Id.ShouldBe(1);
            result.Username.ShouldBe("user");

            passwordHasherMock.Verify(p => p.Hash("pwd"), Times.Once);
            userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
                u.PasswordHash == "hashed" &&
                u.Username == "user"
            )), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DuplicateUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var service = CreateService();

            passwordHasherMock
                .Setup(p => p.Hash(It.IsAny<string>()))
                .Returns("hash");

            userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .ThrowsAsync(new DbUpdateException("UNIQUE constraint failed"));

            var model = new UserModel
            {
                Username = "user",
                Password = "pwd"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(model));
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange
            var service = CreateService();

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_Found_ReturnsModel()
        {
            // Arrange
            var service = CreateService();

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Username = "user",
                    NickName = "nick",
                    AccountType = new AccountType { TypeName = "Regular" }
                });

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result!.Username.ShouldBe("user");
        }

        [Fact]
        public async Task DeleteAsync_RequestingUserNotFound_Throws()
        {
            // Arrange
            var service = CreateService();

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1, 10));
        }

        [Fact]
        public async Task DeleteAsync_TargetUserNotFound_Throws()
        {
            // Arrange
            var service = CreateService();

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(AdminUser());

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1, 10));
        }

        [Fact]
        public async Task DeleteAsync_NonAdmin_ThrowsForbidden()
        {
            // Arrange
            var service = CreateService();

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(NormalUser());

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new User { Id = 1 });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1, 10));
        }

        [Fact]
        public async Task DeleteAsync_Admin_DeletesUser()
        {
            // Arrange
            var service = CreateService();

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(AdminUser());

            userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new User { Id = 1 });

            // Act
            await service.DeleteAsync(1, 10);

            // Assert
            userRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        private static User AdminUser() =>
            new()
            {
                Id = 10,
                AccountType = new AccountType { TypeName = "Admin" }
            };

        private static User NormalUser() =>
            new()
            {
                Id = 10,
                AccountType = new AccountType { TypeName = "Regular" }
            };
    }
}
