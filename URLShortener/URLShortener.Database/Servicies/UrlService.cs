using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Metadata.Ecma335;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Enums;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;
using URLShortener.Services.Services;

namespace URLShortener.Services.Database.Servicies
{
    public class UrlService (IUrlRepository urlRepository, IUserRepository userRepository, IUrlShorteningService urlShorteningService) : IUrlService
    {
        private readonly IUrlRepository urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
        private readonly IUserRepository userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IUrlShorteningService urlShorteningService = urlShorteningService ?? throw new ArgumentNullException(nameof(urlShorteningService));

        public Task<UrlModel> AddAsync(UrlModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrEmpty(model.OriginalUrl))
            {
                throw new ArgumentException("Url cant be null or empty.", nameof(model));
            }

            model.ShortenedUrl = urlShorteningService.GenerateShortCode(model.OriginalUrl);

            var entity = new Url()
            {
                OriginalURL = model.OriginalUrl,
                ShortenedURL = model.ShortenedUrl,
                CreatedAt = DateTime.UtcNow,
                CreatorId = model.CreatorId,
            };

            return AddInternalAsync(entity);
        }

        public Task DeleteAsync(int id, int userId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

            return DeleteIntenalAsync(id, userId);
        }

        public Task<UrlModel?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            return GetByIdInternalAsync(id);
        }

        //public Task<UrlModel> UpdateAsync(UrlModel model)
        //{
        //    ArgumentNullException.ThrowIfNull(model);

        //    if (string.IsNullOrEmpty(model.OriginalUrl))
        //    {
        //        throw new ArgumentException("Url cant be null or empty.", nameof(model));
        //    }

        //    model.ShortenedUrl = urlShorteningService.GenerateShortCode(model.OriginalUrl);

        //    var entity = new Url()
        //    {
        //        OriginalURL = model.OriginalUrl,
        //        ShortenedURL = model.ShortenedUrl,
        //        CreatedAt = DateTime.UtcNow,
        //        CreatorId = model.CreatorId,
        //    };

        //    return UpdateInternalAsync(entity);
        //}

        public async Task<IEnumerable<UrlModel>> GetAllAsync(int pageNumber = 0, int pageSize = 0)
        {
            IEnumerable<Url> entities;

            if(pageNumber <= 0 || pageSize <= 0)
            {
                entities = await this.urlRepository.GetAllAsync();
            }
            else
            {
                entities = await this.urlRepository.GetPaginatedAsync(pageNumber, pageSize);
            }

            return entities.Select(ConvertToModel);
        }

        private async Task<UrlModel> AddInternalAsync(Url entity)
        {
            try
            {
                var created = await this.urlRepository.AddAsync(entity);

                return ConvertToModel(created);
            }
            catch (DbUpdateException ex) when 
            (ex.InnerException?.Message.Contains("UNIQUE") == true || 
            ex.Message.Contains("UNIQUE"))
            {
                throw new InvalidOperationException("URL already exists.", ex);
            }
        }

        private async Task DeleteIntenalAsync(int id, int userId)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId) ?? throw new InvalidOperationException($"User with Id: {userId} was not found.");
                var url = await urlRepository.GetByIdAsync(id) ?? throw new InvalidOperationException($"Url with Id: {userId} was not found.");
                
                if (user.Id == url.CreatorId || Enum.Parse<AccountLevel>(user.AccountType.TypeName) == AccountLevel.Admin)
                {
                    await this.urlRepository.DeleteAsync(id);
                }
                else
                {
                    throw new InvalidOperationException($"User with Id: {userId} has no right to delete Url with Id: {id}.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        private async Task<UrlModel?> GetByIdInternalAsync(int id)
        {
            var foundUrl = await this.urlRepository.GetByIdAsync(id);

            if (foundUrl == null)
            {
                return null;
            }

            foundUrl.ClickCount++;

            var result = await this.urlRepository.UpdateAsync(foundUrl);

            return ConvertToModel(result);
        }

        //private async Task<UrlModel> UpdateInternalAsync(Url entity)
        //{
        //    try
        //    {
        //        var updated = await this.repository.UpdateAsync(entity);

        //        return new UrlModel
        //        {
        //            Id = updated.Id,
        //            OriginalUrl = updated.OriginalURL,
        //            ShortenedUrl = updated.ShortenedURL,
        //            CreatedAt = updated.CreatedAt,
        //            CreatorNickName = updated.Creator.NickName,
        //        };
        //    }
        //    catch (DbUpdateException ex) when
        //    (ex.InnerException?.Message.Contains("UNIQUE") == true ||
        //    ex.Message.Contains("UNIQUE"))
        //    {
        //        throw new InvalidOperationException("URL already exists.", ex);
        //    }
        //}

        private static UrlModel ConvertToModel(Url entity)
        {
            return new UrlModel(entity.Id)
            {
                OriginalUrl = entity.OriginalURL,
                ShortenedUrl = entity.ShortenedURL,
                CreatedAt = entity.CreatedAt,
                CreatorId = entity.Creator.Id,
                CreatorNickName = entity.Creator.NickName,
            };
        }
    }
}
