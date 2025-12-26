using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.Database.Entities
{
    public class Url (int id = 0) : BaseEntity (id)
    {
        public string OriginalURL { get; set; } = null!;
        public string? ShortenedURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ClickCount { get; set; } = 0;

        [ForeignKey("Creator")]
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;
    }
}
