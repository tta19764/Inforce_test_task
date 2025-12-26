using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.Models
{
    public class AboutPageModel
    {
        public string Content { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; }

        public int? LastModifiedById { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
