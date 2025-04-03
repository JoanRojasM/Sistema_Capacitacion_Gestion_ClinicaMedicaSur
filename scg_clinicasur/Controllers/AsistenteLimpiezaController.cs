using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace scg_clinicasur.Controllers
{
    public class AsistenteLimpiezaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsistenteLimpiezaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Evaluaciones()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var evaluaciones = await _context.Evaluaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();

            return View(evaluaciones);
        }

        public async Task<IActionResult> Capacitaciones()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var capacitaciones = await _context.Capacitaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();

            return View(capacitaciones);
        }

        [HttpGet]
        public async Task<IActionResult> SolicitarCancelacion(int capacitacionId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var capacitacion = await _context.Capacitaciones
                                             .Include(e => e.Usuario)
                                             .FirstOrDefaultAsync(m => m.id_capacitacion == capacitacionId);

            if (capacitacion == null)
            {
                return NotFound("Capacitación no encontrada.");
            }

            // Pasar el modelo correctamente a la vista
            return View(capacitacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarSolicitarCancelacion(int capacitacionId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var capacitacion = await _context.Capacitaciones.FindAsync(capacitacionId);
            if (capacitacion == null)
            {
                return NotFound("Capacitación no encontrada.");
            }

            try
            {
                // Configuración del cliente SMTP
                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("jrojas30463@ufide.ac.cr", "QsEfT0809*"), // Cambiar ### por la contraseña real
                    EnableSsl = true,
                };

                // Crear el mensaje de correo
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("jrojas30463@ufide.ac.cr"),
                    Subject = $"Solicitud de Cancelación de Capacitación: {capacitacion.titulo}",
                    Body = $"Estimado administrador,<br/><br/>" +
                           $"El usuario <strong>{usuario.nombre}</strong> (ID: {usuario.id_usuario}) ha solicitado la cancelación de la siguiente capacitación:<br/><br/>" +
                           $"<strong>Título de la Capacitación:</strong> {capacitacion.titulo}<br/>" +
                           $"<strong>Descripción:</strong> {capacitacion.descripcion}<br/>" +
                           $"<strong>Duración:</strong> {capacitacion.duracion}<br/>" +
                           $"<strong>Fecha de Creación:</strong> {capacitacion.fecha_creacion.ToShortDateString()}<br/><br/>" +
                           $"Para cualquier consulta adicional, puede ponerse en contacto con el usuario a través de su correo electrónico: {usuario.correo}.<br/><br/>" +
                           $"Saludos cordiales,<br/>" +
                           $"Sistema de Gestión de Clínica Sur.",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add("jrojas30463@ufide.ac.cr");

                // Intentar enviar el correo
                await smtpClient.SendMailAsync(mailMessage);

                ViewBag.Message = "Solicitud de cancelación enviada correctamente.";
            }
            catch (Exception ex)
            {
                // Si hay un error al enviar el correo
                ViewBag.Message = $"Error al enviar el correo: {ex.Message}";
            }

            return RedirectToAction("Capacitaciones");
        }
    }
}
