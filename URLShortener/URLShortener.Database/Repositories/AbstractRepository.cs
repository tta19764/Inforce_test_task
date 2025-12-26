using URLShortener.Services.Database.Data;

namespace URLShortener.Services.Database.Repositories
{
    public abstract class AbstractRepository (UrlShortenerDbContext context)
    {
        protected UrlShortenerDbContext Context { get; } = context;
    }
}
