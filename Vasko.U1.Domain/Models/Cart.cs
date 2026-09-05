using Vasko.U1.Domain.Entities;

namespace Vasko.U1.Domain.Models
{
    public class Cart
    {
        /// <summary>
        /// Список объектов в корзине
        /// key - идентификатор объекта
        /// </summary>
        public Dictionary<int, CartItem> CartItems { get; set; } = new();

        /// <summary>
        /// Добавить объект в корзину
        /// </summary>
        public virtual void AddToCart(Dish dish)
        {
            if (CartItems.ContainsKey(dish.Id))
            {
                CartItems[dish.Id].Qty++;
            }
            else
            {
                CartItems.Add(dish.Id, new CartItem { Item = dish, Qty = 1 });
            }
        }

        /// <summary>
        /// Удалить один объект из корзины (уменьшить количество)
        /// </summary>
        public virtual void RemoveItem(int id)
        {
            if (CartItems.ContainsKey(id))
            {
                CartItems[id].Qty--;
                if (CartItems[id].Qty == 0)
                {
                    CartItems.Remove(id);
                }
            }
        }

        /// <summary>
        /// Удалить все объекты из корзины
        /// </summary>
        public virtual void Clear()
        {
            CartItems.Clear();
        }

        /// <summary>
        /// Общее количество объектов в корзине
        /// </summary>
        public int Count => CartItems.Sum(item => item.Value.Qty);

        /// <summary>
        /// Суммарное количество калорий всех объектов в корзине
        /// </summary>
        public double TotalCalories => CartItems.Sum(item => item.Value.Item.Calories * item.Value.Qty);
    }
}