using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Services.Interfaces
{
    public interface IUrlShorteningService
    {
        string GenerateShortCode(string originalUrl);
    }
}
