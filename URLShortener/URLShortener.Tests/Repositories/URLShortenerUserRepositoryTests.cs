using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Repositories;

namespace URLShortener.Tests.Repositories
{
    public class URLShortenerUserRepositoryTests
    {
        [Fact]
        public async Task AddUrlWithExistingOriginalUrl_ShouldHandleDuplicates()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UrlRepository(context);
            var user = StaticMethods.CreateUser();
            var url1 = StaticMethods.CreateUrl(0, "https://duplicate.com", "dup1", user);
            var url2 = StaticMethods.CreateUrl(0, "https://duplicate.com", "dup2", user);

            // Act
            await repository.AddAsync(url1);
            var exception = await Should.ThrowAsync<DbUpdateException>(async () => await repository.AddAsync(url2));

            // Assert
            exception.ShouldNotBeNull("Expected exception was not thrown for duplicate OriginalURL.");
        }

        [Fact]
        public async Task AddUser_ShouldAddUserToDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var user = StaticMethods.CreateUser(id: 0);

            // Act
            var addedUser = await repository.AddAsync(user);

            // Assert
            addedUser.ShouldNotBeNull("Added User is null.");
            addedUser.Username.ShouldBe("testuser", "Username does not match.");
        }

        [Fact]
        public async Task GetUserById_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var user = StaticMethods.CreateUser(id: 0);
            user = await repository.AddAsync(user);

            // Act
            var retrievedUser = await repository.GetByIdAsync(user.Id);

            // Assert
            retrievedUser.ShouldNotBeNull("Retrieved User is null.");
            retrievedUser!.Username.ShouldBe("testuser", "Username does not match.");
        }

        [Fact]
        public async Task GetUserByLoginWithEmptyLogin_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(async () => await repository.GetUserByLoginAsync(null));
        }

        [Fact]
        public async Task GetUserByLogin_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var user = StaticMethods.CreateUser(id: 0);
            user = await repository.AddAsync(user);

            // Act
            var retrievedUser = await repository.GetUserByLoginAsync(user.Username);

            // Assert
            retrievedUser.ShouldNotBeNull("Retrieved User is null.");
        }

        [Fact]
        public async Task GetUserByIdWithNegativeId_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await repository.GetByIdAsync(-1));
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var user = StaticMethods.CreateUser(id: 0);
            user = await repository.AddAsync(user);

            // Act
            await repository.DeleteAsync(user.Id);

            // Assert
            var retrievedUser = await context.FindAsync<User>(user.Id);
            retrievedUser.ShouldBeNull("User was not deleted.");
        }

        [Fact]
        public async Task DeleteUserWithNegativeId_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await repository.DeleteAsync(-1));
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyUserInDatabase()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var user = StaticMethods.CreateUser(id: 0);
            user = await repository.AddAsync(user);

            // Act
            user.PasswordHash = "newpassword";
            var updatedUser = await repository.UpdateAsync(user);

            // Assert
            updatedUser.ShouldNotBeNull("Retrieved User is null.");
            updatedUser!.PasswordHash.ShouldBe("newpassword", "Password was not updated.");
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var accountType = await context.AccountTypes.FirstAsync();
            var user1 = StaticMethods.CreateUser(id: 0, username: "user1");
            var user2 = StaticMethods.CreateUser(id: 0, username: "user2");
            user1.AccountType = accountType;
            user2.AccountType = accountType;
            await repository.AddAsync(user1);
            await repository.AddAsync(user2);

            // Act
            var allUsers = await repository.GetAllAsync();

            // Assert
            allUsers.ShouldNotBeNull("Retrieved Users are null.");
            allUsers.Count().ShouldBe(3, "Number of Users retrieved does not match.");
        }

        [Fact]
        public async Task GetPaginatedUsers_ShouldReturnCorrectPage()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var accountType = await context.AccountTypes.FirstAsync();
            for (int i = 1; i <= 10; i++)
            {
                var user = StaticMethods.CreateUser(id: 0, username: $"user{i}");
                user.AccountType = accountType;
                await repository.AddAsync(user);
            }

            // Act
            var paginatedUsers = await repository.GetPaginatedAsync(2, 3);

            // Assert
            paginatedUsers.ShouldNotBeNull("Retrieved Users are null.");
            paginatedUsers.Count().ShouldBe(3, "Number of Users retrieved does not match.");
            paginatedUsers.First().Username.ShouldBe("user3", "First User on the page does not match.");
        }

        [Fact]
        public async Task GetPaginatedUsersWithInvalidPageNumber_ShouldThrowAnException()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);

            // Act & Assert
            await Should.ThrowAsync<ArgumentOutOfRangeException>(async () => await repository.GetPaginatedAsync(0, 5));
        }

        [Fact]
        public async Task AddUserWithExistingUsername_ShouldHandleDuplicates()
        {
            // Arrange
            using var context = StaticMethods.CreateContext();
            var repository = new UserRepository(context);
            var user1 = StaticMethods.CreateUser(id: 0, username: "duplicateUser");
            var user2 = StaticMethods.CreateUser(id: 0, username: "duplicateUser");

            // Act
            await repository.AddAsync(user1);
            var exception = await Should.ThrowAsync<DbUpdateException>(async () => await repository.AddAsync(user2));

            // Assert
            exception.ShouldNotBeNull("Expected exception was not thrown for duplicate Username.");
        }


    }
}
