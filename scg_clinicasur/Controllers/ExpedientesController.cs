using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Linq;

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
            // Guardar el término de búsqueda en ViewData para usarlo en la vista
            ViewData["searchName"] = searchName;

            // Si no se especifica ningún nombre, devolver todos los expedientes
            var expedientes = from e in _context.Expedientes
                              select e;

            // Filtrar por nombre de paciente si el usuario ingresó un término de búsqueda
            if (!string.IsNullOrEmpty(searchName))
            {
                expedientes = expedientes.Where(e => e.nombrePaciente.Contains(searchName));
            }

            return View(expedientes.ToList());
        }

        // GET: CrearExpediente
        [HttpGet]
        public IActionResult CrearExpediente()
        {
            // Consulta para obtener solo los usuarios con el rol de 'paciente'
            ViewBag.Pacientes = _context.Usuarios
                .Where(u => u.id_rol == _context.Roles.FirstOrDefault(r => r.nombre_rol == "paciente").id_rol)
                .Select(u => new { u.id_usuario, NombreCompleto = u.nombre + " " + u.apellido })
                .ToList();

            return View();
        }

        // POST: CrearExpediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearExpediente(Expediente expediente)
        {
            if (ModelState.IsValid)
            {
                // Asignar la fecha de creación actual
                expediente.fechaCreacion = DateTime.Now;

                // Añadir el expediente al contexto de la base de datos
                _context.Add(expediente);

                // Guardar los cambios de forma asíncrona
                await _context.SaveChangesAsync();

                // Redirigir al índice de expedientes u otra vista
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, volver a mostrar la vista de creación con los errores
            return View(expediente);
        }

        public IActionResult DetallesConsulta(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound();
            }
            return View(expediente);
        }

        // Método para mostrar el formulario de edición
        public IActionResult EditarExpedientes(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound();
            }
            return View("EditarExpediente", expediente);
        }

        // Método para guardar los cambios al expediente
        [HttpPost]
        public IActionResult EditarExpedientes(Expediente model)
        {
            if (ModelState.IsValid)
            {
                var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == model.idExpediente);
                if (expediente != null)
                {
                    expediente.ultimaConsulta = model.ultimaConsulta;
                    expediente.diagnostico = model.diagnostico;
                    expediente.descripcion = model.descripcion;
                    expediente.tratamientosPrevios = model.tratamientosPrevios;

                    _context.SaveChanges();
                    return RedirectToAction("DetallesConsulta", new { id = model.idExpediente });
                }
                return NotFound();
            }

            return View("EditarExpediente", model);
        }

        // POST: EliminarExpediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarExpediente(int id)
        {
            Console.WriteLine("ID recibido para eliminar: " + id); // Debugging
            // Buscar el expediente en la base de datos por su ID
            var expediente = await _context.Expedientes.FindAsync(id);

            if (expediente == null)
            {
                // Si no se encuentra el expediente, retornar un error o redirigir a una página de error
                return NotFound();
            }

            try
            {
                // Remover el expediente del contexto de la base de datos
                _context.Expedientes.Remove(expediente);

                // Guardar los cambios de forma asíncrona
                await _context.SaveChangesAsync();

                // Redirigir a la página de listado tras eliminar
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Manejar cualquier error relacionado con la base de datos
                // Podrías registrar el error y mostrar un mensaje adecuado
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el expediente. Inténtalo de nuevo más tarde.");
                return View("Error");
            }
        }
        public IActionResult ImprimirExpedienteCompleto(int id)
        {
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == id);
            if (expediente == null)
            {
                return NotFound();
            }

            // Renderiza la vista de impresión con el modelo de expediente
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

        // GET: CrearResultadosLaboratorio
        [HttpGet]
        public IActionResult CrearResultadosLaboratorio(int idExpediente)
        {
            // Pasa el id del expediente al ViewBag para usarlo en la vista
            ViewBag.IdExpediente = idExpediente;
            return View();
        }

        // POST: CrearResultadosLaboratorio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearResultadosLaboratorio(int idExpediente, IFormFile archivoPDF)
        {
            if (archivoPDF == null || archivoPDF.Length == 0)
            {
                ModelState.AddModelError("archivoPDF", "Debe cargar un archivo PDF.");
                return View();
            }

            // Convierte el archivo PDF a un arreglo de bytes
            byte[] archivoData;
            using (var memoryStream = new MemoryStream())
            {
                await archivoPDF.CopyToAsync(memoryStream);
                archivoData = memoryStream.ToArray();
            }

            // Obtener el id del paciente a partir del expediente
            var expediente = _context.Expedientes.FirstOrDefault(e => e.idExpediente == idExpediente);
            if (expediente == null)
            {
                return NotFound("No se encontró el expediente especificado.");
            }

            // Crear un nuevo resultado de laboratorio
            var resultadoLaboratorio = new ResultadosLaboratorio
            {
                IdExpediente = idExpediente,
                IdPaciente = expediente.idPaciente,  // Asigna el id del paciente del expediente
                FechaPrueba = DateTime.Now,
                ArchivoPDF = archivoData
            };

            // Añadir el resultado al contexto de la base de datos
            _context.ResultadosLaboratorio.Add(resultadoLaboratorio);
            await _context.SaveChangesAsync();

            // Redirigir a la vista de historial de resultados
            return RedirectToAction("VerHistorialResultados", new { idExpediente });
        }

        // GET: DescargarPDF
        [HttpGet]
        public IActionResult DescargarPDF(int id)
        {
            // Buscar el resultado de laboratorio por su IdResultado
            var resultado = _context.ResultadosLaboratorio.FirstOrDefault(r => r.IdResultado == id);

            if (resultado == null || resultado.ArchivoPDF == null)
            {
                return NotFound("El archivo PDF no se encontró.");
            }

            // Retornar el archivo PDF como un archivo descargable
            return File(resultado.ArchivoPDF, "application/pdf", $"Resultado_{id}.pdf");
        }

        // GET: EditarResultado
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

        // POST: EditarResultado
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

                resultado.FechaPrueba = DateTime.Now; // Actualiza la fecha de la prueba si es necesario

                _context.Update(resultado);
                await _context.SaveChangesAsync();

                return RedirectToAction("VerHistorialResultados", "Expedientes", new { idExpediente = resultado.IdExpediente });
            }

            ModelState.AddModelError("archivoPDF", "Debe cargar un archivo PDF.");
            return View(resultado);
        }

        // POST: EliminarResultado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarResultado(int id)
        {
            var resultado = await _context.ResultadosLaboratorio.FindAsync(id);
            if (resultado == null)
            {
                return NotFound("Resultado de laboratorio no encontrado.");
            }

            _context.ResultadosLaboratorio.Remove(resultado);
            await _context.SaveChangesAsync();

            return RedirectToAction("VerHistorialResultados", new { idExpediente = resultado.IdExpediente });
        }
    }
}