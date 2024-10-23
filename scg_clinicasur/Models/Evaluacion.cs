using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    public class Evaluacion
    {
        [Key]
        public int id_evaluacion { get; set; }

       
        [StringLength(255)]
        public string nombre { get; set; }

        [StringLength(1000)]
        public string descripcion { get; set; }

        public TimeSpan tiempo_prueba { get; set; }

        [StringLength(255)]
        public string archivo { get; set; }

        [ForeignKey("Usuario")]
        public int id_usuario { get; set; }
        public Usuario Usuario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime fecha_creacion { get; set; } // Nueva propiedad
    }
}

