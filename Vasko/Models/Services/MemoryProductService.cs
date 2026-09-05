using Microsoft.AspNetCore.Mvc;
using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;

namespace Vasko.Models.Services
{
    public class MemoryProductService : IProductService
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _config;
        private List<Dish> _dishes;
        private List<Category> _categories;

        public MemoryProductService( ICategoryService categoryService, IConfiguration config)
        {
            _categoryService = categoryService;
            _categories = _categoryService.GetCategoryListAsync().Result.Data;
            _config = config;
            SetupData();
        }

        private void SetupData()
        {
            _dishes = new List<Dish>
            {
                new Dish { Id = 1, Name = "Суп-харчо",
                    Description = "Очень острый, невкусный",
                    Calories = 200, Image = "Images/суп.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("soups")).Id
                },
                new Dish { Id = 2, Name = "Борщ",
                    Description = "Много сала, без сметаны",
                    Calories = 330, Image = "Images/Борщ.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("soups")).Id
                },
                new Dish { Id = 3, Name = "Котлета помарская",
                    Description = "Хлеб - 80%, Морковь - 20%",
                    Calories = 635, Image = "Images/Котлета.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("main-dishes")).Id
                },
                new Dish { Id = 4, Name = "Макароны по-флотски",
                    Description = "С охотничьей колбаской",
                    Calories = 524, Image = "Images/Макароны.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("main-dishes")).Id
                },
                new Dish { Id = 5, Name = "Компот",
                    Description = "Быстро растворимый, 2 литра",
                    Calories = 180, Image = "Images/Компот.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("drinks")).Id
                }
            };
        }

        public Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            // Объект результата
            ResponseData<ListModel<Dish>> result = new();
            // Id категории для фильтрации
            int? categoryId = null;

            // если требуется фильтрация, то найти Id категории
            // с заданным categoryNormalizedName
            if (categoryNormalizedName != null)
            {
                categoryId = _categories
                    .Find(c => c.NormalizedName.Equals(categoryNormalizedName))
                    ?.Id;
            }

            // Выбрать объекты, отфильтрованные по Id категории,
            // если этот Id имеется
            var data = _dishes
                .Where(d => categoryId == null || d.CategoryId.Equals(categoryId))
                .ToList();

            // получить размер страницы из конфигурации
            int pageSize = _config.GetSection("ItemsPerPage").Get<int>();
            // получить общее количество страниц
            int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);

            // Создать модель для отображения списка
            var listData = new ListModel<Dish>()
            {
                //Items = data,
                 Items = data.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            result.Data = listData;

            // Если список пустой
            if (data.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            // Вернуть результат
            return Task.FromResult(result);
        }

        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }
}