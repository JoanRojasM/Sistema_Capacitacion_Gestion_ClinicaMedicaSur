using Microsoft.AspNetCore.Mvc;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Globalization;
using System.Linq;

namespace scg_clinicasur.Controllers
{
    public class ContabilidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool EsAdministrador()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "administrador";
        }

        private IActionResult VerificarAcceso()
        {
            if (!EsAdministrador())
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            return null;
        }

        public IActionResult Historial()
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            var historial = _context.Contabilidades.ToList();
            return View(historial);
        }

        [HttpGet]
        public IActionResult AgregarEntrada()
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            return View();
        }

        [HttpPost]
        public IActionResult AgregarEntrada(Contabilidad contabilidad)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (ModelState.IsValid)
            {
                contabilidad.Tipo = "Ingreso"; // Aseguramos que sea "Ingreso"
                _context.Contabilidades.Add(contabilidad);
                _context.SaveChanges();
                return RedirectToAction(nameof(Historial));
            }
            return View(contabilidad);
        }

        [HttpGet]
        public IActionResult RegistrarSalida()
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            return View();
        }

        [HttpPost]
        public IActionResult RegistrarSalida(Contabilidad contabilidad)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (ModelState.IsValid)
            {
                contabilidad.Tipo = "Gasto"; // Aseguramos que sea "Gasto"
                _context.Contabilidades.Add(contabilidad);
                _context.SaveChanges();
                return RedirectToAction(nameof(Historial));
            }
            return View(contabilidad);
        }

        [HttpGet]
        public IActionResult ReporteMensual()
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            // Vista inicial sin filtro
            ViewBag.MesSeleccionado = null;
            return View(Enumerable.Empty<Contabilidad>());
        }

        [HttpPost]
        public IActionResult ReporteMensual(string mes)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (!string.IsNullOrEmpty(mes))
            {
                // Convertir el valor del input (YYYY-MM) a un rango de fechas
                DateTime inicioMes = DateTime.ParseExact(mes + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);

                // Filtrar registros por mes
                var registros = _context.Contabilidades
                    .Where(c => c.FechaRegistro >= inicioMes && c.FechaRegistro <= finMes)
                    .ToList();

                // Pasar el mes seleccionado a la vista
                ViewBag.MesSeleccionado = mes;

                return View(registros);
            }

            // Si no hay filtro, retornar la vista vacía
            ViewBag.MesSeleccionado = null;
            return View(Enumerable.Empty<Contabilidad>());
        }
    }
}
