using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Security.Claims;

namespace scg_clinicasur.Controllers
{
    public class AsistenteLimpiezaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AsistenteLimpiezaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

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

    } }
