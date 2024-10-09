using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("resultados_laboratorio")]
    public class ResultadosLaboratorio
    {
        [Key]
        [Column("IdResultado")]
        public int IdResultado { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        [Column("IdPaciente")]
        public int IdPaciente { get; set; }

        [Required]
        [ForeignKey("Expediente")]
        [Column("IdExpediente")]
        public int IdExpediente { get; set; }  // Relaciona los resultados con un expediente médico

        [Required]
        [Column("TipoPrueba")]
        public string TipoPrueba { get; set; }

        [Required]
        [Column("Resultado")]
        public string Resultado { get; set; }

        [Required]
        [Column("FechaPrueba")]
        public DateTime FechaPrueba { get; set; }

        // Relaciones con la tabla de Pacientes y Expedientes
        public Usuario Paciente { get; set; }
        public Expediente Expediente { get; set; }  // Relación con el expediente
    }
}