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
    public class AboutPageRepository : AbstractRepository, IAboutPageRepository
    {
        private readonly DbSet<AboutPage> dbSet;

        public AboutPageRepository(UrlShortenerDbContext context)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.dbSet = context.Set<AboutPage>();
        }

        public async Task<AboutPage> GetAboutPageInfo()
        {
            return await this.dbSet.FirstOrDefaultAsync() ?? 
                throw new InvalidOperationException("About page info was not found");
        }

        public Task<AboutPage> UpdateAsync(AboutPage page)
        {
            ArgumentNullException.ThrowIfNull(page);

            return UpdateInteranlAsync(page);
        }

        private async Task<AboutPage> UpdateInteranlAsync(AboutPage page)
        {
            var existing = await dbSet.FindAsync(page.Id) ?? 
                throw new KeyNotFoundException($"About Page Info with ID {page.Id} not found.");
            this.Context.Entry(existing).CurrentValues.SetValues(page);
            await this.Context.SaveChangesAsync();
            return existing;
        }
    }
}
