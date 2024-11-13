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

        // GET: Capacitacion/Index
        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var query = _context.Capacitaciones
                                .Include(c => c.Usuario)
                                .AsQueryable();

            // Aplicar filtro de búsqueda si se ha proporcionado
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Usuario.nombre.Contains(searchString) ||
                                         c.Usuario.apellido.Contains(searchString));
            }

            var capacitaciones = query.ToList();
            return View(capacitaciones);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var capacitacion = await _context.Capacitaciones
                                             .Include(c => c.Usuario)
                                             .ThenInclude(u => u.roles)
                                             .FirstOrDefaultAsync(e => e.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        // GET: Capacitacion/Crear
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

        // POST: Capacitacion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Capacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    capacitacion.fecha_creacion = DateTime.Now; // Asigna la fecha de creación actual
                    _context.Add(capacitacion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar la capacitación: {ex.Message}");
                }
            }

            // Recargar la lista de usuarios en caso de error
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
            return View(capacitacion);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var capacitacion = await _context.Capacitaciones
                                             .Include(c => c.Usuario) // Incluir el usuario
                                             .FirstOrDefaultAsync(e => e.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            // Cargar usuarios con roles 1 y 2
            ViewData["Usuarios"] = new SelectList(
                _context.Usuarios
                    .Include(u => u.roles)
                    .Where(u => u.id_rol == 1 || u.id_rol == 2)
                    .ToList()
                    .Select(u => new
                    {
                        id_usuario = u.id_usuario,
                        DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")"
                    }),
                "id_usuario", "DisplayText", capacitacion.id_usuario);

            // Lista de opciones para el estado
            ViewData["Estados"] = new SelectList(new[]
            {
                new { Value = "Pendiente", Text = "Pendiente" },
                new { Value = "Completada", Text = "Completada" }
            }, "Value", "Text", capacitacion.estado);

            return View(capacitacion);
        }

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
                try
                {
                    // Adjuntar la entidad y modificar solo las propiedades necesarias
                    _context.Entry(capacitacion).Property(c => c.titulo).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.descripcion).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.duracion).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.id_usuario).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.archivo).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.estado).IsModified = true;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Capacitaciones");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Error de actualización: {ex.InnerException?.Message ?? ex.Message}");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                }
            }

            // Recargar la lista de usuarios y estados en caso de error de validación
            ViewData["Usuarios"] = new SelectList(
                _context.Usuarios
                    .Include(u => u.roles)
                    .Where(u => u.id_rol == 1 || u.id_rol == 2)
                    .ToList()
                    .Select(u => new
                    {
                        id_usuario = u.id_usuario,
                        DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")"
                    }),
                "id_usuario", "DisplayText", capacitacion.id_usuario);

            ViewData["Estados"] = new SelectList(new[]
            {
                new { Value = "Pendiente", Text = "Pendiente" },
                new { Value = "Completada", Text = "Completada" }
            }, "Value", "Text", capacitacion.estado);

            return View(capacitacion);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Cargar la capacitación con el usuario relacionado
            var capacitacion = await _context.Capacitaciones
                                             .Include(e => e.Usuario)
                                             .FirstOrDefaultAsync(m => m.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        // POST: Eliminar Confirmado
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            // Buscar la capacitación por ID
            var capacitacion = await _context.Capacitaciones.FindAsync(id);

            // Validar si la entidad existe antes de eliminar
            if (capacitacion == null)
            {
                return NotFound();
            }

            // Eliminar la capacitación de la base de datos
            _context.Capacitaciones.Remove(capacitacion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
