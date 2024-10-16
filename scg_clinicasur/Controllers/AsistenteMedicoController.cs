using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;

namespace scg_clinicasur.Controllers
{
    public class AsistenteMedicoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsistenteMedicoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Método para mostrar las evaluaciones del usuario actual
        public async Task<IActionResult> Evaluaciones()
        {
            // Obtener el ID del usuario actual desde la sesión
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Filtrar las evaluaciones por el ID del usuario actual
            var evaluaciones = await _context.Evaluaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();

            return View(evaluaciones);
        }
    }
}
