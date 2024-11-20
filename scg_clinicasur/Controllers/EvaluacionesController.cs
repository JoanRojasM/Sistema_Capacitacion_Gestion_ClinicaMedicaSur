using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Net.Mail;
using System.Net;

namespace scg_clinicasur.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaluacionesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var evaluaciones = await _context.Evaluaciones
                                             .Include(e => e.Usuario)
                                             .ThenInclude(u => u.roles)
                                             .ToListAsync();

            return View(evaluaciones);
        }
        public IActionResult Crear()
        {
            var usuarios = _context.Usuarios.ToList();
            var capacitaciones = _context.Capacitaciones.ToList();

            if (usuarios == null || capacitaciones == null)
            {
                return RedirectToAction("Error");
            }

            ViewData["Usuarios"] = usuarios;
            ViewData["Capacitaciones"] = capacitaciones;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("nombre,descripcion,tiempo_prueba,id_usuario, id_capacitacion")] Evaluacion evaluacion, IFormFile archivo)
        {
            if (ModelState.IsValid)
            {
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

                    evaluacion.archivo = Path.Combine("/archivos", nombreArchivo);
                }

                _context.Add(evaluacion);
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
                    Subject = $"Nueva Evaluación Disponible: {evaluacion.nombre}",
                    Body = $"Estimado usuario,<br/><br/>" +
                       $"Se te ha asignado una nueva evaluación en el sistema.<br/><br/>" +
                       $"Detalles de la capacitación:<br/>" +
                       $"<strong>Título:</strong> {evaluacion.nombre}<br/>" +
                       $"<strong>Descripción:</strong> {evaluacion.descripcion}<br/>" +
                       $"<strong>Duración:</strong> {evaluacion.tiempo_prueba}<br/>" +
                       $"<strong>Fecha de Creación:</strong> {evaluacion.fecha_creacion.ToShortDateString()}<br/><br/>" +
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

            ViewData["Capacitaciones"] = _context.Capacitaciones.ToList();
            return View(evaluacion);
        }
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Capacitacion)
                                           .FirstOrDefaultAsync(m => m.id_evaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "Nombre", evaluacion.id_usuario);
            ViewData["Capacitaciones"] = new SelectList(_context.Capacitaciones, "id_capacitacion", "Nombre", evaluacion.id_capacitacion);

            return View(evaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("id_evaluacion,id_capacitacion,nombre,descripcion,tiempo_prueba,archivo,id_usuario,fecha_creacion")] Evaluacion evaluacion, IFormFile archivo)
        {
            if (id != evaluacion.id_evaluacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (archivo != null && archivo.Length > 0)
                    {
                        var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos", archivo.FileName);

                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        evaluacion.archivo = archivo.FileName;
                    }

                    _context.Update(evaluacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvaluacionExists(evaluacion.id_evaluacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ###
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("daharoni90459@ufide.ac.cr"),
                    Subject = $"Evaluación Editada: {evaluacion.nombre}",
                    Body = $"Estimado usuario,<br/><br/>" +
                       $"Se han realizado modificaciones en la evaluación: {evaluacion.nombre}<br/><br/>" +
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

                return RedirectToAction(nameof(Index));
            }

            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "nombre", evaluacion.id_usuario);
            ViewData["Capacitaciones"] = new SelectList(_context.Capacitaciones, "id_capacitacion", "titulo", evaluacion.id_capacitacion);

            return View(evaluacion);
        }
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.id_evaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.id_evaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var evaluacion = await _context.Evaluaciones.FindAsync(id);
            if (evaluacion != null)
            {
                _context.Evaluaciones.Remove(evaluacion);
                await _context.SaveChangesAsync();
            }

            var smtpClient = new SmtpClient("smtp.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ###
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("daharoni90459@ufide.ac.cr"),
                Subject = $"Evaluación Eliminada: {evaluacion.nombre}",
                Body = $"Estimado usuario,<br/><br/>" +
                   $"Se ha eliminado la evaluación: {evaluacion.nombre}<br/><br/>" +
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

        private bool EvaluacionExists(int id)
        {
            return _context.Evaluaciones.Any(e => e.id_evaluacion == id);
        }
    }
}