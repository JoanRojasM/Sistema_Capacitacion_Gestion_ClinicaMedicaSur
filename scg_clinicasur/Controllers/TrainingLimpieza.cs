using Microsoft.AspNetCore.Mvc;

namespace scg_clinicasur.Controllers
{
    public class TrainingLimpieza : Controller
    {
        // Acción para mostrar la programación de capacitaciones
        public IActionResult Schedule()
        {
            return View();
        }

        // Acción para agendar una capacitación
        public IActionResult ScheduleTraining()
        {
            return View();
        }

        // Acción para gestionar la programación de capacitaciones
        public IActionResult ManageSchedule()
        {
            return View();
        }

        // Acción para ver el historial de capacitaciones
        public IActionResult History()
        {
            return View();
        }

        // Acción para acceder a materiales de capacitación
        public IActionResult Resources()
        {
            return View();
        }

    }
}
