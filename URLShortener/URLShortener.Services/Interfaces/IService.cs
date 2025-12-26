using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URLShortener.Services.Models;

namespace URLShortener.Services.Interfaces
{
    public interface IService<TModel>
        where TModel : AbstractModel
    {
        Task<TModel?> GetByIdAsync(int id);
        Task<TModel> UpdateAsync(TModel model);
        Task DeleteAsync(int id);
        Task<TModel> AddAsync(TModel model);
    }
}
