using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Models;
using scg_clinicasur.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace scg_clinicasur.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string correo, string contraseña)
        {
            // Buscar el usuario en la base de datos con el correo proporcionado e incluir su rol (ignorar mayúsculas y minúsculas)
            var user = _context.Usuarios
                               .Include(u => u.roles) // Cargar el rol asociado al usuario
                               .SingleOrDefault(u => u.correo.ToLower() == correo.ToLower());

            if (user == null)
            {
                // Si el usuario no existe, mostrar un mensaje de error
                ViewBag.ErrorMessage = "El usuario no existe. Por favor, verifica tu correo electrónico.";
                return View();
            }

            // Si el usuario existe, validar la contraseña
            if (user.contraseña != contraseña)
            {
                // Si la contraseña es incorrecta, mostrar un mensaje de error
                ViewBag.ErrorMessage = "Contraseña incorrecta.";
                return View();
            }

            // Guardar el rol en la sesión
            HttpContext.Session.SetString("UserRole", user.roles.nombre_rol);

            // Validar el rol y redirigir según corresponda
            if (user.roles != null)
            {
                switch (user.roles.nombre_rol.ToLower())
                {
                    case "administrador":
                        return RedirectToAction("Index", "Admin");
                    case "asistente_medico":
                        return RedirectToAction("Index", "AsistenteMedico");
                    case "asistente_limpieza":
                        return RedirectToAction("Index", "AsistenteLimpieza");
                    case "doctor":
                        return RedirectToAction("Index", "Doctor");
                    case "paciente":
                        return RedirectToAction("Index", "Paciente");
                    default:
                        return RedirectToAction("Index", "Home"); // Redirigir a una página por defecto
                }
            }
            else
            {
                ViewBag.ErrorMessage = "El usuario no tiene un rol asignado.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string correo)
        {
            // Buscar el usuario en la base de datos por el correo proporcionado
            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.correo == correo);
            if (user != null)
            {
                // Datos de configuración para el correo electrónico
                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587, // Usa el puerto adecuado para tu servidor SMTP
                    Credentials = new NetworkCredential("jrojas30463@ufide.ac.cr", "QsEfT0809*"),
                    EnableSsl = true, // Asegúrate de que SSL está habilitado si es necesario
                };

                // Generar un enlace para restablecer la contraseña
                var resetLink = Url.Action("CambiarContraseña", "Account", new { token = "dummy-token", correo = correo }, Request.Scheme);

                // Construir el mensaje de correo
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("jrojas30463@ufide.ac.cr"),
                    Subject = "Restablecimiento de Contraseña",
                    Body = $"Hola {user.nombre},<br/><br/>" +
                           $"Recibimos una solicitud para restablecer la contraseña de tu cuenta.<br/><br/>" +
                           $"Por favor haz clic en el siguiente enlace para restablecer tu contraseña: <a href='{resetLink}'>Restablecer Contraseña</a><br/><br/>" +
                           $"Si no realizaste esta solicitud, puedes ignorar este correo.",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(correo);

                try
                {
                    // Intentar enviar el correo
                    await smtpClient.SendMailAsync(mailMessage);
                    ViewBag.SuccessMessage = "Se ha enviado un enlace para restablecer tu contraseña al correo proporcionado.";
                }
                catch (Exception ex)
                {
                    // Manejar errores en el envío del correo
                    ViewBag.ErrorMessage = $"Error al enviar el correo: {ex.Message}";
                }
            }
            else
            {
                // Si el usuario no existe, mostrar un mensaje de error
                ViewBag.ErrorMessage = "Correo Electrónico no encontrado. Verifica e intenta nuevamente.";
            }
            return View();
        }

        [HttpGet]
        public IActionResult CambiarContraseña(string token, string correo)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(correo))
            {
                ViewBag.ErrorMessage = "El token o el correo electrónico no son válidos.";
                return View();
            }

            // Crear un modelo para la vista que contenga el correo y el token
            var model = new CambiarContraseña
            {
                Token = token,
                Correo = correo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarContraseña(CambiarContraseña model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar al usuario por su correo electrónico
            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.correo == model.Correo);
            if (user == null)
            {
                ViewBag.ErrorMessage = "El correo electrónico no es válido.";
                return View(model);
            }

            // Actualizar la contraseña del usuario
            user.contraseña = model.NuevaContraseña; // En un entorno real, asegúrate de hashear la contraseña
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Contraseña restablecida con éxito. Ahora puedes iniciar sesión con tu nueva contraseña.";
            return RedirectToAction("Login");
        }
    }
}

