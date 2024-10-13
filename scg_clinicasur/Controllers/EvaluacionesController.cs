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
    }
}
