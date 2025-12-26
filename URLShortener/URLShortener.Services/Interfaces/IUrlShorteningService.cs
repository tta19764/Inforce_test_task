namespace URLShortener.Services.Interfaces
{
    public interface IUrlShorteningService
    {
        string GenerateShortCode(string originalUrl);
    }
}
