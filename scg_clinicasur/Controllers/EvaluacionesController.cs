using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Net.Mail;
using System.Net;

namespace scg_clinicasur.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public EvaluacionesController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<IActionResult> Index()
        {
            var evaluaciones = await _context.Evaluaciones
                                             .Include(e => e.Usuario)
                                             .ThenInclude(u => u.roles)
                                             .Include(e => e.Capacitacion)
                                             .ToListAsync();

            return View(evaluaciones);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            // Cargar la lista de usuarios y capacitaciones para las listas desplegables
            var usuarios = _context.Usuarios.Where(u => u.id_rol == 1 || u.id_rol == 2).ToList();
            var capacitaciones = _context.Capacitaciones.ToList();

            ViewData["Usuarios"] = usuarios;
            ViewData["Capacitaciones"] = capacitaciones;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Evaluacion evaluacion, IFormFile? archivo)
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            if (ModelState.IsValid)
            {
                try
                {
                    evaluacion.fecha_creacion = DateTime.Now;

                    // Guardar el archivo si se proporciona
                    if (archivo != null && archivo.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "evaluaciones");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var fileName = Path.GetFileName(archivo.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        evaluacion.archivo = Path.Combine("/evaluaciones", fileName);
                    }

                    // Guardar la evaluación en la base de datos
                    _context.Evaluaciones.Add(evaluacion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar la evaluación: {ex.Message}");
                }
            }

            // Recargar los datos en caso de error
            ViewBag.Usuarios = new SelectList(_context.Usuarios.Where(u => u.id_rol == 1 || u.id_rol == 2).ToList(), "id_usuario", "nombre");
            ViewBag.Capacitaciones = new SelectList(_context.Capacitaciones.ToList(), "id_capacitacion", "titulo");

            return View(evaluacion);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int? id)
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            var evaluacion = await _context.Evaluaciones.FindAsync(id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            ViewData["Usuarios"] = _context.Usuarios.Where(u => u.id_rol == 1 || u.id_rol == 2).ToList();
            ViewData["Capacitaciones"] = _context.Capacitaciones.ToList();
            return View(evaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Evaluacion evaluacion, IFormFile? archivo)
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            if (id != evaluacion.id_evaluacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar archivo si se proporciona
                    if (archivo != null)
                    {
                        var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "evaluaciones");
                        if (!Directory.Exists(carpetaDestino))
                        {
                            Directory.CreateDirectory(carpetaDestino);
                        }

                        var nombreArchivo = Path.GetFileName(archivo.FileName);
                        var rutaArchivo = Path.Combine(carpetaDestino, nombreArchivo);

                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        evaluacion.archivo = Path.Combine("/evaluaciones", nombreArchivo);                        
                    }

                    _context.Update(evaluacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Evaluaciones.Any(e => e.id_evaluacion == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuarios"] = _context.Usuarios.Where(u => u.id_rol == 1 || u.id_rol == 2).ToList();
            ViewData["Capacitaciones"] = _context.Capacitaciones.ToList();
            return View(evaluacion);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Capacitacion)
                                           .FirstOrDefaultAsync(e => e.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            // Verificar si el rol del usuario es administrador
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "administrador")
            {
                return RedirectToAction("AccessDenied", "Home"); // Redirigir a una página de acceso denegado
            }

            try
            {
                var evaluacion = await _context.Evaluaciones.FindAsync(id);
                if (evaluacion != null)
                {
                    if (!string.IsNullOrEmpty(evaluacion.archivo))
                    {
                        var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "evaluaciones", evaluacion.archivo.TrimStart('/'));
                        if (System.IO.File.Exists(rutaArchivo))
                        {
                            System.IO.File.Delete(rutaArchivo);
                        }
                    }

                    _context.Evaluaciones.Remove(evaluacion);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar la evaluacion: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Capacitacion)
                                           .Include(e => e.Usuario)
                                           .FirstOrDefaultAsync(e => e.id_evaluacion == id);

            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }
    }
}