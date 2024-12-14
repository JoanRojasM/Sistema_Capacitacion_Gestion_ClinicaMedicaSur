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

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var notificacion = await _context.Notificaciones
            .FirstOrDefaultAsync(n => n.id_notificacion == id);

        if (notificacion == null)
        {
            return NotFound();
        }

        return View(notificacion);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var notificacion = await _context.Notificaciones
            .FirstOrDefaultAsync(n => n.id_notificacion == id);

        if (notificacion != null)
        {
            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }


}
