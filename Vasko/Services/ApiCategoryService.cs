using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;
using Vasko.Models.Services;

namespace Vasko.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var response = await _httpClient.GetAsync("api/categories");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>()
                    ?? new ResponseData<List<Category>> { Success = false, ErrorMessage = "Ошибка чтения API" };
            }
            return new ResponseData<List<Category>> { Success = false, ErrorMessage = "Ошибка чтения API" };
        }
    }
}