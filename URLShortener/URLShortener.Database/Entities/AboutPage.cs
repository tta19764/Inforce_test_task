using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.Services.Database.Entities
{
    public class AboutPage
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; }

        [ForeignKey("LastModifiedBy")]
        public int? LastModifiedById { get; set; }
        public User? LastModifiedBy { get; set; }
    }
}
