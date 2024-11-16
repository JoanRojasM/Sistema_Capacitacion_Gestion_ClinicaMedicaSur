using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

namespace scg_clinicasur.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaluacionesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var evaluaciones = await _context.Evaluaciones
                                             .Include(e => e.Usuario)
                                             .ThenInclude(u => u.roles)
                                             .ToListAsync();

            return View(evaluaciones);
        }
        public IActionResult Crear()
        {
            var usuarios = _context.Usuarios.ToList();
            var capacitaciones = _context.Capacitaciones.ToList();

            if (usuarios == null || capacitaciones == null)
            {
                return RedirectToAction("Error");
            }

            ViewData["Usuarios"] = usuarios;
            ViewData["Capacitaciones"] = capacitaciones;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("nombre,descripcion,tiempo_prueba,id_usuario, id_capacitacion")] Evaluacion evaluacion, IFormFile archivo)
        {
            if (ModelState.IsValid)
            {
                if (archivo != null)
                {
                    var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos");
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

                    evaluacion.archivo = Path.Combine("/archivos", nombreArchivo);
                }

                _context.Add(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Capacitaciones"] = _context.Capacitaciones.ToList();
            return View(evaluacion);
        }
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                                           .Include(e => e.Capacitacion)
                                           .FirstOrDefaultAsync(m => m.id_evaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            // Cargar la lista de capacitaciones y usuarios para el dropdown
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "Nombre", evaluacion.id_usuario);
            ViewData["Capacitaciones"] = new SelectList(_context.Capacitaciones, "id_capacitacion", "Nombre", evaluacion.id_capacitacion);

            return View(evaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("id_evaluacion,id_capacitacion,nombre,descripcion,tiempo_prueba,archivo,id_usuario,fecha_creacion")] Evaluacion evaluacion, IFormFile archivo)
        {
            if (id != evaluacion.id_evaluacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (archivo != null && archivo.Length > 0)
                    {
                        var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "archivos", archivo.FileName);

                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        evaluacion.archivo = archivo.FileName;
                    }

                    _context.Update(evaluacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvaluacionExists(evaluacion.id_evaluacion))
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

            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "id_usuario", "nombre", evaluacion.id_usuario);
            ViewData["Capacitaciones"] = new SelectList(_context.Capacitaciones, "id_capacitacion", "titulo", evaluacion.id_capacitacion);

            return View(evaluacion);
        }
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.id_evaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.id_evaluacion == id);
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
            var evaluacion = await _context.Evaluaciones.FindAsync(id);
            if (evaluacion != null)
            {
                _context.Evaluaciones.Remove(evaluacion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EvaluacionExists(int id)
        {
            return _context.Evaluaciones.Any(e => e.id_evaluacion == id);
        }
    }
}