using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vasko.Controllers;
using Vasko.Models.Services;
using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;

namespace Vasko.Tests
{
    public class ProductControllerTests
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductControllerTests()
        {
            _productService = Substitute.For<IProductService>();
            _categoryService = Substitute.For<ICategoryService>();
        }

        // Проверка: список категорий передаётся во ViewData
        [Fact]
        public async Task IndexPutsCategoriesToViewData()
        {
            // arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 2, Name = "Салаты", NormalizedName = "salads" }
            };

            var productData = new ResponseData<ListModel<Dish>>
            {
                Data = new ListModel<Dish> { Items = new List<Dish>(), CurrentPage = 1, TotalPages = 1 },
                Success = true
            };

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(
                new ResponseData<List<Category>> { Data = categories, Success = true }
            ));

            _productService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(productData));

            var controller = new ProductController(_productService, _categoryService);

            // act
            var result = await controller.Index(null);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultCategories = Assert.IsType<List<Category>>(viewResult.ViewData["categories"]);
            Assert.Equal(2, resultCategories.Count);
        }

        // Проверка: имя текущей категории передаётся во ViewData
        [Fact]
        public async Task IndexSetsCorrectCurrentCategory()
        {
            // arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 2, Name = "Салаты", NormalizedName = "salads" }
            };

            var productData = new ResponseData<ListModel<Dish>>
            {
                Data = new ListModel<Dish> { Items = new List<Dish>(), CurrentPage = 1, TotalPages = 1 },
                Success = true
            };

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(
                new ResponseData<List<Category>> { Data = categories, Success = true }
            ));

            _productService.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(productData));

            var controller = new ProductController(_productService, _categoryService);

            // act
            var result = await controller.Index("soups");

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Супы", viewResult.ViewData["currentCategory"]);
        }

        // Проверка: в случае ошибки возвращается NotFoundObjectResult
        [Fact]
        public async Task IndexReturnsNotFound()
        {
            // arrange
            string errorMessage = "Test error";

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(
                new ResponseData<List<Category>> { Success = false, ErrorMessage = errorMessage }
            ));

            var controller = new ProductController(_productService, _categoryService);

            // act
            var result = await controller.Index(null);

            // assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(errorMessage, notFoundResult.Value);
        }
    }
}