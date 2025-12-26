using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;

namespace URLShortener.Services.Database.Servicies
{
    public class UserService : IUserService
    {
        public Task<UserModel> AddAsync(UserModel model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserModel?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserModel> UpdateAsync(UserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
