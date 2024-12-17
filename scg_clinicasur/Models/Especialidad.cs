using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("especialidades")]
    public class Especialidad
    {
        [Key]
        [Column("id_especialidad")]
        public int IdEspecialidad { get; set; }

        [Required(ErrorMessage = "El nombre de la especialidad es requerido.")]
        [StringLength(100)]
        [Column("nombre_especialidad")]
        public string NombreEspecialidad { get; set; }

        // Relación con los Doctores
        public ICollection<DoctorEspecialidad>? DoctoresEspecialidades { get; set; }
    }
}
