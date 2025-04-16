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
    public class CategoriasController : Controller
    {
        private readonly EventCorpDbContext _context;

        public CategoriasController(EventCorpDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin()
        {
            var rol = HttpContext.Session.GetString("Rol");
            return rol == "Administrador";
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var categorias = _context.Categorias.Include(c => c.UsuarioRegistro);
            return View(await categorias.ToListAsync());
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var categoria = await _context.Categorias
                .Include(c => c.UsuarioRegistro)
                .FirstOrDefaultAsync(m => m.CategoriaId == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario");
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoriaId,Nombre,Descripcion,Estado,FechaRegistro,UsuarioRegistroId")] Categoria categoria)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", categoria.UsuarioRegistroId);
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                return NotFound();

            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", categoria.UsuarioRegistroId);
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoriaId,Nombre,Descripcion,Estado,FechaRegistro,UsuarioRegistroId")] Categoria categoria)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (id != categoria.CategoriaId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.CategoriaId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioRegistroId"] = new SelectList(_context.Usuarios, "UsuarioId", "NombreUsuario", categoria.UsuarioRegistroId);
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (id == null)
                return NotFound();

            var categoria = await _context.Categorias
                .Include(c => c.UsuarioRegistro)
                .FirstOrDefaultAsync(m => m.CategoriaId == id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.CategoriaId == id);
        }
    }
}