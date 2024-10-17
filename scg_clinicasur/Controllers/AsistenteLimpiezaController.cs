using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Security.Claims;

namespace scg_clinicasur.Controllers
{
    public class AsistenteLimpiezaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsistenteLimpiezaController(ApplicationDbContext context)
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
            // Obtener el nombre del usuario de la sesión
            var userName = HttpContext.Session.GetString("UserName");
            // Obtener el ID del usuario actual desde la sesión
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Filtrar las evaluaciones por el ID del usuario actual
            var evaluaciones = await _context.Evaluaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();
            return View(evaluaciones);
        }

        public async Task<IActionResult> Capacitaciones()
        {
            // Obtener el nombre del usuario de la sesión
            var userName = HttpContext.Session.GetString("UserName");
            // Obtener el ID del usuario actual desde la sesión
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Filtrar las evaluaciones por el ID del usuario actual
            var capacitaciones = await _context.Capacitaciones
                                             .Where(e => e.id_usuario == userId)
                                             .ToListAsync();
            return View(capacitaciones);
        }
    }
}