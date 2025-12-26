using URLShortener.Services.Models;

namespace URLShortener.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel?> GetUserByLoginAsync(string login);
        Task<UserModel?> GetByIdAsync(int id);
        Task<UserModel> AddAsync(UserModel user);
        Task DeleteAsync(int id, int userId);
        Task<UserModel> UpdateAsync(UserModel user);
    }
}
