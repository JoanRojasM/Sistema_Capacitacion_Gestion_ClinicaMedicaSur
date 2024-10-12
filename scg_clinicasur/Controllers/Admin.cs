using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

namespace scg_clinicasur.Controllers
{
    public class Admin : Controller
    {
        private readonly ApplicationDbContext _context;

        public Admin(ApplicationDbContext context)
        {
            _context = context;
        }
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
        public IActionResult AdminResources()
        {
            return View();
        }
        public IActionResult AdminHistory()
        {
            return View();
        }

    }
}
