using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using Vasko.Data;
using Vasko.Middleware;
using Vasko.Models.Services;
using Vasko.Services;

namespace Vasko
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ---> {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Настройка простых паролей (отключаем требования)
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("admin", p => p.RequireClaim(ClaimTypes.Role, "admin"));
            });

            builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });



            // Регистрация HTTP клиентов для API сервисов
            builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:7002/");
            });

            builder.Services.AddHttpClient<IProductService, ApiProductService>(opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:7002/");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseFileLoggerMiddleware();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // Применение миграций и инициализация БД
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при применении миграций: {ex.Message}");
            }

            await DbInItializer.SetupIdentityAdmin(app);

            app.Run();
            Log.CloseAndFlush();
        }
    }
}