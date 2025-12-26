using Microsoft.EntityFrameworkCore;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Enums;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;

namespace URLShortener.Services.Database.Servicies
{
    public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
    {
        private readonly IUserRepository userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IPasswordHasher passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));

        public Task<UserModel> AddAsync(UserModel user)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrEmpty(user.Username);
            ArgumentException.ThrowIfNullOrEmpty(user.Password);

            return AddInternalAsync(
                new User()
                {
                    Username = user.Username,
                    PasswordHash = this.passwordHasher.Hash(user.Password),
                    NickName = user.Nickname,
                    AccountTypeId = (int)user.AccountType
                });
        }

        public Task DeleteAsync(int id, int userId)
        {

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

            return DeleteInternalAsync(id, userId);
        }

        public Task<UserModel?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            return GetByIdInternalAsync(id);
        }

        public Task<UserModel> UpdateAsync(UserModel user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return UpdateInternalAsync(new User(user.Id)
            {
                Username = user.Username,
                PasswordHash = this.passwordHasher.Hash(user.Password),
                NickName = user.Nickname,
                AccountTypeId = (int)user.AccountType
            });
        }

        private async Task<UserModel> AddInternalAsync(User user)
        {
            try
            {
                var result = await this.userRepository.AddAsync(user);
                return ConvertToModel(result);
            }
            catch (DbUpdateException ex) when
            (ex.InnerException?.Message.Contains("UNIQUE") == true ||
            ex.Message.Contains("UNIQUE"))
            {
                throw new InvalidOperationException("User with this login already exists.", ex);
            }
        }

        private async Task<UserModel?> GetByIdInternalAsync(int id)
        {
            var result = await this.userRepository.GetByIdAsync(id);

            if (result == null)
            {
                return null;
            }
            else
            {
                return ConvertToModel(result);
            }
        }

        private async Task DeleteInternalAsync(int id, int userId)
        {
            try
            {
                var user = await this.userRepository.GetByIdAsync(userId) ?? throw new InvalidOperationException($"User with Id: {userId} was not found.");
                _ = await this.userRepository.GetByIdAsync(id) ?? throw new InvalidOperationException($"User with Id: {id} was not found.");

                if (Enum.Parse<AccountLevel>(user.AccountType.TypeName) != AccountLevel.Admin)
                {
                    throw new InvalidOperationException($"User with Id: {userId} has no right to delete User with Id: {id}.");
                }

                await this.userRepository.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        private static UserModel ConvertToModel(User user)
        {
            return new UserModel(user.Id)
            {
                Username = user.Username,
                Password = user.PasswordHash,
                Nickname = user.NickName,
                AccountType = Enum.Parse<AccountLevel>(user.AccountType.TypeName),
            };
        }

        public Task<UserModel?> GetUserByLoginAsync(string login)
        {
            ArgumentNullException.ThrowIfNull(login);

            return GetUserByLoginInternalAsync(login);
        }

        private async Task<UserModel?> GetUserByLoginInternalAsync(string login)
        {
            var user = await this.userRepository.GetUserByLoginAsync(login);
            if (user == null)
            {
                return null;
            }

            return ConvertToModel(user);
        }

        private async Task<UserModel> UpdateInternalAsync(User user)
        {
            try
            {
                var result = await this.userRepository.UpdateAsync(user);
                return ConvertToModel(result);
            }
            catch (DbUpdateException ex) when
            (ex.InnerException?.Message.Contains("UNIQUE") == true ||
            ex.Message.Contains("UNIQUE"))
            {
                throw new InvalidOperationException("User with this login already exists.", ex);
            }
        }
    }
}
