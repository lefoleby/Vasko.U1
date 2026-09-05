using Microsoft.AspNetCore.Mvc;
using Vasko.Extensions;
using Vasko.Models.Services;
using Vasko.U1.Domain.Models;

namespace Vasko.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Cart/Index
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(cart.CartItems);
        }

        // GET: Cart/Add/{id}
        [Route("[controller]/add/{id:int}")]
        public async Task<IActionResult> Add(int id, string returnUrl)
        {
            var data = await _productService.GetProductByIdAsync(id);
            if (data.Success)
            {
                var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                cart.AddToCart(data.Data);
                HttpContext.Session.Set("cart", cart);
            }
            return Redirect(returnUrl);
        }

        // GET: Cart/Remove/{id}
        [Route("[controller]/remove/{id:int}")]
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            cart.RemoveItem(id);
            HttpContext.Session.Set("cart", cart);
            return RedirectToAction("Index");
        }
    }
}