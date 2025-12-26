using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Database.Entities;

namespace URLShortener.Services.Database.Interfaces
{
    public interface IAboutPageRepository
    {
        Task<AboutPage> UpdateAsync(AboutPage page);

        Task<AboutPage> GetAboutPageInfo();
    }
}
