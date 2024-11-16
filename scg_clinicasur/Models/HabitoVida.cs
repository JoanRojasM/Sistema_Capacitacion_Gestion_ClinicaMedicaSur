using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace scg_clinicasur.Models
{
    [Table("habitosvida")]
    public class HabitoVida
    {
        [Key]
        [Column("id_habito")]
        public int IdHabito { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }
        public Usuario Paciente { get; set; }

        [Required]
        [Column("descripcion")]
        [StringLength(1000)]
        public string Descripcion { get; set; }

        [Required]
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
