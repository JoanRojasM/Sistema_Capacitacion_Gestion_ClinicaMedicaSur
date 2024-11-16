using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace scg_clinicasur.Models
{
    public class PacienteAlergia
    {
        [Key]
        [Column("id_pacientealergias")]
        public int Id { get; set; }

        [ForeignKey("Paciente")]
        public int id_paciente { get; set; }
        public Usuario Paciente { get; set; }

        [ForeignKey("Alergia")]
        public int id_alergia { get; set; }
        public Alergia Alergia { get; set; }

        [Required]
        [Column("fecha_registro")]
        public DateTime fechaRegistro { get; set; }
    }
}