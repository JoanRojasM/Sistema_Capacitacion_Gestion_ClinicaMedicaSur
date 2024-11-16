using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using scg_clinicasur.Services;

namespace scg_clinicasur.Controllers
{
    public class CapacitacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public CapacitacionesController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Capacitacion/Index
        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var query = _context.Capacitaciones
                                .Include(c => c.Usuario)
                                .AsQueryable();

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

        [HttpGet]
        public IActionResult Crear()
        {
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Capacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (capacitacion.id_usuario == null)
                    {
                        ModelState.AddModelError("", "El usuario es obligatorio.");
                        return View(capacitacion);
                    }

                    capacitacion.fecha_creacion = DateTime.Now;

                    _context.Add(capacitacion);
                    await _context.SaveChangesAsync();

                    // Enviar correo
                    var usuario = await _context.Usuarios.FindAsync(capacitacion.id_usuario);
                    if (usuario != null)
                    {
                        string asunto = "Capacitación próxima, prepárese";
                        string cuerpo = $"Hola {usuario.nombre},<br><br>Se ha programado la capacitación <strong>{capacitacion.titulo}</strong>. Prepárese para asistir.";
                        await _emailService.EnviarCorreoAsync(usuario.correo, asunto, cuerpo);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar la capacitación: {ex.Message}");
                }
            }

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

            return View(capacitacion);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var capacitacion = await _context.Capacitaciones
                                             .Include(c => c.Usuario)
                                             .FirstOrDefaultAsync(e => e.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

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

            ViewData["Estados"] = new SelectList(new[] {
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
                    _context.Entry(capacitacion).Property(c => c.titulo).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.descripcion).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.duracion).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.id_usuario).IsModified = true;
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

            ViewData["Estados"] = new SelectList(new[] {
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

            var capacitacion = await _context.Capacitaciones
                                             .Include(e => e.Usuario)
                                             .FirstOrDefaultAsync(m => m.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var capacitacion = await _context.Capacitaciones.FindAsync(id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            var usuario = capacitacion.Usuario;
            _context.Capacitaciones.Remove(capacitacion);
            await _context.SaveChangesAsync();

            // Enviar correo
            if (usuario != null)
            {
                string asunto = "Capacitación cancelada";
                string cuerpo = $"Hola {usuario.nombre},<br><br>La capacitación <strong>{capacitacion.titulo}</strong> ha sido cancelada.";
                await _emailService.EnviarCorreoAsync(usuario.correo, asunto, cuerpo);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Recursos(int id)
        {
            var capacitacion = await _context.Capacitaciones
                                              .Include(c => c.Usuario)
                                              .FirstOrDefaultAsync(c => c.id_capacitacion == id);

            // Verificar si la capacitación existe
            if (capacitacion == null)
            {
                return NotFound();
            }

            // Obtener los recursos asociados a esa capacitación
            var recursos = await _context.RecursosAprendizaje
                                         .Where(r => r.id_capacitacion == id)
                                         .ToListAsync();

            ViewData["Capacitacion"] = capacitacion;
            return View(recursos);
        }
        public IActionResult CrearRecurso()
        {
            // Obtén las capacitaciones disponibles
            var capacitaciones = _context.Capacitaciones.ToList();

            // Crea un SelectList con las capacitaciones (solo nombre y id)
            var capacitacionesList = capacitaciones.Select(c => new
            {
                id_capacitacion = c.id_capacitacion,
                DisplayText = c.titulo // Asumiendo que el nombre es un campo en la tabla
            }).ToList();

            // Pasa el SelectList a la vista
            ViewBag.Capacitaciones = new SelectList(capacitacionesList, "id_capacitacion", "DisplayText");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRecurso(RecursosAprendizaje recurso, IFormFile? archivo, string? enlace)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validación: se debe seleccionar una capacitación
                    if (recurso.id_capacitacion == 0)
                    {
                        ModelState.AddModelError("id_capacitacion", "Debe seleccionar una capacitación.");
                        return View(recurso);
                    }

                    // Validación: archivo o enlace
                    if (archivo == null && string.IsNullOrEmpty(enlace))
                    {
                        ModelState.AddModelError("", "Debe proporcionar un archivo o un enlace.");
                        return View(recurso);
                    }

                    recurso.fecha_creacion = DateTime.Now;

                    // Procesar archivo
                    if (archivo != null)
                    {
                        var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos");
                        if (!Directory.Exists(carpetaDestino))
                        {
                            Directory.CreateDirectory(carpetaDestino);
                        }

                        var nombreArchivo = Path.GetFileName(archivo.FileName);
                        var rutaArchivo = Path.Combine(carpetaDestino, nombreArchivo);

                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        recurso.archivo = Path.Combine("/archivos", nombreArchivo);
                    }
                    else if (!string.IsNullOrEmpty(enlace))
                    {
                        recurso.enlace = enlace;
                    }

                    _context.RecursosAprendizaje.Add(recurso);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Recursos", new { id = recurso.id_capacitacion });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar el recurso: {ex.Message}");
                }
            }

            // Recargamos la lista de capacitaciones si ocurre un error
            var capacitaciones = _context.Capacitaciones
                .Select(c => new { c.id_capacitacion, c.titulo })
                .ToList();
            ViewBag.Capacitaciones = new SelectList(capacitaciones, "id_capacitacion", "titulo");

            return View(recurso);
        }
    }
}
