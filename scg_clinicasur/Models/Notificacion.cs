using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("notificacion")]
    public class Notificacion
    {
        [Key]
        public int id_notificacion { get; set; }

        [ForeignKey("Usuario")]
        [Column("id_usuario")]
        public int id_usuario { get; set; }

        [StringLength(100)]
        public string titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string mensaje { get; set; } = string.Empty;

        public DateTime fecha_envio { get; set; }
    }
}
