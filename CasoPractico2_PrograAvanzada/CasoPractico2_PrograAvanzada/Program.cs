using CasoPractico2_PrograAvanzada.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar soporte para sesiones
builder.Services.AddSession();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Eventos",
        Version = "v1",
        Description = "API para consultar eventos"
    });
});

// Configurar DbContext
builder.Services.AddDbContext<EventCorpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventCorpDb")).EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information));

// Configurar serialización de JSON para manejar referencias circulares
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    // Alternativa: Ignorar referencias circulares
    // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Configurar Swagger solo en entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Eventos v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.MapGet("/api/events", async (EventCorpDbContext dbContext) =>
{
    var eventos = await dbContext.Eventos
        .Include(e => e.Categoria)
        .Select(e => new
        {
            e.EventoId,
            e.Titulo,
            e.Descripcion,
            Categoria = e.Categoria.Nombre,
            Fecha = e.Fecha.ToString("yyyy-MM-dd"),
            Hora = e.Hora.ToString(@"hh\:mm"),
            e.Duracion,
            e.Ubicacion,
            e.CupoMaximo,
            FechaRegistro = e.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss")
        })
        .ToListAsync();

    return Results.Ok(eventos);
})
.WithName("GetEvents")
.WithTags("Eventos");

app.MapGet("/api/events/{id}", async (int id, EventCorpDbContext dbContext) =>
{
    var evento = await dbContext.Eventos
        .Include(e => e.Categoria)
        .Include(e => e.UsuarioRegistro)
        .FirstOrDefaultAsync(e => e.EventoId == id);

    if (evento == null)
    {
        return Results.NotFound($"Evento con ID {id} no encontrado");
    }

    return Results.Ok(new
    {
        evento.EventoId,
        evento.Titulo,
        evento.Descripcion,
        Categoria = evento.Categoria.Nombre,
        CategoriaId = evento.CategoriaId,
        Fecha = evento.Fecha.ToString("yyyy-MM-dd"),
        Hora = evento.Hora.ToString(@"hh\:mm"),
        evento.Duracion,
        evento.Ubicacion,
        evento.CupoMaximo,
        FechaRegistro = evento.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"),
        UsuarioRegistro = new
        {
            evento.UsuarioRegistro.UsuarioId,
            Nombre = evento.UsuarioRegistro.NombreCompleto,
            Email = evento.UsuarioRegistro.Correo
        }
    });
})
.WithName("GetEventById")
.WithTags("Eventos");

app.UseRouting();

// Activar uso de sesiones
app.UseSession();

app.UseAuthorization();

// Redirigir al login como inicio por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}");

app.Run();