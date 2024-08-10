using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class ContabilidadController : Controller
    {
        public IActionResult AgregarEntrada()
        {
            return View();
        }

        public IActionResult RegistrarSalida()
        {
            return View();
        }

        public IActionResult ReporteMensual()
        {
            return View();
        }

        public IActionResult Historial()
        {
            return View();
        }
    }

}
