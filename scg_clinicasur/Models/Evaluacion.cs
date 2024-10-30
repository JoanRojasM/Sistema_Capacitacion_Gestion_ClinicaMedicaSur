using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scg_clinicasur.Models
{
    [Table("evaluacion")] // Especificar el nombre de la tabla
    public class Evaluacion
    {
        [Key]
        public int id_evaluacion { get; set; }

        [StringLength(100)] // Coincide con VARCHAR(100) en la tabla
        public string nombre { get; set; }

        [Column(TypeName = "TEXT")] // Definir como tipo TEXT
        public string descripcion { get; set; }

        // Permitir null para 'tiempo_prueba' si es necesario
        public TimeSpan? tiempo_prueba { get; set; }

        [StringLength(255)]
        public string archivo { get; set; }

        [ForeignKey("Usuario")]
        public int? id_usuario { get; set; } // Permitir null para la clave foránea
        public Usuario? Usuario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime fecha_creacion { get; set; } // Fecha de creación automática
    }
}