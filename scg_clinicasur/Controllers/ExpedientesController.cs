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
    }
}