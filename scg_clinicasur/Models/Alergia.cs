using System.ComponentModel.DataAnnotations;

namespace scg_clinicasur.Models
{
    public class Alergia
    {
        [Key]
        public int id_alergia { get; set; }

        [Required]
        [StringLength(255)]
        public string nombre_alergia { get; set; }
    }
}
