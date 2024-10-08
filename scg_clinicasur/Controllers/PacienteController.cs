using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class PacienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
