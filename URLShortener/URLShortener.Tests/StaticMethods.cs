using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Data;
using URLShortener.Services.Database.Entities;

namespace URLShortener.Tests
{
    public static class StaticMethods
    {
        public static UrlShortenerDbContext CreateContext()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new UrlShortenerDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        public static User CreateUser(int id = 1, string username = "testuser", string password = "password", int accountTypeId = 1)
        {
            return new User
            {
                Id = id,
                Username = username,
                PasswordHash = password,
                AccountTypeId = accountTypeId
            };
        }

        public static Url CreateUrl(string originalUrl, string shortCode, User? user = null)
        {
            return new Url
            {
                OriginalURL = originalUrl,
                ShortenedURL = shortCode,
                CreatedAt = DateTime.UtcNow,
                Creator = user ?? CreateUser()
            };
        }
    }
}
