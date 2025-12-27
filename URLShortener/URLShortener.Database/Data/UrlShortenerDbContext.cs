using Microsoft.EntityFrameworkCore;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Services;

namespace URLShortener.Services.Database.Data
{
    public class UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options, IPasswordHasher passwordHasher) : DbContext(options)
    {
        public DbSet<Url> URLs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AccountType> AccountTypes { get; set; } = null!;
        public DbSet<AboutPage> AboutPages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Url>()
                .HasOne(url => url.Creator)
                .WithMany(user => user.URLs)
                .HasForeignKey(url => url.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(user => user.AccountType)
                .WithMany(accountType => accountType.Users)
                .HasForeignKey(user => user.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Url>()
                .HasIndex(url => url.OriginalURL)
                .IsUnique();

            modelBuilder.Entity<AccountType>()
                .HasIndex(at => at.TypeName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<AccountType>()
                .HasData(
                    new AccountType { Id = 1, TypeName = "Admin" },
                    new AccountType { Id = 2, TypeName = "Regular" }
                );

            modelBuilder.Entity<AboutPage>().HasData(new AboutPage
            {
                Id = 1,
                Content = "This URL Shortener uses Base62 encoding...",
                CreatedDate = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "Admin",
                PasswordHash = passwordHasher.Hash("Admin123$"),
                NickName = "Admin",
                AccountTypeId = 1, 
            });
        }
    }
}
