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

        // Editar Entrada (Ingreso)
        [HttpGet]
        public IActionResult EditarEntrada(int id)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            var entrada = _context.Contabilidades.Find(id);
            if (entrada == null || entrada.Tipo != "Ingreso")
            {
                TempData["ErrorMessage"] = "Ingreso no encontrado.";
                return RedirectToAction(nameof(Historial));
            }

            return View(entrada);
        }

        // Editar Entrada (Ingreso)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarEntrada(Contabilidad contabilidad)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (!ModelState.IsValid)
            {
                return View(contabilidad);
            }

            var entradaExistente = _context.Contabilidades.Find(contabilidad.IdContabilidad);
            if (entradaExistente == null || entradaExistente.Tipo != "Ingreso")
            {
                TempData["ErrorMessage"] = "Ingreso no encontrado.";
                return RedirectToAction(nameof(Historial));
            }

            entradaExistente.Concepto = contabilidad.Concepto;
            entradaExistente.Monto = contabilidad.Monto;
            entradaExistente.FechaRegistro = contabilidad.FechaRegistro; // Permitir modificar la fecha

            _context.Contabilidades.Update(entradaExistente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Ingreso actualizado correctamente.";
            return RedirectToAction(nameof(Historial));
        }

        [HttpGet]
        public IActionResult EditarSalida(int id)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            var salida = _context.Contabilidades.Find(id);
            if (salida == null || salida.Tipo != "Gasto")
            {
                TempData["ErrorMessage"] = "Gasto no encontrado.";
                return RedirectToAction(nameof(Historial));
            }

            return View(salida);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarSalida(Contabilidad contabilidad)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (!ModelState.IsValid)
            {
                return View(contabilidad);
            }

            var salidaExistente = _context.Contabilidades.Find(contabilidad.IdContabilidad);
            if (salidaExistente == null || salidaExistente.Tipo != "Gasto")
            {
                TempData["ErrorMessage"] = "Gasto no encontrado.";
                return RedirectToAction(nameof(Historial));
            }

            salidaExistente.Concepto = contabilidad.Concepto;
            salidaExistente.Monto = contabilidad.Monto;
            salidaExistente.FechaRegistro = contabilidad.FechaRegistro; // Permitir modificar la fecha

            _context.Contabilidades.Update(salidaExistente);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Gasto actualizado correctamente.";
            return RedirectToAction(nameof(Historial));
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            var registro = _context.Contabilidades.Find(id);
            if (registro == null)
            {
                TempData["ErrorMessage"] = "No se encontró el registro a eliminar.";
                return RedirectToAction("Historial");
            }

            return View("EliminarConfirmacion", registro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            var registro = _context.Contabilidades.Find(id);
            if (registro == null)
            {
                TempData["ErrorMessage"] = "El registro que intentas eliminar no existe.";
                return RedirectToAction("Historial");
            }

            try
            {
                _context.Contabilidades.Remove(registro);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Registro eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el registro: {ex.Message}";
            }

            return RedirectToAction("Historial");
        }

        [HttpGet]
        public IActionResult AgregarEntrada()
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarEntrada(Contabilidad contabilidad)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (ModelState.IsValid)
            {
                contabilidad.Tipo = "Ingreso"; // Asegurar que sea ingreso
                _context.Contabilidades.Add(contabilidad);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Ingreso registrado correctamente.";
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
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarSalida(Contabilidad contabilidad)
        {
            var accesoDenegado = VerificarAcceso();
            if (accesoDenegado != null) return accesoDenegado;

            if (ModelState.IsValid)
            {
                contabilidad.Tipo = "Gasto"; // Asegurar que sea gasto
                _context.Contabilidades.Add(contabilidad);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Gasto registrado correctamente.";
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
