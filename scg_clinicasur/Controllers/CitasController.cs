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
        // Método para mostrar el dashboard de citas con filtrado por estatus
        public IActionResult Index(string[] estado)
        {
            // Obtener el ID del usuario y su rol desde la sesión
            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userIdString) ||
                (userRole?.ToLower() != "doctor" && userRole?.ToLower() != "administrador"))
            {
                return RedirectToAction("Login", "Account");
            }

            // Consultar todas las citas
            var citasQuery = _context.Citas
                .Include(c => c.EstadoCita)
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .AsQueryable();

            // Filtrar citas por doctor si el usuario es un doctor
            if (userRole?.ToLower() == "doctor")
            {
                int doctorId = int.Parse(userIdString);
                citasQuery = citasQuery.Where(c => c.IdDoctor == doctorId);
            }

            // Filtrar por estatus de cita si se selecciona un filtro
            if (estado != null && estado.Length > 0)
            {
                citasQuery = citasQuery.Where(c => estado.Contains(c.EstadoCita.EstadoNombre.ToLower()));
            }

            var citas = citasQuery.ToList();
            return View(citas);
        }

        // Método para mostrar el calendario de citas
        public IActionResult Calendario()
        {
            // Obtener el ID del usuario y su rol desde la sesión
            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userIdString) ||
                (userRole?.ToLower() != "doctor" && userRole?.ToLower() != "administrador"))
            {
                return RedirectToAction("Login", "Account");
            }

            // Validar el ID del usuario
            if (!int.TryParse(userIdString, out int userId))
            {
                ViewBag.ErrorMessage = "ID de usuario inválido.";
                return View("Error");
            }

            // Consultar todas las citas, incluyendo el paciente y el doctor
            var citasQuery = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .AsQueryable();

            // Filtrar citas por doctor si el usuario es un doctor
            if (userRole.ToLower() == "doctor")
            {
                citasQuery = citasQuery.Where(c => c.IdDoctor == userId);
            }

            var citas = citasQuery.ToList();

            if (citas == null || !citas.Any())
            {
                ViewBag.WarningMessage = "No se encontraron citas.";
            }

            return View(citas);
        }

        // Método para mostrar los detalles de una cita con manejo de errores
        public IActionResult Detalles(int id)
        {
            try
            {
                // Intentar obtener la cita con el ID especificado
                var cita = _context.Citas
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .Include(c => c.EstadoCita)
                    .SingleOrDefault(c => c.IdCita == id);

                if (cita == null)
                {
                    // Mostrar un mensaje de error si la cita no se encuentra
                    ViewBag.ErrorMessage = "No se pudo encontrar la cita. Por favor, verifica el ID de la cita.";
                    return View("ErrorCita");
                }

                // Retornar la vista de detalles de la cita
                return View(cita);
            }
            catch (DbUpdateException ex)
            {
                // Manejar error de base de datos
                ViewBag.ErrorMessage = "No se pudo acceder a los detalles de la cita debido a un problema con la base de datos. Por favor, intenta más tarde.";
                ViewBag.ErrorDetails = ex.Message;
                return View("ErrorCita");
            }
            catch (TimeoutException ex)
            {
                // Manejar tiempo de espera agotado
                ViewBag.ErrorMessage = "El tiempo de espera para acceder a los detalles de la cita ha expirado. Por favor, intenta más tarde.";
                ViewBag.ErrorDetails = ex.Message;
                return View("ErrorCita");
            }
            catch (Exception ex)
            {
                // Manejar cualquier otro error interno
                ViewBag.ErrorMessage = "Ocurrió un error interno al intentar acceder a los detalles de la cita. Por favor, contacta con soporte si el problema persiste.";
                ViewBag.ErrorDetails = ex.Message;
                return View("ErrorCita");
            }
        }

        // Método para mostrar el formulario de creación de citas
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            // Obtener la lista de pacientes y doctores
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

        // Método para manejar la creación de una nueva cita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cita cita)
        {
            if (ModelState.IsValid)
            {
                cita.FechaCreacion = DateTime.Now;

                _context.Citas.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si el ModelState no es válido, recargar los pacientes y doctores
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

        // GET: Editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .Include(c => c.EstadoCita)
                .SingleOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                ViewBag.ErrorMessage = "No se pudo encontrar la cita. Por favor, verifica el ID.";
                return View("ErrorCita");
            }

            // Asignar los ViewBag para la vista
            await CargarDatosParaEditar(cita.IdEstadoCita);
            return View(cita);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Cita cita)
        {
            if (ModelState.IsValid)
            {
                // Verificar conflicto de horarios con las nuevas fechas de inicio y fin
                bool conflictoHorario = await _context.Citas
                    .AnyAsync(c => c.IdDoctor == cita.IdDoctor
                                   && c.IdCita != cita.IdCita
                                   && ((c.FechaInicio < cita.FechaFin && c.FechaFin > cita.FechaInicio)));

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
                    ViewBag.ErrorMessage = "Error de base de datos: No se pudieron guardar los cambios. Por favor, intenta nuevamente más tarde.";
                    await CargarDatosParaEditar(cita.IdEstadoCita);
                    return View(cita);
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Ocurrió un error interno al intentar actualizar la cita. Por favor, contacta con soporte.";
                    await CargarDatosParaEditar(cita.IdEstadoCita);
                    return View(cita);
                }
            }

            // Volver a cargar los ViewBag y mostrar errores de validación
            await CargarDatosParaEditar(cita.IdEstadoCita);
            return View(cita);
        }

        // Cargar datos para la vista de edición
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

        // GET: Eliminar
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .SingleOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                ViewBag.ErrorMessage = "No se pudo encontrar la cita. Por favor, verifica el ID.";
                return View("ErrorCita");
            }

            // Pasar la cita a la vista para mostrar los detalles
            return View(cita);
        }

        // POST: Confirmar Eliminación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .SingleOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                ViewBag.ErrorMessage = "No se pudo encontrar la cita. Por favor, verifica el ID.";
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
                ViewBag.ErrorMessage = "Error de base de datos: No se pudo eliminar la cita. Intenta nuevamente más tarde.";
                return View("ErrorCita");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al intentar eliminar la cita.";
                ViewBag.ErrorDetails = ex.Message;
                return View("ErrorCita");
            }
        }

    }
}
