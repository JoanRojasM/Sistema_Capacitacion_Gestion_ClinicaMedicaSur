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

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000)]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        [StringLength(255)]
        public string duracion { get; set; }

        [ForeignKey("Usuario")]
        public int? id_usuario { get; set; }
        public Usuario? Usuario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime fecha_creacion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(10)]
        public string estado { get; set; }
    }
}