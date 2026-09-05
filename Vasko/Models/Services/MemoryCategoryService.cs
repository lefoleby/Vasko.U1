
using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;

namespace Vasko.Models.Services
{
    public class MemoryCategoryService : ICategoryService // ← добавить объявление класса
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            List<Category> categories = [
                new Category {Id=1, Name="Стартеры", NormalizedName="starters"},
                new Category {Id=2, Name="Салаты", NormalizedName="salads"},
                new Category {Id=3, Name="Супы", NormalizedName="soups"},
                new Category {Id=4, Name="Основные блюда", NormalizedName="main-dishes"},
                new Category {Id=5, Name="Напитки", NormalizedName="drinks"},
                new Category {Id=6, Name="Десерты", NormalizedName="desserts"}
            ];
            var result = new ResponseData<List<Category>>()
            {
                Data = categories,
                Success = true
            };
            return Task.FromResult(result);
        }
    }
}