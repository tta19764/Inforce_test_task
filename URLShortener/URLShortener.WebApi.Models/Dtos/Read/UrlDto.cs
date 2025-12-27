using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.WebApi.Models.Dtos.Read
{
    public class UrlDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = null!;
        public string ShortUrl { get; set; } = null!;
        public int CreatorId { get; set; }
        public string CreatorNickname { get; set; } = string.Empty!;
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
