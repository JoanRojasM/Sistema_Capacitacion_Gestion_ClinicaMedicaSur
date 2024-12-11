using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Net.Mail;
using System.Net;

namespace scg_clinicasur.Controllers
{
    public class CapacitacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CapacitacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            // Obtener el rol del usuario desde la sesión
            var userRole = HttpContext.Session.GetString("UserRole");

            // Verificar si el rol es administrador
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a la vista de acceso denegado
            }

            ViewData["CurrentFilter"] = searchString;

            // Consulta de capacitaciones
            var query = _context.Capacitaciones.Include(c => c.Usuario).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Usuario.nombre.Contains(searchString) ||
                                         c.Usuario.apellido.Contains(searchString));
            }

            var capacitaciones = query.ToList();
            return View(capacitaciones);
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a la página de acceso denegado
            }

            // Buscar la capacitación en la base de datos
            var capacitacion = await _context.Capacitaciones
                                             .Include(c => c.Usuario)
                                             .ThenInclude(u => u.roles)
                                             .FirstOrDefaultAsync(c => c.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            // Obtener usuarios para la lista desplegable
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
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (capacitacion.id_usuario == null)
                    {
                        ModelState.AddModelError("", "El usuario es obligatorio.");
                        return View(capacitacion);
                    }

                    // Establecer la fecha de creación
                    capacitacion.fecha_creacion = DateTime.Now;

                    // Guardar la capacitación en la base de datos
                    _context.Add(capacitacion);
                    await _context.SaveChangesAsync();

                    // Configuración del cliente SMTP
                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ### por la contraseña real
                        EnableSsl = true,
                    };

                    // Crear el mensaje de correo
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("daharoni90459@ufide.ac.cr"),
                        Subject = $"Nueva Capacitación Disponible: {capacitacion.titulo}",
                        Body = $"Estimado usuario,<br/><br/>" +
                               $"Se te ha asignado una nueva capacitación en el sistema.<br/><br/>" +
                               $"Detalles de la capacitación:<br/>" +
                               $"<strong>Título:</strong> {capacitacion.titulo}<br/>" +
                               $"<strong>Descripción:</strong> {capacitacion.descripcion}<br/>" +
                               $"<strong>Duración:</strong> {capacitacion.duracion}<br/>" +
                               $"<strong>Fecha de Creación:</strong> {capacitacion.fecha_creacion.ToShortDateString()}<br/><br/>" +
                               $"Por favor, ingresa al sistema para más detalles.<br/><br/>" +
                               $"Gracias.",
                        IsBodyHtml = true,
                    };

                    // Agregar destinatarios dinámicos
                    var usuarioResponsable = await _context.Usuarios.FindAsync(capacitacion.id_usuario);
                    if (usuarioResponsable != null)
                    {
                        mailMessage.To.Add(usuarioResponsable.correo);
                    }

                    try
                    {
                        // Enviar el correo
                        await smtpClient.SendMailAsync(mailMessage);
                        ViewBag.Message = "Correo de notificación enviado correctamente.";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = $"Error al enviar el correo: {ex.Message}";
                    }

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
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

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
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            if (id != capacitacion.id_capacitacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar solo las propiedades modificadas
                    _context.Entry(capacitacion).Property(c => c.titulo).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.descripcion).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.duracion).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.id_usuario).IsModified = true;
                    _context.Entry(capacitacion).Property(c => c.estado).IsModified = true;

                    await _context.SaveChangesAsync();

                    // Configuración del cliente SMTP para enviar correo
                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ### por la contraseña real
                        EnableSsl = true,
                    };

                    // Crear el mensaje de correo
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("daharoni90459@ufide.ac.cr"),
                        Subject = $"Capacitación Editada: {capacitacion.titulo}",
                        Body = $"Estimado usuario,<br/><br/>" +
                               $"Se han realizado modificaciones en la capacitación: <strong>{capacitacion.titulo}</strong><br/><br/>" +
                               $"Por favor, ingresa al sistema para revisar los detalles.<br/><br/>" +
                               $"Gracias.",
                        IsBodyHtml = true,
                    };

                    // Aquí puedes agregar correos específicos o dinámicos si tienes el correo del usuario responsable
                    mailMessage.To.Add("daharoni90459@ufide.ac.cr");

                    try
                    {
                        // Intentar enviar el correo
                        await smtpClient.SendMailAsync(mailMessage);
                        ViewBag.Message = "Correo de notificación enviado correctamente.";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = $"Error al enviar el correo: {ex.Message}";
                    }

                    // Redirigir a la página principal de capacitaciones tras la actualización
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

            // Recargar las listas desplegables si ocurre un error
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
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

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
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var capacitacion = await _context.Capacitaciones.FindAsync(id);
            if (capacitacion == null)
            {
                return NotFound();
            }

            try
            {
                _context.Capacitaciones.Remove(capacitacion);
                await _context.SaveChangesAsync();

                // Notificación por correo (opcional)
                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ###
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("daharoni90459@ufide.ac.cr"),
                    Subject = $"Capacitación Eliminada: {capacitacion.titulo}",
                    Body = $"Estimado usuario,<br/><br/>" +
                           $"Se ha eliminado la capacitación: <strong>{capacitacion.titulo}</strong>.<br/><br/>" +
                           $"Gracias por su atención.",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add("daharoni90459@ufide.ac.cr");

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log del error de envío de correo (opcional)
                    Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar la capacitación: {ex.Message}");
                return View(capacitacion);
            }
        }
        public async Task<IActionResult> Recursos(int id, string? buscarTitulo)
        {
            var capacitacion = await _context.Capacitaciones
                                              .Include(c => c.Usuario)
                                              .FirstOrDefaultAsync(c => c.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            var recursosQuery = _context.RecursosAprendizaje
                                        .Where(r => r.id_capacitacion == id);

            if (!string.IsNullOrEmpty(buscarTitulo))
            {
                var lowerBuscarTitulo = buscarTitulo.ToLower();
                recursosQuery = recursosQuery.Where(r => r.titulo.ToLower().Contains(lowerBuscarTitulo));
            }

            var recursos = await recursosQuery.ToListAsync();

            ViewData["Capacitacion"] = capacitacion;
            ViewData["buscarTitulo"] = buscarTitulo;

            return View(recursos);
        }

        [HttpGet]
        public IActionResult CrearRecurso()
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            // Cargar la lista de capacitaciones para la lista desplegable
            var capacitaciones = _context.Capacitaciones.ToList();

            var capacitacionesList = capacitaciones.Select(c => new
            {
                id_capacitacion = c.id_capacitacion,
                DisplayText = c.titulo
            }).ToList();

            ViewBag.Capacitaciones = new SelectList(capacitacionesList, "id_capacitacion", "DisplayText");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRecurso(RecursosAprendizaje recurso, IFormFile? archivo, string? enlace)
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que se haya proporcionado un archivo o un enlace
                    if (archivo == null && string.IsNullOrEmpty(enlace))
                    {
                        ModelState.AddModelError("", "Debe proporcionar un archivo o un enlace.");
                        return View(recurso);
                    }

                    recurso.fecha_creacion = DateTime.Now;

                    // Guardar el archivo si se proporciona
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

                    // Guardar el recurso en la base de datos
                    _context.RecursosAprendizaje.Add(recurso);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Recursos", new { id = recurso.id_capacitacion });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar el recurso: {ex.Message}");
                }
            }

            // Recargar las capacitaciones en caso de error
            var capacitaciones = _context.Capacitaciones.ToList();
            var capacitacionesList = capacitaciones.Select(c => new
            {
                id_capacitacion = c.id_capacitacion,
                DisplayText = c.titulo
            }).ToList();

            ViewBag.Capacitaciones = new SelectList(capacitacionesList, "id_capacitacion", "DisplayText");

            return View(recurso);
        }
    }
}
