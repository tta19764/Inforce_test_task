using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Models;

namespace URLShortener.Services.Interfaces
{
    public interface IAboutPageService
    {
        Task<AboutPageModel> GetAboutPageInfo();

        Task<AboutPageModel> UpdateAsunc(string content, int userId);
    }
}
