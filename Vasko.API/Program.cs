using Microsoft.EntityFrameworkCore;
using Vasko.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Форматирование JSON с отступами для удобного чтения
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Регистрация контекста базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Инициализация базы данных начальными данными
await DbInitializer.SeedData(app);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();