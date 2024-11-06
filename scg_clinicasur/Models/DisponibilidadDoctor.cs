using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("disponibilidad_doctor")]
    public class DisponibilidadDoctor
    {
        [Key]
        [Column("id_disponibilidad")]
        public int IdDisponibilidad { get; set; }

        [Column("id_doctor")]
        [Required]
        public int IdDoctor { get; set; }

        [Column("dia_semana")]
        [Required]
        [StringLength(10)]
        public string DiaSemana { get; set; }  // Día de la semana en texto ("Lunes", "Martes", etc.)

        [Column("hora_inicio")]
        [Required]
        public TimeSpan HoraInicio { get; set; }  // Hora de inicio de disponibilidad

        [Column("hora_fin")]
        [Required]
        public TimeSpan HoraFin { get; set; }  // Hora de fin de disponibilidad

        // Relación con la entidad Usuario
        [ForeignKey("IdDoctor")]
        public Usuario Doctor { get; set; }
    }
}