using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre del usuario es requerido")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico del usuario es requerido")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "La información debe ser un correo electrónico")]
        public string correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(255)]
        public string contraseña { get; set; }

        [StringLength(15)]
        public string? telefono { get; set; }

        public DateTime fecha_registro { get; set; } = DateTime.Now;

        // Campo que debe ser requerido: id_rol, no roles
        [Required(ErrorMessage = "El rol es requerido")]
        public int id_rol { get; set; }

        // La propiedad de navegación para Roles no necesita validación
        [ForeignKey("id_rol")]
        public Roles? roles { get; set; }  // Esto solo es útil cuando consultas el usuario, no durante la creación
    }

    public class Roles
    {
        [Key]
        public int id_rol { get; set; }

        [Required]
        [StringLength(50)]
        public string nombre_rol { get; set; }
    }
}