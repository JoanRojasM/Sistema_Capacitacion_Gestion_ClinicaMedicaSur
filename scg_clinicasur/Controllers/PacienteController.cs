using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace scg_clinicasur.Controllers
{
    public class PacienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inyección del DbContext a través del constructor
        public PacienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Citas(string[] estado)
        {
            // Obtiene el ID del usuario autenticado
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Obtiene las citas asociadas al paciente
            var citasQuery = _context.Citas
            .Include(c => c.EstadoCita)
            .Include(c => c.Paciente)
            .Include(c => c.Doctor)
            .Where(c => c.IdPaciente == userId) // Filtrar por el ID del paciente
            .AsQueryable();

            if (estado != null && estado.Length > 0)
            {
                citasQuery = citasQuery.Where(c => estado.Contains(c.EstadoCita.EstadoNombre.ToLower()));
            }
            var citas = citasQuery.ToList();
            return View(citas);
        }

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
                TempData["ErrorMessage"] = "No se pudo encontrar la cita.";
                return RedirectToAction("Index");
            }

            await CargarDatosParaEditar(cita.IdEstadoCita);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Cita cita)
        {

            if (!ModelState.IsValid)
            {
                await CargarDatosParaEditar(cita.IdEstadoCita);
                return View(cita);
            }

            // 🔹 Verificar conflictos de horario con otras citas
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
                // Verificar que la fecha de inicio esté en el formato correcto
                if (cita.FechaInicio == default(DateTime))
                {
                    ModelState.AddModelError("FechaInicio", "La fecha de inicio es inválida.");
                    await CargarDatosParaEditar(cita.IdEstadoCita);
                    return View(cita);
                }

                // Verificar que la fecha de fin también esté correctamente configurada
                if (cita.FechaFin <= cita.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
                    await CargarDatosParaEditar(cita.IdEstadoCita);
                    return View(cita);
                }

                // Actualizar la cita
                _context.Citas.Update(cita);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "La cita se ha actualizado correctamente.";

                // 🔹 Redirección a `Index`
                return RedirectToAction("Citas");
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Error de base de datos: No se pudieron guardar los cambios.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error interno: {ex.Message}";
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
                ViewBag.ErrorMessage = "No se pudo encontrar la cita.";
                return View("ErrorCita");
            }

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {

            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                TempData["ErrorMessage"] = "No se pudo encontrar la cita.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Citas.Remove(cita);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "La cita se ha eliminado correctamente.";

                // 🔹 Redirección a `Index`
                return RedirectToAction("Citas");
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Error de base de datos: No se pudo eliminar la cita.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error interno: {ex.Message}";
                Console.WriteLine($"[DEBUG] Error inesperado: {ex.Message}");
            }

            {
                return RedirectToAction("Citas");
            }
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
        public IActionResult ListadoPacientes(string searchName)
        {
            // Obtener solo los usuarios con rol "paciente"
            var pacientes = _context.Usuarios
                .Include(u => u.roles) // Incluir roles en la consulta
                .Where(u => u.roles.nombre_rol == "paciente");

            // Aplicar el filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(searchName))
            {
                pacientes = pacientes.Where(u => u.nombre.Contains(searchName) || u.apellido.Contains(searchName));
            }

            // Pasar la lista filtrada de pacientes a la vista
            return View(pacientes.ToList());
        }

        public IActionResult ExpedientesPorPaciente(int id_usuario)
        {
            if (id_usuario == 0)
            {
                TempData["ErrorMessage"] = "ID de paciente no válido.";
                return RedirectToAction("ListadoPacientes");
            }

            // Recuperar los expedientes asociados al paciente usando su ID
            var expedientes = _context.Expedientes
                .Where(e => e.idPaciente == id_usuario)
                .ToList();

            // Verificar si se encontraron expedientes
            if (!expedientes.Any())
            {
                TempData["ErrorMessage"] = "No se encontraron expedientes para este paciente.";
            }

            ViewBag.NombrePaciente = _context.Usuarios
                .Where(u => u.id_usuario == id_usuario)
                .Select(u => u.nombre + " " + u.apellido)
                .FirstOrDefault();

            return View(expedientes);
        }

        [HttpGet]
        public IActionResult GestionarAlergias(int id_paciente)
        {
            var paciente = _context.Usuarios.Find(id_paciente);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "Paciente no encontrado.";
                return RedirectToAction("ListadoPacientes");
            }

            var todasAlergias = _context.Alergias.ToList();
            var alergiasPaciente = _context.PacienteAlergias
                                    .Where(pa => pa.id_paciente == id_paciente)
                                    .Select(pa => pa.id_alergia)
                                    .ToList();

            var historialAlergias = _context.PacienteAlergias
                                    .Where(pa => pa.id_paciente == id_paciente)
                                    .Include(pa => pa.Alergia)
                                    .Select(pa => new
                                    {
                                        nombre_alergia = pa.Alergia.nombre_alergia,
                                        fechaRegistro = pa.fechaRegistro
                                    })
                                    .AsEnumerable()  // Convierte a IEnumerable antes de proyectar a dinámico
                                    .Select(pa => (dynamic)pa)  // Castea a dinámico cada entrada
                                    .OrderBy(pa => pa.fechaRegistro)
                                    .ToList();

            var viewModel = new GestionarAlergiasViewModel
            {
                Paciente = paciente,
                TodasAlergias = todasAlergias,
                AlergiasPaciente = alergiasPaciente,
                HistorialAlergias = historialAlergias
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GuardarAlergias(int id_paciente, List<int> alergiasSeleccionadas)
        {
            var alergiasExistentes = _context.PacienteAlergias
                                             .Where(pa => pa.id_paciente == id_paciente);
            _context.PacienteAlergias.RemoveRange(alergiasExistentes);

            foreach (var id_alergia in alergiasSeleccionadas)
            {
                _context.PacienteAlergias.Add(new PacienteAlergia
                {
                    id_paciente = id_paciente,
                    id_alergia = id_alergia,
                    fechaRegistro = DateTime.Now
                });
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Alergias actualizadas correctamente.";
            return RedirectToAction("ExpedientesPorPaciente", new { id_usuario = id_paciente });
        }

        [HttpGet]
        public IActionResult GestionarContactoEmergencia(int id_paciente)
        {
            var paciente = _context.Usuarios.Find(id_paciente);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "Paciente no encontrado.";
                return RedirectToAction("ListadoPacientes");
            }

            var contactoEmergencia = _context.ContactosEmergencia
                .Where(c => c.IdPaciente == id_paciente)
                .OrderByDescending(c => c.FechaRegistro)
                .ToList();

            var viewModel = new GestionarContactoEmergenciaViewModel
            {
                Paciente = paciente,
                ContactoEmergenciaActual = contactoEmergencia.FirstOrDefault(),
                HistorialContactoEmergencia = contactoEmergencia
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult GuardarContactoEmergencia(int id_paciente, string nombreContacto, string relacion, string telefono)
        {
            var nuevoContacto = new ContactoEmergencia
            {
                IdPaciente = id_paciente,
                NombreContacto = nombreContacto,
                Relacion = relacion,
                TelefonoContacto = telefono,
                FechaRegistro = DateTime.Now
            };

            _context.ContactosEmergencia.Add(nuevoContacto);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Contacto de emergencia guardado correctamente.";
            return RedirectToAction("GestionarContactoEmergencia", new { id_paciente });
        }

        [HttpPost]
        public IActionResult EliminarContactoEmergencia(int id_contacto, int id_paciente)
        {
            var contacto = _context.ContactosEmergencia.FirstOrDefault(c => c.Id == id_contacto && c.IdPaciente == id_paciente);

            if (contacto == null)
            {
                TempData["ErrorMessage"] = "Contacto de emergencia no encontrado.";
                return RedirectToAction("GestionarContactoEmergencia", new { id_paciente });
            }

            _context.ContactosEmergencia.Remove(contacto);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Contacto de emergencia eliminado correctamente.";
            return RedirectToAction("GestionarContactoEmergencia", new { id_paciente });
        }

        // GET: Ver y gestionar antecedentes familiares
        [HttpGet]
        public IActionResult GestionarAntecedentesFamiliares(int id_paciente)
        {
            var paciente = _context.Usuarios.Find(id_paciente);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "Paciente no encontrado.";
                return RedirectToAction("ListadoPacientes");
            }

            var antecedentesFamiliares = _context.AntecedentesFamiliares
                .Where(a => a.IdPaciente == id_paciente)
                .OrderByDescending(a => a.FechaRegistro)
                .ToList();

            var viewModel = new GestionarAntecedentesFamiliaresViewModel
            {
                Paciente = paciente,
                AntecedentesFamiliares = antecedentesFamiliares,
                UltimoAntecedente = antecedentesFamiliares.FirstOrDefault()
            };

            return View(viewModel);
        }

        // POST: Guardar nuevo antecedente familiar
        [HttpPost]
        public IActionResult GuardarAntecedenteFamiliar(int id_paciente, string descripcion)
        {
            var nuevoAntecedente = new AntecedenteFamiliar
            {
                IdPaciente = id_paciente,
                Descripcion = descripcion,
                FechaRegistro = DateTime.Now
            };

            _context.AntecedentesFamiliares.Add(nuevoAntecedente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Antecedente familiar guardado correctamente.";
            return RedirectToAction("GestionarAntecedentesFamiliares", new { id_paciente });
        }

        // POST: Eliminar un antecedente familiar específico
        [HttpPost]
        public IActionResult EliminarAntecedenteFamiliar(int id_antecedente, int id_paciente)
        {
            var antecedente = _context.AntecedentesFamiliares.Find(id_antecedente);
            if (antecedente != null)
            {
                _context.AntecedentesFamiliares.Remove(antecedente);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Antecedente familiar eliminado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Antecedente familiar no encontrado.";
            }

            return RedirectToAction("GestionarAntecedentesFamiliares", new { id_paciente });
        }

        // GET: Ver y gestionar hábitos de vida
        [HttpGet]
        public IActionResult GestionarHabitosVida(int id_paciente)
        {
            var paciente = _context.Usuarios.Find(id_paciente);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "Paciente no encontrado.";
                return RedirectToAction("ListadoPacientes");
            }

            var habitosVida = _context.HabitosVida
                .Where(h => h.IdPaciente == id_paciente)
                .OrderByDescending(h => h.FechaRegistro)
                .ToList();

            var viewModel = new GestionarHabitosVidaViewModel
            {
                Paciente = paciente,
                HabitosVida = habitosVida,
                UltimoHabito = habitosVida.FirstOrDefault()
            };

            return View(viewModel);
        }

        // POST: Guardar nuevo hábito de vida
        [HttpPost]
        public IActionResult GuardarHabitoVida(int id_paciente, string descripcion)
        {
            var nuevoHabito = new HabitoVida
            {
                IdPaciente = id_paciente,
                Descripcion = descripcion,
                FechaRegistro = DateTime.Now
            };

            _context.HabitosVida.Add(nuevoHabito);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Hábito de vida guardado correctamente.";
            return RedirectToAction("GestionarHabitosVida", new { id_paciente });
        }

        // POST: Eliminar un hábito de vida específico
        [HttpPost]
        public IActionResult EliminarHabitoVida(int id_habito, int id_paciente)
        {
            var habito = _context.HabitosVida.Find(id_habito);
            if (habito != null)
            {
                _context.HabitosVida.Remove(habito);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Hábito de vida eliminado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Hábito de vida no encontrado.";
            }

            return RedirectToAction("GestionarHabitosVida", new { id_paciente });
        }

        // GET: Ver y gestionar medicamentos prescritos
        [HttpGet]
        public IActionResult GestionarMedicamentos(int id_paciente)
        {
            var paciente = _context.Usuarios.Find(id_paciente);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "Paciente no encontrado.";
                return RedirectToAction("ListadoPacientes");
            }

            var medicamentos = _context.MedicamentosPrescritos
                .Where(m => m.IdPaciente == id_paciente)
                .OrderByDescending(m => m.FechaPrescripcion)
                .ToList();

            var viewModel = new GestionarMedicamentosViewModel
            {
                Paciente = paciente,
                MedicamentosActivos = medicamentos.Where(m => m.Estado == "activo").ToList(),
                HistorialMedicamentos = medicamentos
            };

            return View(viewModel);
        }

        // POST: Prescribir un nuevo medicamento
        [HttpPost]
        public IActionResult PrescribirMedicamento(int id_paciente, string nombreMedicamento, string dosis)
        {
            var nuevoMedicamento = new MedicamentoPrescrito
            {
                IdPaciente = id_paciente,
                NombreMedicamento = nombreMedicamento,
                Dosis = dosis,
                FechaPrescripcion = DateTime.Now,
                Estado = "activo"
            };

            _context.MedicamentosPrescritos.Add(nuevoMedicamento);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Medicamento prescrito correctamente.";
            return RedirectToAction("GestionarMedicamentos", new { id_paciente });
        }

        // POST: Actualizar un medicamento existente
        [HttpPost]
        public IActionResult ActualizarMedicamento(int id_medicamento, string nombreMedicamento, string dosis)
        {
            var medicamento = _context.MedicamentosPrescritos.Find(id_medicamento);
            if (medicamento != null)
            {
                medicamento.NombreMedicamento = nombreMedicamento;
                medicamento.Dosis = dosis;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Medicamento actualizado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Medicamento no encontrado.";
            }

            return RedirectToAction("GestionarMedicamentos", new { id_paciente = medicamento.IdPaciente });
        }

        // POST: Descontinuar un medicamento
        [HttpPost]
        public IActionResult DescontinuarMedicamento(int id_medicamento)
        {
            var medicamento = _context.MedicamentosPrescritos.Find(id_medicamento);
            if (medicamento != null)
            {
                medicamento.Estado = "descontinuado";
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Medicamento descontinuado.";
            }
            else
            {
                TempData["ErrorMessage"] = "Medicamento no encontrado.";
            }

            return RedirectToAction("GestionarMedicamentos", new { id_paciente = medicamento.IdPaciente });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerHorasDisponibles(int idDoctor, DateTime fecha)
        {

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
