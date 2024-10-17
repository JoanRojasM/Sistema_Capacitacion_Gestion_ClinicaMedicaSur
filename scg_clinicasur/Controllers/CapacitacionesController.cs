using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

namespace scg_clinicasur.Controllers
{
    public class CapacitacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CapacitacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var capacitaciones = _context.Capacitaciones.Include(t => t.Usuario).ThenInclude(u => u.roles).ToList();
            return View(capacitaciones);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var capacitacion = _context.Capacitaciones.FirstOrDefault(e => e.id_capacitacion == id);
            if (capacitacion == null)
            {
                return NotFound();
            }
            return View(capacitacion);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var usuarios = _context.Usuarios
                         .Include(u => u.roles)
                         .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
                         .ToList();
            var usuariosConRoles = usuarios.Select(u => new
            {
                id_usuario = u.id_usuario,
                DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosConRoles, "id_usuario", "DisplayText");
            return View();
        }

        // Crear una nueva capacitacion (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Capacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(capacitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var usuarios = _context.Usuarios
                          .Include(u => u.roles)
                          .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
                          .ToList();
            var usuariosConRoles = usuarios.Select(u => new
            {
                id_usuario = u.id_usuario,
                DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosConRoles, "id_usuario", "DisplayText");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var capacitacion = _context.Capacitaciones.Find(id);
            if (capacitacion == null)
            {
                return NotFound();
            }
            ViewData["Usuarios"] = new SelectList(
    _context.Usuarios
        .Include(u => u.roles) // Incluir la relación con el rol
        .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
        .ToList()
        .Select(u => new // Crear una lista anónima con nombre y rol
        {
            id_usuario = u.id_usuario,
            DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
        }),
    "id_usuario", "DisplayText", capacitacion.id_usuario);

            return View(capacitacion);

        }

        // Editar capacitacion (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Capacitacion capacitacion)
        {
            if (id != capacitacion.id_capacitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(capacitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuarios"] = new SelectList(
    _context.Usuarios
        .Include(u => u.roles) // Incluir la relación con el rol
        .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
        .ToList()
        .Select(u => new // Crear una lista anónima con nombre y rol
        {
            id_usuario = u.id_usuario,
            DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
        }),
    "id_usuario", "DisplayText", capacitacion.id_usuario);

            return View(capacitacion);

        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Usamos Include para cargar la entidad relacionada Usuario
            var capacitacion = await _context.Capacitaciones
                .Include(e => e.Usuario) // Asegura que el Usuario está cargado
                .FirstOrDefaultAsync(m => m.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        // Eliminar evaluación (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var capacitacion = _context.Capacitaciones.Find(id);
            _context.Capacitaciones.Remove(capacitacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
