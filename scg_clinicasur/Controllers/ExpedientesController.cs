using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class ExpedientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Historial()
        {
            return View();
        }

        public IActionResult BusquedaExpedientes()
        {
            return View();
        }

        public IActionResult DetallesConsulta(int id)
        {
            return View();
        }
    }
}
