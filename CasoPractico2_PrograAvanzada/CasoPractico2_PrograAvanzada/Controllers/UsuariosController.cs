using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using CasoPractico2_PrograAvanzada.Models;

namespace CasoPractico2_PrograAvanzada.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly EventCorpDbContext _context;

        public UsuariosController(EventCorpDbContext context)
        {
            _context = context;
        }

        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashed = HashPassword(model.Contrasena);
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario && u.Contrasena == hashed);

                if (usuario != null)
                {
                    HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
                    HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
                    HttpContext.Session.SetString("Rol", usuario.Rol);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Credenciales incorrectas");
            }

            return View(model);
        }

        // ===================== VALIDACIÓN DE ROL =====================
        private bool EsAdministrador() => HttpContext.Session.GetString("Rol") == "Administrador";

        // ===================== CRUD =====================

        public async Task<IActionResult> Index()
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            return View(await _context.Usuarios.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        public IActionResult Create()
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,NombreUsuario,NombreCompleto,Correo,Telefono,Contrasena,Rol")] Usuario usuario)
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                usuario.Contrasena = HashPassword(usuario.Contrasena);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,NombreUsuario,NombreCompleto,Correo,Telefono,Contrasena,Rol")] Usuario usuario)
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            if (id != usuario.UsuarioId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.UsuarioId == id);

                    if (usuarioExistente == null) return NotFound();

                    usuario.Contrasena = usuarioExistente.Contrasena;

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Index", "Home");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}