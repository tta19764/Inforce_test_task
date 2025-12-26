namespace URLShortener.Services.Models
{
    public class UrlModel (int id = 0) : AbstractModel (id)
    {
        public string OriginalUrl { get; set; } = null!;

        public string ShortenedUrl {  get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ClickCount { get; set; } = 0;

        public int CreatorId { get; set; }
    }
}
