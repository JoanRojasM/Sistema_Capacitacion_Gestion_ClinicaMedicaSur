using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Linq;
using System.Threading.Tasks;

namespace scg_clinicasur.Controllers
{
    public class ExpedientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpedientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchName)
        {
            ViewData["searchName"] = searchName;

            var expedientes = from e in _context.Expedientes
                              select e;

            if (!string.IsNullOrEmpty(searchName))
            {
                expedientes = expedientes.Where(e => e.nombrePaciente.Contains(searchName));
            }

            // Crear una lista de objetos anónimos con el expediente y la fecha de nacimiento del usuario
            var expedientesConfecha_nacimiento = expedientes.ToList().Select(e => new
            {
                Expediente = e,
                FechaNacimiento = _context.Usuarios.FirstOrDefault(u => u.id_usuario == e.idPaciente)?.fecha_nacimiento
            }).ToList();

            // Pasar la lista a la vista
            return View(expedientesConfecha_nacimiento);
        }

        [HttpGet]
        public IActionResult CrearExpediente()
        {
            // Obtener el ID del rol "paciente"
            var idRolPaciente = _context.Roles
                .FirstOrDefault(r => r.nombre_rol == "paciente")?.id_rol;

            if (idRolPaciente.HasValue)
            {
                // Obtener la lista de pacientes con sus datos necesarios
                ViewBag.Pacientes = _context.Usuarios
                    .Where(u => u.id_rol == idRolPaciente.Value && u != null) // Filtra pacientes nulos
                    .Select(u => new
                    {
                        u.id_usuario,
                        NombreCompleto = u.nombre + " " + u.apellido,
                        u.fecha_nacimiento // Incluir la fecha de nacimiento
                    })
                    .ToList();
            }
            else
            {
                // Si no hay rol "paciente", inicializar una lista vacía
                ViewBag.Pacientes = new List<object>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearExpediente(Expediente expediente)
        {
            if (ModelState.IsValid)
            {
                // Validar que el ID del paciente no sea cero
                if (expediente.idPaciente == 0)
                {
                    ModelState.AddModelError(string.Empty, "El ID del paciente no es válido.");
                    return View(expediente);
                }

                // Buscar el usuario (paciente) en la base de datos
                var usuario = _context.Usuarios.FirstOrDefault(u => u.id_usuario == expediente.idPaciente);
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no fue encontrado.");
                    return View(expediente);
                }

                // Asignar la fecha de creación del expediente
                expediente.fechaCreacion = DateTime.Now;

                // Guardar el expediente en la base de datos
                _context.Add(expediente);
                await _context.SaveChangesAsync();

                // Redirigir a la lista de expedientes
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, mostrar la vista con errores
            return View(expediente);
        }

        public IActionResult DetallesConsulta(int id)
        {
            // Buscar el expediente por su ID
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound(); // Si no se encuentra el expediente, retornar un error 404
            }

            // Obtener la fecha de nacimiento del usuario (paciente)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.id_usuario == expediente.idPaciente);
            if (usuario != null)
            {
                ViewBag.FechaNacimiento = usuario.fecha_nacimiento; // Asignar la fecha de nacimiento al ViewBag
            }
            else
            {
                ViewBag.FechaNacimiento = null; // Si no se encuentra el usuario, asignar null
            }

            // Pasar el expediente a la vista
            return View(expediente);
        }

        [HttpGet]
        public IActionResult EditarExpedientes(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound(); // Si no se encuentra el expediente, retornar un error 404
            }

            // Obtener la fecha de nacimiento del usuario (paciente)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.id_usuario == expediente.idPaciente);
            if (usuario != null)
            {
                ViewBag.FechaNacimiento = usuario.fecha_nacimiento; // Asignar la fecha de nacimiento al ViewBag
            }
            else
            {
                ViewBag.FechaNacimiento = null; // Si no se encuentra el usuario, asignar null
            }

            // Pasar el expediente a la vista
            return View("EditarExpediente", expediente);
        }

        [HttpPost]
        public IActionResult EditarExpedientes(Expediente model)
        {
            if (ModelState.IsValid)
            {
                var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == model.idExpediente);
                if (expediente != null)
                {
                    // Actualizar los campos editables
                    expediente.ultimaConsulta = model.ultimaConsulta;
                    expediente.diagnostico = model.diagnostico;
                    expediente.descripcion = model.descripcion;
                    expediente.tratamientosPrevios = model.tratamientosPrevios;

                    _context.SaveChanges(); // Guardar los cambios en la base de datos
                    return RedirectToAction("DetallesConsulta", new { id = model.idExpediente }); // Redirigir a la vista de detalles
                }
                return NotFound(); // Si no se encuentra el expediente, retornar un error 404
            }

            // Si el modelo no es válido, mostrar la vista con errores
            return View("EditarExpediente", model);
        }

        [HttpGet]
        public IActionResult EliminarExpediente(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound(); // Si no se encuentra el expediente, retornar un error 404
            }

            // Obtener la fecha de nacimiento del usuario (paciente)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.id_usuario == expediente.idPaciente);
            if (usuario != null)
            {
                ViewBag.FechaNacimiento = usuario.fecha_nacimiento; // Asignar la fecha de nacimiento al ViewBag
            }
            else
            {
                ViewBag.FechaNacimiento = null; // Si no se encuentra el usuario, asignar null
            }

            // Pasar el expediente a la vista
            return View(expediente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound(); // Si no se encuentra el expediente, retornar un error 404
            }

            // Eliminar el expediente
            _context.Expedientes.Remove(expediente);
            _context.SaveChanges();

            // Redirigir al listado de expedientes
            return RedirectToAction("Index");
        }

        public IActionResult ImprimirExpedienteCompleto(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound(); // Si no se encuentra el expediente, retornar un error 404
            }

            // Obtener la fecha de nacimiento del usuario (paciente)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.id_usuario == expediente.idPaciente);
            if (usuario != null)
            {
                ViewBag.FechaNacimiento = usuario.fecha_nacimiento; // Asignar la fecha de nacimiento al ViewBag
            }
            else
            {
                ViewBag.FechaNacimiento = null; // Si no se encuentra el usuario, asignar null
            }

            // Pasar el expediente a la vista
            return View(expediente);
        }

        public IActionResult VerResultadosRecientes(int idExpediente)
        {
            var resultadosRecientes = _context.ResultadosLaboratorio
                .Where(r => r.IdExpediente == idExpediente)
                .OrderByDescending(r => r.FechaPrueba)
                .FirstOrDefault();

            if (resultadosRecientes == null)
            {
                return NotFound("No hay resultados recientes.");
            }

            return View(resultadosRecientes);
        }

        public IActionResult VerHistorialResultados(int idExpediente)
        {
            var historialResultados = _context.ResultadosLaboratorio
                .Where(r => r.IdExpediente == idExpediente)
                .OrderByDescending(r => r.FechaPrueba)
                .ToList();

            return View(historialResultados);
        }

        [HttpGet]
        public IActionResult CrearResultadosLaboratorio(int idExpediente)
        {
            ViewBag.IdExpediente = idExpediente;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearResultadosLaboratorio(int idExpediente, IFormFile archivoPDF)
        {
            if (archivoPDF == null || archivoPDF.Length == 0)
            {
                ModelState.AddModelError("archivoPDF", "Debe cargar un archivo PDF.");
                return View();
            }

            byte[] archivoData;
            using (var memoryStream = new MemoryStream())
            {
                await archivoPDF.CopyToAsync(memoryStream);
                archivoData = memoryStream.ToArray();
            }

            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == idExpediente);
            if (expediente == null)
            {
                return NotFound("No se encontró el expediente especificado.");
            }

            var resultadoLaboratorio = new ResultadosLaboratorio
            {
                IdExpediente = idExpediente,
                IdPaciente = expediente.idPaciente,
                FechaPrueba = DateTime.Now,
                ArchivoPDF = archivoData
            };

            _context.ResultadosLaboratorio.Add(resultadoLaboratorio);
            await _context.SaveChangesAsync();

            return RedirectToAction("VerHistorialResultados", new { idExpediente });
        }

        [HttpGet]
        public IActionResult DescargarPDF(int id)
        {
            var resultado = _context.ResultadosLaboratorio.FirstOrDefault(r => r.IdResultado == id);

            if (resultado == null || resultado.ArchivoPDF == null)
            {
                return NotFound("El archivo PDF no se encontró.");
            }

            return File(resultado.ArchivoPDF, "application/pdf", $"Resultado_{id}.pdf");
        }

        [HttpGet]
        public IActionResult EditarResultado(int id)
        {
            var resultado = _context.ResultadosLaboratorio.FirstOrDefault(r => r.IdResultado == id);
            if (resultado == null)
            {
                return NotFound("Resultado de laboratorio no encontrado.");
            }

            ViewBag.IdExpediente = resultado.IdExpediente;
            return View(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarResultado(int id, IFormFile archivoPDF)
        {
            var resultado = _context.ResultadosLaboratorio.FirstOrDefault(r => r.IdResultado == id);
            if (resultado == null)
            {
                return NotFound("Resultado de laboratorio no encontrado.");
            }

            if (archivoPDF != null && archivoPDF.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archivoPDF.CopyToAsync(memoryStream);
                    resultado.ArchivoPDF = memoryStream.ToArray();
                }

                resultado.FechaPrueba = DateTime.Now;

                _context.Update(resultado);
                await _context.SaveChangesAsync();

                return RedirectToAction("VerHistorialResultados", "Expedientes", new { idExpediente = resultado.IdExpediente });
            }

            ModelState.AddModelError("archivoPDF", "Debe cargar un archivo PDF.");
            return View(resultado);
        }

        [HttpGet]
        public IActionResult EliminarResultadosLaboratorio(int id)
        {
            var resultado = _context.ResultadosLaboratorio.FirstOrDefault(r => r.IdResultado == id);
            if (resultado == null)
            {
                return NotFound(); // Si no se encuentra el resultado, retornar un error 404
            }

            // Pasar el resultado a la vista
            return View(resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarResultadoConfirmado(int id)
        {
            var resultado = _context.ResultadosLaboratorio.FirstOrDefault(r => r.IdResultado == id);
            if (resultado == null)
            {
                return NotFound(); // Si no se encuentra el resultado, retornar un error 404
            }

            // Eliminar el resultado de laboratorio
            _context.ResultadosLaboratorio.Remove(resultado);
            _context.SaveChanges();

            // Redirigir a la vista de detalles del expediente
            return RedirectToAction("DetallesConsulta", new { id = resultado.IdExpediente });
        }
    }
}