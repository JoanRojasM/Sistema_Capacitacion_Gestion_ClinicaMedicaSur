using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

public class NotificacionController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificacionController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userIdString = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userIdString))
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        int userId = int.Parse(userIdString);

        var notificaciones = await _context.Notificaciones
            .Where(n => n.id_usuario == userId)
            .OrderByDescending(n => n.fecha_envio)
            .ToListAsync();

        return View(notificaciones);
    }

    [HttpGet]
    public async Task<IActionResult> EliminarNotificacion(int id_notificacion)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id_notificacion);
        if (notificacion == null)
        {
            return NotFound();
        }

        return View(notificacion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarEliminarNotificacion(int id_notificacion)
    {
        try
        {
            var notificacion = await _context.Notificaciones.FindAsync(id_notificacion);
            if (notificacion == null)
            {
                return NotFound();
            }

            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Notificacion");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al eliminar la notificación: {ex.Message}");
            return RedirectToAction("Index", "Notificacion");
        }
    }

}