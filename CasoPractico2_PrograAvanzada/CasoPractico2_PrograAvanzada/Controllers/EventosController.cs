using CasoPractico2_PrograAvanzada.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasoPractico2_PrograAvanzada.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventCorpDbContext _context;

        public EventosController(EventCorpDbContext context)
        {
            _context = context;
        }

        private bool TienePermiso()
        {
            var rol = HttpContext.Session.GetString("Rol");
            return rol == "Organizador" || rol == "Administrador";
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            var eventos = _context.Eventos.Include(e => e.Categoria).Include(e => e.UsuarioRegistro);
            return View(await eventos.ToListAsync());
        }

        // GET: Eventos/Details/5
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

        // GET: Eventos/Create
        public IActionResult Create()
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            var categorias = _context.Categorias.ToList();
            if (!categorias.Any())
            {
                TempData["ErrorMessage"] = "No hay categorías disponibles. Debe crear categorías primero.";
                return RedirectToAction(nameof(Index));
            }

            System.Diagnostics.Debug.WriteLine($"Categorías disponibles: {categorias.Count}");
            foreach (var cat in categorias)
            {
                System.Diagnostics.Debug.WriteLine($"Categoría ID: {cat.CategoriaId}, Descripción: {cat.Descripcion}");
            }

            ViewData["CategoriaId"] = new SelectList(categorias, "CategoriaId", "Descripcion");
            return View(new Evento { Fecha = DateTime.Now.Date.AddDays(1), Duracion = 60, CupoMaximo = 10 });
        }

        // POST: Eventos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventoId,Titulo,Descripcion,CategoriaId,Fecha,Hora,Duracion,Ubicacion,CupoMaximo")] Evento evento)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            System.Diagnostics.Debug.WriteLine($"CategoriaId: {evento.CategoriaId}");

            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.CategoriaId == evento.CategoriaId);
            if (!categoriaExiste)
            {
                ModelState.AddModelError("CategoriaId", "La categoría seleccionada no es válida");
            }

            if (evento.Fecha.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Fecha", "La fecha del evento debe ser en el futuro");
            }

            if (evento.Duracion <= 0)
            {
                ModelState.AddModelError("Duracion", "La duración debe ser mayor a 0");
            }

            if (evento.CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo", "El cupo máximo debe ser mayor a 0");
            }

            ModelState.Remove("UsuarioRegistro");
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("UsuarioRegistroId");
            ModelState.Remove("Categoria");
            ModelState.Remove("Disponible");

            if (ModelState.IsValid)
            {
                try
                {
                    evento.FechaRegistro = DateTime.Now;
                    evento.UsuarioRegistroId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

                    _context.Add(evento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al guardar: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al guardar el evento.");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {state.Key}, Errors: {state.Value.Errors.Count}");
                    foreach (var error in state.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"-- Error: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Descripcion", evento.CategoriaId);
            return View(evento);
        }

        // GET: Eventos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var evento = await _context.Eventos
                .Include(e => e.Categoria)
                .FirstOrDefaultAsync(e => e.EventoId == id);

            if (evento == null)
                return NotFound();

            var categorias = await _context.Categorias.ToListAsync();
            if (!categorias.Any())
            {
                TempData["ErrorMessage"] = "No hay categorías disponibles. Debe crear categorías primero.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(categorias, "CategoriaId", "Descripcion", evento.CategoriaId);
            return View(evento);
        }

        // POST: Eventos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventoId,Titulo,Descripcion,CategoriaId,Fecha,Hora,Duracion,Ubicacion,CupoMaximo,FechaRegistro,UsuarioRegistroId")] Evento evento)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            if (id != evento.EventoId)
                return NotFound();

            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.CategoriaId == evento.CategoriaId);
            if (!categoriaExiste)
            {
                ModelState.AddModelError("CategoriaId", "La categoría seleccionada no es válida");
            }

            if (evento.Fecha.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Fecha", "La fecha del evento debe ser en el futuro");
            }

            if (evento.Duracion <= 0)
            {
                ModelState.AddModelError("Duracion", "La duración debe ser mayor a 0");
            }

            if (evento.CupoMaximo <= 0)
            {
                ModelState.AddModelError("CupoMaximo", "El cupo máximo debe ser mayor a 0");
            }

            ModelState.Remove("UsuarioRegistro");
            ModelState.Remove("Categoria");

            if (ModelState.IsValid)
            {
                try
                {
                    if (evento.FechaRegistro == DateTime.MinValue || evento.UsuarioRegistroId == 0)
                    {
                        var eventoOriginal = await _context.Eventos
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.EventoId == id);

                        if (eventoOriginal != null)
                        {
                            evento.FechaRegistro = eventoOriginal.FechaRegistro;
                            evento.UsuarioRegistroId = eventoOriginal.UsuarioRegistroId;
                        }
                    }

                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error de concurrencia: {ex.Message}");

                    if (!EventoExists(evento.EventoId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al actualizar: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar el evento.");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {state.Key}, Errors: {state.Value.Errors.Count}");
                    foreach (var error in state.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"-- Error: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["CategoriaId"] = new SelectList(await _context.Categorias.ToListAsync(), "CategoriaId", "Descripcion", evento.CategoriaId);
            return View(evento);
        }

        // GET: Eventos/Delete/5
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

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TienePermiso())
                return RedirectToAction("Index", "Home");

            var evento = await _context.Eventos.FindAsync(id);
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.EventoId == id);
        }
    }
}