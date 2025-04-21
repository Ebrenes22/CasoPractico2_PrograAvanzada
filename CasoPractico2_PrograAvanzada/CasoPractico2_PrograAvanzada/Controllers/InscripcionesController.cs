using CasoPractico2_PrograAvanzada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasoPractico2_PrograAvanzada.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly EventCorpDbContext _context;

        public InscripcionesController(EventCorpDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Usuarios");
            int userId = string.IsNullOrEmpty(userIdStr) ? 0 : int.Parse(userIdStr);

            var eventos = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.UsuarioRegistro)
                .ToListAsync();


            var conteos = await _context.Inscripciones
                .GroupBy(i => i.EventoId)
                .Select(g => new { EventoId = g.Key, Cant = g.Count() })
                .ToListAsync();

            var misEventos = userId > 0
                ? await _context.Inscripciones
                    .Where(i => i.UsuarioId == userId)
                    .Select(i => i.EventoId)
                    .ToListAsync()
                : new List<int>();

            var hoy = DateTime.Today;


            foreach (var e in eventos)
            {
                var inscritos = conteos
                    .FirstOrDefault(c => c.EventoId == e.EventoId)?.Cant ?? 0;

                bool cupoDisponible = inscritos < e.CupoMaximo;
                bool fechaValida = e.Fecha.Date >= hoy;
                bool yaInscrito = misEventos.Contains(e.EventoId);

                e.Disponible = cupoDisponible
                               && fechaValida
                               && !yaInscrito;
            }

            return View(eventos);
        }

        [HttpGet]
        public async Task<IActionResult> Inscribirse(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                return RedirectToAction("Login", "Usuarios");

            int userId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return NotFound();

            var inscritosCount = await _context.Inscripciones
                .CountAsync(i => i.EventoId == id);

            if (inscritosCount >= evento.CupoMaximo || evento.Fecha.Date < DateTime.Today)
            {
                TempData["Error"] = "Este evento ya no está disponible.";
                return RedirectToAction(nameof(Index));
            }

            var eventosDelUsuario = await _context.Inscripciones
                .Include(i => i.Evento)
                .Where(i => i.UsuarioId == userId)
                .Select(i => i.Evento)
                .ToListAsync();

            foreach (var e in eventosDelUsuario)
            {

                if (e.Fecha.Date == evento.Fecha.Date)
                {
                    var inicioExistente = e.Hora;
                    var finExistente = e.Hora.Add(TimeSpan.FromMinutes(e.Duracion));

                    var inicioNuevo = evento.Hora;
                    var finNuevo = evento.Hora.Add(TimeSpan.FromMinutes(evento.Duracion));

                    if (inicioNuevo < finExistente && inicioExistente < finNuevo)
                    {
                        TempData["Error"] = "Ya tienes otro evento inscrito en ese horario.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }


            var ins = new Inscripciones
            {
                UsuarioId = userId,
                EventoId = id,
                FechaRegistro = DateTime.Now
            };

            _context.Inscripciones.Add(ins);
            await _context.SaveChangesAsync();

            TempData["Success"] = "¡Inscripción realizada con éxito!";
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> MisInscripciones()
        {
            string userIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Cuenta");

            int userId = int.Parse(userIdStr);


            var misInscripciones = await _context.Inscripciones
                .Include(i => i.Evento)
                    .ThenInclude(e => e.Categoria)
                .Where(i => i.UsuarioId == userId)
                .ToListAsync();

            return View(misInscripciones);
        }


        [HttpGet]
        public async Task<IActionResult> Desinscribirse(int id)
        {

            string userIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Cuenta");

            int userId = int.Parse(userIdStr);


            var ins = await _context.Inscripciones
                .Include(i => i.Evento)
                .FirstOrDefaultAsync(i => i.InscripcionesId == id && i.UsuarioId == userId);

            if (ins == null)
            {
                TempData["Error"] = "Inscripción no encontrada.";
                return RedirectToAction(nameof(MisInscripciones));
            }

            if (ins.Evento.Fecha.Date <= DateTime.Today)
            {
                TempData["Error"] = "No puedes desinscribirte de eventos ya pasados.";
                return RedirectToAction(nameof(MisInscripciones));
            }

            _context.Inscripciones.Remove(ins);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Te has desinscrito correctamente.";
            return RedirectToAction(nameof(MisInscripciones));
        }
    }
}
