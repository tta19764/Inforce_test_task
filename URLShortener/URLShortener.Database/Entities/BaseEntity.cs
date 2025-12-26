using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.Database.Entities
{
    public abstract class BaseEntity (int id)
    {
        public int Id { get; set; } = id;
    }
}
