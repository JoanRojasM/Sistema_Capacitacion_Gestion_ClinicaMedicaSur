using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using scg_clinicasur.Data;
using scg_clinicasur.Models;

namespace scg_clinicasur.Controllers
{
    public class DoctorEspecialidadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorEspecialidadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para verificar si el usuario tiene rol de administrador
        private bool EsAdministrador()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "administrador";
        }

        // GET: DoctorEspecialidades/Index
        public async Task<IActionResult> Index()
        {
            if (!EsAdministrador())
                return RedirectToAction("AccessDenied", "Home");

            var doctoresConEspecialidades = await _context.DoctorEspecialidades
                .Include(d => d.Usuario)
                .GroupBy(d => d.Usuario)
                .Select(g => g.Key)
                .ToListAsync();

            return View(doctoresConEspecialidades);
        }

        // GET: DoctorEspecialidades/Detalles/5
        public async Task<IActionResult> Detalles(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("AccessDenied", "Home");

            var doctor = await _context.Usuarios
                .Include(u => u.roles)
                .FirstOrDefaultAsync(u => u.id_usuario == id);

            if (doctor == null)
                return NotFound();

            var especialidades = await _context.DoctorEspecialidades
                .Where(de => de.IdUsuario == id)
                .Include(de => de.Especialidad)
                .Select(de => de.Especialidad.NombreEspecialidad)
                .ToListAsync();

            ViewBag.Doctor = doctor;
            ViewBag.Especialidades = especialidades;

            return View();
        }

        // GET: Crear DoctorEspecialidad
        public IActionResult Crear(int? idDoctor)
        {
            // Lista de especialidades
            var especialidades = _context.Especialidades.ToList();

            // Lista de especialidades asignadas al doctor (si se está editando)
            var especialidadesAsignadas = idDoctor.HasValue
                ? _context.DoctorEspecialidades
                    .Where(de => de.IdUsuario == idDoctor)
                    .Select(de => de.IdEspecialidad)
                    .ToList()
                : new List<int>();

            // Pasar datos a la vista
            ViewBag.Especialidades = especialidades;
            ViewBag.IdDoctor = idDoctor ?? 0;
            ViewBag.EspecialidadesAsignadas = especialidadesAsignadas;

            // Lista de doctores para la selección, excluyendo a los doctores que ya tienen especialidades asignadas
            var doctoresSinEspecialidadesAsignadas = _context.Usuarios
                .Where(u => u.roles.nombre_rol == "doctor" && u.estado == "activo"
                            && !_context.DoctorEspecialidades.Any(de => de.IdUsuario == u.id_usuario))
                .Select(u => new { u.id_usuario, NombreCompleto = u.nombre + " " + u.apellido })
                .ToList();

            ViewBag.Doctores = new SelectList(doctoresSinEspecialidadesAsignadas, "id_usuario", "NombreCompleto");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(int IdUsuario, List<int> IdEspecialidadesSeleccionadas)
        {
            if (IdUsuario == 0 || IdEspecialidadesSeleccionadas == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un doctor y al menos una especialidad.");
                return RedirectToAction(nameof(Crear));
            }

            // Eliminar especialidades anteriores
            var especialidadesAnteriores = _context.DoctorEspecialidades
                .Where(de => de.IdUsuario == IdUsuario)
                .ToList();

            _context.DoctorEspecialidades.RemoveRange(especialidadesAnteriores);

            // Agregar nuevas especialidades seleccionadas
            foreach (var idEspecialidad in IdEspecialidadesSeleccionadas)
            {
                _context.DoctorEspecialidades.Add(new DoctorEspecialidad
                {
                    IdUsuario = IdUsuario,
                    IdEspecialidad = idEspecialidad
                });
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Editar DoctorEspecialidad
        public async Task<IActionResult> Editar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("AccessDenied", "Home");

            var doctor = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.id_usuario == id);

            if (doctor == null)
                return NotFound();

            var todasEspecialidades = await _context.Especialidades.ToListAsync();

            var especialidadesAsignadas = await _context.DoctorEspecialidades
                .Where(de => de.IdUsuario == id)
                .Select(de => de.IdEspecialidad)
                .ToListAsync();

            ViewBag.Doctor = doctor;
            ViewBag.TodasEspecialidades = todasEspecialidades;
            ViewBag.EspecialidadesAsignadas = especialidadesAsignadas;

            return View();
        }

        // POST: Editar DoctorEspecialidad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, List<int> EspecialidadesSeleccionadas)
        {
            if (!EsAdministrador())
                return RedirectToAction("AccessDenied", "Home");

            var doctor = await _context.Usuarios.FindAsync(id);

            if (doctor == null)
                return NotFound();

            var especialidadesAnteriores = _context.DoctorEspecialidades
                .Where(de => de.IdUsuario == id);

            _context.DoctorEspecialidades.RemoveRange(especialidadesAnteriores);

            if (EspecialidadesSeleccionadas != null)
            {
                foreach (var idEspecialidad in EspecialidadesSeleccionadas)
                {
                    _context.DoctorEspecialidades.Add(new DoctorEspecialidad
                    {
                        IdUsuario = id,
                        IdEspecialidad = idEspecialidad
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}