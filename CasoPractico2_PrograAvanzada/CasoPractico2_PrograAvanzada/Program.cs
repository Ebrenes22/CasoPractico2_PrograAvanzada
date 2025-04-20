using CasoPractico2_PrograAvanzada.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar soporte para sesiones
builder.Services.AddSession();

// Configurar DbContext
builder.Services.AddDbContext<EventCorpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventCorpDb")).EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Activar uso de sesiones
app.UseSession();

app.UseAuthorization();

// Redirigir al login como inicio por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}");

app.Run();