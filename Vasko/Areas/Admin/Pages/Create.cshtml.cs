using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vasko.U1.Domain.Entities;
using Vasko.Models.Services;

namespace Vasko.Areas.Admin.Pages
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CreateModel(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> OnGet()
        {
            var categoryListData = await _categoryService.GetCategoryListAsync();
            ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _productService.CreateProductAsync(Dish, Image);
            return RedirectToPage("./Index");
        }
    }
}