using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Metadata.Ecma335;
using URLShortener.Services.Database.Entities;
using URLShortener.Services.Database.Interfaces;
using URLShortener.Services.Interfaces;
using URLShortener.Services.Models;
using URLShortener.Services.Services;

namespace URLShortener.Services.Database.Servicies
{
    public class UrlService (IUrlRepository repository, IUrlShorteningService urlShorteningService) : IUrlService
    {
        private readonly IUrlRepository repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

        public Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return DeleteIntenalAsync(id);
        }

        public Task<UrlModel?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return GetByIdInternalAsync(id);
        }

        public Task<UrlModel> UpdateAsync(UrlModel model)
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

            return UpdateInternalAsync(entity);
        }

        private async Task<UrlModel> AddInternalAsync(Url entity)
        {
            try
            {
                var created = await this.repository.AddAsync(entity);

                return new UrlModel
                {
                    Id = created.Id,
                    OriginalUrl = created.OriginalURL,
                    ShortenedUrl = created.ShortenedURL,
                    CreatedAt = created.CreatedAt,
                    CreatorNickName = created.Creator.NickName,
                };
            }
            catch (DbUpdateException ex) when 
            (ex.InnerException?.Message.Contains("UNIQUE") == true || 
            ex.Message.Contains("UNIQUE"))
            {
                throw new InvalidOperationException("URL already exists.", ex);
            }
        }

        private async Task DeleteIntenalAsync(int id)
        {
            try
            {
                await this.repository.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        private async Task<UrlModel?> GetByIdInternalAsync(int id)
        {
            var foundUrl = await this.repository.GetByIdAsync(id);

            return (foundUrl is null) ?
                null :
                new UrlModel(id)
                {
                    OriginalUrl = foundUrl.OriginalURL,
                    ShortenedUrl = foundUrl.ShortenedURL,
                    CreatedAt = foundUrl.CreatedAt,
                    CreatorId = foundUrl.Creator.Id,
                    CreatorNickName = foundUrl.Creator.NickName,
                };
        }

        private async Task<UrlModel> UpdateInternalAsync(Url entity)
        {
            try
            {
                var updated = await this.repository.UpdateAsync(entity);

                return new UrlModel
                {
                    Id = updated.Id,
                    OriginalUrl = updated.OriginalURL,
                    ShortenedUrl = updated.ShortenedURL,
                    CreatedAt = updated.CreatedAt,
                    CreatorNickName = updated.Creator.NickName,
                };
            }
            catch (DbUpdateException ex) when
            (ex.InnerException?.Message.Contains("UNIQUE") == true ||
            ex.Message.Contains("UNIQUE"))
            {
                throw new InvalidOperationException("URL already exists.", ex);
            }
        }
    }
}
