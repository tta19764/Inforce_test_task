using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Data;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;

namespace URLShortener.Services.Database.Repositories
{
    public class UrlRepository : AbstractRepository, IUrlRepository
    {
        private readonly DbSet<Url> dbSet;

        public UrlRepository(UrlShortenerDbContext context) : base(context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.dbSet = context.Set<Url>();
        }

        public Task<Url> AddAsync(Url entity)
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

        public async Task<IEnumerable<Url>> GetAllAsync()
        {
            return await dbSet
                .Include(url => url.Creator)
                .ToListAsync();
        }

        public Task<Url?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            return GetByIdInternalAsync(id);
        }

        public Task<IEnumerable<Url>> GetPaginatedAsync(int pageNumber, int pageSize)
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

        public Task<Url> UpdateAsync(Url entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return UpdateInternalAsync(entity);
        }

        private async Task<Url> AddInternalAsync(Url entity)
        {
            var newUrl = await dbSet.AddAsync(entity);
            await Context.SaveChangesAsync();

            return newUrl.Entity;
        }

        private async Task DeleteInternalAsync(int id)
        {
            if (await dbSet.FindAsync(id) is not Url urlToDelte)
            {
                throw new KeyNotFoundException($"Url with ID {id} not found.");
            }

            dbSet.Remove(urlToDelte);
            await Context.SaveChangesAsync();
        }

        private async Task<Url?> GetByIdInternalAsync(int id)
        {
            return await dbSet
                .Include(url => url.Creator)
                .FirstOrDefaultAsync(url => url.Id == id);
        }

        private async Task<IEnumerable<Url>> GetPaginatedInternalAsync(int pageNumber, int pageSize)
        {
            return await dbSet
                .Include(url => url.Creator)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task<Url> UpdateInternalAsync(Url entity)
        {
            var existing = await dbSet.FindAsync(entity.Id) ?? throw new KeyNotFoundException($"Url with ID {entity.Id} not found.");
            this.Context.Entry(existing).CurrentValues.SetValues(entity);
            await this.Context.SaveChangesAsync();
            return existing;
        }
    }
}
