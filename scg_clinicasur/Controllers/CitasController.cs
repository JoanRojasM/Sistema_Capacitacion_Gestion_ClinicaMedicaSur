using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

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
