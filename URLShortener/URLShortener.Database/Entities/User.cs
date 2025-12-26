using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.Database.Entities
{
    public class User (int id = 0) : BaseEntity (id)
    {
        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        [ForeignKey("AccountType")]
        public int AccountTypeId { get; set; }

        public AccountType AccountType { get; set; } = null!;

        public virtual IList<Url> URLs { get; set; } = [];
    }
}
