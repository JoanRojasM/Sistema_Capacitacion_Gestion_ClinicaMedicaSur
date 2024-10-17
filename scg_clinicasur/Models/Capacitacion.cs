using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    public class Capacitacion
    {
        [Key]
        public int id_capacitacion { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(255)]
        public string titulo { get; set; }

        [StringLength(1000)]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        public TimeSpan duracion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario")]
        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}

