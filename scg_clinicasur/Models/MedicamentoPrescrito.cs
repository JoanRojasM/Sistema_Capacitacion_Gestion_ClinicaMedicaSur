using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace scg_clinicasur.Models
{
    [Table("medicamentosprescritos")]
    public class MedicamentoPrescrito
    {
        [Key]
        [Column("id_medicamento")]
        public int IdMedicamento { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }
        public Usuario Paciente { get; set; }

        [Required]
        [Column("nombre_medicamento")]
        [StringLength(255)]
        public string NombreMedicamento { get; set; }

        [Required]
        [Column("dosis")]
        [StringLength(100)]
        public string Dosis { get; set; }

        [Required]
        [Column("fecha_prescripcion")]
        public DateTime FechaPrescripcion { get; set; }

        [Required]
        [Column("estado")]
        [StringLength(20)]
        public string Estado { get; set; } = "activo";  // Valores: "activo" o "descontinuado"
    }
}
