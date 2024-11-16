using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("recursos_aprendizaje")]
    public class RecursosAprendizaje
    {
        [Key]
        [Column("id_recurso")]
        public int id_recurso { get; set; }

        [Required]
        [ForeignKey("Capacitacion")]
        [Column("id_capacitacion")]
        public int id_capacitacion { get; set; }

        [StringLength(255)]
        [Column("archivo")]
        public string? archivo { get; set; }

        [StringLength(500)]
        [Column("enlace")]
        public string? enlace { get; set; }

        [Column("fecha_creacion")]
        public DateTime fecha_creacion { get; set; } = DateTime.Now;

        public virtual Capacitacion? Capacitacion { get; set; }
    }
}
