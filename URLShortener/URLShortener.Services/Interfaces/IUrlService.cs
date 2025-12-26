using URLShortener.Services.Models;

namespace URLShortener.Services.Interfaces
{
    public interface IUrlService
    {
        Task<UrlModel?> GetByIdAsync(int id);
        Task DeleteAsync(int id, int userId);
        Task<UrlModel> AddAsync(UrlModel model);
        Task<IEnumerable<UrlModel>> GetAllAsync(int pageNumber = 0, int pageSize = 0);
    }
}
