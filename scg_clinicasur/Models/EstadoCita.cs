using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("estado_citas")] // Nombre de la tabla
    public class EstadoCita
    {
        [Key]
        [Column("id_estado_cita")]
        public int IdEstadoCita { get; set; }

        [Column("estado_nombre")]
        public string EstadoNombre { get; set; }
    }
}