using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CasoPractico2_PrograAvanzada.Models;
using Microsoft.AspNetCore.Http;

namespace CasoPractico2_PrograAvanzada.Controllers
{
    public class EventoesController : Controller
    {
        private readonly EventCorpDbContext _context;

        public EventoesController(EventCorpDbContext context)
        {
            _context = context;
        }

        private bool TienePermiso()
        {
            var rol = HttpContext.Session.GetString("Rol");
            return rol == "Organizador" || rol == "Administrador";
        }

        // GET: Eventoes
        public async Task<IActionResult> Index()
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            var eventos = _context.Eventos.Include(e => e.Categoria).Include(e => e.UsuarioRegistro);
            return View(await eventos.ToListAsync());
        }

        // GET: Eventoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.UsuarioRegistro)
                .FirstOrDefaultAsync(m => m.EventoId == id);

            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // GET: Eventoes/Create
        public IActionResult Create()
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion");
            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            return View();
        }

        // POST: Eventoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventoId,Titulo,Descripcion,CategoriaId,Fecha,Hora,Duracion,Ubicacion,CupoMaximo,FechaRegistro,UsuarioRegistroId")] Evento evento)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", evento.CategoriaId);
            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", evento.UsuarioRegistroId);
            return View(evento);
        }

        // GET: Eventoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", evento.CategoriaId);
            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", evento.UsuarioRegistroId);
            return View(evento);
        }

        // POST: Eventoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventoId,Titulo,Descripcion,CategoriaId,Fecha,Hora,Duracion,Ubicacion,CupoMaximo,FechaRegistro,UsuarioRegistroId")] Evento evento)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (id != evento.EventoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.EventoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", evento.CategoriaId);
            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", evento.UsuarioRegistroId);
            return View(evento);
        }

        // GET: Eventoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.UsuarioRegistro)
                .FirstOrDefaultAsync(m => m.EventoId == id);

            if (evento == null)
                return NotFound();

            return View(evento);
        }

        // POST: Eventoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            var evento = await _context.Eventos.FindAsync(id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.EventoId == id);
        }
    }
}