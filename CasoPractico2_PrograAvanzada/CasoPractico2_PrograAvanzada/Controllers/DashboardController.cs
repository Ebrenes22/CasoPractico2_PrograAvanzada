using CasoPractico2_PrograAvanzada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasoPractico2_PrograAvanzada.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EventCorpDbContext _context;

        public DashboardController(EventCorpDbContext context)
        {
            _context = context;
        }

        private bool EsAdministrador() => HttpContext.Session.GetString("Rol") == "Administrador";

        public async Task<IActionResult> Index()
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            var fechaActual = DateTime.Now;
            var primerDiaMesActual = new DateTime(fechaActual.Year, fechaActual.Month, 1);
            var ultimoDiaMesActual = primerDiaMesActual.AddMonths(1).AddDays(-1);

            var totalEventos = await _context.Eventos.CountAsync();

            var totalUsuarios = await _context.Usuarios.CountAsync();

            var asistentesRegistradosMesActual = await _context.Inscripciones
                .Where(i => i.FechaRegistro >= primerDiaMesActual && i.FechaRegistro <= ultimoDiaMesActual)
                .CountAsync();

            var topEventos = await _context.Inscripciones
                .GroupBy(i => i.EventoId)
                .Select(g => new
                {
                    EventoId = g.Key,
                    CantidadAsistentes = g.Count()
                })
                .OrderByDescending(g => g.CantidadAsistentes)
                .Take(5)
                .ToListAsync();

            var eventosPopulares = new List<EventoPopular>();
            foreach (var item in topEventos)
            {
                var evento = await _context.Eventos
                    .Include(e => e.Categoria)
                    .FirstOrDefaultAsync(e => e.EventoId == item.EventoId);

                if (evento != null)
                {
                    eventosPopulares.Add(new EventoPopular
                    {
                        EventoId = evento.EventoId,
                        Titulo = evento.Titulo,
                        Categoria = evento.Categoria?.Descripcion,
                        Fecha = evento.Fecha,
                        CantidadAsistentes = item.CantidadAsistentes
                    });
                }
            }

            var inscripcionesPorMes = await _context.Inscripciones
                .GroupBy(i => new { Mes = i.FechaRegistro.Month, Año = i.FechaRegistro.Year })
                .Select(g => new
                {
                    Mes = g.Key.Mes,
                    Año = g.Key.Año,
                    Cantidad = g.Count()
                })
                .OrderBy(r => r.Año)
                .ThenBy(r => r.Mes)
                .Take(12)
                .ToListAsync();

            var dashboardViewModel = new DashboardViewModel
            {
                TotalEventos = totalEventos,
                TotalUsuarios = totalUsuarios,
                AsistentesRegistradosMesActual = asistentesRegistradosMesActual,
                EventosPopulares = eventosPopulares,
                InscripcionesPorMes = inscripcionesPorMes.Select(i => new InscripcionesMensuales
                {
                    Mes = i.Mes,
                    Año = i.Año,
                    Cantidad = i.Cantidad
                }).ToList()
            };

            return View(dashboardViewModel);
        }
    }

    public class DashboardViewModel
    {
        public int TotalEventos { get; set; }
        public int TotalUsuarios { get; set; }
        public int AsistentesRegistradosMesActual { get; set; }
        public List<EventoPopular> EventosPopulares { get; set; }
        public List<InscripcionesMensuales> InscripcionesPorMes { get; set; }
    }

    public class EventoPopular
    {
        public int EventoId { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadAsistentes { get; set; }
    }

    public class InscripcionesMensuales
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public int Cantidad { get; set; }
        public string NombreMes => new DateTime(Año, Mes, 1).ToString("MMMM");
    }
}