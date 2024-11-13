using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("capacitacion")]
    public class Capacitacion
    {
        [Key]
        public int id_capacitacion { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(255)]
        public string titulo { get; set; }

        [StringLength(1000)]
        public string descripcion { get; set; }

        // Duración no requerida para permitir NULL en la base de datos
        public TimeSpan? duracion { get; set; }

        // id_usuario no requerido para permitir NULL en la base de datos
        [ForeignKey("Usuario")]
        public int? id_usuario { get; set; }
        public Usuario? Usuario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime fecha_creacion { get; set; }

        [StringLength(255)]
        public string archivo { get; set; }

        // Estado permitido como NULL y sin inicialización por defecto
        [StringLength(10)]
        public string estado { get; set; }
    }
}