using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Vasko.U1.Domain.Entities;
using Vasko.U1.Domain.Models;

namespace Vasko.Blazor.Services
{
    public class ApiProductService : IProductService<Dish>
    {
        private readonly HttpClient _httpClient;
        private List<Dish> _dishes = new();
        private int _currentPage = 1;
        private int _totalPages = 1;

        public event Action? ListChanged;

        public IEnumerable<Dish> Products => _dishes;
        public int CurrentPage => _currentPage;
        public int TotalPages => _totalPages;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetProducts(int pageNo = 1, int pageSize = 3)
        {
            var uri = _httpClient.BaseAddress?.AbsoluteUri ?? "https://localhost:7002/api/dishes";

            var queryData = new Dictionary<string, string>
            {
                { "pageNo", pageNo.ToString() },
                { "pageSize", pageSize.ToString() }
            };

            var query = QueryString.Create(queryData);
            var result = await _httpClient.GetAsync(uri + query.Value);

            if (result.IsSuccessStatusCode)
            {
                var responseData = await result.Content
                    .ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();

                if (responseData != null && responseData.Success)
                {
                    _currentPage = responseData.Data.CurrentPage;
                    _totalPages = responseData.Data.TotalPages;
                    _dishes = responseData.Data.Items ?? new List<Dish>();
                    ListChanged?.Invoke();
                }
            }
            else
            {
                _dishes = new List<Dish>();
                _currentPage = 1;
                _totalPages = 1;
                ListChanged?.Invoke();
            }
        }
    }
}