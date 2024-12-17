using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHost;

        public EvaluacionesController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<IActionResult> Index()
        {
            var evaluaciones = await _context.Evaluaciones
                                             .Include(e => e.Usuario)
                                             .ThenInclude(u => u.roles)
                                             .Include(e => e.Capacitacion)
                                             .ToListAsync();

            return View(evaluaciones);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var usuarios = _context.Usuarios
                           .Where(u => u.id_rol == 1 || u.id_rol == 2)
                           .ToList();

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
        public async Task<IActionResult> Crear([Bind("nombre,descripcion,tiempo_prueba,id_usuario,id_capacitacion")] Evaluacion evaluacion, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                if (file != null && file.Length > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string fileSavePath = Path.Combine(uploadsFolder, fileName);


                    using (FileStream stream = new FileStream(fileSavePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    ViewBag.Message = fileName + "bien subido";
                }

                evaluacion.fecha_creacion = DateTime.Now;
                _context.Add(evaluacion);
                await _context.SaveChangesAsync();


                // Enviar correo
                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ### por contraseña
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
            ViewData["Usuarios"] = _context.Usuarios.ToList();
            return View(evaluacion);
        }
        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Capacitacion)
                                           .FirstOrDefaultAsync(e => e.id_evaluacion == id);

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

            if (evaluacion == null)
            {
                return NotFound();
            }

            // Eliminar la evaluación
            _context.Evaluaciones.Remove(evaluacion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Capacitacion)
                                           .Include(e => e.Usuario)
                                           .FirstOrDefaultAsync(e => e.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

    }

}