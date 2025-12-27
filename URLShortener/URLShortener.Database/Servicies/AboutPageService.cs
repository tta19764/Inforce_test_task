using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Database.Repositories;
using URLShortener.Services.Enums;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;

namespace URLShortener.Services.Database.Servicies
{
    public class AboutPageService(IAboutPageRepository aboutPageRepository, IUserRepository userRepository) : IAboutPageService
    {
        private readonly IAboutPageRepository aboutPageRepository = aboutPageRepository ?? throw new ArgumentNullException(nameof(aboutPageRepository));
        private readonly IUserRepository userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        public async Task<AboutPageModel> GetAboutPageInfoAsync()
        {
            return ConvertToModel(
                await this.aboutPageRepository.GetAboutPageInfo());
        }

        public Task<AboutPageModel> UpdateAsync(string content, int userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(content);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);  

            return UpdateInternalAsync(content, userId);
        }

        private async Task<AboutPageModel> UpdateInternalAsync(string content, int userId)
        {
            var page = await this.aboutPageRepository.GetAboutPageInfo();
            var user = await this.userRepository.GetByIdAsync(userId) ?? throw new InvalidOperationException($"User with Id: {userId} was not found.");

            if (Enum.Parse<AccountLevel>(user.AccountType.TypeName) != AccountLevel.Admin)
            {
                throw new InvalidOperationException($"User with Id: {userId} has no right to update AboutPage.");
            }

            page.CreatedDate = DateTime.UtcNow;
            page.Content = content;
            page.LastModifiedById = user.Id;

            return ConvertToModel(await this.aboutPageRepository.UpdateAsync(page));
        }

        private static AboutPageModel ConvertToModel(AboutPage page)
        {
            return new AboutPageModel()
            {
                Content = page.Content,
                CreatedDate = page.CreatedDate,
                LastModified = page.LastModified,
                LastModifiedBy = page.LastModifiedBy is null ? "Admin" : page.LastModifiedBy.NickName,
                LastModifiedById = page.LastModifiedById,
            };
        }
    }
}
