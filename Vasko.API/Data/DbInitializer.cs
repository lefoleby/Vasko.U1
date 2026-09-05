using Microsoft.EntityFrameworkCore;
using Vasko.U1.Domain.Entities;

namespace Vasko.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Точечное обновление пути к картинке для Супа-харчо
            var soupDish = context.Dishes.FirstOrDefault(d => d.Name == "Суп-харчо");
            if (soupDish != null)
            {
                soupDish.Image = "https://localhost:7002/Images/soup.jpg";
                await context.SaveChangesAsync();
            }

            // Применяем миграции
            await context.Database.MigrateAsync();

            // Заполнение данными, если база пустая
            if (!context.Categories.Any() && !context.Dishes.Any())
            {
                // Создание категорий
                var categories = new Category[]
                {
                    new Category { Name = "Стартеры", NormalizedName = "starters" },
                    new Category { Name = "Салаты", NormalizedName = "salads" },
                    new Category { Name = "Супы", NormalizedName = "soups" },
                    new Category { Name = "Основные блюда", NormalizedName = "main-dishes" },
                    new Category { Name = "Напитки", NormalizedName = "drinks" },
                    new Category { Name = "Десерты", NormalizedName = "desserts" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var uri = "https://localhost:7002/";

                var dishes = new List<Dish>
                {
                    new Dish
                    {
                        Name = "Суп-харчо",
                        Description = "Очень острый, невкусный",
                        Calories = 200,
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "soups"),
                        Image = uri + "Images/суп.jpg"
                    },
                    new Dish
                    {
                        Name = "Борщ",
                        Description = "Много сала, без сметаны",
                        Calories = 330,
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "soups"),
                        Image = uri + "Images/борщ.jpg"
                    },
                    new Dish
                    {
                        Name = "Котлета помарская",
                        Description = "Хлеб - 80%, Морковь - 20%",
                        Calories = 635,
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "main-dishes"),
                        Image = uri + "Images/котлета.jpg"
                    },
                    new Dish
                    {
                        Name = "Макароны по-флотски",
                        Description = "С охотничьей колбаской",
                        Calories = 524,
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "main-dishes"),
                        Image = uri + "Images/макароны.jpg"
                    },
                    new Dish
                    {
                        Name = "Компот",
                        Description = "Быстро растворимый, 2 литра",
                        Calories = 180,
                        Category = categories.FirstOrDefault(c => c.NormalizedName == "drinks"),
                        Image = uri + "Images/компот.jpg"
                    }
                };

                await context.Dishes.AddRangeAsync(dishes);
                await context.SaveChangesAsync();
            }
        }
    }
}