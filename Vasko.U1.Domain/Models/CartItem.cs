using Vasko.U1.Domain.Entities;

namespace Vasko.U1.Domain.Models
{
    public class CartItem
    {
        public Dish Item { get; set; }
        public int Qty { get; set; }
    }
}