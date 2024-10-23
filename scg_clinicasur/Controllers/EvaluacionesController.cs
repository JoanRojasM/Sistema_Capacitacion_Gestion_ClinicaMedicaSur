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

        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var evaluaciones = _context.Evaluaciones.Include(t => t.Usuario).ThenInclude(u => u.roles).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                evaluaciones = evaluaciones.Where(e => e.Usuario.nombre.Contains(searchString) || e.Usuario.apellido.Contains(searchString));
            }

            return View(evaluaciones.ToList());
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
            var usuarios = _context.Usuarios
                         .Include(u => u.roles)
                         .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
                         .ToList();
            var usuariosConRoles = usuarios.Select(u => new
            {
                id_usuario = u.id_usuario,
                DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosConRoles, "id_usuario", "DisplayText");
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
             var usuarios = _context.Usuarios
                          .Include(u => u.roles)
                          .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
                          .ToList();
            var usuariosConRoles = usuarios.Select(u => new
            {
                id_usuario = u.id_usuario,
                DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
            }).ToList();

            ViewBag.Usuarios = new SelectList(usuariosConRoles, "id_usuario", "DisplayText");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var evaluacion = _context.Evaluaciones.Find(id);
            if (evaluacion == null)
            {
                return NotFound();
            }
            ViewData["Usuarios"] = new SelectList(
     _context.Usuarios
         .Include(u => u.roles) // Incluir la relación con el rol
         .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
         .ToList()
         .Select(u => new // Crear una lista anónima con nombre y rol
         {
             id_usuario = u.id_usuario,
             DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
         }),
     "id_usuario", "DisplayText", evaluacion.id_usuario);

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
            ViewData["Usuarios"] = new SelectList(
     _context.Usuarios
         .Include(u => u.roles) // Incluir la relación con el rol
         .Where(u => u.id_rol == 1 || u.id_rol == 2) // Filtrar por roles 1 y 2
         .ToList()
         .Select(u => new // Crear una lista anónima con nombre y rol
         {
             id_usuario = u.id_usuario,
             DisplayText = u.nombre + " (" + u.roles.nombre_rol + ")" // Combina el nombre y el rol
         }),
     "id_usuario", "DisplayText", evaluacion.id_usuario);

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
