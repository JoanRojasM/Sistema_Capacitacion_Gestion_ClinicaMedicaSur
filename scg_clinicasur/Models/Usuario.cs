using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required (ErrorMessage = "El nombre del usuario es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico del usuario es requerido")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "La información debe ser un correo electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(255)]
        public string Contraseña { get; set; }

        [StringLength(15)]
        public string? Telefono { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación con la tabla Roles
        public int IdRol { get; set; }

        [ForeignKey("IdRol")]
        public Rol Rol { get; set; }
    }

    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreRol { get; set; }
    }
}