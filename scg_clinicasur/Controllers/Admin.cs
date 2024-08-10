using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
