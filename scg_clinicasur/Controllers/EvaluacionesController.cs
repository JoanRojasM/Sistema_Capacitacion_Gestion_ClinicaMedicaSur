using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

namespace scg_clinicasur.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaluacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Evaluacion/Index
        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // Consultar las evaluaciones con el usuario y roles relacionados
            var query = _context.Evaluaciones
                                .Include(e => e.Usuario)
                                .ThenInclude(u => u.roles)
                                .AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.Usuario != null &&
                                         (e.Usuario.nombre.Contains(searchString) ||
                                          e.Usuario.apellido.Contains(searchString)));
            }

            var evaluaciones = query.ToList();
            return View(evaluaciones);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Usuario)
                                           .ThenInclude(u => u.roles) // Incluir la relación con roles
                                           .FirstOrDefaultAsync(e => e.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            // Obtener usuarios con roles 1 y 2
            var usuarios = _context.Usuarios
                                   .Include(u => u.roles)
                                   .Where(u => u.id_rol == 1 || u.id_rol == 2)
                                   .ToList();

            // Crear una lista de usuarios con nombre y rol
            var usuariosConRoles = usuarios.Select(u => new
            {
                id_usuario = u.id_usuario,
                DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")"
            }).ToList();

            // Pasar la lista de usuarios a la vista
            ViewBag.Usuarios = new SelectList(usuariosConRoles, "id_usuario", "DisplayText");

            return View();
        }

        // POST: Crear una nueva evaluación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Evaluacion evaluacion)
        {
            if (evaluacion.id_usuario == null)
            {
                ModelState.AddModelError("id_usuario", "Debe seleccionar un usuario responsable.");
            }

            if (ModelState.IsValid)
            {
                // Asignar la fecha de creación actual
                evaluacion.fecha_creacion = DateTime.Now;

                // Agregar la evaluación a la base de datos
                _context.Add(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar la lista de usuarios
            var usuarios = _context.Usuarios
                                   .Include(u => u.roles)
                                   .Where(u => u.id_rol == 1 || u.id_rol == 2)
                                   .ToList();

            var usuariosConRoles = usuarios.Select(u => new
            {
                id_usuario = u.id_usuario,
                DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")"
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosConRoles, "id_usuario", "DisplayText");

            return View(evaluacion);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Usuario) // Incluir la relación con el usuario
                                           .FirstOrDefaultAsync(e => e.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            // Cargar la lista de usuarios con roles 1 y 2
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
                "id_usuario", "DisplayText", evaluacion.id_usuario);

            return View(evaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Evaluacion evaluacion)
        {
            if (id != evaluacion.id_evaluacion)
            {
                return NotFound();
            }

            // Verificar si el modelo es válido
            if (ModelState.IsValid)
            {
                // Buscar la evaluación existente para actualizar
                var evaluacionExistente = await _context.Evaluaciones.FindAsync(id);
                if (evaluacionExistente == null)
                {
                    return NotFound();
                }

                // Actualizar los campos de la evaluación existente
                evaluacionExistente.nombre = evaluacion.nombre;
                evaluacionExistente.descripcion = evaluacion.descripcion;
                evaluacionExistente.tiempo_prueba = evaluacion.tiempo_prueba;
                evaluacionExistente.id_usuario = evaluacion.id_usuario;

                // Guardar los cambios en la base de datos
                _context.Update(evaluacionExistente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar la lista de usuarios en caso de error de validación
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
                "id_usuario", "DisplayText", evaluacion.id_usuario);

            return View(evaluacion);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Cargar la evaluación con el usuario relacionado
            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        // Eliminar evaluación (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var evaluacion = await _context.Evaluaciones.FindAsync(id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            try
            {
                _context.Evaluaciones.Remove(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar la evaluación: {ex.Message}");
                return View(evaluacion);
            }
        }
    }
}
