using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace scg_clinicasur.Controllers
{
    public class DisponibilidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DisponibilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int ObtenerIdDoctor()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userIdString) || userRole != "doctor") return -1;
            return int.TryParse(userIdString, out int userId) ? userId : -1;
        }

        // Mostrar la disponibilidad del doctor actual
        public async Task<IActionResult> Gestionar()
        {
            int idDoctor = ObtenerIdDoctor();
            if (idDoctor == -1) return RedirectToAction("AccessDenied", "Home");

            var disponibilidad = await _context.DisponibilidadDoctor
                .Where(d => d.IdDoctor == idDoctor)
                .OrderBy(d => d.DiaSemana)
                .ThenBy(d => d.HoraInicio)
                .ToListAsync();

            return View(disponibilidad);
        }

        // Agregar disponibilidad
        [HttpPost]
        public async Task<IActionResult> Agregar(DisponibilidadDoctor disponibilidad)
        {
            int idDoctor = ObtenerIdDoctor();
            if (idDoctor == -1) return RedirectToAction("AccessDenied", "Home");

            disponibilidad.IdDoctor = idDoctor;

            // Validación: evitar superposición de horarios
            bool conflicto = await _context.DisponibilidadDoctor.AnyAsync(d =>
                d.IdDoctor == idDoctor &&
                d.DiaSemana == disponibilidad.DiaSemana &&
                ((disponibilidad.HoraInicio >= d.HoraInicio && disponibilidad.HoraInicio < d.HoraFin) ||
                (disponibilidad.HoraFin > d.HoraInicio && disponibilidad.HoraFin <= d.HoraFin)));

            if (conflicto)
            {
                TempData["ErrorMessage"] = "El horario se solapa con otro bloque existente.";
                return RedirectToAction(nameof(Gestionar));
            }

            _context.DisponibilidadDoctor.Add(disponibilidad);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Disponibilidad agregada correctamente.";
            return RedirectToAction(nameof(Gestionar));
        }

        // Eliminar disponibilidad
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            int idDoctor = ObtenerIdDoctor();
            if (idDoctor == -1) return RedirectToAction("AccessDenied", "Home");

            var disponibilidad = await _context.DisponibilidadDoctor
                .FirstOrDefaultAsync(d => d.IdDisponibilidad == id && d.IdDoctor == idDoctor);

            if (disponibilidad != null)
            {
                _context.DisponibilidadDoctor.Remove(disponibilidad);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Disponibilidad eliminada.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se encontró el bloque de horario.";
            }

            return RedirectToAction(nameof(Gestionar));
        }
    }
}

