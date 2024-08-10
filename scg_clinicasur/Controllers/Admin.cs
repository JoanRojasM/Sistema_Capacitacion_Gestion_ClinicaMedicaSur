using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminSchedule()
        {
            return View();
        }
        public IActionResult AdminScheduleMed()
        {
            return View();
        }
        public IActionResult AdminSchedulelimpieza()
        {
            return View();
        }
    }
}
