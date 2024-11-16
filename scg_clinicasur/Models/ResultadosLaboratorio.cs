using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("resultadoslaboratorio")]
    public class ResultadosLaboratorio
    {
        [Key]
        [Column("id_resultado")]
        public int IdResultado { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Required]
        [ForeignKey("Expediente")]
        [Column("id_expediente")]
        public int IdExpediente { get; set; }  // Relaciona los resultados con un expediente médico

        [Required]
        [Column("fechaPrueba")]
        public DateTime FechaPrueba { get; set; }

        [Required]
        [Column("ArchivoPDF")]
        public byte[] ArchivoPDF { get; set; } // Campo para almacenar el archivo PDF

        // Relaciones con la tabla de Pacientes y Expedientes
        public Usuario Paciente { get; set; }
        public Expediente Expediente { get; set; }  // Relación con el expediente
    }
}