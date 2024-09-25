using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Data;
using scg_clinicasur.Models;
using System.Threading.Tasks;

namespace scg_clinicasur.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inyectacción del DbContext a través del constructor
        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //Consultar los usuarios y cargarlos con su rol correspondiente
            var usuarios = await _context.Usuarios.Include(u => u.roles).ToListAsync();

            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            //Este viewbag consulta los roles y los muestra en la vista crear
            ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Establecemos la fecha de registro del usuario. DateTime.Now es para determinar la fecha actual
                usuario.fecha_registro = DateTime.Now;

                // Aquí debes añadir el proceso de hasheo de la contraseña antes de guardar
                usuario.contraseña = usuario.contraseña;  // Aquí aplicas tu método de hasheo

                // Se guarda el nuevo usuario en la base de datos si el modelo es valido
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Guardar el mensaje en TempData
                TempData["SuccessMessage"] = "Usuario creado exitosamente";

                return RedirectToAction("Index");
            }

            // Si el modelo no es válido, volver a cargar los roles para mostrarlos en la vista
            ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol");
            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            // Buscamos el usuario por su ID, este debe ser el que hemos consultado en la vista.
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Cargar los roles para el dropdown
            ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol", usuario.id_rol);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.id_usuario)
            {
                return BadRequest("El ID en la URL no coincide con el ID del usuario.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cargar el usuario actual desde la base de datos para preservar la contraseña si no se cambia
                    var usuarioExistente = await _context.Usuarios.FindAsync(id);
                    if (usuarioExistente == null)
                    {
                        return NotFound("El usuario no existe.");
                    }

                    // Si el campo contraseña no está vacío, actualizar la contraseña
                    if (!string.IsNullOrWhiteSpace(usuario.contraseña))
                    {
                        usuarioExistente.contraseña = usuario.contraseña; // Aquí puedes aplicar el hasheo si es necesario
                    }

                    // Actualizar otros campos
                    usuarioExistente.nombre = usuario.nombre;
                    usuarioExistente.apellido = usuario.apellido;
                    usuarioExistente.correo = usuario.correo;
                    usuarioExistente.telefono = usuario.telefono;
                    usuarioExistente.id_rol = usuario.id_rol;

                    // Guardar los cambios
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id_usuario))
                    {
                        return NotFound("El usuario no existe.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si hay errores, volver a cargar los roles y mostrar el formulario con los datos actuales
            ViewBag.Roles = new SelectList(_context.Roles, "id_rol", "nombre_rol", usuario.id_rol);
            return View(usuario);
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.id_usuario == id);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario); // Pasamos el usuario a la vista de confirmación
        }
        
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Eliminar el usuario de la base de datos
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
            return RedirectToAction("Index");
        }
    }
}
