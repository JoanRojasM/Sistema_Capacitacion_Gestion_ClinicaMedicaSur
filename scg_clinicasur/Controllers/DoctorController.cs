using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
