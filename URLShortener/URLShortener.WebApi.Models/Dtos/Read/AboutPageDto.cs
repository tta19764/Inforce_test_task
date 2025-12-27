using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.WebApi.Models.Dtos.Read
{
    public class AboutPageDto
    {
        public string Content { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        public int? LastModifiedById { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
