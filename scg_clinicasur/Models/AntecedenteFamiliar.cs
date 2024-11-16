using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace scg_clinicasur.Models
{
    [Table("antecedentesfamiliares")]
    public class AntecedenteFamiliar
    {
        [Key]
        [Column("id_antecedente")]
        public int IdAntecedente { get; set; }

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
