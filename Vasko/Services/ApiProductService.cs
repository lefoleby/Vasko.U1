using System.Text.Json;
using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;
using Vasko.Models.Services;

namespace Vasko.Services
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var url = $"api/dishes?pageNo={pageNo}";
            if (!string.IsNullOrEmpty(categoryNormalizedName))
            {
                url += $"&category={categoryNormalizedName}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>()
                    ?? new ResponseData<ListModel<Dish>> { Success = false, ErrorMessage = "Ошибка чтения API" };
            }
            return new ResponseData<ListModel<Dish>> { Success = false, ErrorMessage = "Ошибка чтения API" };
        }

        public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/dishes/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseData<Dish>>()
                    ?? new ResponseData<Dish> { Success = false, ErrorMessage = "Ошибка чтения API" };
            }
            return new ResponseData<Dish> { Success = false, ErrorMessage = "Ошибка чтения API" };
        }

        public async Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            var responseData = new ResponseData<Dish>();

            try
            {
                // 1. Отправляем запрос на создание блюда в API
                var response = await _httpClient.PostAsJsonAsync("api/dishes", product);

                if (!response.IsSuccessStatusCode)
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = $"Не удалось создать объект: {response.StatusCode}";
                    return responseData;
                }

                // 2. Получаем созданный объект из ответа API
                var createdDish = await response.Content.ReadFromJsonAsync<Dish>();

                // 3. Если есть изображение — отправляем его в API
                if (formFile != null && createdDish != null)
                {
                    var imageResponse = await SaveImageAsync(createdDish.Id, formFile);
                    if (!imageResponse)
                    {
                        responseData.Success = false;
                        responseData.ErrorMessage = "Не удалось сохранить изображение";
                        return responseData;
                    }

                    // Обновляем URL изображения в объекте
                    createdDish.Image = $"https://localhost:7002/Images/{formFile.FileName}";
                }

                responseData.Data = createdDish;
                responseData.Success = true;
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Ошибка: {ex.Message}";
                return responseData;
            }
        }

        public async Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/dishes/{id}", product);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/dishes/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Вспомогательный метод для сохранения изображения
        private async Task<bool> SaveImageAsync(int id, IFormFile formFile)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = formFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                content.Add(streamContent, "image", formFile.FileName);

                var response = await _httpClient.PostAsync($"api/dishes/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}