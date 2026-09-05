//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Vasko.Models.Services;

//namespace Vasko.Controllers
//{
//    public class ProductController(IProductService pService, ICategoryService cService) : Controller
//    {

//        public async Task<IActionResult> Index(string category)
//        {
//            ViewData["categories"] = await cService.GetCategoryListAsync();
//            var dishes = await pService.GetProductListAsync(category);
//            return View(dishes.Data.Items);
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Vasko.Models.Services;
using Vasko.U1.Domain.Entities;

namespace Vasko.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [Route("Catalog")]
        [Route("Catalog/{category}")]
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            // Получить список категорий
            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (!categoriesResponse.Success)
                return NotFound(categoriesResponse.ErrorMessage);

            ViewData["categories"] = categoriesResponse.Data;

            // Определить имя текущей категории для отображения
            var currentCategory = category == null ? "Все" : categoriesResponse.Data.FirstOrDefault(c => c.NormalizedName == category)?.Name;
            ViewData["currentCategory"] = currentCategory ?? "Все";

            // Получить список блюд с фильтрацией и пагинацией
            var productResponse = await _productService.GetProductListAsync(category, pageNo);

            if (!productResponse.Success || productResponse.Data == null)
            {
                ViewData["Error"] = productResponse.ErrorMessage ?? "Данные не найдены";
                return View(new ListModel<Dish> { Items = new List<Dish>(), CurrentPage = 1, TotalPages = 1 });
            }

            // Формируем returnUrl для кнопки "В корзину" (с проверкой на null)
            var request = HttpContext?.Request;
            var returnUrl = request != null
                ? request.Path + request.QueryString.ToUriComponent()
                : "/Product/Index";
            ViewData["returnUrl"] = returnUrl;

            return View(productResponse.Data);
        }
    }
}

