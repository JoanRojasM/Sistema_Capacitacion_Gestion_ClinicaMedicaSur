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

        [StringLength(255)]
        public string contraseña { get; set; } // Eliminar el [Required] para hacerlo opcional en la edición

        [StringLength(15)]
        public string? telefono { get; set; }

        public DateTime fecha_registro { get; set; } = DateTime.Now;

        public DateTime fecha_nacimiento { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public int id_rol { get; set; }

        [ForeignKey("id_rol")]
        public Roles? roles { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression("(activo|inactivo)", ErrorMessage = "El estado debe ser 'activo' o 'inactivo'.")]
        public string estado { get; set; } = "activo";

        //Relacion de Pacientes y Alergias
        public ICollection<scg_clinicasur.Models.PacienteAlergia>? PacienteAlergias { get; set; }
        //Relación Contacto de Emergencia
        public ICollection<ContactoEmergencia>? ContactosEmergencia { get; set; }

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