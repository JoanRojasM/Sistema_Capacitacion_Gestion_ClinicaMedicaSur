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

}
