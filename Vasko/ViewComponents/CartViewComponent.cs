using Microsoft.AspNetCore.Mvc;
using Vasko.Extensions;
using Vasko.U1.Domain.Models;

namespace Vasko.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(cart);
        }
    }
}