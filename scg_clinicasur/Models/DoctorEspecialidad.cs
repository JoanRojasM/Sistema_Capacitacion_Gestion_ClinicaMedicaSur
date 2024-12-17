using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("doctor_especialidades")]
    public class DoctorEspecialidad
    {
        [Key]
        [Column("id_doctor_especialidad")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        [ForeignKey("Especialidad")]
        [Column("id_especialidad")]
        public int IdEspecialidad { get; set; }
        public Especialidad Especialidad { get; set; }
    }
}
