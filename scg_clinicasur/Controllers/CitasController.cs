using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Net.Mail;
using System.Net;

namespace scg_clinicasur.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para verificar roles permitidos
        private bool EsRolPermitido()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole?.ToLower() == "doctor" || userRole?.ToLower() == "administrador";
        }

        public IActionResult Index(string[] estado)
        {
            // Verificar si el usuario tiene rol permitido
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            var citasQuery = _context.Citas
                .Include(c => c.EstadoCita)
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .AsQueryable();

            if (userRole?.ToLower() == "doctor")
            {
                int doctorId = int.Parse(userIdString);
                citasQuery = citasQuery.Where(c => c.IdDoctor == doctorId);
            }

            if (estado != null && estado.Length > 0)
            {
                citasQuery = citasQuery.Where(c => estado.Contains(c.EstadoCita.EstadoNombre.ToLower()));
            }

            var citas = citasQuery.ToList();
            return View(citas);
        }

        public IActionResult Calendario()
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                ViewBag.ErrorMessage = "ID de usuario inválido.";
                return View("Error");
            }

            var citasQuery = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .AsQueryable();

            if (userRole.ToLower() == "doctor")
            {
                citasQuery = citasQuery.Where(c => c.IdDoctor == userId);
            }

            var citas = citasQuery.ToList();
            return View(citas);
        }

        public IActionResult Detalles(int id)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            try
            {
                var cita = _context.Citas
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .Include(c => c.EstadoCita)
                    .SingleOrDefault(c => c.IdCita == id);

                if (cita == null)
                {
                    ViewBag.ErrorMessage = "No se pudo encontrar la cita.";
                    return View("ErrorCita");
                }

                return View(cita);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error interno al acceder a los detalles.";
                ViewBag.ErrorDetails = ex.Message;
                return View("ErrorCita");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            ViewBag.Pacientes = await _context.Usuarios
                .Where(u => u.roles.nombre_rol == "paciente")
                .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                .ToListAsync();

            ViewBag.Doctores = await _context.Usuarios
                .Where(u => u.roles.nombre_rol == "doctor")
                .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cita cita)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                bool existeConflicto = await _context.Citas
                    .AnyAsync(c => c.IdDoctor == cita.IdDoctor
                                && c.FechaInicio < cita.FechaFin
                                && c.FechaFin > cita.FechaInicio);

                if (existeConflicto)
                {
                    ModelState.AddModelError("", "No se puede programar la cita, ya que el doctor tiene un conflicto de horario.");
                    ViewBag.Pacientes = await _context.Usuarios
                        .Where(u => u.roles.nombre_rol == "paciente")
                        .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                        .ToListAsync();

                    ViewBag.Doctores = await _context.Usuarios
                        .Where(u => u.roles.nombre_rol == "doctor")
                        .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                        .ToListAsync();

                    return View(cita);
                }

                cita.FechaCreacion = DateTime.Now;
                _context.Citas.Add(cita);
                await _context.SaveChangesAsync();

                // Obtener nombres de Doctor y Paciente
                var doctor = await _context.Usuarios
                    .Where(u => u.id_usuario == cita.IdDoctor)
                    .Select(u => new { NombreCompleto = u.nombre + " " + u.apellido })
                    .FirstOrDefaultAsync();

                var paciente = await _context.Usuarios
                    .Where(u => u.id_usuario == cita.IdPaciente)
                    .Select(u => new { NombreCompleto = u.nombre + " " + u.apellido })
                    .FirstOrDefaultAsync();

                // Determinar la contraparte (destinatario de la notificación)
                var userIdString = HttpContext.Session.GetString("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userRole))
                {
                    ModelState.AddModelError("", "No se pudo determinar el usuario actual.");
                    return View(cita);
                }

                int userId = int.Parse(userIdString);
                int idDestinatario;
                string mensajeNotificacion;

                if (userId == cita.IdDoctor)
                {
                    // Notificación al paciente
                    idDestinatario = cita.IdPaciente;
                    mensajeNotificacion = $"Estimado {paciente?.NombreCompleto}, el doctor {doctor?.NombreCompleto} ha programado una cita contigo para el {cita.FechaInicio}.";
                }
                else if (userId == cita.IdPaciente)
                {
                    // Notificación al doctor
                    idDestinatario = cita.IdDoctor;
                    mensajeNotificacion = $"Estimado {doctor?.NombreCompleto}, el paciente {paciente?.NombreCompleto} ha programado una cita contigo para el {cita.FechaInicio}.";
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo determinar el destinatario de la notificación.");
                    return View(cita);
                }

                try
                {
                    // Registrar notificación
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC [dbo].[RegistrarNotificacion] @id_usuario = {0}, @titulo = {1}, @mensaje = {2}, @fecha_envio = {3}",
                        idDestinatario,
                        "Cita Agendada",
                        mensajeNotificacion,
                        DateTime.Now
                    );
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al registrar la notificación: {ex.Message}");
                }

                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ### por la contraseña real
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("daharoni90459@ufide.ac.cr"),
                    Subject = "Cita Agendada",
                    Body = $"Estimado usuario,<br/><br/>" +
                           $"Se ha agendado una nueva cita en el sistema.<br/><br/>" +
                           $"Detalles de la cita:<br/>" +
                           $"<strong>Paciente:</strong> {paciente?.NombreCompleto}<br/>" +
                           $"<strong>Doctor:</strong> {doctor?.NombreCompleto}<br/>" +
                           $"<strong>Motivo:</strong> {cita.MotivoCita}<br/>" +
                           $"<strong>Fecha de la Cita:</strong> {cita.FechaInicio}<br/><br/>" +
                           $"Por favor, ingresa al sistema para más detalles.<br/>" +
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

            ViewBag.Pacientes = await _context.Usuarios
                .Where(u => u.roles.nombre_rol == "paciente")
                .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                .ToListAsync();

            ViewBag.Doctores = await _context.Usuarios
                .Where(u => u.roles.nombre_rol == "doctor")
                .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                .ToListAsync();

            return View(cita);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .Include(c => c.EstadoCita)
                .SingleOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                ViewBag.ErrorMessage = "No se pudo encontrar la cita.";
                return View("ErrorCita");
            }

            await CargarDatosParaEditar(cita.IdEstadoCita);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Cita cita)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                bool conflictoHorario = await _context.Citas
                    .AnyAsync(c => c.IdDoctor == cita.IdDoctor
                                   && c.IdCita != cita.IdCita
                                   && (c.FechaInicio < cita.FechaFin && c.FechaFin > cita.FechaInicio));

                if (conflictoHorario)
                {
                    ModelState.AddModelError("FechaInicio", "El horario seleccionado ya está ocupado por otra cita.");
                    await CargarDatosParaEditar(cita.IdEstadoCita);
                    return View(cita);
                }

                try
                {
                    _context.Citas.Update(cita);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "La cita se ha actualizado correctamente.";

                    // Obtener nombres de Doctor y Paciente
                    var doctor = await _context.Usuarios
                        .Where(u => u.id_usuario == cita.IdDoctor)
                        .Select(u => new { NombreCompleto = u.nombre + " " + u.apellido })
                        .FirstOrDefaultAsync();

                    var paciente = await _context.Usuarios
                        .Where(u => u.id_usuario == cita.IdPaciente)
                        .Select(u => new { NombreCompleto = u.nombre + " " + u.apellido })
                        .FirstOrDefaultAsync();

                    // Determinar la contraparte (destinatario de la notificación)
                    var userIdString = HttpContext.Session.GetString("UserId");
                    var userRole = HttpContext.Session.GetString("UserRole");

                    if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userRole))
                    {
                        ModelState.AddModelError("", "No se pudo determinar el usuario actual.");
                        return View(cita);
                    }

                    int userId = int.Parse(userIdString);
                    int idDestinatario;
                    string mensajeNotificacion;

                    if (userId == cita.IdDoctor)
                    {
                        // Notificación al paciente
                        idDestinatario = cita.IdPaciente;
                        mensajeNotificacion = $"Estimado {paciente?.NombreCompleto}, el doctor {doctor?.NombreCompleto} ha modificado una cita contigo para el {cita.FechaInicio}.";
                    }
                    else if (userId == cita.IdPaciente)
                    {
                        // Notificación al doctor
                        idDestinatario = cita.IdDoctor;
                        mensajeNotificacion = $"Estimado {doctor?.NombreCompleto}, el paciente {paciente?.NombreCompleto} ha modificado una cita contigo para el {cita.FechaInicio}.";
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo determinar el destinatario de la notificación.");
                        return View(cita);
                    }

                    try
                    {
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC [dbo].[RegistrarNotificacion] @id_usuario = {0}, @titulo = {1}, @mensaje = {2}, @fecha_envio = {3}",
                            idDestinatario,
                            "Cita Modificada",
                            mensajeNotificacion,
                            DateTime.Now
                        );
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error al registrar la notificación: {ex.Message}");
                    }

                    var smtpClient = new SmtpClient("smtp.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ### por la contraseña real
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("daharoni90459@ufide.ac.cr"),
                        Subject = "Cita Modificada",
                        Body = $"Estimado usuario,<br/><br/>" +
                               $"Se ha modificado una de sus citas en el sistema.<br/><br/>" +
                               $"Detalles de la cita:<br/>" +
                               $"<strong>Paciente:</strong> {paciente?.NombreCompleto}<br/>" +
                               $"<strong>Doctor:</strong> {doctor?.NombreCompleto}<br/>" +
                               $"<strong>Motivo:</strong> {cita.MotivoCita}<br/>" +
                               $"<strong>Fecha de la Cita:</strong> {cita.FechaInicio}<br/><br/>" +
                               $"Por favor, ingresa al sistema para más detalles.<br/>" +
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
                catch (DbUpdateException)
                {
                    ViewBag.ErrorMessage = "Error de base de datos: No se pudieron guardar los cambios.";
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Ocurrió un error interno.";
                }
            }

            await CargarDatosParaEditar(cita.IdEstadoCita);
            return View(cita);
        }

        private async Task CargarDatosParaEditar(int? idEstadoSeleccionado)
        {
            ViewBag.Pacientes = await _context.Usuarios
                .Where(u => u.roles.nombre_rol == "paciente")
                .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                .ToListAsync();

            ViewBag.Doctores = await _context.Usuarios
                .Where(u => u.roles.nombre_rol == "doctor")
                .Select(u => new { u.id_usuario, u.nombre, u.apellido })
                .ToListAsync();

            ViewBag.EstadosCita = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Programada" },
                new SelectListItem { Value = "2", Text = "Cancelada" },
                new SelectListItem { Value = "3", Text = "Completada" }
            }, "Value", "Text", idEstadoSeleccionado);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .SingleOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                ViewBag.ErrorMessage = "No se pudo encontrar la cita.";
                return View("ErrorCita");
            }

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                ViewBag.ErrorMessage = "No se pudo encontrar la cita.";
                return View("ErrorCita");
            }

            try
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "La cita se ha eliminado correctamente.";

                var doctor = await _context.Usuarios
                    .Where(u => u.id_usuario == cita.IdDoctor)
                    .Select(u => new { NombreCompleto = u.nombre + " " + u.apellido })
                    .FirstOrDefaultAsync();

                var paciente = await _context.Usuarios
                    .Where(u => u.id_usuario == cita.IdPaciente)
                    .Select(u => new { NombreCompleto = u.nombre + " " + u.apellido })
                    .FirstOrDefaultAsync();

                var userIdString = HttpContext.Session.GetString("UserId");
                var userRole = HttpContext.Session.GetString("UserRole");

                if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userRole))
                {
                    ModelState.AddModelError("", "No se pudo determinar el usuario actual.");
                    return View(cita);
                }

                int userId = int.Parse(userIdString);
                int idDestinatario;
                string mensajeNotificacion;

                if (userId == cita.IdDoctor)
                {
                    idDestinatario = cita.IdPaciente;
                    mensajeNotificacion = $"Estimado {paciente?.NombreCompleto}, el doctor {doctor?.NombreCompleto} ha eliminado una cita contigo para el {cita.FechaInicio}.";
                }
                else if (userId == cita.IdPaciente)
                {
                    idDestinatario = cita.IdDoctor;
                    mensajeNotificacion = $"Estimado {doctor?.NombreCompleto}, el paciente {paciente?.NombreCompleto} ha eliminado una cita contigo para el {cita.FechaInicio}.";
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo determinar el destinatario de la notificación.");
                    return View(cita);
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC [dbo].[RegistrarNotificacion] @id_usuario = {0}, @titulo = {1}, @mensaje = {2}, @fecha_envio = {3}",
                        idDestinatario,
                        "Cita Eliminada",
                        mensajeNotificacion,
                        DateTime.Now
                    );
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al registrar la notificación: {ex.Message}");
                }

                var smtpClient = new SmtpClient("smtp.outlook.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("daharoni90459@ufide.ac.cr", "###"), // Cambiar ### por la contraseña real
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("daharoni90459@ufide.ac.cr"),
                    Subject = "Cita Eliminada",
                    Body = $"Estimado usuario,<br/><br/>" +
                           $"Se ha eliminado una de sus citas en el sistema.<br/><br/>" +
                           $"Detalles de la cita:<br/>" +
                           $"<strong>Paciente:</strong> {paciente?.NombreCompleto}<br/>" +
                           $"<strong>Doctor:</strong> {doctor?.NombreCompleto}<br/>" +
                           $"<strong>Motivo:</strong> {cita.MotivoCita}<br/>" +
                           $"<strong>Fecha de la Cita:</strong> {cita.FechaInicio}<br/><br/>" +
                           $"Por favor, ingresa al sistema para más detalles.<br/>" +
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
            catch (DbUpdateException)
            {
                ViewBag.ErrorMessage = "Error de base de datos: No se pudo eliminar la cita.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al intentar eliminar la cita.";
                ViewBag.ErrorDetails = ex.Message;
            }

            return View("ErrorCita");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerHorasDisponibles(int idDoctor, DateTime fecha)
        {
            if (!EsRolPermitido())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var diaSemana = fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")).ToLower();

            var disponibilidad = await _context.DisponibilidadDoctor
                .Where(d => d.IdDoctor == idDoctor && d.DiaSemana.ToLower() == diaSemana)
                .ToListAsync();

            var citasReservadas = await _context.Citas
                .Where(c => c.IdDoctor == idDoctor && c.FechaInicio.Date == fecha.Date)
                .Select(c => new { c.FechaInicio, c.FechaFin })
                .ToListAsync();

            var horasDisponibles = new List<object>();

            foreach (var disponibilidadBloque in disponibilidad)
            {
                bool bloqueLibre = !citasReservadas.Any(cita =>
                    cita.FechaInicio.TimeOfDay < disponibilidadBloque.HoraFin &&
                    cita.FechaFin.TimeOfDay > disponibilidadBloque.HoraInicio);

                var horario = new
                {
                    HoraInicio = disponibilidadBloque.HoraInicio.ToString(@"hh\:mm"),
                    HoraFin = disponibilidadBloque.HoraFin.ToString(@"hh\:mm"),
                    Ocupada = !bloqueLibre
                };

                horasDisponibles.Add(horario);
            }

            return Json(horasDisponibles);
        }
    }
}
