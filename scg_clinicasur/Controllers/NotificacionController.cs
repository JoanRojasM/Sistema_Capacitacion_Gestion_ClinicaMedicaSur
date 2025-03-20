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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarNotificacion(int id_notificacion)
    {
        var userIdString = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userIdString))
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        int userId = int.Parse(userIdString);

        // Buscar la notificación por id y usuario
        var notificacion = await _context.Notificaciones
            .FirstOrDefaultAsync(n => n.id_notificacion == id_notificacion && n.id_usuario == userId);

        if (notificacion == null)
        {
            // Si la notificación no existe o no pertenece al usuario, redirigir a una página de error
            return RedirectToAction("AccessDenied", "Home");
        }

        // Eliminar la notificación
        _context.Notificaciones.Remove(notificacion);
        await _context.SaveChangesAsync();

        // Redirigir de vuelta al índice de notificaciones
        return RedirectToAction("Index", "Notificacion");
    }
}