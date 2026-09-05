using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;

namespace Vasko.Models.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}