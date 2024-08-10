using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class NotificacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalles(int id)
        {
            ViewData["NotificationId"] = id;
            ViewData["Tipo"] = "Recordatorio";
            ViewData["Mensaje"] = "Consulta programada para mañana.";
            ViewData["Fecha"] = "2024-08-10";

            return View();
        }

        public IActionResult Configuracion()
        {
            return View();
        }
    }
}
