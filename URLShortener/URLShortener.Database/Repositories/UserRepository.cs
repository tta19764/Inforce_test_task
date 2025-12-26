using Microsoft.EntityFrameworkCore;
using URLShortener.Services.Database.Data;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;

namespace URLShortener.Services.Database.Repositories
{
    public class UserRepository : AbstractRepository, IUserRepository
    {
        private readonly DbSet<User> dbSet;

        public UserRepository(UrlShortenerDbContext context) : base(context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.dbSet = context.Set<User>();
        }
        public Task<User> AddAsync(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return AddInternalAsync(entity);
        }

        public Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            return DeleteInternalAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await dbSet
                .Include(user => user.AccountType)
                .ToListAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            return GetByIdInternalAsync(id);
        }

        public Task<IEnumerable<User>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            }

            return GetPaginatedInternalAsync(pageNumber, pageSize);
        }

        public Task<User> UpdateAsync(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return UpdateInternalAsync(entity);
        }

        private async Task<User> AddInternalAsync(User entity)
        {
            var newUser = await dbSet.AddAsync(entity);
            await Context.SaveChangesAsync();

            return newUser.Entity;
        }

        private async Task DeleteInternalAsync(int id)
        {
            if (await dbSet.FindAsync(id) is not User userToDelte)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            dbSet.Remove(userToDelte);
            await Context.SaveChangesAsync();
        }

        private async Task<User?> GetByIdInternalAsync(int id)
        {
            return await dbSet
                .Include(url => url.AccountType)
                .FirstOrDefaultAsync(url => url.Id == id);
        }

        private async Task<IEnumerable<User>> GetPaginatedInternalAsync(int pageNumber, int pageSize)
        {
            return await dbSet
                .Include(url => url.AccountType)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task<User> UpdateInternalAsync(User entity)
        {
            var existing = await dbSet.FindAsync(entity.Id) ?? throw new KeyNotFoundException($"Url with ID {entity.Id} not found.");
            this.Context.Entry(existing).CurrentValues.SetValues(entity);
            await this.Context.SaveChangesAsync();
            return existing;
        }
    }
}
