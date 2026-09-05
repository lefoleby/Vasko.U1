using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Vasko.Data;

    public class DbInItializer 
    {
    public static async Task SetupIdentityAdmin(WebApplication application)
    {
        try
        {
            using var scope = application.Services.CreateScope();
            var userManager = scope
                .ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync("admin@gmail.com");
            if (user == null)
            {
                user = new ApplicationUser();
                await userManager.SetEmailAsync(user, "admin@gmail.com");
                await userManager.SetUserNameAsync(user, user.Email);
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, "123456");


                var claim = new Claim(ClaimTypes.Role, "admin");
                await userManager.AddClaimAsync(user, claim);
            }
        }
        catch (Exception ex)
        {
            // Если БД недоступна — просто логируем ошибку и продолжаем
            System.Diagnostics.Debug.WriteLine($"Ошибка при инициализации admin: {ex.Message}");
        }
    }
}

