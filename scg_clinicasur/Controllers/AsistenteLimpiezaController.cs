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

        // Método para mostrar las evaluaciones del usuario actual
        public async Task<IActionResult> Evaluaciones()
        {
            // Obtener el nombre del usuario de la sesión
            var userName = HttpContext.Session.GetString("UserName");
            // Obtener el ID del usuario actual desde la sesión
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Filtrar las evaluaciones por el ID del usuario actual
            var evaluaciones = await _context.Evaluaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();
            return View(evaluaciones);
        }

        public async Task<IActionResult> Capacitaciones()
        {
            // Obtener el nombre del usuario de la sesión
            var userName = HttpContext.Session.GetString("UserName");
            // Obtener el ID del usuario actual desde la sesión
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Filtrar las evaluaciones por el ID del usuario actual
            var capacitaciones = await _context.Capacitaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();
            return View(capacitaciones);
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarCancelacion(int capacitacionId)
        {
            // Obtener el ID del usuario actual desde la sesión
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Obtener los detalles del usuario
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Obtener los detalles de la capacitación
            var capacitacion = await _context.Capacitaciones.FindAsync(capacitacionId);
            if (capacitacion == null)
            {
                return NotFound("Capacitación no encontrada.");
            }

            // Configuración del cliente SMTP
            var smtpClient = new SmtpClient("smtp.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), //En donde dice ### colocar la contraseña real del correo electrónico seleccionado para que funcione la acción
                EnableSsl = true,
            };

            // Construcción del mensaje de correo electrónico
            var mailMessage = new MailMessage
            {
                From = new MailAddress("daharoni90459@ufide.ac.cr"),
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

            // Dirección de correo del administrador
            mailMessage.To.Add("daharoni90459@ufide.ac.cr");

            // Enviar el correo electrónico
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                ViewBag.Message = "Solicitud de cancelación enviada correctamente.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al enviar el correo: {ex.Message}";
            }

            // Redirigir a la vista de capacitaciones con un mensaje de éxito o error
            return RedirectToAction("Capacitaciones");
        }
    }
}