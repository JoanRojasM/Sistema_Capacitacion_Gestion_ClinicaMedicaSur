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

                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ###
                        EnableSsl = true,
                    };

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

                    mailMessage.To.Add("daharoni90459@ufide.ac.cr");

                    try
                    {
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

                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ###
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("daharoni90459@ufide.ac.cr"),
                        Subject = $"Capacitación Editada: {capacitacion.titulo}",
                        Body = $"Estimado usuario,<br/><br/>" +
                           $"Se han realizado modificaciones en la capacitacion: {capacitacion.titulo}<br/><br/>" +
                           $"Por favor, ingresa al sistema para revisar los detalles.<br/><br/>" +
                           $"Gracias.",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add("daharoni90459@ufide.ac.cr");

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        ViewBag.Message = "Correo de notificación enviado correctamente.";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = $"Error al enviar el correo: {ex.Message}";
                    }

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
                   $"Se ha eliminado la capacitacion: {capacitacion.titulo}<br/><br/>" +
                   $"Gracias por su atención.",
                IsBodyHtml = true,
            };

            mailMessage.To.Add("daharoni90459@ufide.ac.cr");

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                ViewBag.Message = "Correo de notificación enviado correctamente.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al enviar el correo: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Recursos(int id)
        {
            var capacitacion = await _context.Capacitaciones
                                              .Include(c => c.Usuario)
                                              .FirstOrDefaultAsync(c => c.id_capacitacion == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            var recursos = await _context.RecursosAprendizaje
                                         .Where(r => r.id_capacitacion == id)
                                         .ToListAsync();

            ViewData["Capacitacion"] = capacitacion;
            return View(recursos);
        }
        public IActionResult CrearRecurso()
        {
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
            if (ModelState.IsValid)
            {
                try
                {
                    if (recurso.id_capacitacion == 0)
                    {
                        ModelState.AddModelError("id_capacitacion", "Debe seleccionar una capacitación.");
                        return View(recurso);
                    }

                    if (archivo == null && string.IsNullOrEmpty(enlace))
                    {
                        ModelState.AddModelError("", "Debe proporcionar un archivo o un enlace.");
                        return View(recurso);
                    }

                    recurso.fecha_creacion = DateTime.Now;

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

            var capacitaciones = _context.Capacitaciones
                .Select(c => new { c.id_capacitacion, c.titulo })
                .ToList();
            ViewBag.Capacitaciones = new SelectList(capacitaciones, "id_capacitacion", "titulo");

            return View(recurso);
        }
    }
}
