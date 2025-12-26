using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;

namespace URLShortener.Services.Database.Servicies
{
    public class UrlService : IUrlService
    {
        public Task<UrlModel> AddAsync(UrlModel model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UrlModel?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UrlModel> UpdateAsync(UrlModel model)
        {
            throw new NotImplementedException();
        }
    }
}
