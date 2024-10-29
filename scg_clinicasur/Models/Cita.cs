using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("citas")] // Nombre de la tabla
    public class Cita
    {
        [Key]
        [Column("id_cita")]
        public int IdCita { get; set; }

        [Column("id_paciente")]
        [Required(ErrorMessage = "El ID del paciente es requerido")]
        public int IdPaciente { get; set; }

        [Column("id_doctor")]
        [Required(ErrorMessage = "El ID del doctor es requerido")]
        public int IdDoctor { get; set; }

        [Required(ErrorMessage = "La fecha de inicio de la cita es requerida.")]
        [DataType(DataType.DateTime)]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin de la cita es requerida.")]
        [DataType(DataType.DateTime)]
        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("motivo_cita")]
        [Required(ErrorMessage = "El motivo de la cita es requerido")]
        public string? MotivoCita { get; set; }

        [Column("id_estado_cita")]
        [Required(ErrorMessage = "El motivo de la cita es requerido")]
        public int IdEstadoCita { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        // Relaciones del modelo
        [ForeignKey("IdPaciente")]
        public Usuario? Paciente { get; set; }

        [ForeignKey("IdDoctor")]
        public Usuario? Doctor { get; set; }

        [ForeignKey("IdEstadoCita")]
        public EstadoCita? EstadoCita { get; set; }
    }
}