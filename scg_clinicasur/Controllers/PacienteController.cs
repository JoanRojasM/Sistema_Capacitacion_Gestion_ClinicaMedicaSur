using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Diagnostics;
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

        public async Task<IActionResult> Citas()
        {
            // Obtiene el ID del usuario autenticado
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Obtiene las citas asociadas al paciente
            var citas = await _context.Citas
                .Where(c => c.IdPaciente == userId)
                .ToListAsync();

            return View(citas);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Citas));
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
    }
}
