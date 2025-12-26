using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.Models
{
    public abstract class AbstractModel (int id)
    {
        public int Id { get; set; } = id;
    }
}
