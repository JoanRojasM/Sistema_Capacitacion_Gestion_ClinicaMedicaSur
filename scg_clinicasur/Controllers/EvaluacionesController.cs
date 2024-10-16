using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

namespace scg_clinicasur.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaluacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var evaluaciones = _context.Evaluaciones.Include(t => t.Usuario).ThenInclude(u => u.roles).ToList();
            return View(evaluaciones);
        }

        [HttpGet]
        public IActionResult CrearEvaluacion()
        {
            var usuarios = _context.Usuarios.Include(u => u.roles).Where(u => u.id_rol != 5).ToList();
            ViewBag.Usuarios = new SelectList(usuarios, "id_usuario", "nombre"); // Ajusta para incluir nombre y rol
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvaluacion(Evaluacion evaluacion, IFormFile archivo)
        {
            if (ModelState.IsValid)
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", archivo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }
                    evaluacion.archivo = "/uploads/" + archivo.FileName;
                }

                _context.Evaluaciones.Add(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Tareas");
            }

            var usuarios = _context.Usuarios.Include(u => u.roles).ToList();
            ViewBag.Usuarios = new SelectList(usuarios, "id_usuario", "nombre"); // Ajusta para incluir nombre y rol
            return View(evaluacion);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var evaluacion = _context.Evaluaciones.FirstOrDefault(e => e.id_evaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }
            return View(evaluacion);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "nombre");
            return View();
        }

        // Crear una nueva evaluación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Evaluacion evaluacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "nombre", evaluacion.id_usuario);
            return View(evaluacion);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var evaluacion = _context.Evaluaciones.Find(id);
            if (evaluacion == null)
            {
                return NotFound();
            }
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "nombre", evaluacion.id_usuario);
            return View(evaluacion);
        }

        // Editar evaluación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Evaluacion evaluacion)
        {
            if (id != evaluacion.id_evaluacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "nombre", evaluacion.id_usuario);
            return View(evaluacion);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Usamos Include para cargar la entidad relacionada Usuario
            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Usuario) // Asegura que el Usuario está cargado
                .FirstOrDefaultAsync(m => m.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        // Eliminar evaluación (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var evaluacion = _context.Evaluaciones.Find(id);
            _context.Evaluaciones.Remove(evaluacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
