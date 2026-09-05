using Vasko.Blazor.Components;
using Vasko.Blazor.Services;
using Vasko.U1.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Регистрация HttpClient для API (используем HTTPS, так как API на 7002)
builder.Services.AddHttpClient<IProductService<Dish>, ApiProductService>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7002/api/dishes");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Временно отключаем перенаправление на HTTPS, чтобы Blazor работал через HTTP
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();